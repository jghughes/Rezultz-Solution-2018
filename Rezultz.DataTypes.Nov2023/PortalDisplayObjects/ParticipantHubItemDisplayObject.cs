using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.PortalDisplayObjects;

public class ParticipantHubItemDisplayObject : HubItemDisplayObjectBase
{
    #region props

    public string Gender { get; private set; } = string.Empty;

    public string BirthYear { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public string Team { get; } = string.Empty;

    public string IsSeries { get; private set; }

    public string Series { get; private set; } = string.Empty;

    public string EventIdentifiers { get; private set; } = string.Empty;

    public string DateOfRaceGroupTransition { get; set; } = string.Empty;

    public string Rfid { get; set; } = string.Empty;

    public string Reservation { get; set; } = string.Empty;

    #endregion

    #region methods

    public static ParticipantHubItemDisplayObject FromModel(ParticipantHubItem model)
    {
        const string failure = "Formatting ParticipantHubItemDisplayObject.";
        const string locus = "[FromModel]";

        const string ellipsis = "...";

        try
        {
            model ??= new ParticipantHubItem();

            var displayObject = new ParticipantHubItemDisplayObject
            {
                ClickCounter = JghString.ToStringMin3(model.ClickCounter),
                Bib = JghString.RightAlign(model.Bib, 4, ' '),
                Rfid = model.Rfid,

                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                Gender = model.Gender,
                BirthYear = model.BirthYear.ToString(),
                City = model.City,
                IsSeries = JghString.BooleanTrueToSeriesOrOneOff(model.IsSeries),
                Series = model.Series,
                EventIdentifiers = model.EventIdentifiers,
                Reservation = model.Reservation,
                DatabaseActionEnum = model.DatabaseActionEnum,
                MustDitchOriginatingItem = JghString.BooleanTrueToDitchOrBlank(model.MustDitchOriginatingItem),
                IsStillToBeBackedUp = JghString.BooleanTrueToSaveOrBlank(model.IsStillToBeBackedUp),
                IsStillToBePushed = JghString.BooleanTrueToPushOrBlank(model.IsStillToBePushed),
                TouchedBy = model.TouchedBy,
                TimeStamp = JghDateTime.ToTimeLocalhhmmssf(model.TimeStampBinaryFormat),
                WhenTouched = JghDateTime.ToDateLocalSortable(model.WhenTouchedBinaryFormat),
                WhenPushed = JghDateTime.ToDateLocalSortable(model.WhenPushedBinaryFormat),
                DisplayVersionOfSourceItemOriginatingItemGuid = JghString.Substring(0, 4, model.OriginatingItemGuid) + ellipsis,
                DisplayVersionOfSourceItemGuid = JghString.Substring(0, 4, model.Guid) + ellipsis,
                Comment = model.Comment,
                ID = model.ID,
                Label = model.Label,
                EnumString = model.EnumString,
                Guid = model.Guid,
                SourceHubItem = model
            };

            if (string.IsNullOrWhiteSpace(model.RaceGroupAfterTransition) || model.RaceGroupAfterTransition == model.RaceGroupBeforeTransition)
            {
                displayObject.RaceGroup = model.RaceGroupBeforeTransition;
                displayObject.DateOfRaceGroupTransition = string.Empty;
            }
            else
            {
                displayObject.RaceGroup = JghString.Concat(model.RaceGroupBeforeTransition, " -> ", model.RaceGroupAfterTransition);
                displayObject.DateOfRaceGroupTransition = model.DateOfRaceGroupTransition.ToShortDateString();
            }

            return displayObject;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    public static ParticipantHubItemDisplayObject[] FromModel(ParticipantHubItem[] model)
    {
        const string failure = "Populating ParticipantHubItemDisplayObject[].";
        const string locus = "[FromModel]";

        try
        {
            if (model == null)
                return Array.Empty<ParticipantHubItemDisplayObject>();

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

    public static SearchQueryItem ToSearchQuerySuggestionItem(ParticipantHubItemDisplayObject displayObject)
    {
        if (displayObject == null)
            return null;

        var answer = new SearchQueryItem(0, displayObject.Guid, ToLabel(displayObject));

        return answer;
    }

    public static SearchQueryItem[] ToSearchQuerySuggestionItem(ParticipantHubItemDisplayObject[] displayObject)
    {
        const string failure = "Populating SearchQueryItem[].";
        const string locus = "[ToSearchQuerySuggestionItem]";

        try
        {
            if (displayObject == null)
                return Array.Empty<SearchQueryItem>();

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

    public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForRawParticipantEntriesAsDisplayObjects()
    {
        var template = new List<ColumnSpecificationItem>
        {
            new(HubItemDtoNames.XeClickCounter, "Tap", "ClickCounter"),
            new(ParticipantHubItemDto.XeFirstName, "First", "FirstName"),
            new(ParticipantHubItemDto.XeMiddleInitial, "M", "MiddleInitial"),
            new(ParticipantHubItemDto.XeLastName, "Last", "LastName"),
            new(HubItemDtoNames.XeBib, "Bib", "Bib"),
            new(ParticipantHubItemDto.XeRfid, "rfid", "Rfid"),
            new(ParticipantHubItemDto.XeGender, "Gender", "Gender"),
            new(ParticipantHubItemDto.XeBirthYear, "Birth", "BirthYear"),
            new(ParticipantHubItemDto.XeCity, "City", "City"),

            new(ParticipantHubItemDto.XeRace, "RaceGroup", "RaceGroup"),
            new(ParticipantHubItemDto.XeDateOfRaceGroupTransition, "Transition", "DateOfRaceGroupTransition"),

            new(ParticipantHubItemDto.XeIsSeries, "Registration", "IsSeries"),
            new(ParticipantHubItemDto.XeSeries, "Series", "Series"),
            new(ParticipantHubItemDto.XeEventIdentifiers, "Events", "EventIdentifiers"),


            new(ParticipantHubItemDto.XeReservation, "Reservation", "Reservation"),


            new(HubItemDtoNames.XeMustDitchOriginatingItem, "Ditch?", "MustDitchOriginatingItem"),
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

    public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForParticipantHubItemAsAbridgedDisplayObject()
    {
        var template = new List<ColumnSpecificationItem>
        {
            //new(HubItemBaseSerialiserNames.ClickCounter, "Tap", "ClickCounter"),
            new(ParticipantHubItemDto.XeFirstName, "First", "FirstName"),
            new(ParticipantHubItemDto.XeMiddleInitial, "M", "MiddleInitial"),
            new(ParticipantHubItemDto.XeLastName, "Last", "LastName"),
            new(HubItemDtoNames.XeBib, "Bib", "Bib"),
            new(ParticipantHubItemDto.XeGender, "Gender", "Gender"),
            new(ParticipantHubItemDto.XeBirthYear, "Birth", "BirthYear"),
            new(ParticipantHubItemDto.XeCity, "City", "City"),

            new(ParticipantHubItemDto.XeRace, "RaceGroup", "RaceGroup"),
            new(ParticipantHubItemDto.XeDateOfRaceGroupTransition, "Transition", "DateOfRaceGroupTransition"),

            new(ParticipantHubItemDto.XeIsSeries, "Registration", "IsSeries"),
            //new(ParticipantHubItemSerialiserNames.Series, "Series", "Series"),
            //new(ParticipantHubItemSerialiserNames.EventIdentifiers, "Events", "EventIdentifiers"),
            new(HubItemDtoNames.XeMustDitchOriginatingItem, "Ditch?", "MustDitchOriginatingItem"),
            //new(HubItemBaseSerialiserNames.DatabaseActionEnum, "Action", "DatabaseActionEnum"),

            //new(HubItemBaseSerialiserNames.Guid, "This Guid", "DisplayVersionOfSourceItemGuid"),
            //new(HubItemBaseSerialiserNames.OriginatingItemGuid, "Origin Guid", "DisplayVersionOfSourceItemOriginatingItemGuid"),
            //new(HubItemBaseSerialiserNames.TouchedBy, "By", "TouchedBy"),
            //new(HubItemBaseSerialiserNames.TimeStamp, "TimeStamp", "TimeStamp"),
            //new(HubItemBaseSerialiserNames.WhenTouched, "Inputted", "WhenTouched"),
            //new(HubItemBaseSerialiserNames.IsStillToBePushed, "Push", "IsStillToBePushed"),
            //new(HubItemBaseSerialiserNames.IsStillToBeBackedUp, "Save", "IsStillToBeBackedUp"),
            new(HubItemDtoNames.XeComment, "Comment", "Comment")
        };

        return template;
    }

    public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForParticipantMasterListAsDisplayObjects()
    {
        // Note: this is same as MakeDataGridColumnSpecificationsForRawParticipantEntriesAsDisplayObjects except it
        // excludes MustDitchOriginatingItem because ditched records aren't included in the master list by definition

        var template = new List<ColumnSpecificationItem>
        {
            //new(HubItemBaseSerialiserNames.ClickCounter, "Tap", "ClickCounter"),
            new(ParticipantHubItemDto.XeFirstName, "First", "FirstName"),
            new(ParticipantHubItemDto.XeMiddleInitial, "M", "MiddleInitial"),
            new(ParticipantHubItemDto.XeLastName, "Last", "LastName"),
            new(HubItemDtoNames.XeBib, "Bib", "Bib"),
            new(ParticipantHubItemDto.XeRfid, "rfid", "Rfid"),
            new(ParticipantHubItemDto.XeGender, "Sex", "Gender"),
            new(ParticipantHubItemDto.XeBirthYear, "Birth", "BirthYear"),
            new(ParticipantHubItemDto.XeCity, "City", "City"),

            new(ParticipantHubItemDto.XeRace, "Race group", "RaceGroup"),
            new(ParticipantHubItemDto.XeDateOfRaceGroupTransition, "Effective", "DateOfRaceGroupTransition"),


            new(ParticipantHubItemDto.XeIsSeries, "Registration", "IsSeries"),
            new(ParticipantHubItemDto.XeSeries, "Series", "Series"),
            new(ParticipantHubItemDto.XeEventIdentifiers, "Events", "EventIdentifiers"),
            new(ParticipantHubItemDto.XeReservation, "Reservation", "Reservation"),

            //new(HubItemBaseSerialiserNames.DatabaseActionEnum, "Action", "DatabaseActionEnum"),

            //new(HubItemBaseSerialiserNames.Guid, "This Guid", "DisplayVersionOfSourceItemGuid"),
            //new(HubItemBaseSerialiserNames.OriginatingItemGuid, "Origin Guid", "DisplayVersionOfSourceItemOriginatingItemGuid"),
            new(HubItemDtoNames.XeTouchedBy, "Author", "TouchedBy"),
            //new(HubItemBaseSerialiserNames.TimeStamp, "TimeStamp", "TimeStamp"),
            //new(HubItemBaseSerialiserNames.WhenTouched, "Inputted", "WhenTouched"),
            new(HubItemDtoNames.XeIsStillToBePushed, "Push", "IsStillToBePushed"),
            new(HubItemDtoNames.XeIsStillToBeBackedUp, "Save", "IsStillToBeBackedUp"),
            new(HubItemDtoNames.XeComment, "Comment", "Comment")
        };

        return template;
    }

    public ParticipantHubItemDisplayObject ToShallowMemberwiseClone()
    {
        var clone = (ParticipantHubItemDisplayObject) MemberwiseClone();

        return clone;
    }

    public static string ToLabel(ParticipantHubItemDisplayObject displayObject)
    {
        if (displayObject == null)
            return string.Empty;

        var label = JghString.ConcatWithSeparator("    ",
            displayObject.ClickCounter,
            displayObject.FirstName,
            displayObject.MiddleInitial,
            displayObject.LastName,
            displayObject.Bib,
            displayObject.Rfid,
            displayObject.Gender,
            displayObject.BirthYear,
            displayObject.City,
            displayObject.RaceGroup,
            displayObject.DateOfRaceGroupTransition,
            displayObject.IsSeries,
            displayObject.Series,
            displayObject.EventIdentifiers,
            displayObject.MustDitchOriginatingItem,
            displayObject.DatabaseActionEnum,
            displayObject.DisplayVersionOfSourceItemGuid,
            displayObject.Reservation
            //displayObject.DisplayVersionOfSourceItemOriginatingItemGuid,
            //displayObject.TouchedBy,
            //displayObject.TimeStamp,
            //displayObject.IsStillToBePushed,
            //displayObject.IsStillToBeBackedUp,
            //displayObject.Comment
        );
        return label;
    }

    #endregion
}