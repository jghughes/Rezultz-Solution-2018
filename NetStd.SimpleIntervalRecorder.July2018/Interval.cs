namespace NetStd.SimpleIntervalRecorder.July2018
{
    /// <summary>
    /// Duration of an interval is a calculated field 
    /// that excludes the duration of contained pauses, likewise
    /// the duration as a portion of the total of all saved intervals.
    /// </summary>
    public class Interval
    {
        private readonly double _nanosecPerTick;

        public int Index;
        public string Description;
        public long BeginTimestampTicks;
        public long EndTimestampTicks;
        public long EndMinusBeginTimestampTicks;
        public long DurationTicksExcludingPauses;
        public double DurationMillisecExcludingPauses
        {
            get { return ConvertTicksToMillisec(DurationTicksExcludingPauses); }
        }
        public double DurationAsProportionOfCumulativeTotalIntervalsExcludingPauses;

        public Interval()
        {
        }

        public Interval(double nanosecPerTick)
        {
            _nanosecPerTick = nanosecPerTick;
        }

        private double ConvertTicksToMillisec(long ticks)
        {
            var nanoSeconds = ticks * _nanosecPerTick;

            var milliSeconds = nanoSeconds / 1e6;

            return milliSeconds;
        }

        public override string ToString()
        {
            return
                $"Index {Index} Duration {DurationMillisecExcludingPauses:n2}ms {Description}";
        }
    }
}