using COMMON;
using Irisa.DataLayer;
using Irisa.DataLayer.Oracle;
using Irisa.Logger;
using Irisa.Common.Utils;
using Irisa.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DCIS
{
    public class Repository : IRepository
    {
        private readonly ILogger _logger;
        private readonly DataManager _staticDataManager;
        private readonly DataManager _historicalDataManager;
        //  private readonly OracleDataManager _oracleDataLayer;
        public readonly Dictionary<int, ArchivedPoint> _accPoints;
        private readonly List<ShiftTimeInfo> _shiftTimeList;
        private readonly List<int> _meterIdList;

        public Repository(ILogger logger, DataManager staticDataManager, DataManager historicalDataManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _staticDataManager = staticDataManager ?? throw new ArgumentNullException(nameof(staticDataManager));
            _historicalDataManager = historicalDataManager ?? throw new ArgumentNullException(nameof(historicalDataManager));
            _accPoints = new Dictionary<int, ArchivedPoint>();
            _shiftTimeList = new List<ShiftTimeInfo>();
            _meterIdList = new List<int>();
        }

        public bool Build()
        {
            var isBuild = true;

            try
            {
                PopulateMeterAndAccumulatorData();
                PopulateShiftTimeFromDatabase();

                if (_shiftTimeList == null || _shiftTimeList.Count == 0)
                    isBuild = false;
            }
            catch (Exception ex)
            {
                if (ex is Irisa.DataLayer.DataException)
                    _logger.WriteEntry(ex.ToString(), LogLevels.Error);
                else
                    _logger.WriteEntry(ex.Message, LogLevels.Error);
                isBuild = false;
            }

            return isBuild;
        }

        private void PopulateMeterAndAccumulatorData()
        {
            //var dataTable = _staticDataManager.GetRecord(
            //    "SELECT METER_PROXY, METER_NAME, ACCUMULATOR_MEASUREMENT_ID, ACCUMULATOR_NETWORK_PATH, " +
            //    "ANALOG_MEASUREMENT_ID, ANALOG_NETWORK_PATH FROM SCADA.APP_DCIS_PARAMS");
            //var dataTable = _staticDataManager.GetRecord(
            //    "SELECT METER_PROXY, METER_NAME, ACCUMULATOR_MEASUREMENT_ID, ACCUMULATOR_NETWORK_PATH, " +
            //    "ANALOG_MEASUREMENT_ID, ANALOG_NETWORK_PATH FROM APP.DCIS_PARAMS");

            var dataTable = _staticDataManager.GetRecord(
                "SELECT METER_PROXY, METER_NAME, ACCUMULATOR_NETWORK_PATH, " +
                "ANALOG_NETWORK_PATH FROM APP.DCIS_PARAMS");

            foreach (DataRow row in dataTable.Rows)
            {
                _accPoints.TryAdd(Convert.ToInt32(row["METER_PROXY"]),
                        new ArchivedPoint(
                             meterProxyId: Convert.ToInt32(row["METER_PROXY"]),
                             meterName: row["METER_NAME"].ToString(),
                              //accumulatorMeasurementId: Guid.Parse(row["ACCUMULATOR_MEASUREMENT_ID"].ToString()),
                              accumulatorMeasurementId: GetGuid(row["ACCUMULATOR_NETWORK_PATH"].ToString()),
                             accumulatorNetworkPath: row["ACCUMULATOR_NETWORK_PATH"].ToString(),
                             //analogMeasurementId: Guid.Parse(row["ANALOG_MEASUREMENT_ID"].ToString()),
                             analogMeasurementId: GetGuid(row["ANALOG_NETWORK_PATH"].ToString()),
                             analogNetworkPath: row["ANALOG_NETWORK_PATH"].ToString()));
                _meterIdList.Add(Convert.ToInt32(row["METER_PROXY"]));
            }

            _logger.WriteEntry("Meters populated successfully", LogLevels.Info);
        }

        private void PopulateShiftTimeFromDatabase()
        {
            var shiftDataTable = _staticDataManager.GetRecord("SELECT ShiftID, ShiftStartTime, ShiftEndTime, CalculationStartTime FROM APP.WS_CALCULATIONTIMEPERSHIFT");
            //var shiftDataTable = _staticDataManager.GetRecord("SELECT ShiftID, ShiftStartTime, ShiftEndTime, CalculationStartTime FROM SCADA.WS_CALCULATIONTIMEPERSHIFT");

            foreach (DataRow row in shiftDataTable.Rows)
            {
                var shiftWorkId = Convert.ToInt32(row["ShiftID"]);
                var shiftStartTime = TimeSpan.Parse(row["ShiftStartTime"].ToString());
                var shiftEndTime = TimeSpan.Parse(row["ShiftEndTime"].ToString());
                var calculationStartTime = TimeSpan.Parse(row["CalculationStartTime"].ToString());

                _shiftTimeList.Add(new ShiftTimeInfo(shiftWorkId, shiftStartTime, shiftEndTime, calculationStartTime));
            }

            _logger.WriteEntry("Shift time list populated successfully", LogLevels.Info);

        }

        //public bool TryGetArchiveFromExactDateTime(Guid analogMeasurementId, DateTime calclulationStartTime, out float value, out QualityCodes quality)
        //{
        //    try
        //    {
        //        var command = "SELECT VALUE, QUALITY FROM SCADAHIS.HISANALOGS WHERE MEASUREMENTID = :1 AND TIMESTAMP = :2";
        //        var parameters = new IDbDataParameter[]
        //        {
        //            _historicalDataManager.CreateParameter(":1", analogMeasurementId.ToString().ToUpper()),
        //            _historicalDataManager.CreateParameter(":2", calclulationStartTime.ToUniversalTime())
        //        };

        //        var archiveDataTable = _historicalDataManager.GetRecord(command, CommandType.Text, parameters);

        //        if (archiveDataTable.Rows.Count > 0)
        //        {
        //            value = Convert.ToSingle(archiveDataTable.Rows[0][0]);
        //            quality = (QualityCodes)Convert.ToInt32(archiveDataTable.Rows[0][1]);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.WriteEntry(ex is Irisa.DataLayer.DataException ? ex.ToString() : ex.Message, LogLevels.Error);
        //    }

        //    value = 0;
        //    quality = 0;
        //    return false;
        //}

        public bool TryGetArchiveFromExactDateTime(Guid analogMeasurementId, CalculationTimePerShift duration, Queue<SampleData> archData)
        {
            try
            {

                var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM HISANALOGS WHERE MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND TIMESTAMP >=  '{duration.ShiftStartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}' AND TIMESTAMP <=  '{duration.ShiftEndTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}' ORDER BY  TIMESTAMP ASC ";
                //var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM SCADAHIS.HISANALOGS WHERE MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND TIMESTAMP >=  timestamp'{duration.ShiftStartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}' AND TIMESTAMP <=  timestamp '{duration.ShiftEndTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}'";

                var archiveDataTable = _historicalDataManager.GetRecord(command);

                if (archiveDataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in archiveDataTable.Rows)
                    {
                        archData.Enqueue(new SampleData
                        {
                            value = Convert.ToSingle(row["VALUE"]),
                            qualityCode = (QualityCodes)Convert.ToInt16(row["QUALITY"]),
                            dateTime = Convert.ToDateTime(row["TIMESTAMP"])
                        });
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry(ex is Irisa.DataLayer.DataException ? ex.ToString() : ex.Message, LogLevels.Error);
            }
            return false;
        }

        public bool GetFirstData(Guid analogMeasurementId, CalculationTimePerShift duration, out float value)
        {
            try
            {
                //var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM SCADAHIS.HISANALOGS WHERE " +
                //              $"TIMESTAMP = (SELECT MAX(TIMESTAMP) FROM SCADAHIS.HISANALOGS  WHERE MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND TIMESTAMP <=  timestamp '{duration.ShiftStartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}') AND " +
                //              $"MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}'";

                var command = $"SELECT VALUE, TIMESTAMP, QUALITY FROM HISANALOGS WHERE " +
                              $"TIMESTAMP = (SELECT MAX(TIMESTAMP) FROM HISANALOGS  WHERE MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}' AND TIMESTAMP <=  '{duration.ShiftStartTime.ToString("yyyy-MM-dd HH:mm:ss.ff")}') AND " +
                              $"MEASUREMENTID = '{analogMeasurementId.ToString().ToUpper()}'";

                var archiveDataTable = _historicalDataManager.GetRecord(command);

                value = 0;
                if (archiveDataTable.Rows.Count > 0)
                {
                    DataRow row = archiveDataTable.Rows[0];
                    value = Convert.ToSingle(row["VALUE"]);
                    //_logger.WriteEntry($"First Data: time = {Convert.ToDateTime(row["TIMESTAMP"])} Value = {value}", LogLevels.Info);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                value = 0;
                _logger.WriteEntry(ex is Irisa.DataLayer.DataException ? ex.ToString() : ex.Message, LogLevels.Error);
                return false;
            }
        }

        public ArchivedPoint? GetHisPoint(int metereId)
        {
            _accPoints.TryGetValue(metereId, out var accPoint);
            if (accPoint != null)
                return accPoint;
            else
                return null;
        }

        public IEnumerable<ShiftTimeInfo> GetShiftTimeInfoList()
        {
            return _shiftTimeList;
        }
        public IEnumerable<int> GetMeterIdList()
        {

            return _meterIdList;
        }

        public Guid GetGuid(String networkpath)
        {
            
            string sql = "SELECT * FROM NodesFullPath where FullPath = '" + networkpath + "'";

            try
            {
                var dataTable = _staticDataManager.GetRecord(sql);
                Guid id = Guid.Empty;
                if (dataTable != null && dataTable.Rows.Count == 1)
                {
                    //foreach (DataRow row in dataTable.Rows)
                    //{
                        id = Guid.Parse(dataTable.Rows[0]["GUID"].ToString());
                     //   id = Guid.Parse(row["GUID"].ToString());
                    //}
                    return id;
                }
                else if (dataTable.Rows.Count > 1)
                {
                    _logger.WriteEntry("Error More Guid found for " + networkpath, LogLevels.Error);
                    return Guid.Empty;
                }
                else
                {
                    _logger.WriteEntry("Error in loading Guid for " + networkpath, LogLevels.Error);
                    return Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteEntry("Error in loading Guid for " + networkpath, LogLevels.Error, ex);
                return Guid.Empty;
            }
        }
    }
}