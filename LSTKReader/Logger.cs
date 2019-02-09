using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTKReader
{
    class Logger
    {
        public static void error(string message)
        {
            log(LogType.ERROR, message);
        }

        public static void info(string message)
        {
            log(LogType.INFO, message);
        }

        public static void debug(string message)
        {
            log(LogType.DEBUG, message);
        }

        public static void warning(string message)
        {
            log(LogType.WARNING, message);
        }

        private static void log(LogType logType, string message)
        {
            Console.Write("{0} [", DateTime.Now);
            writeLogType(logType);
            Console.WriteLine("] {0}", message);
        }

        private static void writeLogType(LogType logType)
        {
            switch(logType)
            {
                case LogType.INFO:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.WARNING:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
            Console.Write(logType.ToString());
            Console.ResetColor();
        }
        private enum LogType
        {
            INFO, ERROR, DEBUG, WARNING
        }
    }

}
