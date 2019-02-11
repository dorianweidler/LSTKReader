using System;
using System.Collections.Generic;
using System.Net.Mail;
using S22.Imap;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using LSTKReader.AlarmReading;
using LSTKReader.AlarmProcessing;

namespace LSTKReader
{
    class Program
    {
        static readonly ApplicationConfiguration CONFIG = ApplicationConfiguration.getConfig();

        static void Main(string[] args)
        {

            if(args.Length != 1)
            {
                Logger.error("Wrong usage. Usage: LSTKReader.exe {Alarmcode}|{Unit}|");
                return;
            }

            // process args, extract alarmcode
            string alarmcode = args[0].Split('|')[0];

            IAlarmReader alarmReader = new EmailAlarmReader();
            Einsatz einsatz = alarmReader.readAlarmData();

            Console.WriteLine(einsatz);

            IAlarmProcess alarmProcess = new AlarmProcessFactory().GetAlarmProcess();
            //alarmProcess.process(alarmcode, einsatz);

            Console.ReadKey();
        }  
    }
}
