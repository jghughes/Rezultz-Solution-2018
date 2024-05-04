using System;
using System.Linq;
using System.Runtime.Serialization;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;

namespace Rezultz.DataTypes.Nov2023.PortalHubItems
{
    // the only reason we need this class to be serialisable is so that we can do backups of the repository in local storage

    [Serializable]
    public class TimeStampHubItem : HubItemBase
    {
        #region properties

        [DataMember] public string DnxSymbol { get; set; } = string.Empty;

        #endregion

        #region methods
        
        public static TimeStampHubItem Create(int clickCounter, string identifier, string recordingModeEnum, long binaryTimeStamp, string touchedBy)
        {
            var thisIdentifier = JghString.CleanAndConvertToLetterOrDigitOrHyphen(identifier);

            var thisTimeStampBinaryFormat = binaryTimeStamp;
            //var thisTimeStampBinaryFormat = JghDateTime.RoundedToTenthOfSecond(DateTime.Now).ToBinary();

            var thisRecordingModeEnum = string.IsNullOrWhiteSpace(recordingModeEnum) ? string.Empty : recordingModeEnum;

            var thisTouchedBy = string.IsNullOrWhiteSpace(touchedBy) ? string.Empty : touchedBy;

            var answer = new TimeStampHubItem
            {
                ClickCounter = clickCounter,
                Identifier = thisIdentifier,
                TimeStampBinaryFormat = thisTimeStampBinaryFormat,
                RecordingModeEnum = thisRecordingModeEnum,
                DatabaseActionEnum = EnumStrings.DatabaseAdd, // NB add is to create
                TouchedBy = thisTouchedBy,
                WhenTouchedBinaryFormat = DateTime.Now.ToBinary(),
                Guid = System.Guid.NewGuid().ToString()
                // NB Guid assigned here at moment of population by user (not in ctor). the ctor does not create the Guid fields. only in TimeStampHubItem.CreateItem() and ClockHubItemEditTemplate.MergeEditsBackIntoItemBeingModified()
            };

            answer.Label = ToLabel(answer);

            answer.OriginatingItemGuid = answer.Guid;

            // special case. there is only ever a single mass start.
            //if (clockHubItem.RecordingModeEnum == EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody)
            //    clockHubItem.Identifier = JghString.CleanAndConvertToLetterOrDigit("0");

            return answer;
        }

        public static TimeStampHubItem FromDataTransferObject(TimeStampHubItemDto dto)
        {

            const string failure = "Populating TimeStampHubItem.";
            const string locus = "[FromDataTransferObject]";

            var src = dto ?? new TimeStampHubItemDto();

            try
            {
                var answer = new TimeStampHubItem
                {
                    //ID = 0,
                    //Label = string.Empty,
                    //EnumString = string.Empty,
                    ClickCounter = src.ClickCounter,
                    Identifier = src.Identifier,
                    RecordingModeEnum = src.RecordingModeEnum,
                    DatabaseActionEnum = src.DatabaseActionEnum,
                    MustDitchOriginatingItem = src.MustDitchOriginatingItem,
                    IsStillToBeBackedUp = src.IsStillToBeBackedUp,
                    IsStillToBePushed = src.IsStillToBePushed,
                    TouchedBy = src.TouchedBy,
                    TimeStampBinaryFormat = src.TimeStampBinaryFormat,
                    WhenTouchedBinaryFormat = src.WhenTouchedBinaryFormat,
                    WhenPushedBinaryFormat = src.WhenPushedBinaryFormat,
                    OriginatingItemGuid = src.OriginatingItemGuid,
                    Guid = src.Guid,
                    DnxSymbol = src.DnxSymbol
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

        public static TimeStampHubItem[] FromDataTransferObject(TimeStampHubItemDto[] dto)
        {

            const string failure = "Populating TimeStampHubItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                if (dto == null)
                    return Array.Empty<TimeStampHubItem>();

                var answer = dto.Select(FromDataTransferObject).Where(z => z != null).ToArray();

                return answer;

            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static TimeStampHubItemDto ToDataTransferObject(TimeStampHubItem item)
        {
            const string failure = "Populating TimeStampHubItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";

            var src = item ?? new TimeStampHubItem();

            try
            {
                var answer = new TimeStampHubItemDto
                {
                    DnxSymbol = src.DnxSymbol,
                    ClickCounter = src.ClickCounter,
                    Identifier = src.Identifier,
                    RecordingModeEnum = src.RecordingModeEnum,
                    DatabaseActionEnum = src.DatabaseActionEnum,
                    MustDitchOriginatingItem = src.MustDitchOriginatingItem,
                    IsStillToBeBackedUp = src.IsStillToBeBackedUp,
                    IsStillToBePushed = src.IsStillToBePushed,
                    TouchedBy = src.TouchedBy,
                    TimeStampBinaryFormat = src.TimeStampBinaryFormat,
                    WhenTouchedBinaryFormat = src.WhenTouchedBinaryFormat,
                    WhenPushedBinaryFormat = src.WhenPushedBinaryFormat,
                    OriginatingItemGuid = src.OriginatingItemGuid,
                    Guid = src.Guid
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

        public static TimeStampHubItemDto[] ToDataTransferObject(TimeStampHubItem[] item)
        {
            const string failure = "Populating TimeStampHubItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";

            try
            {
                if (item == null)
                    return Array.Empty<TimeStampHubItemDto>();

                var answer = item.Select(ToDataTransferObject).Where(z => z != null).ToArray();

                return answer;

            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public TimeStampHubItem ToShallowMemberwiseClone()
        {
            var clone = (TimeStampHubItem)MemberwiseClone();

            return clone;
        }

        public static string ToLabel(TimeStampHubItem item)
        {
            if (item == null) return string.Empty;

            return JghString.ConcatWithSeparator(" ", item.ClickCounter.ToString(), item.DatabaseActionEnum, item.Identifier, item.RecordingModeEnum, JghDateTime.ToTimeLocalhhmmssf(item.TimeStampBinaryFormat));

        }

        #endregion

    }
}
