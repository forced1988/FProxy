using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FProxy
{
    public enum LogSource
    {
        Info = 0,
        Auth =1,
        GameServer = 2,
        
    }
    public static class Log
    {
        public static void LogToConsole(string message, LogSource logSource = LogSource.Info)
        {
            string source = $"[{logSource.ToString()}]";
            //Set color
            switch (logSource)
            {
                case LogSource.Auth:
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogSource.GameServer:
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            Console.WriteLine($"{FormattedTimeStamp()}{source} - {message}");
       }

        private static string FormattedTimeStamp()
        {
            return $"[{DateTime.Now.ToString("HH:mm:ss")}]";
        }
    }
}
