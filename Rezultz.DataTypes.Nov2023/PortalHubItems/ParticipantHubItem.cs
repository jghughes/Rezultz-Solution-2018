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
    public class ParticipantHubItem : HubItemBase
    {
        #region ctor

        public ParticipantHubItem()
        {
            RecordingModeEnum = EnumStrings.KindOfEntryIsParticipantEntry;

            //N.B. never new up the GUIDS here! only in HubItem.CreateItem() and HubItemEditTemplate.MergeEditsBackIntoItemBeingModified()
        }

        #endregion

        #region properties

        [DataMember] public string FirstName { get; set; } = string.Empty;

        [DataMember] public string MiddleInitial { get; set; } = string.Empty;

        [DataMember] public string LastName { get; set; } = string.Empty;

        [DataMember] public string Gender { get; set; } = string.Empty;

        [DataMember] public int BirthYear { get; set; } = 1900;

        [DataMember] public string City { get; set; } = string.Empty;

        [DataMember] public string Team { get; set; } = string.Empty;

        [DataMember] public string RaceGroupBeforeTransition { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

        [DataMember] public string RaceGroupAfterTransition { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

        [DataMember] public DateTime DateOfRaceGroupTransition { get; set; } = DateTime.Today;

        [DataMember] public bool IsSeries { get; set; }

        [DataMember] public string Series { get; set; } = string.Empty;

        [DataMember] public string EventIdentifiers { get; set; } = string.Empty;

        [DataMember] public string Reservation { get; set; } = string.Empty;


        #endregion

        #region methods

        public static ParticipantHubItem Create(int clickCounter, string bib, string rfid, string recordingModeEnum, string touchedBy)
        {

            var thisBib = JghString.CleanAndConvertToLetterOrDigitOrHyphen(bib);

            var thisTimeStampBinaryFormat = JghDateTime.RoundedToTenthOfSecond(DateTime.Now).ToBinary();

            var thisRecordingModeEnum = string.IsNullOrWhiteSpace(recordingModeEnum) ? string.Empty : recordingModeEnum;

            var thisTouchedBy = string.IsNullOrWhiteSpace(touchedBy) ? string.Empty : touchedBy;

            var answer = new ParticipantHubItem
            {
                ClickCounter = clickCounter,
                Bib = thisBib,
                Rfid = rfid,
                TimeStampBinaryFormat = thisTimeStampBinaryFormat,
                RecordingModeEnum = thisRecordingModeEnum,
                DatabaseActionEnum = EnumStrings.DatabaseAdd, // NB this is for an addition
                TouchedBy = thisTouchedBy,
                WhenTouchedBinaryFormat = DateTime.Now.ToBinary(),
                DateOfRaceGroupTransition = DateTime.Today, // merely a default for the brand new hub item i.e. the same as WhenTouchedBinaryFormat
                Guid = System.Guid.NewGuid().ToString()
                // NB Guid assigned here at moment of population by user (not in ctor). the ctor does not create the Guid fields. only in ParticipantHubItem.CreateItem() and ParticipantHubItemEditTemplate.MergeEditsBackIntoItemBeingModified()

            };

            answer.Label = ToLabel(answer);

            answer.OriginatingItemGuid = answer.Guid;

            return answer;
        }
        
        public static ParticipantHubItem FromDataTransferObject(ParticipantHubItemDto dto)
        {
            const string failure = "Populating ParticipantHubItem.";
            const string locus = "[FromDataTransferObject]";


            try
            {
                var src = dto ?? new ParticipantHubItemDto();

                var answer = new ParticipantHubItem
                {
                    Label = string.Empty,
                    ClickCounter = src.ClickCounter,
                    Bib = src.Bib,
                    Rfid = src.Rfid,
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
                    FirstName = src.FirstName,
                    MiddleInitial = src.MiddleInitial,
                    LastName = src.LastName,
                    Gender = src.Gender,
                    BirthYear = src.BirthYear,
                    City = src.City,
                    Team = src.Team,
                    RaceGroupBeforeTransition = src.RaceGroupBeforeTransition,
                    RaceGroupAfterTransition = string.IsNullOrWhiteSpace(src.RaceGroupAfterTransition) ? src.RaceGroupBeforeTransition : src.RaceGroupAfterTransition,
                    DateOfRaceGroupTransition = DateTime.TryParse(src.DateOfRaceGroupTransitionAsString, out var dateTime) ? dateTime.Date : DateTime.Today,
                    IsSeries = src.IsSeries,
                    Series = src.Series,
                    EventIdentifiers = src.EventIdentifiers,
                    Reservation = src.Reservation
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

        public static ParticipantHubItem[] FromDataTransferObject(ParticipantHubItemDto[] dto)
        {
            const string failure = "Populating ParticipantHubItem.";
            const string locus = "[FromDataTransferObject]";


            try
            {
                if (dto == null)
                    return [];

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

        public static ParticipantHubItemDto ToDataTransferObject(ParticipantHubItem item)
        {
            const string failure = "Populating ParticipantHubItemDto.";
            const string locus = "[ToDataTransferObject]";

            try
            {
                var src = item ?? new ParticipantHubItem();

                var answer = new ParticipantHubItemDto
                {
                    Bib = src.Bib,
                    Rfid = src.Rfid,
                    FirstName = src.FirstName,
                    MiddleInitial = src.MiddleInitial,
                    LastName = src.LastName,
                    Gender = src.Gender,
                    BirthYear = src.BirthYear,
                    City = src.City,
                    Team = src.Team,
                    RaceGroupBeforeTransition = src.RaceGroupBeforeTransition,
                    RaceGroupAfterTransition = src.RaceGroupAfterTransition,
                    IsSeries = src.IsSeries,
                    Series = src.Series,
                    EventIdentifiers = src.EventIdentifiers,
                    Reservation = src.Reservation,
                    ClickCounter = src.ClickCounter,
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

                if (string.IsNullOrWhiteSpace(src.RaceGroupAfterTransition) || src.RaceGroupAfterTransition == src.RaceGroupBeforeTransition)
                {
                    answer.DateOfRaceGroupTransitionAsString = string.Empty;
                }
                else
                {
                    answer.DateOfRaceGroupTransitionAsString = src.DateOfRaceGroupTransition.ToShortDateString();
                }

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static ParticipantHubItemDto[] ToDataTransferObject(ParticipantHubItem[] item)
        {
            const string failure = "Populating ParticipantHubItemDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                if (item == null)
                    return [];

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

        public ParticipantHubItem ToShallowMemberwiseClone()
        {
            var clone = (ParticipantHubItem)MemberwiseClone();

            return clone;
        }

        public static string ToLabel(ParticipantHubItem item)
        {
            if (item == null) return string.Empty;

            return JghString.ConcatWithSeparator(" ", item.ClickCounter.ToString(), item.DatabaseActionEnum, item.Bib, item.FirstName, item.LastName, item.Rfid);
        }

        #endregion
    }
}
