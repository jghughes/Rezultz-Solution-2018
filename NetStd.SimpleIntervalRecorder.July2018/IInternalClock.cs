namespace NetStd.SimpleIntervalRecorder.July2018
{
    internal interface IInternalClock
    {
        double NanosecPerTick { get; }
        long TimeNowTicks { get; }
        string MeasureReportedAccuracy();
    }
}