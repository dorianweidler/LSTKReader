using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTKReader.AlarmProcessing
{
    class LegacyAlarmProcess : IAlarmProcess
    {
        static readonly ApplicationConfiguration CONFIG = ApplicationConfiguration.getConfig();
        static readonly Logger logger = Logger.GetInstance();

        public void process(string alarmcode, Einsatz einsatz)
        {
            string alarmtext = "";
            if (einsatz == null)
            {
                // No Alarmmail found
                logger.info("No matching alarmmail found within " + CONFIG.WaitTimeBeforeAlarmWithoutMail + "s. Alarm without data!");
                alarmtext = CONFIG.NoAlarmmailDefaultAlarmtext;
            }
            else
            {
                alarmtext = string.Format("{0} {1} - {2} .:. Bemerkung: {3} .::. {4} {5} .::. {6} {7}",
                    einsatz.Einsatzstichwort, einsatz.Stichwortmemo, einsatz.Diagnose,
                    einsatz.Bemerkung,
                    einsatz.Strasse, einsatz.Hausnummer,
                    einsatz.Plz, einsatz.Ort);
                if (einsatz.Ort != einsatz.Ortsteil)
                {
                    alarmtext = string.Format("{0} - {1}", alarmtext, einsatz.Ortsteil);
                }
            }
            string args = string.Format("{0} | {1} | {1}", alarmcode, alarmtext);
            Process.Start("CMD.exe", "/C " + CONFIG.AlarmProgramPath + " \"" + args + "\"");
        }
    }
}
