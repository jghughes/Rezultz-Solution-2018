namespace Tool09
{
    internal class MyLapsResultItem
    {
        public string Bib { get; set; }

        public string FullName { get; set; }

        public string DurationAsString { get; set; }

        public string RaceGroup { get; set; }

        public string Comment { get; set; }

        public MyLapsResultItem(string bib, string fullName, string durationAsString, string raceGroup, string comment)
        {
            Bib = bib;
            FullName = fullName;
            DurationAsString = durationAsString;
            RaceGroup = raceGroup;
            Comment = comment;
        }
    }
}