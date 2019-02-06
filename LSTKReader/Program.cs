using System;
using System.Collections.Generic;
using System.Net.Mail;
using S22.Imap;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;


namespace LSTKReader
{
    class Program
    {
        const double VALID_TIME_IN_MINUTES = 120;

        static void Main(string[] args)
        {
            // Connect and login
            using (ImapClient Client = new ImapClient("sslin.df.eu", 993, "testalarm@feuerwehr-kusel.de", "bEIg8LT7", AuthMethod.Login, true))
            {
                DateTime now = DateTime.Now.AddMinutes(VALID_TIME_IN_MINUTES);
                // Get all messages
                IEnumerable<uint> uids = Client.Search(SearchCondition.Undeleted());
                foreach (uint uid in uids)
                {
                    MailMessage message = Client.GetMessage(uid);
                    DateTime? messageDateTime = message.Date();
                    // Check if message is valid
                    if (message.Subject == "ELR-Mail" && messageDateTime.HasValue && DateTime.Compare(messageDateTime.Value, now) <= 0)
                    {
                        IEnumerable<Attachment> attachments = message.Attachments;
                        // iterate through attachments
                        foreach (Attachment attachment in attachments)
                        {
                            // check for valid attachment
                            if (attachment.ContentType.MediaType == "text/plain" && attachment.ContentType.Name.ToString().StartsWith("Alarmdruck"))
                            {

                                XDocument xdocument = XDocument.Load(attachment.ContentStream);
                                Einsatz einsatz = Einsatz.FromLeitstellenXml(xdocument);
                                Console.WriteLine(einsatz);
                            }
                        }
                    }
                    Client.DeleteMessage(uid);
                }
            }
        }
    }
}
