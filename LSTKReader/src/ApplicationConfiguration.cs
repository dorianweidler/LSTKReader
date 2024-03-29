﻿using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LSTKReader
{
    class ApplicationConfiguration
    {
        private static ApplicationConfiguration _instance = null;

        // general config
        private string loglevel;

        // email config
        private string emailHostname;
        private int emailPort;
        private string emailUsername;
        private string emailPassword;
        private bool emailUseSSL;
        private List<string> allowedSenders = new List<string>();

        // processing config
        private long alarmmailLifetime;
        private int waitTimeBeforeAlarmWithoutMail;
        private int sleepBetweenChecks;
        private string alarmProgramPath;
        private bool legacyProcessing;
        private string zipPassword;

        // extras config
        private string noAlarmmailDefaultAlarmtext;

        public string EmailHostname { get => emailHostname; set => emailHostname = value; }
        public int EmailPort { get => emailPort; set => emailPort = value; }
        public string EmailUsername { get => emailUsername; set => emailUsername = value; }
        public string EmailPassword { get => emailPassword; set => emailPassword = value; }
        public bool EmailUseSSL { get => emailUseSSL; set => emailUseSSL = value; }
        public long AlarmmailLifetime { get => alarmmailLifetime; set => alarmmailLifetime = value; }
        public int WaitTimeBeforeAlarmWithoutMail { get => waitTimeBeforeAlarmWithoutMail; set => waitTimeBeforeAlarmWithoutMail = value; }
        public int SleepBetweenChecks { get => sleepBetweenChecks; set => sleepBetweenChecks = value; }
        public string AlarmProgramPath { get => alarmProgramPath; set => alarmProgramPath = value; }
        public string NoAlarmmailDefaultAlarmtext { get => noAlarmmailDefaultAlarmtext; set => noAlarmmailDefaultAlarmtext = value; }
        public bool LegacyProcessing { get => legacyProcessing; set => legacyProcessing = value; }
        public string Loglevel { get => loglevel; set => loglevel = value; }
        public List<string> AllowedSenders { get => allowedSenders; set => allowedSenders = value; }
        public string ZipPassword { get => zipPassword; set => zipPassword = value; }

        public static ApplicationConfiguration getConfig()
        {
            if(_instance == null)
            {
                _instance = new ApplicationConfiguration("config.ini");
            }
            return _instance;
        }

        private ApplicationConfiguration(string fileName)
        {
            var iniParser = new FileIniDataParser();
            Console.WriteLine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + fileName);
            IniData data = iniParser.ReadFile(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + fileName);

            // general config
            loglevel = data["General"]["loglevel"];

            // email config
            emailHostname = data["Email"]["hostname"];
            emailPort = int.Parse(data["Email"]["port"]);
            emailUsername = data["Email"]["username"];
            emailPassword = data["Email"]["password"];
            emailUseSSL = bool.Parse(data["Email"]["useSSL"]);

            // processing config
            alarmmailLifetime = long.Parse(data["Processing"]["alarmmailLifetime"]);
            waitTimeBeforeAlarmWithoutMail = int.Parse(data["Processing"]["waitTimeBeforeAlarmWithoutMail"]);
            sleepBetweenChecks = int.Parse(data["Processing"]["sleepBetweenChecks"]);
            alarmProgramPath = data["Processing"]["alarmProgramPath"];
            legacyProcessing = bool.Parse(data["Processing"]["legacyProcessing"]);
            zipPassword = data["Processing"]["zipPassword"];

            // extras config
            noAlarmmailDefaultAlarmtext = data["Extras"]["noAlarmmailDefaultAlarmtext"];

            // allowed senders
            foreach (KeyData allowedSender in data["AllowedSenders"])
            {
                AllowedSenders.Add(allowedSender.Value);
            }

        }
    }
}
