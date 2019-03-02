using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTKReader
{
    class Logger
    {
        static readonly ApplicationConfiguration CONFIG = ApplicationConfiguration.getConfig();
        private String fileName;
        private static Logger instance = null;

        private Logger(string fileName)
        {
            this.fileName = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + "_log.txt";
        }

        public static Logger GetInstance()
        {
            if(instance == null)
            {
                instance = new Logger(DateTime.UtcNow + "_log.txt");
            }
            return instance;
        }

        public static Logger GetInstance(string alarmCode)
        {
            if (instance == null)
            {
                instance = new Logger(DateTime.UtcNow + "_" + alarmCode + "_log.txt");
            }
            return instance;
        }

        public void error(string message)
        {
            string[] allowedLevels = { "debug", "info", "warning", "error" };
            if (allowedLevels.Contains(CONFIG.Loglevel))
            {
                log(LogType.ERROR, message);
            }
        }

        public void warning(string message)
        {
            string[] allowedLevels = { "debug", "info", "warning" };
            if (allowedLevels.Contains(CONFIG.Loglevel))
            {
                log(LogType.WARNING, message);
            }
        }

        public void info(string message)
        {
            string[] allowedLevels = { "debug", "info" };            
            if (allowedLevels.Contains(CONFIG.Loglevel))
            {
                log(LogType.INFO, message);
            }
        }

        public void debug(string message)
        {
            string[] allowedLevels = { "debug" };
            if (allowedLevels.Contains(CONFIG.Loglevel))
            {
                log(LogType.DEBUG, message);
            }
        }

        private void log(LogType logType, string message)
        {
            Console.Write("{0} [", DateTime.Now);
            writeLogType(logType);
            Console.WriteLine("] {0}", message);
        }

        private void writeLogType(LogType logType)
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
