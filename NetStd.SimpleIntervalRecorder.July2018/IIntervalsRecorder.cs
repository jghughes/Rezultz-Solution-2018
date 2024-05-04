using System.Collections.Generic;

namespace NetStd.SimpleIntervalRecorder.July2018
{
    public interface IIntervalsRecorder
    {
        string Name { get; }
        Interval MostRecentlyCompletedInterval { get; }
        List<Interval> CompletedIntervalsToDate { get; }
        double CumulativeTotalIntervalsMillisec { get; }
        double AverageIntervalMillisec { get; }
        double LongestIntervalMillisec { get; }
        Interval LongestInterval { get; }
        Interval ShortestInterval { get; }
        List<Interval> ShortestIntervalsTop100 { get; }
        double ShortestIntervalMillisec { get; }
        double MedianIntervalMillisec { get; }
        double CumulativeTotalPausesMillisec { get; }
        string SummaryOfResults { get; }

        /// <summary>
        /// Measured immediately for a manufactured interval of approximately twenty seconds
        /// </summary>
        string ReportedAccuracy { get; }

        /// <summary>
        /// Close current ongoing interval, end current pause if relevant, save them, and then open new interval. 
        /// </summary>
        /// <param name="name">Name of interval</param>
        void BeginInterval(string name);

        /// <summary>
        /// Close current ongoing interval, end current pause if relevant, save them, and then open new interval. 
        /// </summary>
        /// <param name="name">Name of interval</param>
        void BeginIntervalWithAPause(string name);

        /// <summary>
        /// Close current ongoing interval, end current pause if relevant, save them, and then open new interval. 
        /// </summary>
        void EndInterval();

        /// <summary>
        /// Begin new pause 
        /// </summary>
        void BeginPauseToHaltInterval();

        /// <summary>
        /// End pause and resume current open interval
        /// </summary>
        void EndPauseToResumeInterval();

        /// <summary>
        /// Close final interval and final pause if any
        /// </summary>
        void Reset();
    }
}