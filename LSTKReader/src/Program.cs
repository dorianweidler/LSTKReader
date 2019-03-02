using System;
using LSTKReader.AlarmReading;
using LSTKReader.AlarmProcessing;

namespace LSTKReader
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Logger errorLogger = Logger.GetInstance();
                errorLogger.error("Wrong usage. Usage: LSTKReader.exe {Alarmcode}|{Unit}|");
                return;
            }

            // process args, extract alarmcode
            string alarmcode = args[0].Split('|')[0];
            Logger logger = Logger.GetInstance(alarmcode);
            ApplicationConfiguration CONFIG = ApplicationConfiguration.getConfig();

            IAlarmReader alarmReader = new EmailAlarmReader();
            Einsatz einsatz = alarmReader.readAlarmData();

            Console.WriteLine(einsatz);

            IAlarmProcess alarmProcess = new AlarmProcessFactory().GetAlarmProcess();
            alarmProcess.process(alarmcode, einsatz);
        }  
    }
}
