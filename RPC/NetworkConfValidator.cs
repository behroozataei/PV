using System;
using System.Collections.Generic;
using System.Text;

using Irisa.Logger;
using Irisa.Message.CPS;

namespace RPC
{
    internal class NetworkConfValidator
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

        private RPCScadaPoint _MAB;
        private RPCScadaPoint _MAC_B;
        private RPCScadaPoint _MBD_B;
        private RPCScadaPoint _RPCAlarm;


        internal NetworkConfValidator(IRepository repository, ILogger logger, UpdateScadaPointOnServer updateScadaPointOnServer)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateScadaPointOnServer = updateScadaPointOnServer ?? throw new ArgumentNullException(nameof(updateScadaPointOnServer));
        }


        public bool isAdmittedNetConf()
        {
            bool result = false;
            try
            {
                _MAB = _repository.GetRPCScadaPoint("MAB");
                _MAC_B = _repository.GetRPCScadaPoint("MAC_B");
                _MBD_B = _repository.GetRPCScadaPoint("MBD_B");
                _RPCAlarm = _repository.GetRPCScadaPoint("RPCAlarm");

                if ((_MAB.Value == (float)DigitalSingleStatus.Open && ((_MAC_B.Value == (float)DigitalDoubleStatus.Open && _MBD_B.Value == (float)DigitalDoubleStatus.Close) ||
                  (_MAC_B.Value == (float)DigitalDoubleStatus.Close && _MBD_B.Value == (float)DigitalDoubleStatus.Open))) ||
                  (_MAC_B.Value == (float)DigitalDoubleStatus.Open && _MBD_B.Value == (float)DigitalDoubleStatus.Open))
                {
                    result = true;
                }
                else
                {
                    result = false;
                    if (!_updateScadaPointOnServer.SendAlarm(_RPCAlarm, SinglePointStatus.Appear, "Network Configuration is Not Admitted"))
                    {
                        _logger.WriteEntry("Sending alarm \" Network Configuration is Not Admitted\" failed.", LogLevels.Error);
                    }
                }
            }
            catch (System.Exception excep)
            {

                _logger.WriteEntry(excep.Message, LogLevels.Error);
                result = false;
            }

            return result;
        }
    }
}
