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

            Einsatz einsatz = null;
            DateTime endTime = DateTime.Now.AddSeconds(CONFIG.WaitTimeBeforeAlarmWithoutMail);

            do
            {
                einsatz = readEmail();
                if(einsatz == null)
                {
                    Logger.info("Found no Einsatz. Checking again in " + CONFIG.SleepBetweenChecks + "ms.");
                    Thread.Sleep(CONFIG.SleepBetweenChecks);
                }
            } while (DateTime.Compare(DateTime.Now, endTime) <= 0 && einsatz == null);

            // process
            if(CONFIG.LegacyProcessing)
            {
                processEinsatzLegacy(alarmcode, einsatz);
            } else
            {

            }

        }

        private static void processEinsatzLegacy(string alarmcode, Einsatz einsatz)
        {
            string alarmtext = "";
            if (einsatz == null)
            {
                // No Alarmmail found
                Logger.info("No matching alarmmail found within " + CONFIG.WaitTimeBeforeAlarmWithoutMail + "s. Alarm without data!");
                alarmtext = CONFIG.NoAlarmmailDefaultAlarmtext;
            } else
            {
                alarmtext = string.Format("{0} {1} - {2} .:. Bemerkung: {3} .::. {4} {5} .::. {6} {7}", 
                    einsatz.Einsatzstichwort, einsatz.Stichwortmemo, einsatz.Diagnose,
                    einsatz.Bemerkung,
                    einsatz.Strasse, einsatz.Hausnummer,
                    einsatz.Plz, einsatz.Ort);
                if(einsatz.Ort != einsatz.Ortsteil)
                {
                    alarmtext = string.Format("{0} - {1}", alarmtext, einsatz.Ortsteil);
                }
            }
            string args = string.Format("{0} | {1} | {1}", alarmcode, alarmtext);
            Process.Start("CMD.exe", "/C " + CONFIG.AlarmProgramPath + " \"" + args + "\"");
        }

        private static Einsatz readEmail()
        {
            // Connect and login
            using (ImapClient Client = new ImapClient(CONFIG.EmailHostname, CONFIG.EmailPort, CONFIG.EmailUsername, CONFIG.EmailPassword, AuthMethod.Login, CONFIG.EmailUseSSL))
            {
                Logger.info("Retreiving emails.");
                // Get all messages
                IEnumerable<uint> uids = Client.Search(SearchCondition.Undeleted());
                Logger.debug("Found " + uids.Count() + " possible matches.");

                IList<Einsatz> einsaetze = new List<Einsatz>();

                foreach (uint uid in uids)
                {
                    MailMessage message = Client.GetMessage(uid);
                    DateTime? messageDateTime = message.Date();
                    // Check if message is valid
                    if (message.Subject == "ELR-Mail" && messageDateTime.HasValue && DateTime.Compare(messageDateTime.Value.AddSeconds(CONFIG.AlarmmailLifetime), DateTime.Now) >= 0)
                    {
                        IEnumerable<Attachment> attachments = message.Attachments;
                        // iterate through attachments
                        foreach (Attachment attachment in attachments)
                        {
                            // check for valid attachment
                            if (attachment.ContentType.MediaType == "text/plain" && attachment.ContentType.Name.ToString().StartsWith("Alarmdruck"))
                            {
                                // Generate einsatz from document
                                XDocument xdocument = XDocument.Load(attachment.ContentStream);
                                Einsatz einsatz = Einsatz.FromLeitstellenXml(xdocument);
                                einsatz.EmpfangsZeitpunkt = messageDateTime.Value;
                                einsaetze.Add(einsatz);
                            }
                        }
                    } else
                    {
                        Client.DeleteMessage(uid);
                    }
                }

                try
                {
                    Einsatz currentEinsatz = einsaetze.OrderByDescending(e => e.EmpfangsZeitpunkt).First();
                    Logger.info("Aktuellster Einsatz: " + currentEinsatz);
                    return currentEinsatz;
                } catch(InvalidOperationException ioex)
                {
                    return null;
                }

            }
        }
    }
}
