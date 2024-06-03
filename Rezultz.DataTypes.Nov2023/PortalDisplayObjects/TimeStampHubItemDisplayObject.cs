using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.PortalDisplayObjects
{
    public class TimeStampHubItemDisplayObject : HubItemDisplayObjectBase
    {
        #region props

        public string DnxSymbol { get; private set; } = string.Empty;

        #endregion

        #region methods

        public static TimeStampHubItemDisplayObject FromModel(TimeStampHubItem model)
        {
            const string failure = "Populating TimeStampHubItemDisplayObject.";
            const string locus = "[TimeStampHubItemDisplayObject]";

            const string ellipsis = "...";

            try
            {
                model ??= new TimeStampHubItem();

                var displayObject = new TimeStampHubItemDisplayObject
                {
                    ClickCounter = JghString.ToStringMin3(model.ClickCounter),
                    Bib = JghString.RightAlign(model.Bib, 4, ' '),
                    Rfid = JghString.RightAlign(model.Rfid, 4, ' '),
                    RecordingModeEnum = model.RecordingModeEnum,
                    DatabaseActionEnum = model.DatabaseActionEnum,
                    DnxSymbol = model.DnxSymbol,
                    MustDitchOriginatingItem = JghString.BooleanTrueToDitchedOrBlank(model.MustDitchOriginatingItem),
                    IsStillToBeBackedUp = JghString.BooleanTrueToSaveOrBlank(model.IsStillToBeBackedUp),
                    IsStillToBePushed = JghString.BooleanTrueToPushOrBlank(model.IsStillToBePushed),
                    TouchedBy = model.TouchedBy,
                    TimeStamp = JghDateTime.ToTimeLocalhhmmssf(model.TimeStampBinaryFormat),
                    WhenTouched = JghDateTime.ToDateTimeLocalSortable(model.WhenTouchedBinaryFormat),
                    WhenPushed = JghDateTime.ToDateTimeLocalSortable(model.WhenPushedBinaryFormat),
                    DisplayVersionOfSourceItemOriginatingItemGuid = JghString.Substring(0, 4, model.OriginatingItemGuid) + ellipsis,
                    DisplayVersionOfSourceItemGuid = JghString.Substring(0, 4, model.Guid) + ellipsis,
                    Comment = model.Comment,
                    ID = model.ID,
                    Label = model.Label,
                    EnumString = model.EnumString,
                    Guid = model.Guid,
                    SourceHubItem = model
                };

                return displayObject;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static TimeStampHubItemDisplayObject[] FromModel(TimeStampHubItem[] model)
        {
            const string failure = "Populating TimeStampHubItemDisplayObject[]";
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

        public static SearchQueryItem ToSearchQuerySuggestionItem(TimeStampHubItemDisplayObject displayObject)
        {
            if (displayObject == null)
                return null;

            var answer = new SearchQueryItem(0, displayObject.Guid, ToLabel(displayObject));

            return answer;
        }

        public static SearchQueryItem[] ToSearchQuerySuggestionItem(TimeStampHubItemDisplayObject[] displayObject)
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

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForTimeStampHubItemAsDisplayObject()
        {
            // Note 1: this is the format that appears in the datagrid on the TimeStampEditPage. 

            // Note 2: On the TimeStampCreateNewPage we use a subset of these columns because the others are irrelevant or semantically duplicated. 
            // For the create columns, we use a ListView not a DataGrid - see TimeStampCreateNewPage and DataTemplateForNewlyCreatedTimeStamps.
            // At time of writing the template excludes MiddleInitial, DnxSymbol, MustDitchOriginatingItem, TouchedBy, WhenTouched, and Comment.
            // Of these, DnxSymbol and MustDitchOriginatingItem are amalgamated into TimeStamp field.

            var template = new List<ColumnSpecificationItem>
            {
                new(HubItemDtoNames.XeClickCounter, "Tap", "ClickCounter"),
                new(TimeStampHubItemDto.XeTimeStamp, "Timestamp", "TimeStamp"),

                new(ParticipantHubItemDto.XeFirstName, "First", "FirstName"),
                new(ParticipantHubItemDto.XeMiddleInitial, "M", "MiddleInitial"),
                new(ParticipantHubItemDto.XeLastName, "Last", "LastName"),
                new(HubItemDtoNames.XeBib, "Bib", "Bib"),
                new(HubItemDtoNames.XeBib, "rfid", "Rfid"),
                new(TimeStampHubItemDto.XeDnxSymbol, "Dnx", "DnxSymbol"),
                new(ParticipantHubItemDto.XeRace, "RaceGroup", "RaceGroup"),

                new(HubItemDtoNames.XeRecordingModeEnum, "Signal", "RecordingModeEnum"),
                new(HubItemDtoNames.XeMustDitchOriginatingItem, "Ditch", "MustDitchOriginatingItem"),
                new(HubItemDtoNames.XeDatabaseActionEnum, "Action", "DatabaseActionEnum"),
                new(HubItemDtoNames.XeGuid, "ThisGuid", "DisplayVersionOfSourceItemGuid"),
                new(HubItemDtoNames.XeOriginatingItemGuid, "OriginGuid", "DisplayVersionOfSourceItemOriginatingItemGuid"),
                new(HubItemDtoNames.XeTouchedBy, "Author", "TouchedBy"),
                new(HubItemDtoNames.XeWhenTouched, "WhenEntered", "WhenTouched"),
                new(HubItemDtoNames.XeIsStillToBePushed, "Push", "IsStillToBePushed"),
                new(HubItemDtoNames.XeIsStillToBeBackedUp, "Save", "IsStillToBeBackedUp"),
                new(HubItemDtoNames.XeComment, "Comment", "Comment")
            };
            return template;
        }

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForTimeStampHubItemAsAbridgedDisplayObject()
        {
            // Note 1: this is the format that appears in the datagrid on the TimeStampEditPage. 

            var template = new List<ColumnSpecificationItem>
            {
                new(HubItemDtoNames.XeClickCounter, "Tap", "ClickCounter"),
                new(TimeStampHubItemDto.XeTimeStamp, "Timestamp", "TimeStamp"),

                new(ParticipantHubItemDto.XeFirstName, "First", "FirstName"),
                //new(ParticipantHubItemSerialiserNames.MiddleInitial, "M", "MiddleInitial"),
                new(ParticipantHubItemDto.XeLastName, "Last", "LastName"),
                new(HubItemDtoNames.XeBib, "Bib", "Bib"),
                //new(HubItemDto.XeBib, "rfid", "Rfid"),
                new(TimeStampHubItemDto.XeDnxSymbol, "Dnx", "DnxSymbol"),
                new(ParticipantHubItemDto.XeRace, "RaceGroup", "RaceGroup"),

                //new(HubItemBaseSerialiserNames.DatabaseActionEnum, "Action", "DatabaseActionEnum"),
                new(HubItemDtoNames.XeRecordingModeEnum, "Signal", "RecordingModeEnum"),
                new(HubItemDtoNames.XeMustDitchOriginatingItem, "Ditch", "MustDitchOriginatingItem"),
                //new(HubItemBaseSerialiserNames.OriginatingItemGuid, "Origin Guid", "DisplayVersionOfSourceItemOriginatingItemGuid"),
                //new(HubItemBaseSerialiserNames.Guid, "This Guid", "DisplayVersionOfSourceItemGuid"),
                new(HubItemDtoNames.XeTouchedBy, "Author", "TouchedBy"),
                //new(HubItemBaseSerialiserNames.WhenTouched, "When entered", "WhenTouched"),
                //new(HubItemBaseSerialiserNames.IsStillToBePushed, "Push", "IsStillToBePushed"),
                //new(HubItemBaseSerialiserNames.IsStillToBeBackedUp, "Save", "IsStillToBeBackedUp"),
                new(HubItemDtoNames.XeComment, "Comment", "Comment")
            };
            return template;
        }

        public TimeStampHubItemDisplayObject ToShallowMemberwiseClone()
        {
            var clone = (TimeStampHubItemDisplayObject) MemberwiseClone();

            return clone;
        }

        public static string ToLabel(TimeStampHubItemDisplayObject displayObject)
        {
            if (displayObject == null)
                return string.Empty;

            var label = JghString.ConcatWithSeparator(" ",
                displayObject.ClickCounter,
                displayObject.Bib,
                displayObject.Rfid,
                displayObject.TimeStamp,
                displayObject.DnxSymbol,

                displayObject.FirstName,
                displayObject.LastName,
                displayObject.RaceGroup,
                displayObject.RecordingModeEnum,
                displayObject.MustDitchOriginatingItem,
                displayObject.DatabaseActionEnum,
                //displayObject.DisplayVersionOfSourceItemOriginatingItemGuid,
                displayObject.DisplayVersionOfSourceItemGuid
                //displayObject.IsStillToBePushed,
                //displayObject.IsStillToBeBackedUp,
                //displayObject.Comment
            );

            return label;
        }

        #endregion
    }
}