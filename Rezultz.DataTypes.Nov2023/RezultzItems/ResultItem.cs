using System;
using System.Linq;
using System.Runtime.Serialization;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using Rezultz.DataTransferObjects.Nov2023.Results;

// ReSharper disable InconsistentNaming

namespace Rezultz.DataTypes.Nov2023.RezultzItems
{
    [Serializable]
    public class ResultItem : IHasRace, IHasGender, IHasAgeGroup, IHasCity, IHasUtilityClassification, IHasTeam,
        IHasFirstName, IHasLastName, IHasMiddleInitial, IHasFullName, IHasBib, IHasCollectionLineItemPropertiesV2
    {
        #region ctor

        public ResultItem()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        #endregion

        #region properties

        #region boilerplate

        public int ID { get; set; }

        public string Label { get; set; } = string.Empty;

        public string EnumString { get; set; } = string.Empty; // effectively the field used for switches in "case" statements and what have you

        public string Guid { get; set; }

        #endregion

        #region features of a item

        public string Bib { get; set; } = string.Empty;

        public string Rfid { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string MiddleInitial { get; set; } = string.Empty;

        public string FullName => FormatFullName(this);

        public Tuple<string, string, string, string> Identifier => new(LastName, FirstName, MiddleInitial, Bib);

        public int Age { get; set; }

        public string RaceGroup { get; set; } = string.Empty; // emanates from what we call Race in the Rezultz timing module but RaceSpecificationItem prior to 2019

        public string Gender { get; set; } = string.Empty;

        public string AgeGroup { get; set; } = string.Empty;

        public string UtilityClassification { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public bool IsSeries { get; set; }

        public string DnxString { get; set; } = string.Empty;
    
        public string T01 { get; set; } = string.Empty;

        public string T02 { get; set; } = string.Empty;

        public string T03 { get; set; } = string.Empty;

        public string T04 { get; set; } = string.Empty;

        public string T05 { get; set; } = string.Empty;

        public string T06 { get; set; } = string.Empty;

        public string T07 { get; set; } = string.Empty;

        public string T08 { get; set; } = string.Empty;

        public string T09 { get; set; } = string.Empty;

        public string T10 { get; set; } = string.Empty;

        public string T11 { get; set; } = string.Empty;

        public string T12 { get; set; } = string.Empty;

        public string T13 { get; set; } = string.Empty;

        public string T14 { get; set; } = string.Empty;

        public string T15 { get; set; } = string.Empty;

        public bool IsExcludedFromAllSeriesPointsCalculationsForSpecialReasons { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DerivedDataItem DerivedData { get; set; } = new();

        public int ScratchPadIndex { get; set; } // foreign key linking to each event for calculating series points

        #endregion

        #endregion

        #region methods

        public static ResultItem FromDataTransferObject(ResultDto dto)
        {
            const string failure = "Populating ResultItem.";
            const string locus = "[FromDataTransferObject]";


            try
            {
                var x = dto ?? new ResultDto();

                var answer = new ResultItem()
                {
                    Bib = JghString.TmLr(x.Bib),
                    Rfid = JghString.TmLr(x.Rfid),
                    FirstName = JghString.TmLr(x.First),
                    LastName = JghString.TmLr(x.Last),
                    MiddleInitial = JghString.TmLr(x.MiddleInitial),
                    Gender = JghString.TmLr(x.Sex),
                    Age = x.Age,
                    AgeGroup = JghString.TmLr(x.AgeGroup),
                    City = JghString.TmLr(x.City),
                    Team = JghString.TmLr(x.Team),
                    RaceGroup = JghString.TmLr(x.RaceGroup),
                    IsSeries = x.IsSeries,
                    DnxString = x.DnxString,
                    T01 = JghTimeSpan.ToDurationOrDnxV2(x.T01, x.DnxString),
                    T02 = JghTimeSpan.ToDurationOrDnxV2(x.T02, x.DnxString),
                    T03 = JghTimeSpan.ToDurationOrDnxV2(x.T03, x.DnxString),
                    T04 = JghTimeSpan.ToDurationOrDnxV2(x.T04, x.DnxString),
                    T05 = JghTimeSpan.ToDurationOrDnxV2(x.T05, x.DnxString),
                    T06 = JghTimeSpan.ToDurationOrDnxV2(x.T06, x.DnxString),
                    T07 = JghTimeSpan.ToDurationOrDnxV2(x.T07, x.DnxString),
                    T08 = JghTimeSpan.ToDurationOrDnxV2(x.T08, x.DnxString),
                    T09 = JghTimeSpan.ToDurationOrDnxV2(x.T09, x.DnxString),
                    T10 = JghTimeSpan.ToDurationOrDnxV2(x.T10, x.DnxString),
                    T11 = JghTimeSpan.ToDurationOrDnxV2(x.T11, x.DnxString),
                    T12 = JghTimeSpan.ToDurationOrDnxV2(x.T12, x.DnxString),
                    T13 = JghTimeSpan.ToDurationOrDnxV2(x.T13, x.DnxString),
                    T14 = JghTimeSpan.ToDurationOrDnxV2(x.T14, x.DnxString),
                    T15 = JghTimeSpan.ToDurationOrDnxV2(x.T15, x.DnxString),
                    IsExcludedFromAllSeriesPointsCalculationsForSpecialReasons = x.IsExcludedFromAllSeriesPointsCalculationsForSpecialReasons,
                    Comment = JghString.TmLr(x.Comment),
                };

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }

        public static ResultItem[] FromDataTransferObject(ResultDto[] dataTransferObject)
        {
            const string failure = "Populating ResultItem.";
            const string locus = "[FromDataTransferObject]";


            try
            {
                if (dataTransferObject is null)
                    return [];

                var answer = dataTransferObject.Select(FromDataTransferObject).Where(z => z is not null).ToArray();

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }

        public static ResultDto ToDataTransferObject(ResultItem item)
        {
            const string failure = "Populating ResultItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";

            try
            {

                var x = item ?? new ResultItem();

                var answer = new ResultDto()
                {
                    Bib = JghString.TmLr(x.Bib),
                    Rfid = JghString.TmLr(x.Rfid),
                    First = JghString.TmLr(x.FirstName),
                    Last = JghString.TmLr(x.LastName),
                    MiddleInitial = JghString.TmLr(x.MiddleInitial),
                    Sex = JghString.TmLr(x.Gender),
                    Age = x.Age,
                    AgeGroup = JghString.TmLr(x.AgeGroup),
                    City = JghString.TmLr(x.City),
                    Team = JghString.TmLr(x.Team),
                    RaceGroup = JghString.TmLr(x.RaceGroup),
                    IsSeries = x.IsSeries,
                    DnxString = x.DnxString,
                    T01 = JghTimeSpan.ToDurationOrDnx(x.T01, x.DnxString),
                    T02 = JghTimeSpan.ToDurationOrDnx(x.T02, x.DnxString),
                    T03 = JghTimeSpan.ToDurationOrDnx(x.T03, x.DnxString),
                    T04 = JghTimeSpan.ToDurationOrDnx(x.T04, x.DnxString),
                    T05 = JghTimeSpan.ToDurationOrDnx(x.T05, x.DnxString),
                    T06 = JghTimeSpan.ToDurationOrDnx(x.T06, x.DnxString),
                    T07 = JghTimeSpan.ToDurationOrDnx(x.T07, x.DnxString),
                    T08 = JghTimeSpan.ToDurationOrDnx(x.T08, x.DnxString),
                    T09 = JghTimeSpan.ToDurationOrDnx(x.T09, x.DnxString),
                    T10 = JghTimeSpan.ToDurationOrDnx(x.T10, x.DnxString),
                    T11 = JghTimeSpan.ToDurationOrDnx(x.T11, x.DnxString),
                    T12 = JghTimeSpan.ToDurationOrDnx(x.T12, x.DnxString),
                    T13 = JghTimeSpan.ToDurationOrDnx(x.T13, x.DnxString),
                    T14 = JghTimeSpan.ToDurationOrDnx(x.T14, x.DnxString),
                    T15 = JghTimeSpan.ToDurationOrDnx(x.T15, x.DnxString),
                    IsExcludedFromAllSeriesPointsCalculationsForSpecialReasons = x.IsExcludedFromAllSeriesPointsCalculationsForSpecialReasons,
                    Comment = JghString.TmLr(x.Comment)
                };

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }

        public static ResultDto[] ToDataTransferObject(ResultItem[] item)
        {
            const string failure = "Populating ResultItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";

            try
            {
                if (item is null)
                    return [];

                var answer = item.Select(ToDataTransferObject).Where(z => z is not null).ToArray();

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }

        public static SearchQueryItem ToSearchQuerySuggestionItem(ResultItem item)
        {
            if (item is null)
                return null;

            var answer = new SearchQueryItem(0, item.Guid, JghString.ConcatAsSentences(item.Bib, item.FirstName, item.LastName, item.RaceGroup, item.AgeGroup));

            return answer;
        }

        public static SearchQueryItem[] ToSearchQuerySuggestionItem(ResultItem[] item)
        {
            const string failure = "Populating SearchQueryItem[].";
            const string locus = "[ToSearchQuerySuggestionItem]";

            try
            {
                if (item is null)
                    return [];

                var answer = item.Select(ToSearchQuerySuggestionItem).Where(z => z is not null).ToArray();

                return answer;
            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion

        }
        
        public ResultItem ShallowMemberwiseCloneCopyExcludingDerivedData
        {
            get
            {
                var clone = (ResultItem)MemberwiseClone();

                clone.DerivedData = new DerivedDataItem();

                return clone;
            }
        }

        public ResultItem ShallowMemberwiseCloneCopyIncludingDerivedData
        {
            get
            {
                var clone = (ResultItem)MemberwiseClone();

                clone.DerivedData =
                    DerivedData is not null ? DerivedData.ShallowMemberwiseCloneCopy : new DerivedDataItem();

                return clone;
            }
        }

        public static string FormatFullName(ResultItem item)
        {
            return JghString.ConcatAsSentences(item.FirstName, item.MiddleInitial, item.LastName);
        }

        public override string ToString()
        {
            return JghString.ConcatAsSentences(DerivedData?.PlaceCalculatedOverallInt.ToString(), FirstName, MiddleInitial, LastName,
                RaceGroup, AgeGroup, Bib, Rfid);
        }


        #endregion

    }
}