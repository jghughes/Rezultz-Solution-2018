using System;
using System.Collections.Generic;
using System.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

namespace Rezultz.DataTypes.Nov2023.PortalSplitIntervalItems
{
    public class SplitIntervalConsolidationForParticipantItem
    {

        #region props

        public string Bib { get; set; } = string.Empty;

        public string Rfid { get; set; } = string.Empty;

        public ParticipantHubItem Participant { get; set; }

        public string DnxRecordedByTimekeeper { get; set; } = string.Empty;

        public string DnxSurmisedByThisAlgorithm { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public string KindOfGunStart { get; set; } = string.Empty;

        public TimeStampHubItem GunStartTimeStamp { get; set; }

        public List<TimeStampHubItem> ListOfTimingMatTimeStamps { get; set; } = [];

        public List<TimeStampHubItem> ConsolidatedListOfAllTimeStamps { get; set; } = [];

        public List<SplitIntervalAsPairOfTimeStampsItem> ListOfCalculatedPairedTimeStampIntervals { get; set; } = [];

        public string RaceGroupDeducedFromParticipant { get; set; } = string.Empty;

        public string AgeGroupDeducedFromAgeGroupSpecificationsForEvent { get; set; } = string.Empty;

        public long CalculatedCumulativeTotalDurationTicks { get; set; }

        public int CalculatedRankOverall { get; set; }

        public int TallyOfTimingMatTimeStamps { get; set; }

        public int TallyOfSplitIntervals { get; set; }

        public string Guid { get; set; } = System.Guid.NewGuid().ToString();

        #endregion

        #region methods

        public static string ToLabel(SplitIntervalConsolidationForParticipantItem item)
        {
            if (item == null) return string.Empty;

            if (item.Participant == null)
                return item.Bib;

            var prettyGunStartDateTime = item.GunStartTimeStamp.TimeStampBinaryFormat == 0 ? string.Empty : JghDateTime.ToTimeLocalhhmmssf(item.GunStartTimeStamp.TimeStampBinaryFormat);

            var prettyDuration = JghTimeSpan.ToPrettyDurationFromTicks(item.CalculatedCumulativeTotalDurationTicks);

            var dnxSymbol = string.Empty;

            if (!string.IsNullOrWhiteSpace(item.DnxRecordedByTimekeeper))
                dnxSymbol = JghString.TmLr(item.DnxRecordedByTimekeeper);
            else if (!string.IsNullOrWhiteSpace(item.DnxSurmisedByThisAlgorithm))
                dnxSymbol = JghString.TmLr(item.DnxSurmisedByThisAlgorithm);


            return JghString.ConcatWithSeparator(" ", item.CalculatedRankOverall.ToString(), item.Bib, item.Rfid,  item.Participant.FirstName, item.Participant.LastName, item.RaceGroupDeducedFromParticipant, item.KindOfGunStart, prettyGunStartDateTime, prettyDuration, dnxSymbol);

        }

        public static ResultDto ToResultItemDataTransferObject(SplitIntervalConsolidationForParticipantItem item)
        {
            string ToSplitIntervalDuration(long intervalDurationTicks)
            {
                var answer = intervalDurationTicks == 0
                    ? string.Empty
                    : TimeSpan.FromTicks(intervalDurationTicks)
                        .ToString(JghTimeSpan.GeneralLongPattern);

                return answer;
            }

            if (item == null) return new ResultDto();
            // nb. don't return null. logic downstream requires to know that IsAuthorisedToOperate == false

            // important to ensure that we always have a non-blank, valid Race. this will ensure
            // that Bibs with otherwise empty particulars always show up in the splits. if this is blank, they won't.
            // this exact same enum string MUST be present in SeriesSettings file as a designated RaceSpecificationItem.

            var answer = new ResultDto
            {
                Bib = JghString.TmLr(item.Bib),
                Rfid = JghString.TmLr(item.Rfid),
                RaceGroup = string.IsNullOrWhiteSpace(item.RaceGroupDeducedFromParticipant) ? Symbols.SymbolUncategorised : JghString.TmLr(item.RaceGroupDeducedFromParticipant)// must always have a default Race
            };

            if (item.Participant != null)
            {
                var xx = item.Participant;

                //answer.Rfid = JghString.TmLr(xx.Rfid);
                answer.First = JghString.TmLr(xx.FirstName);
                answer.Last = JghString.TmLr(xx.LastName);
                answer.MiddleInitial = JghString.TmLr(xx.MiddleInitial);
                answer.Sex = JghString.TmLr(xx.Gender);
                answer.Age = JghConvert.ToAgeFromYearOfBirth(xx.BirthYear);
                answer.City = JghString.TmLr(xx.City);
                answer.Team = JghString.TmLr(xx.Team);
                answer.IsSeries = xx.IsSeries;
                answer.AgeGroup = string.Empty; // can't specify this here. information not yet available. must do it in calling method or even above that
            }

            if (!string.IsNullOrWhiteSpace(item.DnxRecordedByTimekeeper))
                answer.DnxString = JghString.TmLr(item.DnxRecordedByTimekeeper);
            else if (!string.IsNullOrWhiteSpace(item.DnxSurmisedByThisAlgorithm))
                answer.DnxString = JghString.TmLr(item.DnxSurmisedByThisAlgorithm);

            var i = 1;

            foreach (var intervalDurationTicks in item.ListOfCalculatedPairedTimeStampIntervals.Select(z => z.CalculatedIntervalDurationTicks))
            {
                switch (i)
                {
                    case 1:
                        answer.T01 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 2:
                        answer.T02 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 3:
                        answer.T03 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 4:
                        answer.T04 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 5:
                        answer.T05 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 6:
                        answer.T06 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 7:
                        answer.T07 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 8:
                        answer.T08 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 9:
                        answer.T09 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 10:
                        answer.T10 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 11:
                        answer.T11 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 12:
                        answer.T12 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 13:
                        answer.T13 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 14:
                        answer.T14 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                    case 15:
                        answer.T15 = ToSplitIntervalDuration(intervalDurationTicks);
                        break;
                }

                i++;
            }


            return answer;
        }

        #endregion

    }
}