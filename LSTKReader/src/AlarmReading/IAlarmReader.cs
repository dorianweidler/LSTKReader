using S22.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LSTKReader.AlarmReading
{
    interface IAlarmReader
    {
        Einsatz readAlarmData();
    }
}
