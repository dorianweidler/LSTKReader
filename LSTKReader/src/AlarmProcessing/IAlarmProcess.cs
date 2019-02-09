using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTKReader.AlarmProcessing
{
    interface IAlarmProcess 
    {

        void process(string alarmcode, Einsatz einsatz);

    }
}
