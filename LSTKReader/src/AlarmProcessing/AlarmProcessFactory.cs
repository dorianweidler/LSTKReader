using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTKReader.AlarmProcessing
{
    class AlarmProcessFactory
    {
        readonly ApplicationConfiguration CONFIG = ApplicationConfiguration.getConfig();

        public IAlarmProcess GetAlarmProcess()
        {
            if(CONFIG.LegacyProcessing)
            {
                return new LegacyAlarmProcess();
            } else
            {
                throw new NotImplementedException();
            }
        }

    }
}
