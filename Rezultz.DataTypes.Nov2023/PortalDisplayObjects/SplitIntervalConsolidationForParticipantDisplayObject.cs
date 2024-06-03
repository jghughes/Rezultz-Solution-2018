using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalSplitIntervalItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.PortalDisplayObjects
{
    public class SplitIntervalConsolidationForParticipantDisplayObject : BindableBase, IHasIdentifier, IHasCollectionLineItemPropertiesV2
    {

        #region CellXElementNames in ColumnSpecificationItem

        public const string XePlace = "place";
        public const string XeKindOfGunStart = "kind-of-gunstart";
        public const string XePrettyGunStartDateTime = "pretty-gunstart-datetime";
        public const string XeTallyOfTimingMatActivations = "tally-of-timingmat-activations";
        public const string XeTotalNumberOfSplitIntervals = "tally-of-split-intervals";
        public const string XeCalculatedTotalCumulativeDuration = "calculated-total-cumulative-duration";

        #endregion

        #region props

        public string Bib { get; set; } = string.Empty;

        public string Rfid { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string MiddleInitial { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Age { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;

        public string RaceGroup { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

        public string IsSeries { get; set; }

        public string DnxSymbol { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public string AgeGroupDeducedFromAgeGroupSpecificationsForEvent { get; set; } = string.Empty;
    
        public string CalculatedRankOverall { get; set; } = string.Empty;

        public string KindOfGunStart { get; set; } = string.Empty;

        public string PrettyGunStartDateTime { get; set; } = string.Empty;

        public string TallyOfTimingMatActivations { get; set; } = string.Empty;

        public string TallyOfSplitIntervals { get; set; } = string.Empty;

        public string CalculatedTotalCumulativeDuration { get; set; } = string.Empty;

        public string Guid { get; set; } // not used for anything.

        #region T01 - T15

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

        #endregion

        #endregion

        #region unused props merely to satisfy IHasCollectionLineItemPropertiesV2

        public int ID { get; set; }
        public string Label { get; set; } = string.Empty;
        public string EnumString { get; set; } = string.Empty;

        #endregion

        #region methods

        public static SplitIntervalConsolidationForParticipantDisplayObject FromModel(SplitIntervalConsolidationForParticipantItem model)
        {

            if (model == null) return new SplitIntervalConsolidationForParticipantDisplayObject();
            // nb. don't return null. logic downstream requires to know that IsAuthorisedToOperate == false

            var answer = new SplitIntervalConsolidationForParticipantDisplayObject
            {
                Bib = JghString.RightAlign(model.Bib, 4, ' '),
                Rfid = JghString.RightAlign(model.Rfid, 4, ' '),
                CalculatedRankOverall = JghString.PadLeftOrBlankIfZero(model.CalculatedRankOverall, 4, ' '),
                TallyOfTimingMatActivations = JghString.RightAlign(model.TallyOfTimingMatTimeStamps.ToString(), 5, ' '),
                TallyOfSplitIntervals = JghString.RightAlign(model.TallyOfSplitIntervals.ToString(), 5, ' '),
                RaceGroup = model.RaceGroupDeducedFromParticipant,
                AgeGroupDeducedFromAgeGroupSpecificationsForEvent = model.AgeGroupDeducedFromAgeGroupSpecificationsForEvent,
                Comment = model.Comment,
                Guid = model.Guid,
            };

            if (model.GunStartTimeStamp == null)
            {
                answer.KindOfGunStart = model.TallyOfTimingMatTimeStamps > 1 ? "first mat" : "indeterminate";
            }
            else
            {
                answer.KindOfGunStart = model.KindOfGunStart;

                answer.PrettyGunStartDateTime = model.GunStartTimeStamp.TimeStampBinaryFormat == 0 ? string.Empty : JghDateTime.ToTimeLocalhhmmssf(model.GunStartTimeStamp.TimeStampBinaryFormat);
            }

            if (!string.IsNullOrWhiteSpace(model.DnxRecordedByTimekeeper))
                answer.DnxSymbol = JghString.TmLr(model.DnxRecordedByTimekeeper);
            else if (!string.IsNullOrWhiteSpace(model.DnxSurmisedByThisAlgorithm)) 
                answer.DnxSymbol = JghString.TmLr(model.DnxSurmisedByThisAlgorithm);

            answer.CalculatedTotalCumulativeDuration = JghTimeSpan.ToPrettyDurationFromTicks(model.CalculatedCumulativeTotalDurationTicks);

            if (model.Participant != null)
            {
                var xx = model.Participant;

                answer.FirstName = xx.FirstName;
                answer.MiddleInitial = xx.MiddleInitial;
                answer.LastName = xx.LastName;
                answer.Gender = xx.Gender;
                answer.Age = JghConvert.ToAgeFromYearOfBirth(xx.BirthYear).ToString();
                answer.City = xx.City;
                answer.Team = xx.Team;
                answer.IsSeries = JghString.BooleanTrueToSeriesOrOneOff(xx.IsSeries);
            }

            var i = 1;

            foreach (var intervalDurationTicks in model.ListOfCalculatedPairedTimeStampIntervals.Select(z => z.CalculatedIntervalDurationTicks))
            {
                switch (i)
                {
                    case 1:
                        answer.T01 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 2:
                        answer.T02 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 3:
                        answer.T03 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 4:
                        answer.T04 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 5:
                        answer.T05 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 6:
                        answer.T06 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 7:
                        answer.T07 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 8:
                        answer.T08 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 9:
                        answer.T09 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 10:
                        answer.T10 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 11:
                        answer.T11 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 12:
                        answer.T12 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 13:
                        answer.T13 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 14:
                        answer.T14 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                    case 15:
                        answer.T15 = JghTimeSpan.ToPrettyDurationFromTicks(intervalDurationTicks);
                        break;
                }

                i++;
            }


            return answer;
        }

        public static SplitIntervalConsolidationForParticipantDisplayObject[] FromModel(SplitIntervalConsolidationForParticipantItem[] model)
        {
            const string failure = "Populating SplitIntervalConsolidationForParticipantDisplayObject[]";
            const string locus = "[FromModel]";

            try
            {
                if (model == null)
                    return [];

                var answer = model.Select(FromModel).Where(z => z != null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SplitIntervalConsolidationForParticipantDisplayObject FromModelShowingTimeStampTxx(SplitIntervalConsolidationForParticipantItem model)
        {

            if (model == null) return new SplitIntervalConsolidationForParticipantDisplayObject();
            // nb. don't return null. logic downstream requires to know that IsAuthorisedToOperate == false

            var answer = new SplitIntervalConsolidationForParticipantDisplayObject
            {
                Bib = JghString.RightAlign(model.Bib, 4, ' '),
                Rfid = JghString.RightAlign(model.Rfid, 4, ' '),
                CalculatedRankOverall = JghString.PadLeftOrBlankIfZero(model.CalculatedRankOverall, 4, ' '),
                TallyOfTimingMatActivations = JghString.RightAlign(model.TallyOfTimingMatTimeStamps.ToString(), 5, ' '),
                TallyOfSplitIntervals = JghString.RightAlign(model.TallyOfSplitIntervals.ToString(), 5, ' '),
                RaceGroup = model.RaceGroupDeducedFromParticipant,
                AgeGroupDeducedFromAgeGroupSpecificationsForEvent = model.AgeGroupDeducedFromAgeGroupSpecificationsForEvent,
                Comment = model.Comment,
                Guid = string.Empty
            };

            if (model.GunStartTimeStamp == null)
            {
                answer.KindOfGunStart = model.TallyOfTimingMatTimeStamps > 1 ? "first mat" : "indeterminate";
            }
            else
            {
                answer.KindOfGunStart = model.KindOfGunStart;

                answer.PrettyGunStartDateTime = model.GunStartTimeStamp.TimeStampBinaryFormat == 0 ? string.Empty : JghDateTime.ToDateTimeLocalSortable(model.GunStartTimeStamp.TimeStampBinaryFormat);
            }



            if (!string.IsNullOrWhiteSpace(model.DnxRecordedByTimekeeper))
                answer.DnxSymbol = JghString.TmLr(model.DnxRecordedByTimekeeper);
            else if (!string.IsNullOrWhiteSpace(model.DnxSurmisedByThisAlgorithm))
                answer.DnxSymbol = JghString.TmLr(model.DnxSurmisedByThisAlgorithm);

            answer.CalculatedTotalCumulativeDuration = JghTimeSpan.ToPrettyDurationFromTicks(model.CalculatedCumulativeTotalDurationTicks);

            if (model.Participant != null)
            {
                var xx = model.Participant;

                answer.FirstName = xx.FirstName;
                answer.MiddleInitial = xx.MiddleInitial;
                answer.LastName = xx.LastName;
                answer.Gender = xx.Gender;
                answer.Age = JghConvert.ToAgeFromYearOfBirth(xx.BirthYear).ToString();
                answer.City = xx.City;
                answer.Team = xx.Team;
                //answer.RaceGroup = xx.RaceGroupBeforeTransition;
                answer.IsSeries = JghString.BooleanTrueToSeriesOrOneOff(xx.IsSeries);
            }

            var i = 1;

        
            foreach (var timeStampBinaryFormat in model.ListOfTimingMatTimeStamps.OrderBy(z => z.TimeStampBinaryFormat).Select(z => z.TimeStampBinaryFormat))
            {
                switch (i)
                {
                    case 1:
                        answer.T01 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 2:
                        answer.T02 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 3:
                        answer.T03 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 4:
                        answer.T04 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 5:
                        answer.T05 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 6:
                        answer.T06 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 7:
                        answer.T07 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 8:
                        answer.T08 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 9:
                        answer.T09 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 10:
                        answer.T10 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 11:
                        answer.T11 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 12:
                        answer.T12 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 13:
                        answer.T13 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 14:
                        answer.T14 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                    case 15:
                        answer.T15 = JghDateTime.ToDateTimeLocalSortable(timeStampBinaryFormat);
                        break;
                }

                i++;
            }


            return answer;
        }

        public static SplitIntervalConsolidationForParticipantDisplayObject[] FromModelShowingTimeStampTxx(SplitIntervalConsolidationForParticipantItem[] model)
        {
            const string failure = "Populating SplitIntervalConsolidationForParticipantDisplayObject[]";
            const string locus = "[FromModelShowingTimeStampTxx]";

            try
            {
                if (model == null)
                    return [];

                var answer = model.Select(FromModelShowingTimeStampTxx).Where(z => z != null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static SearchQueryItem ToSearchQuerySuggestionItem(SplitIntervalConsolidationForParticipantDisplayObject displayObject)
        {
            if (displayObject == null)
                return null;

            var answer = new SearchQueryItem(0, displayObject.Guid, ToLabel(displayObject));

            return answer;
        }

        public static SearchQueryItem[] ToSearchQuerySuggestionItem(SplitIntervalConsolidationForParticipantDisplayObject[] displayObject)
        {
            const string failure = "Populating SearchQueryItem[].";
            const string locus = "[ToSearchQuerySuggestionItem]";

            try
            {
                if (displayObject == null)
                    return [];

                var answer = displayObject.Select(ToSearchQuerySuggestionItem).Where(z => z != null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForSplitIntervalsPerParticipantAsDisplayObject()
        {
            var template = new List<ColumnSpecificationItem>
            {
                new(XePlace, "Rank", "CalculatedRankOverall"),
                new(HubItemDtoNames.XeBib, "Bib", "Bib"),
                new(HubItemDtoNames.XeRfid, "Rfid", "Rfid"),
                new(ParticipantHubItemDto.XeFirstName, "First", "FirstName"),
                new(ParticipantHubItemDto.XeMiddleInitial, "M", "MiddleInitial"),
                new(ParticipantHubItemDto.XeLastName, "Last", "LastName"),
                new(XeCalculatedTotalCumulativeDuration, "Duration", "CalculatedTotalCumulativeDuration"),
                new(TimeStampHubItemDto.XeDnxSymbol, "Dnx", "DnxSymbol"),
                new(XeTallyOfTimingMatActivations, "Mats", "TallyOfTimingMatActivations"),
                new(XeTotalNumberOfSplitIntervals, "Splits", "TallyOfSplitIntervals"),
                new(ParticipantHubItemDto.XeRace, "RaceGroup", "RaceGroup"),
                new(ParticipantHubItemDto.XeGender, "G", "Gender"),
                new(ParticipantHubItemDto.XeAge, "Age", "Age"),
                new(ParticipantHubItemDto.XeAgeGroup, "AgeGroup", "AgeGroupDeducedFromAgeGroupSpecificationsForEvent"),
                new(ParticipantHubItemDto.XeCity, "City", "City"),
                new(ParticipantHubItemDto.XeRace, "Team", "Team"),
                new(ParticipantHubItemDto.XeIsSeries, "Registration", "IsSeries"),
                new(XeKindOfGunStart, "StartType", "KindOfGunStart"),
                new(XePrettyGunStartDateTime, "GunStart", "PrettyGunStartDateTime"),
                new(ResultDto.XeT01, "T01", "T01"),
                new(ResultDto.XeT02, "T02", "T02"),
                new(ResultDto.XeT03, "T03", "T03"),
                new(ResultDto.XeT04, "T04", "T04"),
                new(ResultDto.XeT05, "T05", "T05"),
                new(ResultDto.XeT06, "T06", "T06"),
                new(ResultDto.XeT07, "T07", "T07"),
                new(ResultDto.XeT08, "T08", "T08"),
                new(ResultDto.XeT09, "T09", "T09"),
                new(ResultDto.XeT10, "T10", "T10"),
                new(ResultDto.XeT10, "T11", "T11"),
                new(ResultDto.XeT10, "T12", "T12"),
                new(ResultDto.XeT10, "T13", "T13"),
                new(ResultDto.XeT10, "T14", "T14"),
                new(ResultDto.XeT10, "T15", "T15"),

                new(HubItemDtoNames.XeComment, "Anomalies", "Comment") // Comment field contains description of anomaly
            };
            return template;
        }

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForSplitIntervalsPerParticipantAsAbridgedDisplayObject()
        {
            var template = new List<ColumnSpecificationItem>
            {
                new(XePlace, "Rank", "CalculatedRankOverall"),
                new(HubItemDtoNames.XeBib, "Bib", "Bib"),
                new(HubItemDtoNames.XeRfid, "Rfid", "Bib"),
                new(ParticipantHubItemDto.XeFirstName, "First", "FirstName"),
                //new(ParticipantHubItemSerialiserNames.MiddleInitial, "M", "MiddleInitial"),
                new(ParticipantHubItemDto.XeLastName, "Last", "LastName"),
                new(XeCalculatedTotalCumulativeDuration, "Duration", "CalculatedTotalCumulativeDuration"),
                new(TimeStampHubItemDto.XeDnxSymbol, "Dnx", "DnxSymbol"),
                new(XeTallyOfTimingMatActivations, "Mats", "TallyOfTimingMatActivations"),
                new(XeTotalNumberOfSplitIntervals, "Splits", "TallyOfSplitIntervals"),
                new(ParticipantHubItemDto.XeRace, "RaceGroup", "RaceGroup"),
                //new(ParticipantHubItemSerialiserNames.Gender, "G", "Gender"),
                //new(ParticipantHubItemSerialiserNames.Age, "Age", "Age"),
                //new(ParticipantHubItemSerialiserNames.AgeGroup, "AgeGroup", "AgeGroupDeducedFromAgeGroupSpecificationsForEvent"),
                //new(ParticipantHubItemSerialiserNames.City, "City", "City"),
                //new(ParticipantHubItemSerialiserNames.Race, "Team", "Team"),
                //new(ParticipantHubItemSerialiserNames.IsSeries, "Single?", "IsSeries"),
                new(XeKindOfGunStart, "StartType", "KindOfGunStart"),
                new(XePrettyGunStartDateTime, "GunStart", "PrettyGunStartDateTime"),
                new(ResultDto.XeT01, "T01", "T01"),
                new(ResultDto.XeT02, "T02", "T02"),
                new(ResultDto.XeT03, "T03", "T03"),
                new(ResultDto.XeT04, "T04", "T04"),
                new(ResultDto.XeT05, "T05", "T05"),
                new(ResultDto.XeT06, "T06", "T06"),
                new(ResultDto.XeT07, "T07", "T07"),
                new(ResultDto.XeT08, "T08", "T08"),
                new(ResultDto.XeT09, "T09", "T09"),
                new(ResultDto.XeT10, "T10", "T10"),
                new(ResultDto.XeT10, "T11", "T11"),
                new(ResultDto.XeT10, "T12", "T12"),
                new(ResultDto.XeT10, "T13", "T13"),
                new(ResultDto.XeT10, "T14", "T14"),
                new(ResultDto.XeT10, "T15", "T15"),

                new(HubItemDtoNames.XeComment, "Anomalies", "Comment") // Comment field contains description of anomaly
            };
            return template;
        }

        public SplitIntervalConsolidationForParticipantDisplayObject ToShallowMemberwiseClone()
        {
            var clone = (SplitIntervalConsolidationForParticipantDisplayObject)MemberwiseClone();

            return clone;
        }

        public static string ToLabel(SplitIntervalConsolidationForParticipantDisplayObject displayObject)
        {
            if (displayObject == null)
                return string.Empty;

            var label = JghString.ConcatWithSeparator(" ",
                displayObject.CalculatedRankOverall,
                displayObject.Bib,
                displayObject.Rfid,
                displayObject.FirstName,
                displayObject.LastName,
                displayObject.RaceGroup,
                displayObject.KindOfGunStart,
                displayObject.PrettyGunStartDateTime,
                displayObject.TallyOfTimingMatActivations,
                displayObject.TallyOfSplitIntervals,
                displayObject.CalculatedTotalCumulativeDuration,
                displayObject.DnxSymbol
            );

            return label;
        }

        #endregion
    }
}