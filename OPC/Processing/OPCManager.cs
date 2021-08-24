using Irisa.Common;
using System;
using System.Timers;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Irisa.Logger;
using Irisa.Message;
using System.Threading;

namespace OPC
{
    public sealed class OPCManager : IProcessing
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly UpdateScadaPointOnServer _updateScadaPointOnServer;

        public OPCManager(ILogger logger, IRepository repository, ICpsCommandService cpsCommandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, cpsCommandService);

            _logger.WriteEntry("", LogLevels.Info);
            _logger.WriteEntry("=================================================================", LogLevels.Info);
            _logger.WriteEntry(" DC.OPC is started . . . ", LogLevels.Info);
            _logger.WriteEntry("", LogLevels.Info);

        }
        public void RunClient()
        {
            try
            {
                _logger.WriteEntry("OPC Client is start", LogLevels.Debug);
                int stopTimeout = Timeout.Infinite;
                bool autoAccept = false;
                string endpointURL;

                endpointURL = "opc.tcp://"
                            + _repository.GetOPCConnectionParams().Connection.IP
                            +
                            ":"
                            + _repository.GetOPCConnectionParams().Connection.Port;

                OpcClient _opcClient = new OpcClient(endpointURL, autoAccept, stopTimeout);
                _opcClient.OPCDataChange += OnOPCDataChange;
                _opcClient.Run(_repository.GetTags());
             
                _logger.WriteEntry("OPC Client is Stoped", LogLevels.Debug);
            }
            catch(Exception ex)
            {
                _logger.WriteEntry("OPC Client Stop with Exception", LogLevels.Debug);

            }
        }

        public void ScadaPointReceived(ScadaPoint scadaPoint)
        {
            _logger.WriteEntry($"=================================================================", LogLevels.Info);
            _logger.WriteEntry($"SCADA Point \"{scadaPoint.Name}\" with value \"{scadaPoint.Value}\" received.", LogLevels.Info);
        }

        private void OnOPCDataChange(object sender, OPCDataEventArgs e)
        {
            //e.Items.ForEach(item => Console.WriteLine($"Name: {item.ShortName}, " +
            //                                          $"Value: {item.Value} " +
            //                                          //$"GUID: {_repository.GetMeasurementID(item.OPCTagName)}\t" +
            //                                          $"ServerTimestamp: {item.SourceTimestamp}, " +
            //                                          $"Status: {item.StatusCode}"));


            //TODO: check for GOOD status then write to SCADA...
            e.Items.ForEach(item =>
            {
                Console.WriteLine($"Name: {item.ShortName}, " +
                                                      $"Value: {item.Value} " +
                                                      //$"GUID: {_repository.GetMeasurementID(item.OPCTagName)}\t" +
                                                      $"ServerTimestamp: {item.SourceTimestamp}, " +
                                                      $"Status: {item.StatusCode}");

                WriteScadaPoint(_repository.GetMeasurementID(item.OPCTagName), item.Value);
            });
        }

        void WriteScadaPoint(Guid id, object value)
        {
            var scadaPoint = new ScadaPoint(id, Convert.ToSingle(value));
           // Console.WriteLine($"{scadaPoint.Id}\t {scadaPoint.Value}");
            
            if (!_updateScadaPointOnServer.WriteSCADAPoint(scadaPoint))
            {
                _logger.WriteEntry("Error in read a value from WS response!", LogLevels.Error);
            }
           // _logger.WriteEntry(scadaPoint.Id.ToString() + "  " + scadaPoint.Value.ToString(), LogLevels.Info);

        }
             
    }
}