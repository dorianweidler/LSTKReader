using Ionic.Zip;
using S22.Imap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LSTKReader.AlarmReading
{
    class EmailAlarmReader : IAlarmReader
    {
        static readonly ApplicationConfiguration CONFIG = ApplicationConfiguration.getConfig();
        static readonly Logger logger = Logger.GetInstance();

        Einsatz IAlarmReader.readAlarmData()
        {
            Einsatz einsatz = null;
            DateTime endTime = DateTime.Now.AddSeconds(CONFIG.WaitTimeBeforeAlarmWithoutMail);

            do
            {
                einsatz = readMails();
                if (einsatz == null)
                {
                    logger.info("Found no Einsatz. Checking again in " + CONFIG.SleepBetweenChecks + "ms.");
                    Thread.Sleep(CONFIG.SleepBetweenChecks);
                }
            } while (DateTime.Compare(DateTime.Now, endTime) <= 0 && einsatz == null);

            return einsatz;
        }

        private Einsatz getEinsatzFromStream(Stream stream, DateTime empfangsZeit)
        {
            string einsatzXml = repairXml(new StreamReader(stream).ReadToEnd());
            XDocument xdocument = XDocument.Parse(einsatzXml);
            Einsatz einsatz = Einsatz.FromLeitstellenXml(xdocument);
            einsatz.EmpfangsZeitpunkt = empfangsZeit;
            return einsatz;
        }

        public static string repairXml(string xml)
        {
            string[] lines = xml.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string newXml = "";
            foreach(string line in lines)
            {
                string newLine = line;
                if(!String.IsNullOrWhiteSpace(line))
                {
                    string found = Regex.Match(line, "value=\".*\"").Groups[0].Value;
                    if(!String.IsNullOrWhiteSpace(found))
                    {
                        string replaced = found.Replace(">", "&gt;").Replace("<", "&lt;");
                        newLine = line.Replace(found, replaced);
                    }
                }
                newXml += newLine;
            }
            return newXml;
        }

        private Einsatz readMails()
        {
            // Connect and login
            using (ImapClient Client = new ImapClient(CONFIG.EmailHostname, CONFIG.EmailPort, CONFIG.EmailUsername, CONFIG.EmailPassword, AuthMethod.Login, CONFIG.EmailUseSSL))
            {
                logger.info("Retreiving emails.");
                // Get all messages
                IEnumerable<uint> uids = Client.Search(SearchCondition.Undeleted());
                logger.debug("Found " + uids.Count() + " possible matches.");

                IList<Einsatz> einsaetze = new List<Einsatz>();

                foreach (uint uid in uids)
                {
                    MailMessage message = Client.GetMessage(uid);
                    DateTime? messageDateTime = message.Date();
                    // Check if message is valid
                    if (CONFIG.AllowedSenders.Contains(message.From.Address) && message.Attachments.Count > 0 && messageDateTime.HasValue && DateTime.Compare(messageDateTime.Value.AddSeconds(CONFIG.AlarmmailLifetime), DateTime.Now) >= 0)
                    {
                        IEnumerable<Attachment> attachments = message.Attachments;
                        // iterate through attachments
                        foreach (Attachment attachment in attachments)
                        {
                            // check for valid attachment
                            if (attachment.ContentType.MediaType == "text/plain" && attachment.ContentType.Name.ToString().StartsWith("Alarmdruck"))
                            {
                                // Generate einsatz from document
                                einsaetze.Add(getEinsatzFromStream(attachment.ContentStream, messageDateTime.Value));
                            }

                            else if(attachment.ContentType.MediaType == "application/zip")
                            {
                                // encrypted zip!
                                ZipFile zip = ZipFile.Read(attachment.ContentStream);
                                foreach (ZipEntry entry in zip)
                                {
                                    if(entry.FileName.StartsWith("Alarmdruck"))
                                    {
                                        MemoryStream ms = new MemoryStream();
                                        entry.ExtractWithPassword(ms, CONFIG.ZipPassword);
                                        ms.Seek(0, SeekOrigin.Begin);
                                        einsaetze.Add(getEinsatzFromStream(ms, messageDateTime.Value));
                                    }
                                }
                            }

                            
                        }
                    }
                    else
                    {
                        Client.DeleteMessage(uid);
                    }
                }

                try
                {
                    Einsatz currentEinsatz = einsaetze.OrderByDescending(e => e.EmpfangsZeitpunkt).First();
                    logger.info("Aktuellster Einsatz: " + currentEinsatz);
                    return currentEinsatz;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }

            }
        }
    }
}
