using ClientSocket;
using FProxy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FProxy.Client
{
    public class LoginPlayer
    {
        Socket _clientSocket;
        WinsockClient _serverSocket;
        byte[] authServerBuffer;



        public LoginPlayer(CSockState clientSocket,string authIp,int authPort)
        {
            _clientSocket = clientSocket.Socket;

            authServerBuffer = new byte[1024];
            //Establish connection to actual auth server
            _serverSocket = new WinsockClient(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            _serverSocket.Enable(authIp, (ushort)authPort, authServerBuffer);
            _serverSocket.OnConnect += _serverSocket_OnConnect;
            _serverSocket.OnDisconnect += _serverSocket_OnDisconnect;
            _serverSocket.OnReceive += _serverSocket_OnReceive;

        }

        private void _serverSocket_OnReceive(WinsockClient Sender, byte[] Arg)
        {
            //Decrypt

        }

        private void _serverSocket_OnDisconnect(WinsockClient Sender, object Arg)
        {
            //Disconnect both sockets
            try
            {
                if (_serverSocket.Connected)
                    _serverSocket.Disconnect();
                if (_clientSocket.Connected)
                    _clientSocket.Disconnect(false);
            }
            catch { }
        }

        private void _serverSocket_OnConnect(WinsockClient Sender, object Arg)
        {
            Log.LogToConsole("Connected to auth server", LogSource.Auth);
        }
    }
}
