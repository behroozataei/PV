using Irisa.Logger;
using Irisa.Message;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DCIS
{
    internal class DCISManager:IProcessing
    {
        private const int TIMER_TICKS =90000;
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly Timer _timer_15_Min;
        private UpdateScadaPointOnServer _updateScadaPointOnServer;

        private CalculationTimePerShift shiftTime;
        private DateTime CalcTime;
        private bool Executed;

        internal DCISManager(ILogger logger, IRepository repository, ICpsCommandService commandService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));


            _timer_15_Min = new Timer();
            _timer_15_Min.Interval = TIMER_TICKS;
            _timer_15_Min.Elapsed += RunCyclicOperation;

            _updateScadaPointOnServer = new UpdateScadaPointOnServer(_logger, commandService);
            CalcTime = new DateTime();
        }

        public void StartCyclicOperation()
        {
           
            _timer_15_Min.Start();

        }

        private void RunCyclicOperation(object sender, ElapsedEventArgs e)
        {
            shiftTime = DetectShift(DateTime.Now);
            
            if (DateTime.UtcNow.Hour == shiftTime.ShiftEndTime.Hour && Executed == true)
            {
                Executed = false;

            }
            if (DateTime.UtcNow >= shiftTime.CalculationStartTime && Executed == false)
            {

                Queue<SampleData> _hisValuesInIntervalTime = new Queue<SampleData>();


                //shiftTime.ShiftStartTime = DateTime.UtcNow.AddSeconds(-120);
                //shiftTime.ShiftEndTime = DateTime.UtcNow.AddSeconds(-60);
                GenReport genReport = new GenReport();


                foreach (var pointId in _repository.GetMeterIdList())
                {
                    _hisValuesInIntervalTime.Clear();
                    _hisValuesInIntervalTime.Enqueue(GetFirstSampleData(pointId, shiftTime));
                    if (!GetValuesInIntervalTime(pointId, shiftTime, _hisValuesInIntervalTime))
                    {
                       // _logger.WriteEntry($"No Sample Found in HIS in Interval Time: {shiftTime.ShiftStartTime.ToLocalTime()}  Until {shiftTime.ShiftEndTime.ToLocalTime()} for {_repository.GetHisPoint(pointId).AnalogNetworkPath} ", LogLevels.Warn);
                        _hisValuesInIntervalTime.Enqueue(new SampleData { dateTime = shiftTime.ShiftEndTime, value = _hisValuesInIntervalTime.First().value, qualityCode = 0 });

                    }
                    else
                    {
                        var LastValue = _hisValuesInIntervalTime.Last();
                        LastValue.dateTime = shiftTime.ShiftEndTime;
                        _hisValuesInIntervalTime.Enqueue(LastValue);
                    }
                    //foreach (var value in _hisValuesInIntervalTime)
                    //    _logger.WriteEntry($"{value.dateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.ff")} , {value.value}", LogLevels.Info);
                    var total_Value = Totalizer(_hisValuesInIntervalTime);
                    _logger.WriteEntry($"{_repository.GetHisPoint(pointId).AccumulatorNetworkPath} : {shiftTime.ShiftEndTime.ToLocalTime()}  ,  {total_Value}", LogLevels.Info);
                    _updateScadaPointOnServer.WriteSCADAPoint(_repository.GetHisPoint(pointId), total_Value);
                    genReport.AddConsumed(_repository.GetHisPoint(pointId).AccumulatorNetworkPath, total_Value) ;
                }
                genReport.save();

                Executed = true;
                CalcTime = DateTime.Now;

            }
            else
            {
                _logger.WriteEntry($"Calculation of Totalizer has been done at {CalcTime}", LogLevels.Info);

            }

        }

        private float Totalizer(Queue<SampleData> pointValues)
        {
            var preSampled = pointValues.Dequeue();
            float sumvalue = 0;
            while(pointValues.Count > 0)
            {
                var NextSampled = pointValues.Dequeue();
                var deltatime = NextSampled.dateTime - preSampled.dateTime;
                sumvalue += preSampled.value * (float)deltatime.TotalMilliseconds / 3600000.0f;
                preSampled = NextSampled;
            }
            return sumvalue;

        }

        private bool GetValuesInIntervalTime(int pointId, CalculationTimePerShift shiftTime, Queue<SampleData> _pointValues)
        {
            var Point = _repository.GetHisPoint(pointId);
            if (_repository.TryGetArchiveFromExactDateTime(Point.AnalogMeasurementId, shiftTime, _pointValues))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private SampleData GetFirstSampleData(int pointId, CalculationTimePerShift shiftTime)
        {
            var Point = _repository.GetHisPoint(pointId);
            //List<PointValues> framevalues= new List<PointValues>();
            //CalculationTimePerShift timeFrame = new CalculationTimePerShift(shiftTime.ShiftWorkId, shiftTime.ShiftStartTime.AddMinutes(-10), shiftTime.ShiftStartTime, shiftTime.CalculationStartTime);
            //int count = 0;
            float V1 = 0;
            if(_repository.GetFirstData(Point.AnalogMeasurementId, shiftTime, out V1))
            {
                ;
            }
            //while (count < 10)
            //{
            //    framevalues.Clear();

            //    if (_repository.TryGetArchiveFromExactDateTime(Point.AnalogMeasurementId, timeFrame, framevalues))
            //    {
            //        var firstValue = framevalues.Last();
            //        firstValue.dateTime = shiftTime.ShiftStartTime;
            //    }

            //    else
            //    {
            //        timeFrame.ShiftStartTime = timeFrame.ShiftStartTime.AddMinutes(-10);
            //        timeFrame.ShiftEndTime = timeFrame.ShiftEndTime.AddMinutes(-10);
            //        count++;
            //    }
            //}
            SampleData FirtSample = new SampleData();
            FirtSample.dateTime = shiftTime.ShiftStartTime;
            FirtSample.value = V1;
            return FirtSample;
        }



        private CalculationTimePerShift DetectShift(DateTime dateTime)
        {
            CalculationTimePerShift currentShift = null;

            var timeOfDay = dateTime.TimeOfDay;
            var shiftList = _repository.GetShiftTimeInfoList().ToList();
            var shift1 = shiftList.Find(s => s.ShiftWorkId == 1);
            var shift2 = shiftList.Find(s => s.ShiftWorkId == 2);

            if (shift1 is null || shift2 is null)
                throw new InvalidOperationException("Incorrect Shift Time list info");


            if (timeOfDay >= shift2.ShiftStartTime && timeOfDay <= shift2.ShiftEndTime)
            {
                //Shift1 must be calculated
                var startShift = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, shift1.ShiftStartTime.Hours, shift1.ShiftStartTime.Minutes, 0).AddDays(-1).ToUniversalTime();
                var endShift = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, shift1.ShiftEndTime.Hours, shift1.ShiftEndTime.Minutes, 0).ToUniversalTime();
                var calcuationTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, shift1.CalculationStartTime.Hours, shift1.CalculationStartTime.Minutes, 0).ToUniversalTime();
                currentShift = new CalculationTimePerShift(shift1.ShiftWorkId, startShift, endShift, calcuationTime);
            }
            else
            {
                //Shift2 must be calculated
                var startShift = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, shift2.ShiftStartTime.Hours, shift2.ShiftStartTime.Minutes, 0).ToUniversalTime();
                var endShift = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, shift2.ShiftEndTime.Hours, shift2.ShiftEndTime.Minutes, 0).ToUniversalTime();
                var calcuationTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, shift2.CalculationStartTime.Hours, shift2.CalculationStartTime.Minutes, 0).ToUniversalTime();

                if (timeOfDay < shift2.ShiftStartTime)
                {
                    startShift = startShift.AddDays(-1).ToUniversalTime();
                    endShift = endShift.AddDays(-1).ToUniversalTime();
                    calcuationTime = calcuationTime.AddDays(-1).ToUniversalTime();
                }

                currentShift = new CalculationTimePerShift(shift1.ShiftWorkId, startShift, endShift, calcuationTime);
            }

            return currentShift;
        }

        public void SCADAEventRaised(ScadaPoint scadaPoint)
        {
            throw new NotImplementedException();
        }
    }
}
