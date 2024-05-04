using NetStd.Interfaces01.July2018.HasProperty;

namespace Rezultz.DataTypes.Nov2023.RezultzItems
{
    public class PopulationCohortItem : IHasCollectionLineItemPropertiesV1
    {
        public string NameOfCohort { get; set; }

        public int FinishersCount { get; set; }

        public int SexMaleCount { get; set; }

        public int SexFemaleCount { get; set; }

        public int SexOtherCount { get; set; }

        //public int Other { get; set; }

        public int DnfCount { get; set; }

        public int DnsCount { get; set; }

        public int DqCount { get; set; }

        public int TotalCount { get; set; }

        public double Percent { get; set; }

        public int ID { get; set; }

        public string Label { get; set; }

        public string EnumString { get; set; }
    }
}