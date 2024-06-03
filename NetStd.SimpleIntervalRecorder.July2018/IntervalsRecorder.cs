using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable UnusedMember.Local

namespace NetStd.SimpleIntervalRecorder.July2018
{
    /// <summary>
    ///     Records and saves a sequence of intervals and optional pauses.
    ///     The .NET version uses System.Diagnostics.Stopwatch class as its internal clock. Silverlight uses DateTime class.
    ///     Based on empirical research, system noise is such that neither this nor any other timer can
    ///     perform quality benchmarking using test periods less than 30 seconds. Increase the number of
    ///     iterations of any operation being benchmarked so as to satisfy this requirement. For three-sig fig
    ///     repeatability of results, follow a test protocol that repeats thirty times.
    ///     To measure the once-off performance of this timer over 30 seconds, get the ReportedAccuracy property.
    /// </summary>
    public class IntervalsRecorder : IIntervalsRecorder
    {
        #region ctors

        /// <summary>
        ///     Constructor
        /// </summary>
        public IntervalsRecorder()
        {
            ZeroiseData();

            _nameOfTimer = "Simple interval recorder";

            _internalClock = new InternalClockV2();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name">Optional name for the recorder</param>
        public IntervalsRecorder(string name)
        {
            ZeroiseData();

            _nameOfTimer = name ?? "Simple interval recorder";

            _internalClock = new InternalClockV1();
        }

        #endregion

        #region constants - decimal and timespan formats

        private const string DecimalFormat0Dp = "n0";
        private const string DecimalFormat1Dp = "n1";
        private const string DecimalFormat2Dp = "n2";
        private const string TimeSpanGeneralShortFormat = "g";
        private const string TimeSpanGeneralLongFormat = "G";
        private const string TimeSpanInvariantFormat = "c";
        private const string TimeSpanSecondsFormat1Dp = "h\\:mm\\:ss\\.f";
        private const string TimeSpanSecondsFormat0Dp = "h\\:mm\\:ss";

        #endregion

        #region fields

        private readonly string _nameOfTimer;

        private readonly IInternalClock _internalClock;

        private List<Interval> _listOfCompletedIntervals;

        private List<PausedPeriod> _listOfCompletedPauses;

        private PausedPeriod _openOngoingPause;

        private Interval _openOngoingInterval;

        private long CumulativeTotalIntervalsTicks
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return 0;

                var totalIntervalsTicks = _listOfCompletedIntervals.Where(z => z != null)
                    .Sum(z => z.DurationTicksExcludingPauses);

                return totalIntervalsTicks;
            }
        }

        private long CumulativeTotalPausesTicks
        {
            get
            {
                if (_listOfCompletedPauses == null)
                    return 0;

                if (_listOfCompletedPauses.Count == 0)
                    return 0;

                var totalPausesTicks = _listOfCompletedPauses.Where(z => z != null)
                    .Sum(pause => pause.EndMinusBeginTimestampTicks);

                return totalPausesTicks;
            }
        }

        #endregion

        #region props

        public string Name => _nameOfTimer ?? string.Empty;

        public Interval MostRecentlyCompletedInterval
        {
            get
            {
                if (_listOfCompletedIntervals == null || _listOfCompletedIntervals.Count == 0)
                    return new Interval(_internalClock.NanosecPerTick);

                return _listOfCompletedIntervals.Last();
            }
        }

        public List<Interval> CompletedIntervalsToDate
        {
            get
            {
                if (_listOfCompletedIntervals == null || _listOfCompletedIntervals.Count == 0)
                    return [];

                return _listOfCompletedIntervals;
            }
        }

        public double CumulativeTotalIntervalsMillisec => ConvertTicksToMillisec(CumulativeTotalIntervalsTicks);

        public double AverageIntervalMillisec
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return 0;

                return _listOfCompletedIntervals.Where(z => z != null).Average(z => z.DurationMillisecExcludingPauses);
            }
        }

        public double LongestIntervalMillisec
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return 0;

                return LongestInterval.DurationMillisecExcludingPauses;
            }
        }

        public Interval LongestInterval
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return new Interval {Index = 0};

                return _listOfCompletedIntervals.OrderByDescending(z => z.DurationMillisecExcludingPauses).First();
            }
        }

        public Interval ShortestInterval
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return new Interval {Index = 0};

                return _listOfCompletedIntervals.OrderBy(z => z.DurationMillisecExcludingPauses).First();
            }
        }

        public List<Interval> ShortestIntervalsTop100
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return [];

                return _listOfCompletedIntervals.OrderBy(z => z.DurationMillisecExcludingPauses).Take(100).ToList();
            }
        }

        public double ShortestIntervalMillisec
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return 0;

                return ShortestInterval.DurationMillisecExcludingPauses;
            }
        }

        public double MedianIntervalMillisec
        {
            get
            {
                if (_listOfCompletedIntervals == null || !_listOfCompletedIntervals.Any())
                    return 0;

                var sorted = _listOfCompletedIntervals.Where(z => z != null)
                    .OrderBy(z => z.DurationMillisecExcludingPauses);

                var midpoint = Convert.ToInt32(_listOfCompletedIntervals.Count / 2.0);

                var medianInterval = sorted.ElementAt(midpoint);

                return medianInterval.DurationMillisecExcludingPauses;
            }
        }

        public double CumulativeTotalPausesMillisec => ConvertTicksToMillisec(CumulativeTotalPausesTicks);

        public string SummaryOfResults => string.IsNullOrWhiteSpace(ComposeNicelyFormattedSummaryOfResults())
            ? string.Empty
            : ComposeNicelyFormattedSummaryOfResults();

        /// <summary>
        ///     Measured immediately for a manufactured interval of approximately twenty seconds
        /// </summary>
        public string ReportedAccuracy
        {
            get
            {
                var testTimer = new IntervalsRecorder("timer test");

                const int testPeriodThreadSleep = 20000;

                testTimer.BeginInterval("");

                for (long i = 0; i < Convert.ToInt64(21 * 10e6); i++)
                {
#pragma warning disable 219
                    var dummy = "a" + "b" + "c" + "d" + "e" + "f";
#pragma warning restore 219
                    // ReSharper disable once RedundantAssignment
                    dummy = null;
                }
                // arbitrary - approx 30 seconds on Intel Core i5


                testTimer.EndInterval();

                var sb = new StringBuilder();

                if (_internalClock == null)
                {
                    sb.Append("Internal clock is null");
                }
                else
                {
                    var actual = testPeriodThreadSleep.ToString("n0");
                    var measured =
                        testTimer.MostRecentlyCompletedInterval.DurationMillisecExcludingPauses.ToString("n0");

                    var error =
                        Math.Abs(testPeriodThreadSleep -
                                 testTimer.MostRecentlyCompletedInterval.DurationMillisecExcludingPauses);

                    var errorpercent = (100 * error / testPeriodThreadSleep).ToString("n2");
                    sb.AppendLine(
                        $"Test interval using Thread.Sleep: {actual}ms. Measured result: {measured}. Error margin: {errorpercent}%");
                }

                return sb.ToString();
            }
        }

        #endregion

        #region methods

        /// <summary>
        ///     Close current ongoing interval, end current pause if relevant, save them, and then open new interval.
        /// </summary>
        /// <param name="name">Name of interval</param>
        public void BeginInterval(string name)
        {
            var timeNowTicks = _internalClock?.TimeNowTicks ?? 0;

            TransitionToNewInterval(name ?? string.Empty, timeNowTicks);
        }

        /// <summary>
        ///     Close current ongoing interval, end current pause if relevant, save them, and then open new interval.
        /// </summary>
        /// <param name="name">Name of interval</param>
        public void BeginIntervalWithAPause(string name)
        {
            BeginInterval(name);

            BeginPauseToHaltInterval();
        }

        /// <summary>
        ///     Close current ongoing interval, end current pause if relevant, save them, and then open new interval.
        /// </summary>
        public void EndInterval()
        {
            var timeNowTicks = _internalClock?.TimeNowTicks ?? 0;

            // close out the current pause if one exists
            if (_openOngoingPause != null)
                StopPauseAndSave(timeNowTicks);

            // close out the current interval if one exists
            if (_openOngoingInterval != null)
                StopIntervalAndSave(timeNowTicks);
        }

        /// <summary>
        ///     Begin new pause
        /// </summary>
        public void BeginPauseToHaltInterval()
        {
            var timeNowTicks = _internalClock?.TimeNowTicks ?? 0;

            StartPause(timeNowTicks);
        }

        /// <summary>
        ///     End pause and resume current open interval
        /// </summary>
        public void EndPauseToResumeInterval()
        {
            var timeNowTicks = _internalClock?.TimeNowTicks ?? 0;

            StopPauseAndSave(timeNowTicks);
        }

        /// <summary>
        ///     Close final interval and final pause if any
        /// </summary>
        public void Reset()
        {
            ZeroiseData();
        }

        #endregion

        #region helpers

        private void StartPause(long pointInTimeTicks)
        {
            // pauses can only exist within a containing interval
            if (_openOngoingInterval == null)
                return;

            // can't start a pause if a pause is already ongoing
            if (_openOngoingPause != null)
                return;

            // list is zero-based of course so index++ is the same as count
            _openOngoingPause = new PausedPeriod
            {
                Index = _listOfCompletedPauses.Count,
                IndexOfContainingInterval = _openOngoingInterval.Index,
                Description =
                    $"Containing interval index = {_openOngoingInterval.Index} {_openOngoingInterval.Description ?? string.Empty}",
                BeginTimestampTicks = pointInTimeTicks
            };
        }

        private void StopPauseAndSave(long pointInTimeTicks)
        {
            // if pause doesn't exit nothing further to be done
            if (_openOngoingPause == null)
                return;

            _openOngoingPause.EndTimestampTicks = pointInTimeTicks;

            _openOngoingPause.EndMinusBeginTimestampTicks =
                _openOngoingPause.EndTimestampTicks - _openOngoingPause.BeginTimestampTicks;

            _listOfCompletedPauses.Add(_openOngoingPause);

            _openOngoingPause = null;
        }

        private void TransitionToNewInterval(string description, long pointInTimeTicks)
        {
            if (_openOngoingPause != null)
                StopPauseAndSave(pointInTimeTicks);

            if (_openOngoingInterval != null)
                StopIntervalAndSave(pointInTimeTicks);

            // list is zero-based of course so index++ == count
            _openOngoingInterval = new Interval(_internalClock.NanosecPerTick)
            {
                Index = _listOfCompletedIntervals.Count,
                Description = description ?? string.Empty,
                BeginTimestampTicks = pointInTimeTicks
            };
        }

        private void StopIntervalAndSave(long pointInTimeTicks)
        {
            // if interval doesn't exit nothing further to be done
            if (_openOngoingInterval == null)
                return;

            // fail safe. pauses can't exist unless inside an interval. 
            if (_openOngoingPause != null)
                StopPauseAndSave(pointInTimeTicks);

            _openOngoingInterval.EndTimestampTicks = pointInTimeTicks;

            _openOngoingInterval.EndMinusBeginTimestampTicks =
                _openOngoingInterval.EndTimestampTicks - _openOngoingInterval.BeginTimestampTicks;

            _openOngoingInterval.DurationTicksExcludingPauses =
                _openOngoingInterval.EndMinusBeginTimestampTicks -
                CalculateDurationOfAllPausesInThisInterval(_openOngoingInterval.Index);

            _listOfCompletedIntervals.Add(_openOngoingInterval);

            _openOngoingInterval = null;
        }

        private long CalculateDurationOfAllPausesInThisInterval(int intervalIndex)
        {
            if (_listOfCompletedPauses == null)
                return 0;

            var duration =
                _listOfCompletedPauses.Where(z => z.IndexOfContainingInterval == intervalIndex)
                    .Sum(z => z.EndMinusBeginTimestampTicks);

            return duration;
        }

        private void ZeroiseData()
        {
            _openOngoingInterval = null;

            _openOngoingPause = null;

            _listOfCompletedIntervals = new List<Interval>(10000);

            _listOfCompletedPauses = new List<PausedPeriod>(10000);
        }

        private string ComposeNicelyFormattedSummaryOfResults()
        {
            const string failure = "Doing results as string _summaryOfResults.";
            const string locus = "[JghSimpleIntervalTimer.UpdateResults]";

            try
            {
                if (_listOfCompletedIntervals == null) return "Sequence of completed and saved intervals is null.";

                if (_listOfCompletedIntervals.Count == 0) return "Sequence of completed and saved intervals is empty.";

                FillOutDurationAsProportionOfCumulativeTotalIntervalsExcludingPausesFieldForAllIntervals();

                var sb = new StringBuilder();

                foreach (var interval in _listOfCompletedIntervals)
                    sb.AppendLine(string.Format("{0} {2}ms ({1}%) {3}", interval.Index,
                        Math.Round(interval.DurationAsProportionOfCumulativeTotalIntervalsExcludingPauses * 100, 0),
                        ConvertTicksToMillisec(interval.DurationTicksExcludingPauses).ToString(DecimalFormat2Dp),
                        interval.Description));

                var footer1 =
                    $"{_listOfCompletedPauses.Count} pauses {CumulativeTotalPausesMillisec.ToString(DecimalFormat2Dp)}ms.";

                var footer2 =
                    $"{_listOfCompletedIntervals.Count} intervals {CumulativeTotalIntervalsMillisec.ToString(DecimalFormat2Dp)}ms (100%).";

                sb.AppendLine(string.Empty);

                sb.AppendLine(footer2 + " " + footer1);

                sb.AppendLine(string.Empty);

                return sb.ToString();
            }

            #region trycatch

            catch (Exception ex)
            {
                throw new Exception($"{failure} {locus}", ex);
            }

            #endregion
        }

        private void FillOutDurationAsProportionOfCumulativeTotalIntervalsExcludingPausesFieldForAllIntervals()
        {
            var totalDurationTicks = CumulativeTotalIntervalsTicks;

            if (_listOfCompletedIntervals == null)
                return;

            foreach (var interval in _listOfCompletedIntervals)
                if (totalDurationTicks > 0)
                    interval.DurationAsProportionOfCumulativeTotalIntervalsExcludingPauses =
                        interval.DurationTicksExcludingPauses / Convert.ToDouble(totalDurationTicks);
                else
                    interval.DurationAsProportionOfCumulativeTotalIntervalsExcludingPauses = 0;
        }

        private double ConvertTicksToMillisec(long ticks)
        {
            var nanoSeconds = ticks * _internalClock.NanosecPerTick;

            var milliSeconds = nanoSeconds / 1e6;

            return milliSeconds;
        }

        #endregion
    }
}