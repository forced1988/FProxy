using FProxy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.LogToConsole("FProxy starting");

            CSocket authSocket = new CSocket("95.172.92.14", 9958, 9958);
            authSocket.ConnectionReceived += AuthSocket_NewConquerClientConnection;
            authSocket.DataReceived += AuthSocket_DataReceived;
            authSocket.ConnectionTerminated += AuthSocket_ConnectionTerminated;
            Log.LogToConsole($"Accepting incoming requests on port 9958");


            Console.ReadLine();
        }

        #region Auth handling

        private static void AuthSocket_ConnectionTerminated(CSockState obj)
        {
           
        }

        private static void AuthSocket_DataReceived(byte[] buffer, CSockState cSockState, byte[] question)
        {
            Log.LogToConsole($"Received data from client : {System.Text.Encoding.ASCII.GetString(buffer)}", LogSource.Auth);
        }

        private static void AuthSocket_NewConquerClientConnection(CSockState obj)
        {
            Log.LogToConsole("Client connected to Proxy", LogSource.Auth);
        }

        #endregion
    }

}
