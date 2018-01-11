using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FProxy.Network
{
    /// <summary>
    /// Socket logic for handling conquer data exchange
    /// </summary>
    public class CSockState
    {
        public Socket Socket;
        public byte[] CBuffer;
    }

    public class CSocket
    {

        #region events
        public event Action<CSockState> ConnectionReceived;
        public event Action<byte[], CSockState, byte[]> DataReceived;
        public event Action<CSockState> ConnectionTerminated;
        #endregion

        private Socket _socket;
        public CSocket(string externalIp, int internalPort, int externalPort)
        {

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind socket to  any known IP (127.0.0.1,Local ip etc) and supplied internal port
            _socket.Bind(new IPEndPoint(IPAddress.Any, internalPort));
            _socket.Listen(1000);
            //We pass a new CSockStateObject as stateObject, on accept we will fill it with required properties
            _socket.BeginAccept(AcceptSocketConnection, new CSockState());
        }

        /// <summary>
        /// Accepts incoming connections on supplied port
        /// </summary>
        /// <param name="result"></param>
        private void AcceptSocketConnection(IAsyncResult result)
        {
            //We received a connection, parse supplied state object to CSockState
            CSockState cSockState = result.AsyncState as CSockState;
            //Create a buffer for received data in the stateobject
            cSockState.CBuffer = new byte[1024];
            //End the acceptCall, and store created socket connection in our stateobject
            cSockState.Socket = _socket.EndAccept(result);

            ConnectionReceived.Invoke(cSockState);
            //Start listening for data on the established socket connection
            cSockState.Socket.BeginReceive(cSockState.CBuffer, 0, cSockState.CBuffer.Length, SocketFlags.None, ReceiveData, cSockState);

            //Start listening for new connections
            _socket.BeginAccept(AcceptSocketConnection, new CSockState());
        }



        private void ReceiveData(IAsyncResult result)
        {
            try
            {
                CSockState cSockState = result.AsyncState as CSockState;
                //No connection ? 
                if (!cSockState.Socket.Connected) { return; }
                SocketError socketStatus = SocketError.Disconnecting;
                int dataLength = cSockState.Socket.EndReceive(result, out socketStatus);
                if (socketStatus == SocketError.Success && dataLength != 0)
                {
                    //We received data, copy to local byte buffer
                    byte[] buffer = new byte[dataLength];
                    Buffer.BlockCopy(cSockState.CBuffer,0, buffer, 0, dataLength);
                    //TODO: Determine what this is used for ?
                    byte[] question = new byte[] { 1 };

                    DataReceived.Invoke(buffer, cSockState, question);

                    //If socket still connected receive more data
                    if(cSockState.Socket.Connected && question[0] == 1)
                    {
                        cSockState.Socket.BeginReceive(cSockState.CBuffer, 0, cSockState.CBuffer.Length, SocketFlags.None, ReceiveData, cSockState);
                    }
                    
                }
                else
                {
                    //On error disconnect the socket
                    if (cSockState.Socket.Connected) { cSockState.Socket.Disconnect(true); }
                    ConnectionTerminated.Invoke(cSockState);
                }
            }
            catch (Exception ex)
            {

                //TODO:Log exceptions
            }
        }
    }
}
