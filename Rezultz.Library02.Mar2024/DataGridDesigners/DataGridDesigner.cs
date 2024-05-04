using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Goodies.Xml.July2018;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.PortalDisplayObjects;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.PortalSplitIntervalItems;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

// ReSharper disable UnusedMember.Local

namespace Rezultz.Library02.Mar2024.DataGridDesigners;

/// <summary>
///     Concept is that this designer is extensible to be able to print/display/publish any/all types of info
///     by simply adding further methods.
/// </summary>
public class DataGridDesigner
{
    private const string Locus2 = nameof(DataGridDesigner);
    private const string Locus3 = "[Rezultz.Library02.Mar2024]";

    #region ctor

    public DataGridDesigner()
    {
        _knownTypesForSerialisation = new[]
        {
            typeof(CboLookupItem),
            typeof(BlobSpecificationItem),
            typeof(EntityLocationItem),
            typeof(OrganizerItem),
            typeof(SeasonProfileItem),
            typeof(SeriesProfileItem),
            typeof(EventProfileItem),
            typeof(EventSettingsItem),
            typeof(ColumnSpecificationItem),
            typeof(RaceSpecificationItem),
            typeof(AgeGroupSpecificationItem),
            typeof(UriItem),
            typeof(ResultItem),
            typeof(ResultItemDisplayObject),
            typeof(DerivedDataItem),
            typeof(IdentityItem),
            typeof(PopulationCohortItem),
            typeof(AzureStorageLocationItem),
            typeof(TimeStampHubItem),
            typeof(SplitIntervalItem),
            typeof(ParticipantHubItem),
            typeof(TimeStampHubItemDisplayObject),
            typeof(ParticipantHubItemDisplayObject),
            typeof(SplitIntervalConsolidationForParticipantDisplayObject),
            typeof(XElement),
            typeof(byte[])
        };

        ZeroiseDesigner();
    }

    #endregion

    #region const

    // - for reflection of property names -  legacy from Silverlight which requires a SomethingViewModel class with an associated Something as a property named "Model." 

    public const string PrefixForViewModelChildPropertyNames = "";
    public const int LengthOfPrefixForViewModelChildPropertyNames = 0;

    private const string XeArrayOfSplitIntervalsForParticipant = "ArrayOfSplitIntervalsForParticipant";
    private const string XeSplitIntervalsForParticipantItem = "SplitIntervalsForParticipant";

    #endregion

    #region parameters

    public const string SpacerBetweenColumnsInPrintVersions = "   ";

    public const string SeparatorBetweenColumnsInGuiVersions = "  ";

    public static readonly string[] LabelsOfColumnsToBeLeftAligned =
    {
        UbiquitousFieldNames.FirstName,
        UbiquitousFieldNames.MiddleInitial,
        UbiquitousFieldNames.LastName,
        UbiquitousFieldNames.FullName,
        ResultDto.XeSex,
        ResultDto.XeCity,
        ResultDto.XeRace,
        ResultDto.XeTeam,

        ParticipantHubItemDto.XeFirstName,
        ParticipantHubItemDto.XeMiddleInitial,
        ParticipantHubItemDto.XeLastName,
        ParticipantHubItemDto.XeGender,
        ParticipantHubItemDto.XeCity,
        ParticipantHubItemDto.XeRace,
        ParticipantHubItemDto.XeIsSeries,
        ParticipantHubItemDto.XeSeries,
        ParticipantHubItemDto.XeEventIdentifiers
    };

    public static readonly string[] LabelsOfColumnsToBeUpperCase =
    {
        UbiquitousFieldNames.FirstName,
        UbiquitousFieldNames.MiddleInitial,
        UbiquitousFieldNames.LastName,
        UbiquitousFieldNames.FullName,

        UbiquitousFieldNames.FirstName,
        UbiquitousFieldNames.MiddleInitial,
        UbiquitousFieldNames.LastName
    };

    public static readonly string[] LabelsOfColumnsToBeNonSpecificCase =
    {
        ResultDto.XeTeam
    };

    #endregion

    #region fields

    private readonly Type[] _knownTypesForSerialisation;

    private SeriesProfileItem _parentSeriesProfile;

    private EventProfileItem _parentEventProfile;

    private string _parentColumnFormatEnum;

    private bool _isSingleEvent = true;

    private bool _resultsMustBeGroupedByRace;


    private MoreInformationItem _localCboListOfMoreInfoCurrentItem; // used only for cohort analyses

    private static Dictionary<int, string> _parentDictionaryOfTxxColumnHeaders;

    private ResultItemDisplayObject[] _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects = Array.Empty<ResultItemDisplayObject>();

    private PopulationCohortItemDisplayObject[] _finalisedDataGridRowsOfPopulationCohortItemDisplayObjects = Array.Empty<PopulationCohortItemDisplayObject>();

    private ParticipantHubItemDisplayObject[] _finalisedDataGridRowsOfParticipantHubItemDisplayObjects = Array.Empty<ParticipantHubItemDisplayObject>();

    private TimeStampHubItemDisplayObject[] _finalisedDataGridRowsOfTimeStampHubItemDisplayObjects = Array.Empty<TimeStampHubItemDisplayObject>();

    private SplitIntervalConsolidationForParticipantDisplayObject[] _finalisedDataGridRowsOfSplitIntervalsPerPersonDisplayObjects = Array.Empty<SplitIntervalConsolidationForParticipantDisplayObject>();

    #endregion

    #region props

    public MoreInformationItem DatagridTitleAndBlurbInformationItem { get; private set; }

    public bool DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects { get; set; }

    public bool DesignerIsInitialisedForPopulationCohortItemDisplayObjects { get; set; }

    public bool DesignerIsInitialisedForParticipantHubItemDisplayObjects { get; set; }

    public bool DesignerIsInitialisedForTimeStampHubItemDisplayObjects { get; set; }

    public bool DesignerIsInitialisedForSplitIntervalDisplayObjects { get; set; }

    #endregion

    #region methods

    public bool ZeroiseDesigner()
    {
        DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects = false;

        DesignerIsInitialisedForPopulationCohortItemDisplayObjects = false;

        DesignerIsInitialisedForParticipantHubItemDisplayObjects = false;

        DesignerIsInitialisedForTimeStampHubItemDisplayObjects = false;

        DesignerIsInitialisedForSplitIntervalDisplayObjects = false;

        _parentSeriesProfile = new SeriesProfileItem();

        _parentColumnFormatEnum = string.Empty;

        _parentDictionaryOfTxxColumnHeaders = new Dictionary<int, string>();

        _parentEventProfile = new EventProfileItem(); // legitimate to be null in certain circumstances

        DatagridTitleAndBlurbInformationItem = new MoreInformationItem(); // legitimate to be null for leaderboard but not for average EntriesInMemoryCacheTablePresenter times or series standings

        _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects = Array.Empty<ResultItemDisplayObject>();

        _finalisedDataGridRowsOfPopulationCohortItemDisplayObjects = Array.Empty<PopulationCohortItemDisplayObject>();

        _finalisedDataGridRowsOfParticipantHubItemDisplayObjects = Array.Empty<ParticipantHubItemDisplayObject>();

        _finalisedDataGridRowsOfTimeStampHubItemDisplayObjects = Array.Empty<TimeStampHubItemDisplayObject>();

        _finalisedDataGridRowsOfSplitIntervalsPerPersonDisplayObjects = Array.Empty<SplitIntervalConsolidationForParticipantDisplayObject>();


        return true;
    }

    public bool InitialiseDesigner(SeriesProfileItem theSeriesProfile, EventProfileItem theEventProfile, string columnFormatEnum, ResultItemDisplayObject[] rowCollection, MoreInformationItem dataGridTitleAndBlurb,
        Dictionary<int, string> dictionaryOfTxxColumnHeaders)
    {
        theSeriesProfile ??= new SeriesProfileItem();
        columnFormatEnum ??= string.Empty;
        dictionaryOfTxxColumnHeaders ??= new Dictionary<int, string>();
        rowCollection ??= Array.Empty<ResultItemDisplayObject>();

        DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects = false;

        _parentSeriesProfile = theSeriesProfile;

        _parentEventProfile = theEventProfile; // legitimate to be null in certain circumstances

        _parentColumnFormatEnum = columnFormatEnum;

        DatagridTitleAndBlurbInformationItem = dataGridTitleAndBlurb; // legitimate to be null for leaderboard but not for average Split times or series standings

        _parentDictionaryOfTxxColumnHeaders = dictionaryOfTxxColumnHeaders;

        LoadRowItemsForLeaderboardOfResultItemDisplayObjects(rowCollection.ToArray());

        return true;
    }

    public bool InitialiseDesigner(SeriesProfileItem theSeriesProfile, EventProfileItem theEventProfile, string columnFormatEnum, PopulationCohortItemDisplayObject[] rowCollection, MoreInformationItem datagridTitleAndBlurb,
        MoreInformationItem cboListOfMoreInfoCurrentItem)
    {
        theSeriesProfile ??= new SeriesProfileItem();
        columnFormatEnum ??= string.Empty;

        DesignerIsInitialisedForPopulationCohortItemDisplayObjects = false;

        _parentSeriesProfile = theSeriesProfile;

        _parentEventProfile = theEventProfile; // legitimate to be null in certain circumstances

        _parentColumnFormatEnum = columnFormatEnum;

        DatagridTitleAndBlurbInformationItem = datagridTitleAndBlurb; // legitimate to be null for leaderboard but not for average Split times or series standings

        _localCboListOfMoreInfoCurrentItem = cboListOfMoreInfoCurrentItem;

        LoadRowItemsForPopulationCohortItemDisplayObjects(rowCollection);

        return true;
    }

    public bool InitialiseDesigner(SeriesProfileItem theSeriesProfile, EventProfileItem theEventProfile, string columnFormatEnum, ParticipantHubItemDisplayObject[] rowCollection)
    {
        theSeriesProfile ??= new SeriesProfileItem();
        columnFormatEnum ??= string.Empty;

        rowCollection ??= Array.Empty<ParticipantHubItemDisplayObject>();

        DesignerIsInitialisedForParticipantHubItemDisplayObjects = false;

        _parentSeriesProfile = theSeriesProfile;

        _parentEventProfile = theEventProfile; // legitimate to be null in certain circumstances

        _parentColumnFormatEnum = columnFormatEnum;

        LoadRowItemsForParticipantHubItemDisplayObjects(rowCollection);

        return true;
    }

    public bool InitialiseDesigner(SeriesProfileItem theSeriesProfile, EventProfileItem theEventProfile, string columnFormatEnum, TimeStampHubItemDisplayObject[] rowCollection)
    {
        theSeriesProfile ??= new SeriesProfileItem();
        columnFormatEnum ??= string.Empty;

        DesignerIsInitialisedForTimeStampHubItemDisplayObjects = false;

        _parentSeriesProfile = theSeriesProfile;

        _parentEventProfile = theEventProfile; // legitimate to be null in certain circumstances

        _parentColumnFormatEnum = columnFormatEnum;

        LoadRowItemsForTimeStampHubItemDisplayObjects(rowCollection);

        return true;
    }

    public bool InitialiseDesigner(SeriesProfileItem theSeriesProfile, EventProfileItem theEventProfile, string columnFormatEnum, SplitIntervalConsolidationForParticipantDisplayObject[] rowCollection)
    {
        theSeriesProfile ??= new SeriesProfileItem();
        columnFormatEnum ??= string.Empty;

        DesignerIsInitialisedForSplitIntervalDisplayObjects = false;

        _parentSeriesProfile = theSeriesProfile;

        _parentColumnFormatEnum = columnFormatEnum;

        _parentEventProfile = theEventProfile; // legitimate to be null in certain circumstances

        LoadRowItemsForSplitIntervalsDisplayObjects(rowCollection);

        return true;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForLeaderboardOfResultItemDisplayObjects()
    {
        if (!DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(GenerateColumnsTemplate(
                    ResultItemDisplayObject.MakeDataGridColumnSpecificationsForResultsLeaderboard(_parentColumnFormatEnum, _parentDictionaryOfTxxColumnHeaders)),
                _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForConciseLeaderboardOfResultItemDisplayObjects()
    {
        if (!DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(GenerateColumnsTemplate(
                    ResultItemDisplayObject.MakeDataGridColumnSpecificationsForConciseLeaderboard(_parentColumnFormatEnum, _parentDictionaryOfTxxColumnHeaders)),
                _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForPopulationCohortItemDisplayObjects()
    {
        if (!DesignerIsInitialisedForPopulationCohortItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(GenerateColumnsTemplate(PopulationCohortItemDisplayObject.DataGridColumnsToBeDisplayed()), _finalisedDataGridRowsOfPopulationCohortItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForParticipantHubItemDisplayObjects()
    {
        if (!DesignerIsInitialisedForParticipantHubItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(GenerateColumnsTemplate(ParticipantHubItemDisplayObject.MakeDataGridColumnSpecificationsForRawParticipantEntriesAsDisplayObjects()),
                _finalisedDataGridRowsOfParticipantHubItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForAbridgedParticipantHubItemDisplayObjects()
    {
        if (!DesignerIsInitialisedForParticipantHubItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(GenerateColumnsTemplate(ParticipantHubItemDisplayObject.MakeDataGridColumnSpecificationsForParticipantHubItemAsAbridgedDisplayObject()),
                _finalisedDataGridRowsOfParticipantHubItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForParticipantHubItemMasterList()
    {
        if (!DesignerIsInitialisedForParticipantHubItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(GenerateColumnsTemplate(ParticipantHubItemDisplayObject.MakeDataGridColumnSpecificationsForParticipantMasterListAsDisplayObjects()),
                _finalisedDataGridRowsOfParticipantHubItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForTimeStampHubItemDisplayObjects()
    {
        if (!DesignerIsInitialisedForTimeStampHubItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(
                GenerateColumnsTemplate(TimeStampHubItemDisplayObject.MakeDataGridColumnSpecificationsForTimeStampHubItemAsDisplayObject()),
                _finalisedDataGridRowsOfTimeStampHubItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForAbridgedTimeStampHubItemDisplayObjects()
    {
        if (!DesignerIsInitialisedForTimeStampHubItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(
                GenerateColumnsTemplate(TimeStampHubItemDisplayObject.MakeDataGridColumnSpecificationsForTimeStampHubItemAsAbridgedDisplayObject()),
                _finalisedDataGridRowsOfTimeStampHubItemDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForSplitIntervalsPerPersonDisplayObjects()
    {
        if (!DesignerIsInitialisedForSplitIntervalDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(
                GenerateColumnsTemplate(SplitIntervalConsolidationForParticipantDisplayObject.MakeDataGridColumnSpecificationsForSplitIntervalsPerParticipantAsDisplayObject()),
                _finalisedDataGridRowsOfSplitIntervalsPerPersonDisplayObjects);

        return activeColumns;
    }

    public ColumnSpecificationItem[] GetNonEmptyColumnSpecificationItemsForAbridgedSplitIntervalsPerPersonDisplayObjects()
    {
        if (!DesignerIsInitialisedForSplitIntervalDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        var activeColumns =
            ExtractColumnSpecificationItemsAssociatedWithActiveContent(
                GenerateColumnsTemplate(SplitIntervalConsolidationForParticipantDisplayObject.MakeDataGridColumnSpecificationsForSplitIntervalsPerParticipantAsAbridgedDisplayObject()),
                _finalisedDataGridRowsOfSplitIntervalsPerPersonDisplayObjects);

        return activeColumns;
    }


    public static ColumnSpecificationItem[] GenerateColumnsTemplate(List<ColumnSpecificationItem> columnsToBeDisplayed)
    {
        const string failure = "Unable to generate column collection.";
        const string locus = "[GenerateColumnsTemplate]";

        try
        {
            if (columnsToBeDisplayed == null)
                return new ColumnSpecificationItem[] { };

            columnsToBeDisplayed = columnsToBeDisplayed.Where(z => z != null).ToList();

            // generate fully qualified property names for accessing viewmodels later on using reflection

            foreach (var item in columnsToBeDisplayed)
                item.NameOfAssociatedPropertyInXamlBindingSyntax = string.Concat(PrefixForViewModelChildPropertyNames, item.NameOfAssociatedPropertyInXamlBindingSyntax);

            // do cell text formatting

            var answer = columnsToBeDisplayed
                .Where(z => z != null)
                .Select(z => DoTextFormattingSpecifications(z, LabelsOfColumnsToBeLeftAligned, LabelsOfColumnsToBeUpperCase, LabelsOfColumnsToBeNonSpecificCase));


            return answer.ToArray();
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }


    public async Task<string> GetLeaderboardStyleResultsArrayAsPrettyPrintedWebPageOrTextFileAsync(bool mustReturnPreFormattedBodyTextOnly)
    {
        const string failure = "Unable to obtain results in the form of a html web document or text file.";
        const string locus = "[GetLeaderboardStyleResultsArrayAsPrettyPrintedWebPageOrTextFileAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null || _parentEventProfile == null || !DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects)
                throw new JghAlertMessageException("Designer not yet initialised. No results displayed.");

            #endregion

            #region initialise titles

            var seriesTitle = $"Series: {_parentSeriesProfile.Title.ToUpper()}";

            var eventTitle = $"Event:  {_parentEventProfile.Title.ToUpper()}";

            var title1 = string.Empty;

            var title2 = string.Empty;

            var subjectMatter = string.Empty;

            var narrativeBlurb = string.Empty;

            #endregion

            #region populate variables for PrintResultsAsLeaderboardStyleWebPage

            switch (_parentColumnFormatEnum)
            {
                case EnumStrings.SingleEventResultsColumnFormat:

                    #region populate variables for this columnformat

                    // ReSharper disable once RedundantAssignment
                    _isSingleEvent = true;

                    _resultsMustBeGroupedByRace = true;

                    title1 = seriesTitle;

                    title2 = eventTitle;

                    subjectMatter = "RESULTS";

                    narrativeBlurb = string.Empty;

                    #endregion

                    break;

                case EnumStrings.AverageSplitTimesColumnFormat:

                    #region populate variables for this columnformat

                    // ReSharper disable once RedundantAssignment
                    _isSingleEvent = true;

                    // ReSharper disable once RedundantAssignment
                    _resultsMustBeGroupedByRace = false;

                    title1 = seriesTitle;

                    title2 = eventTitle;

                    if (DatagridTitleAndBlurbInformationItem != null)
                    {
                        subjectMatter = DatagridTitleAndBlurbInformationItem.Title;

                        subjectMatter = subjectMatter?.ToUpper();

                        narrativeBlurb = string.Format(DatagridTitleAndBlurbInformationItem.Blurb);
                    }

                    #endregion

                    break;

                case EnumStrings.SeriesTotalPointsColumnFormat:

                    #region populate variables for this columnformat

                    _isSingleEvent = false;

                    _resultsMustBeGroupedByRace = true;

                    title1 = seriesTitle;

                    //title2 = seriesSubtitle;

                    if (DatagridTitleAndBlurbInformationItem != null)
                    {
                        subjectMatter = DatagridTitleAndBlurbInformationItem.Title;

                        subjectMatter = subjectMatter?.ToUpper();

                        narrativeBlurb = string.Format(
                            DatagridTitleAndBlurbInformationItem.Blurb,
                            _parentSeriesProfile.NumOfScoresToCountForSeriesTotalPoints);
                    }

                    #endregion

                    break;

                case EnumStrings.SeriesTourDurationColumnFormat:

                    #region populate variables for this columnformat

                    _isSingleEvent = false;

                    _resultsMustBeGroupedByRace = true;

                    title1 = seriesTitle;

                    //title2 = seriesSubtitle;

                    if (DatagridTitleAndBlurbInformationItem != null)
                    {
                        subjectMatter = DatagridTitleAndBlurbInformationItem.Title;

                        subjectMatter = subjectMatter?.ToUpper();

                        narrativeBlurb = string.Format(DatagridTitleAndBlurbInformationItem.Blurb);
                    }

                    #endregion

                    break;
            }

            #endregion

            #region generate html wrapping containing text contents or text contents only

            string htmlAsString;

            if (mustReturnPreFormattedBodyTextOnly)
                htmlAsString = await GenerateAbridgedLeaderboardStyleResultsAsPreFormattedTextAsync(
                    title1,
                    title2,
                    subjectMatter,
                    narrativeBlurb,
                    _resultsMustBeGroupedByRace,
                    _isSingleEvent);
            else
                htmlAsString = await GenerateLeaderboardStyleResultsAsWebPageAsync(
                    title1,
                    title2,
                    subjectMatter,
                    narrativeBlurb,
                    _resultsMustBeGroupedByRace,
                    _isSingleEvent);

            return htmlAsString ?? string.Empty;

            #endregion
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public async Task<XElement> GetLeaderboardStyleResultsArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat desiredFormatEnumEnum)
    {
        const string failure = "Unable to transform results into a flat file of xml data.";
        const string locus = "[GetLeaderboardAsResultItemViewModelsAsXmlAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null || _parentEventProfile == null || !DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects)
                throw new JghAlertMessageException("Designer not yet initialised. No results displayed.");

            #endregion

            #region generate parameters for the file

            var seriesTitle = $"{_parentSeriesProfile.Title.ToUpper()}";

            string title1;

            var subjectMatter = string.Empty;

            var narrativeBlurb = string.Empty;

            ColumnSpecificationItem[] columnSpecifications;

            const string nameToBeUsedForParentXe = ResultDto.XeArrayOfResult;

            ResultItemDisplayObject[] displayObjects;

            switch (_parentColumnFormatEnum)
            {
                case EnumStrings.SingleEventResultsColumnFormat:

                    #region prepare text for titles, subject matter and narrative blurb

                    title1 = seriesTitle;

                    subjectMatter = "RESULTS";

                    narrativeBlurb = string.Empty;

                    #endregion

                    displayObjects = _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects
                        .Where(z => z != null)
                        .OrderBy(z => z.RaceGroup)
                        .ThenBy(z => z.PlaceCalculatedOverallAsString).ToArray();

                    break;

                case EnumStrings.AverageSplitTimesColumnFormat:

                    #region prepare text for titles, subject matter and narrative blurb

                    title1 = seriesTitle;

                    if (DatagridTitleAndBlurbInformationItem != null)
                    {
                        subjectMatter = DatagridTitleAndBlurbInformationItem.Title;

                        subjectMatter = subjectMatter?.ToUpper();

                        narrativeBlurb = string.Format(DatagridTitleAndBlurbInformationItem.Blurb);
                    }

                    #endregion

                    displayObjects = _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects
                        .Where(z => z != null)
                        .OrderBy(z => z.PlaceCalculatedOverallAsString).ToArray();

                    break;

                case EnumStrings.SeriesTotalPointsColumnFormat:

                    #region prepare text for titles, subject matter and narrative blurb

                    title1 = seriesTitle;

                    if (DatagridTitleAndBlurbInformationItem != null)
                    {
                        subjectMatter = DatagridTitleAndBlurbInformationItem.Title;

                        subjectMatter = subjectMatter?.ToUpper();

                        narrativeBlurb = string.Format(
                            DatagridTitleAndBlurbInformationItem.Blurb,
                            _parentSeriesProfile.NumOfScoresToCountForSeriesTotalPoints);
                    }

                    #endregion

                    displayObjects = _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects
                        .Where(z => z != null)
                        .OrderBy(z => z.RaceGroup)
                        .ThenByDescending(z => z.PointsCalculatedRank).ToArray();

                    break;

                case EnumStrings.SeriesTourDurationColumnFormat:

                    #region prepare text for titles, subject matter and narrative blurb

                    title1 = seriesTitle;

                    if (DatagridTitleAndBlurbInformationItem != null)
                    {
                        subjectMatter = DatagridTitleAndBlurbInformationItem.Title;

                        subjectMatter = subjectMatter?.ToUpper();

                        narrativeBlurb = string.Format(DatagridTitleAndBlurbInformationItem.Blurb);
                    }

                    #endregion

                    displayObjects = _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects
                        .Where(z => z != null)
                        .OrderBy(z => z.RaceGroup)
                        .ThenByDescending(z => z.TotalDurationFromAlgorithmInSecondsRank).ToArray();
                    break;

                default:
                    throw new Exception(
                        "The desired columns for the xml data is unspecified or unrecognised. Sorry. Coding error.");
            }

            columnSpecifications = desiredFormatEnumEnum switch
            {
                EnumForXmlDataExportFormat.SameAsGuiLayout => GenerateColumnsTemplate(ResultItemDisplayObject.MakeDataGridColumnSpecificationsForResultsLeaderboard(_parentColumnFormatEnum, _parentDictionaryOfTxxColumnHeaders)),
                _ => throw new Exception("The desired layout for the xml file is unspecified or unrecognised. Sorry. Coding error.")
            };

            #endregion

            #region generate parentArrayOfResultItemsXe;

            var parentArrayOfResultItemsXe = await
                GenerateLeaderboardStyleResultsAsSimpleFlatXmlFileAsync(displayObjects, columnSpecifications, title1, subjectMatter, narrativeBlurb, nameToBeUsedForParentXe);

            #endregion

            return parentArrayOfResultItemsXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public async Task<XElement> GetParticipantHubItemArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat desiredFormatEnumEnum)
    {
        const string failure = "Unable to transform participant hub item array into flat file of xml data.";
        const string locus = "[GetParticipantHubItemArrayAsXmlFileContentsAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null || _parentEventProfile == null || !DesignerIsInitialisedForParticipantHubItemDisplayObjects)
                throw new JghAlertMessageException("Designer not yet initialised. No results displayed.");

            #endregion

            #region generate parameters for the file

            var seriesTitle = $"{_parentSeriesProfile.Title.ToUpper()}";

            const string nameToBeUsedForParentXe = ParticipantHubItemDto.XeArrayOfParticipant;

            #region prepare text for titles, subject matter and narrative blurb

            var title1 = seriesTitle;

            var subjectMatter = "PARTICIPANTS";

            var narrativeBlurb = string.Empty;

            #endregion

            var displayObjects = _finalisedDataGridRowsOfParticipantHubItemDisplayObjects
                .Where(z => z != null)
                .OrderBy(z => z.Identifier)
                .ThenBy(z => z.LastName).ToArray();

            var columnSpecifications = desiredFormatEnumEnum switch
            {
                EnumForXmlDataExportFormat.SameAsGuiLayout => GenerateColumnsTemplate(ParticipantHubItemDisplayObject.MakeDataGridColumnSpecificationsForRawParticipantEntriesAsDisplayObjects()),
                _ => throw new Exception("Layout for the columns in the xml file is unspecified or unrecognised. Sorry. Coding error.")
            };

            #endregion

            #region generate parentArrayOfItemsXe;

            var parentArrayOfItemsXe = await
                GenerateParticipantHubItemDisplayObjectsAsSimpleFlatXmlFileAsync(displayObjects, columnSpecifications,
                    title1,
                    subjectMatter, narrativeBlurb, nameToBeUsedForParentXe);

            #endregion

            return parentArrayOfItemsXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public async Task<XElement> GetTimeStampHubItemArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat desiredFormatEnumEnum)
    {
        const string failure = "Unable to transform timestamp hub item array into flat file of xml data.";
        const string locus = "[GetTimeStampHubItemArrayAsXmlFileContentsAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null || _parentEventProfile == null || !DesignerIsInitialisedForTimeStampHubItemDisplayObjects)
                throw new JghAlertMessageException("Designer not yet initialised. No results displayed.");

            #endregion

            #region generate parameters for the file

            var seriesTitle = $"{_parentSeriesProfile.Title.ToUpper()}";

            const string nameToBeUsedForParentXe = TimeStampHubItemDto.XeArrayOfTimeStamp;

            #region prepare text for titles, subject matter and narrative blurb

            var title1 = seriesTitle;

            var subjectMatter = "TimeStamps";

            var narrativeBlurb = string.Empty;

            #endregion

            var displayObjects = _finalisedDataGridRowsOfTimeStampHubItemDisplayObjects
                .Where(z => z != null)
                .OrderBy(z => z.TimeStamp)
                .ToArray();

            var columnSpecifications = desiredFormatEnumEnum switch
            {
                EnumForXmlDataExportFormat.SameAsGuiLayout => GenerateColumnsTemplate(TimeStampHubItemDisplayObject.MakeDataGridColumnSpecificationsForTimeStampHubItemAsDisplayObject()),
                _ => throw new Exception("Layout for the columns in the xml file is unspecified or unrecognised. Sorry. Coding error.")
            };

            #endregion

            #region generate parentArrayOfItemsXe;

            var parentArrayOfItemsXe = await
                GenerateTimeStampHubItemDisplayObjectsAsSimpleFlatXmlFileAsync(displayObjects, columnSpecifications,
                    title1,
                    subjectMatter, narrativeBlurb, nameToBeUsedForParentXe);

            #endregion

            return parentArrayOfItemsXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public async Task<XElement> GetSplitIntervalsPerPersonItemArrayAsXmlFileContentsAsync(EnumForXmlDataExportFormat desiredFormatEnumEnum)
    {
        const string failure = "Unable to transform the array of split intervals per person into flat file of xml data.";
        const string locus = "[GetSplitIntervalsPerPersonItemArrayAsXmlFileContentsAsync]";


        try
        {
            #region null checks

            if (_parentSeriesProfile == null || _parentEventProfile == null || !DesignerIsInitialisedForSplitIntervalDisplayObjects)
                throw new JghAlertMessageException("Designer not yet initialised. No results displayed.");

            #endregion

            #region generate parameters for the file

            var seriesTitle = $"{_parentSeriesProfile.Title.ToUpper()}";

            const string nameToBeUsedForParentXe = XeArrayOfSplitIntervalsForParticipant;

            #region prepare text for titles, subject matter and narrative blurb

            var title1 = seriesTitle;

            var subjectMatter = "Split intervals for participants";

            var narrativeBlurb = string.Empty;

            #endregion

            var displayObjects = _finalisedDataGridRowsOfSplitIntervalsPerPersonDisplayObjects
                .Where(z => z != null)
                .OrderBy(z => z.RaceGroup)
                .ThenBy(z => z.CalculatedRankOverall)
                .ToArray();

            var columnSpecifications = desiredFormatEnumEnum switch
            {
                EnumForXmlDataExportFormat.SameAsGuiLayout => GenerateColumnsTemplate(SplitIntervalConsolidationForParticipantDisplayObject.MakeDataGridColumnSpecificationsForSplitIntervalsPerParticipantAsDisplayObject()),
                _ => throw new Exception("Layout for the columns in the xml file is unspecified or unrecognised. Sorry. Coding error.")
            };

            #endregion

            #region generate parentArrayOfItemsXe;

            var parentArrayOfItemsXe = await
                GenerateSplitIntervalsForParticipantDisplayObjectsAsSimpleFlatXmlFileAsync(displayObjects, columnSpecifications,
                    title1,
                    subjectMatter, narrativeBlurb, nameToBeUsedForParentXe);

            #endregion

            return parentArrayOfItemsXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }
    
    public XElement GetLeaderboardAsResultItemsAsXml()
    {
        const string failure =
            "Unable to serialise results into unabridged file of xml data by means of System.Runtime.Serialization.DataContractSerializer.";
        const string locus = "[GetLeaderboardAsResultItemsAsXml]";

        try
        {
            if (!DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects)
                throw new InvalidOperationException("Designer is not yet initialised.");

            #region generate results as xml by means of system serialiser

            var seriesParticulars = new CboLookupItem
            {
                Title = _parentSeriesProfile.Title,
                AdvertisedDateTime = _parentSeriesProfile.AdvertisedDate
            };

            var seriesInfoXe = JghSerialisation.ToXElementFromObject(seriesParticulars,
                _knownTypesForSerialisation);

            var eventParticulars = new CboLookupItem
            {
                Title = _parentEventProfile.Title,
                AdvertisedDateTime = _parentEventProfile.AdvertisedDate
            };

            var eventInfoXe = JghSerialisation.ToXElementFromObject(eventParticulars,
                _knownTypesForSerialisation);

            var orderedResults = _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects
                .OrderBy(z => z.RaceGroup)
                .ThenBy(z => z.PlaceCalculatedOverallAsString)
                .Select(z => z)
                .ToArray();

            var resultsItemsWrappedInAParentXe =
                JghSerialisation.ToXElementFromObject(orderedResults, _knownTypesForSerialisation) ?? new XElement("Saved data is null");

            #endregion

            #region make an NTFS compatible sortable timestamp

            var timestamp = DateTime.Now.ToString(JghDateTime.Iso8601Pattern);


            var hopefullyValidTimestamp =
                JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters(
                    '-', timestamp);

            if (
                !JghFilePathValidator.IsValidNtfsFileOrFolderName(hopefullyValidTimestamp,
                    // ReSharper disable once UnusedVariable
                    out var errorMessage))
                throw new JghInvalidValueException(
                    $"Unable to create timestamp for the file. Sorry. Timestamp is invalid. '{hopefullyValidTimestamp}'");

            #endregion

            #region package into xml file to be saved

            var root = new XElement("root");

            var originatingBlobNames = string.Join("++",
                _parentEventProfile.LocationsOfPublishedResultsXmlFiles.Select(z => z.EntityName).ToArray());

            root.ReplaceAttributes(
                new XAttribute("timestamp", hopefullyValidTimestamp),
                new XAttribute("source", originatingBlobNames)
            );

            root.Add(resultsItemsWrappedInAParentXe, eventInfoXe, seriesInfoXe);

            root = JghXElementHelpers.RemoveDescendantsHavingSystemDefaultValues(root);
            // never attempt to load this data into access or excel if you remove descendents (because the outcome will be unreliable), but it saves tons of bandwidth. only use the deserialiser

            #endregion

            return root;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public string GetBlobNameToBeUsedForExportingResultsAsWebpage()
    {
        const string failure = "Unable to obtain a designated blobname for the upload.";
        const string locus = "[GetBlobNameToBeUsedForExportingResultsAsWebpage]";

        try
        {
            if (!DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects)
                throw new InvalidOperationException("Designer is not yet initialised.");

            var answer = _parentColumnFormatEnum switch
            {
                EnumStrings.SingleEventResultsColumnFormat => _parentEventProfile.HtmlDocumentNameForPostedResults,
                EnumStrings.AverageSplitTimesColumnFormat => null, // do nothing. at present we don't intend to publish average lap times
                EnumStrings.SeriesTotalPointsColumnFormat => _parentSeriesProfile.HtmlDocumentNameForSeriesTotalPointsStandings,
                EnumStrings.SeriesTourDurationColumnFormat => _parentSeriesProfile.HtmlDocumentNameForSeriesTourDurationStandings,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(answer))
                throw new JghInvalidValueException(
                    "A blob name is sourced from the metadata file for this series and/or event. It would appear that the pipeline for conveying the blob name to this point has been compromised.");

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public int GetCohortLineItemsCount()
    {
        if (!DesignerIsInitialisedForPopulationCohortItemDisplayObjects)
            throw new InvalidOperationException("Designer is not yet initialised.");

        return _finalisedDataGridRowsOfPopulationCohortItemDisplayObjects.Length;
    }

    public async Task<string> GetCohortAnalysisAsPrettyPrintedWebPageAsync()
    {
        const string failure = "Unable to obtain results in the form of a webpage.";
        const string locus = "[GetLeaderboardStyleResultsAsPrettyPrintedWebPageAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null)
                throw new JghNullObjectInstanceException(nameof(_parentSeriesProfile));

            if (_parentEventProfile == null)
                throw new JghNullObjectInstanceException(nameof(_parentEventProfile));

            if (!DesignerIsInitialisedForPopulationCohortItemDisplayObjects)
                throw new InvalidOperationException("Designer is not yet initialised.");

            #endregion

            #region initialise titles

            var seriesTitle = $"Series: {_parentSeriesProfile.Title.ToUpper()}";

            //var seriesSubtitle = $"        {_parentSeries.Subtitle.ToUpper()}";

            var eventTitle = $"Event:  {_parentEventProfile.Title.ToUpper()}";


            // ReSharper disable once RedundantAssignment
            var title1 = string.Empty;

            // ReSharper disable once RedundantAssignment
            var title2 = string.Empty;

            var title3 = string.Empty;

            var title4 = string.Empty;

            #endregion

            #region switches

            // ReSharper disable once RedundantAssignment
            var isSingleEvent = true;

            #endregion

            #region populate variables for this columnformat

            // ReSharper disable once RedundantAssignment
            isSingleEvent = true;

            title1 = seriesTitle;

            title2 = eventTitle;

            if (DatagridTitleAndBlurbInformationItem != null)
            {
                title3 = string.Format(DatagridTitleAndBlurbInformationItem?.Title ?? string.Empty);

                title3 = title3.ToUpper();
            }

            if (_localCboListOfMoreInfoCurrentItem != null)
            {
                title4 = string.Format(_localCboListOfMoreInfoCurrentItem?.Title ?? string.Empty);

                title4 = title4.ToUpper();
            }

            #endregion

            #region generate webpage

            var htmlWebPageAsString = await GenerateCohortAnalysisAsWebPageAsync(
                title1,
                title2,
                title3,
                title4,
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                isSingleEvent
            ) ?? string.Empty;

            #endregion

            return htmlWebPageAsString;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region helpers

    #region column processing

    private static ColumnSpecificationItem[] ExtractColumnSpecificationItemsAssociatedWithActiveContent<T>(ColumnSpecificationItem[] columnSpecificationItems, T[] dataGridRows) where T : class, new()
    {
        const string failure = "Unable to distil column specifications from source data.";
        const string locus = "[ExtractColumnSpecificationItemsAssociatedWithActiveContent]";

        try
        {
            #region null checks

            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();

            dataGridRows ??= Array.Empty<T>();

            #endregion

            List<ColumnSpecificationItem> list = new();

            foreach (var columnSpecificationItem in columnSpecificationItems)
            {
                var associatedPropertyPath = columnSpecificationItem.NameOfAssociatedPropertyInXamlBindingSyntax;

                foreach (var thisDataGridRowDisplayObject in dataGridRows)
                {
                    if (JghReflectionHelpers.DescendentPropertyIsNullOrValueIsSystemDefault(associatedPropertyPath, thisDataGridRowDisplayObject)) continue;

                    list.Add(columnSpecificationItem);

                    break;
                }
            }

            var inventoryOfActiveColumns = list.ToArray();

            var answer =
                DoColumnWidthRequirementSpecifications(dataGridRows, inventoryOfActiveColumns);

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    private static ColumnSpecificationItem[] DoColumnWidthRequirementSpecifications<T>(T[] dataGridRows, ColumnSpecificationItem[] columnSpecificationItems) where T : class
    {
        const string failure = "Unable to establish required width of each column.";
        const string locus = "[DoColumnWidthRequirementSpecifications]";

        try
        {
            #region null checks

            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();

            dataGridRows ??= Array.Empty<T>();

            var answerAsColumnSpecInventory =
                columnSpecificationItems.Where(z => z != null).Select(z => z.ShallowMemberwiseCloneCopy).ToArray();

            #endregion

            #region erect 2D array as a worksheet of the results corresponding to the originating datagrid

            var numberOfColumns = answerAsColumnSpecInventory.Length;

            var numberOfRows = dataGridRows.Length;

            var workSheetCell = new string[numberOfColumns, numberOfRows];

            #endregion

            #region populate array with cell contents from vms

            var i = 0;

            foreach (var columnSpecItem in answerAsColumnSpecInventory)
            {
                var j = 0;

                foreach (var resultViewModel in dataGridRows)
                {
                    var cellContentAsObject =
                        JghReflectionHelpers.GetDescendentProperty(
                            columnSpecItem.NameOfAssociatedPropertyInXamlBindingSyntax, resultViewModel);

                    var text = string.Empty;

                    if (cellContentAsObject != null)
                        text = cellContentAsObject.ToString();

                    workSheetCell[i, j] = text;

                    j++;
                }

                i++;
            }

            #endregion

            #region determine required cell width of each column to avoid truncation of text. update columnSpecInventory accordingly

            for (var ii = 0; ii < numberOfColumns; ii++)
            {
                var desiredWidthForColumnHeaderText =
                    MeasureLengthOfString(answerAsColumnSpecInventory[ii].ColumnHeaderText);

                var maxCellWidth = desiredWidthForColumnHeaderText; // initialise

                for (var jj = 0; jj < numberOfRows; jj++)
                {
                    var lengthOfTextInThisCell = MeasureLengthOfString(workSheetCell[ii, jj]);

                    maxCellWidth = Math.Max(lengthOfTextInThisCell, maxCellWidth); // update
                    //maxCellWidth = JghMath.Max(lengthOfTextInThisCell, maxCellWidth); // update
                }

                answerAsColumnSpecInventory[ii].ColumnWidthChars = maxCellWidth;
            }

            #endregion

            #region align and pad and do casing of all text in all cells in each column

            for (var ii = 0; ii < numberOfColumns; ii++)
            for (var jj = 0; jj < numberOfRows; jj++)
            {
                var locusOfErrorMsg =
                    $"The current cell has a column index of {ii} and a row index of {jj}. It is embedded in an array of {numberOfColumns} columns and {numberOfRows} rows.";

                workSheetCell[ii, jj] = FormatTextAsPerSpecifications(answerAsColumnSpecInventory[ii],
                    workSheetCell[ii, jj], locusOfErrorMsg, false);
            }

            #endregion

            return answerAsColumnSpecInventory;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    private static string FormatTextAsPerSpecifications(ColumnSpecificationItem columnSpecification, string textInCell, string locusOfCurrentCellForErrorMsg, bool isInHeaderRow)
    {
        #region text casing

        if (isInHeaderRow)
            switch (columnSpecification.CaseOfHeaderTextEnum)
            {
                case EnumStrings.LowerCase:
                    textInCell = textInCell?.ToLower();
                    break;
                case EnumStrings.UpperCase:
                    textInCell = textInCell?.ToUpper();
                    break;
                case EnumStrings.NonSpecificCase:
                    break;
            }
        else
            switch (columnSpecification.CaseOfLineItemTextEnum)
                // in future we can specifiy a different case here for line items
            {
                case EnumStrings.LowerCase:
                    textInCell = textInCell?.ToLower();
                    break;
                case EnumStrings.UpperCase:
                    textInCell = textInCell?.ToUpper();
                    break;
                case EnumStrings.NonSpecificCase:
                    break;
            }

        #endregion

        #region alignment

        var columnWidth = columnSpecification.ColumnWidthChars;

        textInCell ??= string.Empty;

        if (columnWidth - textInCell.Length < 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                $"In this row, the column headed <{columnSpecification.ColumnHeaderText}> and labeled <{columnSpecification.CellXElementName}> contains the characters <{textInCell}>.");
            sb.AppendLine(
                "The number of characters exceeds the allowable width previously determined for the column for formatting purposes.");
            sb.AppendLine(
                $"The allowable width is {columnWidth} characters. The number of characters is {textInCell.Length}.");
            sb.AppendLine(locusOfCurrentCellForErrorMsg);
            throw new JghInvalidValueException(sb.ToString());
        }

        var spacer = new string(' ', columnWidth - textInCell.Length);

        var answerAsAlignedTextInCell = columnSpecification.TextAlignmentEnum switch
        {
            EnumStrings.LeftAlign => string.Concat(textInCell, spacer),
            EnumStrings.RightAlign => string.Concat(spacer, textInCell),
            _ => string.Concat(spacer, textInCell)
        };

        #endregion

        return answerAsAlignedTextInCell;
    }

    private static int MeasureLengthOfString(string theText)
    {
        return string.IsNullOrWhiteSpace(theText) ? 0 : theText.Length;
    }

    #endregion

    #region loading of row items

    private void LoadRowItemsForLeaderboardOfResultItemDisplayObjects(ResultItemDisplayObject[] rowCollection)
    {
        const string failure =
            "Unable to initialise designer with column specifications and results from source data.";
        const string locus = "[LoadRowItemsForLeaderboardOfResultItemDisplayObjects]";

        try
        {
            if (DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects)
                throw new InvalidOperationException(
                    "Designer is already initialised. You are not allowed to initialise it more than once. Program error. Sorry.");

            rowCollection ??= Array.Empty<ResultItemDisplayObject>();

            var dataGridRows = rowCollection.Where(z => z != null).ToArray();
            // be sure to remove nulls here, before we do any array ops

            _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects = dataGridRows;

            DesignerIsInitialisedForLeaderboardOfResultItemDisplayObjects = true;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private void LoadRowItemsForPopulationCohortItemDisplayObjects(PopulationCohortItemDisplayObject[] rowCollection)
    {
        const string failure =
            "Unable to initialise designer with column specifications and results from source data.";
        const string locus = "[LoadRowItemsForPopulationCohortItemDisplayObjects]";

        try
        {
            rowCollection ??= Array.Empty<PopulationCohortItemDisplayObject>();

            if (DesignerIsInitialisedForPopulationCohortItemDisplayObjects)
                throw new InvalidOperationException(
                    "Designer is already initialised. You are not allowed to initialise it more than once. Program error. Sorry.");


            var cohortAnalysisLineItemViewModels = rowCollection.Where(z => z != null).ToArray();
            // be sure to remove nulls here, before we do any array ops

            _finalisedDataGridRowsOfPopulationCohortItemDisplayObjects = cohortAnalysisLineItemViewModels;

            DesignerIsInitialisedForPopulationCohortItemDisplayObjects = true;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private void LoadRowItemsForParticipantHubItemDisplayObjects(ParticipantHubItemDisplayObject[] rowCollection)
    {
        const string failure =
            "Unable to initialise designer with column specifications and results from source data.";
        const string locus = "[LoadRowItemsForParticipantHubItemDisplayObjects]";

        try
        {
            rowCollection ??= Array.Empty<ParticipantHubItemDisplayObject>();

            if (DesignerIsInitialisedForParticipantHubItemDisplayObjects)
                throw new InvalidOperationException(
                    "Designer is already initialised. You are not allowed to initialise it more than once. Program error. Sorry.");

            var lineItemViewModels = rowCollection
                .Where(z => z != null)
                .ToArray();

            _finalisedDataGridRowsOfParticipantHubItemDisplayObjects = lineItemViewModels;

            DesignerIsInitialisedForParticipantHubItemDisplayObjects = true;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private void LoadRowItemsForTimeStampHubItemDisplayObjects(TimeStampHubItemDisplayObject[] rowCollection)
    {
        const string failure =
            "Unable to initialise designer with column specifications and results from source data.";
        const string locus = "[LoadRowItemsForTimeStampHubItemDisplayObjects]";

        try
        {
            rowCollection ??= Array.Empty<TimeStampHubItemDisplayObject>();

            if (DesignerIsInitialisedForTimeStampHubItemDisplayObjects)
                throw new InvalidOperationException(
                    "Designer is already initialised. You are not allowed to initialise it more than once. Program error. Sorry.");

            var lineItemViewModels = rowCollection.Where(z => z != null)
                .ToArray();

            _finalisedDataGridRowsOfTimeStampHubItemDisplayObjects = lineItemViewModels;

            DesignerIsInitialisedForTimeStampHubItemDisplayObjects = true;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private void LoadRowItemsForSplitIntervalsDisplayObjects(SplitIntervalConsolidationForParticipantDisplayObject[] rowCollection)
    {
        const string failure =
            "Unable to initialise designer with column specifications and results from source data.";
        const string locus = "[LoadRowItemsForSplitIntervalsDisplayObjects]";

        try
        {
            rowCollection ??= Array.Empty<SplitIntervalConsolidationForParticipantDisplayObject>();

            if (DesignerIsInitialisedForSplitIntervalDisplayObjects)
                throw new InvalidOperationException(
                    "Designer is already initialised. You are not allowed to initialise it more than once. Program error. Sorry.");

            var lineItemViewModels = rowCollection
                .Where(z => z != null)
                .ToArray();

            _finalisedDataGridRowsOfSplitIntervalsPerPersonDisplayObjects = lineItemViewModels;

            DesignerIsInitialisedForSplitIntervalDisplayObjects = true;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region generating documents

    private async Task<string> GenerateAbridgedLeaderboardStyleResultsAsPreFormattedTextAsync(
        string pageTitle1,
        string pageTitle2,
        string pageTitle3,
        string pageTitle4,
        bool resultsMustBeGroupedByRace,
        bool isJustForSingleEvent)
    {
        const string failure = "Unable to compose html document.";
        const string locus = "[GenerateAbridgedLeaderboardStyleResultsAsPreFormattedTextAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null)
                throw new JghNullObjectInstanceException(nameof(_parentSeriesProfile));

            if (isJustForSingleEvent)
                if (_parentEventProfile == null)
                    throw new JghNullObjectInstanceException(nameof(_parentEventProfile));

            if (_finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects == null)
                throw new JghNullObjectInstanceException(nameof(_finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects));

            if (_parentSeriesProfile.DefaultEventSettingsForAllEvents.RaceSpecificationItems == null)
                throw new JghNullObjectInstanceException(nameof(_parentSeriesProfile.DefaultEventSettingsForAllEvents.RaceSpecificationItems));

            if (isJustForSingleEvent)
                if (_parentEventProfile.EventSettingsItem.RaceSpecificationItems == null)
                    throw new JghNullObjectInstanceException(nameof(_parentEventProfile.EventSettingsItem.RaceSpecificationItems));

            #endregion

            #region page titles

            var preformattedBodyText = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(pageTitle1))
            {
                preformattedBodyText.AppendLine(string.Format(pageTitle1));
                preformattedBodyText.AppendLine(new string('=', pageTitle1.Length));
                preformattedBodyText.AppendLine("");
            }

            if (!string.IsNullOrWhiteSpace(pageTitle2))
            {
                preformattedBodyText.AppendLine(string.Format(pageTitle2));
                preformattedBodyText.AppendLine(new string('=', pageTitle2.Length));
                preformattedBodyText.AppendLine("");
            }

            if (!string.IsNullOrWhiteSpace(pageTitle3))
            {
                preformattedBodyText.AppendLine(string.Format(pageTitle3));
                preformattedBodyText.AppendLine(new string('=', pageTitle3.Length));
                preformattedBodyText.AppendLine("");
            }

            if (!string.IsNullOrWhiteSpace(pageTitle4))
            {
                preformattedBodyText.AppendLine(string.Format(pageTitle4));
                preformattedBodyText.AppendLine("");
            }

            preformattedBodyText.AppendLine($"Participants: {_finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects.Length}");

            preformattedBodyText.AppendLine("");

            #endregion

            #region compose inventory of relevant column specifications

            var relevantColumnSpecificationItems =
                ExtractColumnSpecificationItemsAssociatedWithActiveContent(
                    GenerateColumnsTemplate(ResultItemDisplayObject.MakeDataGridColumnSpecificationsForAbridgedLeaderboard(_parentColumnFormatEnum, _parentDictionaryOfTxxColumnHeaders)),
                    _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects);

            #endregion

            #region generate dictionaryOfResults, one entry for each race. each race consisting of a set of results

            var dictionaryOfResultsPerRace = new JghListDictionary<string, ResultItemDisplayObject>();

            foreach (var resultViewModel in _finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects.Where(z => z != null))
                if (resultsMustBeGroupedByRace)
                    dictionaryOfResultsPerRace.Add(
                        string.IsNullOrWhiteSpace(resultViewModel.RaceGroup)
                            ? Symbols.SymbolUncategorised
                            : resultViewModel.RaceGroup, resultViewModel);
                else
                    dictionaryOfResultsPerRace.Add(Symbols.SymbolUncategorised, resultViewModel);

            #endregion

            #region transcribe each race into a block of printed results

            var arrayOfRaceSb = new List<StringBuilder>();

            foreach (var thisRaceKvp in dictionaryOfResultsPerRace.Where(z => z.Value != null))
            {
                #region process this race

                var raceSb = new StringBuilder();

                if (resultsMustBeGroupedByRace)
                    raceSb.AppendLine(FormatRaceTitleBlockForPrintVersions(thisRaceKvp.Key));

                raceSb.AppendLine(FormatColumnsHeaderRow(SpacerBetweenColumnsInPrintVersions,
                    relevantColumnSpecificationItems));

                raceSb.AppendLine(FormatUnderliningRowForColumnsHeaderRow(SpacerBetweenColumnsInPrintVersions,
                    relevantColumnSpecificationItems));

                var prettyRowsOfText = await JghParallel.SelectAsParallelWorkStealingAsync(
                    thisRaceKvp.Value.ToArray(),
                    z =>
                        PrintRowAsPrettyConcatenatedString(z, relevantColumnSpecificationItems, SpacerBetweenColumnsInPrintVersions, false),
                    500);

                foreach (var prettyLineOfText in prettyRowsOfText)
                    raceSb.AppendLine(prettyLineOfText);

                raceSb.AppendLine(string.Empty);

                raceSb.AppendLine(string.Empty);

                arrayOfRaceSb.Add(raceSb);

                #endregion
            }

            foreach (var raceSb in arrayOfRaceSb)
                preformattedBodyText.Append(raceSb);

            #endregion

            var answer = preformattedBodyText.ToString();

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private async Task<string> GenerateLeaderboardStyleResultsAsWebPageAsync(
        string pageTitle1,
        string pageTitle2,
        string pageTitle3,
        string pageTitle4,
        bool resultsMustBeGroupedByRace,
        bool isJustForSingleEvent)
    {
        const string failure = "Unable to compose html document.";
        const string locus = "[GenerateLeaderboardStyleResultsAsWebPageAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null)
                throw new JghNullObjectInstanceException(nameof(_parentSeriesProfile));

            if (isJustForSingleEvent)
                if (_parentEventProfile == null)
                    throw new JghNullObjectInstanceException(nameof(_parentEventProfile));

            if (_finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects == null)
                throw new JghNullObjectInstanceException(nameof(_finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects));

            if (_parentSeriesProfile.DefaultEventSettingsForAllEvents.RaceSpecificationItems == null)
                throw new JghNullObjectInstanceException(nameof(_parentSeriesProfile.DefaultEventSettingsForAllEvents.RaceSpecificationItems));

            if (isJustForSingleEvent)
                if (_parentEventProfile.EventSettingsItem.RaceSpecificationItems == null)
                    throw new JghNullObjectInstanceException(nameof(_parentEventProfile.EventSettingsItem.RaceSpecificationItems));

            #endregion

            #region generate the preformatted body text - heap powerful

            var preformattedBodyText = new StringBuilder();

            preformattedBodyText.Append(await GenerateAbridgedLeaderboardStyleResultsAsPreFormattedTextAsync(pageTitle1,
                pageTitle2, pageTitle3, pageTitle4, resultsMustBeGroupedByRace, isJustForSingleEvent));

            preformattedBodyText.AppendLine(
                $"Printed: {DateTime.Now.DayOfWeek}, {DateTime.Now:f}");

            #endregion

            #region package webpage in HTML format

            var webpageHeadTabTitle = isJustForSingleEvent
                ? _parentEventProfile.Label
                : _parentSeriesProfile.Label;

            var htmlSourceComment = $"Timestamp: {DateTime.Now.DayOfWeek}, {DateTime.Now:f}";

            var webpageAsHtml = JghString.WrapTextInHtmlPreambleAsWebpage(webpageHeadTabTitle, htmlSourceComment,
                preformattedBodyText.ToString());

            #endregion

            return webpageAsHtml;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static async Task<XElement> GenerateLeaderboardStyleResultsAsSimpleFlatXmlFileAsync(ResultItemDisplayObject[] displayObjects,
        ColumnSpecificationItem[] columnSpecificationItems,
        string xAttribute1,
        string xAttribute2,
        string xAttribute3,
        string nameToBeUsedForParentXe)
    {
        const string failure = "Unable to compose xml data.";
        const string locus = "[SerialiseResultsAndWrapIntoAParentArrayOfResultXe]";

        try
        {
            #region null checks

            displayObjects ??= Array.Empty<ResultItemDisplayObject>();

            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();

            #endregion

            #region compose inventory of relevant column specifications

            var relevantColumnSpecificationItems =
                ExtractColumnSpecificationItemsAssociatedWithActiveContent(columnSpecificationItems,
                    displayObjects);

            #endregion

            #region TransformSingleresultViewModelIntoXElement for each result one by one

            var displayObjectArray = displayObjects
                .Where(z => z != null).ToArray();


            var collectionOfXe = await JghParallel.SelectAsParallelWorkStealingAsync(
                displayObjectArray,
                z =>
                    TransformDataGridRowItemIntoXElement(z, relevantColumnSpecificationItems,
                        ResultDto.XeResult),
                500);
            if (collectionOfXe == null) throw new ArgumentNullException(nameof(collectionOfXe));

            // be sure to delete any duplicates that inadvertently creep in (which they do!)

            foreach (var xE in collectionOfXe)
            {
                var xNames = xE.Elements().Select(child => child.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(xName => xE.Elements(xName).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);
            }

            // Handle the infuriating eccentricities of the Excel import wizard.
            // Rid problematic XElement values of characters or combinations of characters
            // that discombobulate or flummox imports of specific columns into Excel such
            // as a field that Excel stupidly interprets as a date because it starts with a digit
            // and contains a slash whereas is fact it's meant to be interpreted as
            // an uncomplicated string that intentionally begins with up to four digits followed by a slash/.
            // As time goes by you might need to change other troublesome characters in other columns
            // as new versions of Excel materialise and parse fields too cleverly by half in different ways on import.
            // Also, the only way to reliably get Xml data into Excel is to load it into Access and then export from Access
            // as an Excel file. Second best is to add the 'Developer' tab to the Excel ribbon, select it and click 'Import'
            // To do this go File>Options>Customise Ribbon>main Tabs>Developer>OK then Developer>Import

            foreach (var xE in collectionOfXe)
            {
                // Step 1. deal with fields that we know contain / but which are not dates

                foreach (var xElement in xE.Elements())
                {
                    if (xElement.Name != ResultItemDisplayObject
                            .XeFractionalPlaceInRaceInNumeratorOverDenominatorFormat
                        && xElement.Name != ResultItemDisplayObject
                            .XeFractionalPlaceBySexInNumeratorOverDenominatorFormat
                        && xElement.Name != ResultItemDisplayObject
                            .XeFractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat) continue;

                    var valueAsString = xElement.Value;

                    var modifiedValue = valueAsString
                        .Replace(@"/", " of ");

                    xElement.Value = modifiedValue;
                }


                var xNames = xE.Elements().Select(z => z.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(z => xE.Elements(z).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);

                foreach (var element in xE.Descendants()) element.Value = JghString.ToTrimmedString(element.Value);
            }

            #endregion

            #region make a filetimestamp

            var fileTimeStamp = DateTime.Now.ToString(JghDateTime.Iso8601Pattern);


            var hopefullyValidTimestamp =
                JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters(
                    '-', fileTimeStamp);

            if (
                !JghFilePathValidator.IsValidNtfsFileOrFolderName(hopefullyValidTimestamp,
                    // ReSharper disable once SuggestVarOrType_BuiltInTypes
                    // ReSharper disable once UnusedVariable
                    out var errorMessage))
                throw new JghInvalidValueException(
                    $"Unable to create valid NTFS file name. Sorry. Timestamp is invalid.'{fileTimeStamp}'");

            #endregion

            #region wrap resultsInfo into parentXe with attributes hopefully describing series, event etc

            var parentXe =
                JghXElementHelpers.WrapCollectionOfXElementsInNewParent(nameToBeUsedForParentXe,
                    collectionOfXe);

            parentXe.ReplaceAttributes(
                new XAttribute("timestamp", hopefullyValidTimestamp),
                new XAttribute("title3", xAttribute3 ?? ""),
                new XAttribute("title2", xAttribute2 ?? ""),
                new XAttribute("title1", xAttribute1 ?? ""));

            // In retrospect I learn that by inserting attributes we flummox the ability of Excel to import the xml file because it doesn't have a xml schema to work with.
            // I hate deleting the attributes we have so lovingly created and which make such a nice description to do this. Hopefully in a future life we won't need to remove the attributess.

            parentXe.RemoveAttributes(); // wish i didn't need to remove the attributes. oh well.

            #endregion

            return parentXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private async Task<string> GenerateCohortAnalysisAsWebPageAsync(string pageTitle1, string pageTitle2,
        string pageTitle3, string pageTitle4, bool isJustForSingleEvent)
    {
        const string failure = "Unable to compose html document.";
        const string locus = "[GenerateLeaderboardStyleResultsAsWebPageAsync]";

        try
        {
            #region null checks

            if (_parentSeriesProfile == null)
                throw new JghNullObjectInstanceException(nameof(_parentSeriesProfile));

            if (isJustForSingleEvent)
                if (_parentEventProfile == null)
                    throw new JghNullObjectInstanceException(nameof(_parentEventProfile));

            if (_finalisedDataGridRowsOfPopulationCohortItemDisplayObjects == null)
                throw new JghNullObjectInstanceException(nameof(_finalisedDataGridRowsOfLeaderboardOfResultItemDisplayObjects));

            #endregion

            #region page titles

            var webPageSb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(pageTitle1))
            {
                webPageSb.AppendLine(string.Format(pageTitle1));
                webPageSb.AppendLine(new string('=', pageTitle1.Length));
                webPageSb.AppendLine("");
            }

            if (!string.IsNullOrWhiteSpace(pageTitle2))
            {
                webPageSb.AppendLine(string.Format(pageTitle2));
                webPageSb.AppendLine(new string('=', pageTitle2.Length));
                webPageSb.AppendLine("");
            }

            if (!string.IsNullOrWhiteSpace(pageTitle3))
            {
                webPageSb.AppendLine(string.Format(pageTitle3));
                webPageSb.AppendLine(new string('=', pageTitle3.Length));
                webPageSb.AppendLine("");
            }

            if (!string.IsNullOrWhiteSpace(pageTitle4))
            {
                webPageSb.AppendLine(string.Format(pageTitle4));
                webPageSb.AppendLine("");
            }

            webPageSb.AppendLine($"Line items {_finalisedDataGridRowsOfPopulationCohortItemDisplayObjects.Length}");

            webPageSb.AppendLine("");

            #endregion

            #region compose inventory of relevant column specifications

            var relevantColumnSpecificationItems =
                ExtractColumnSpecificationItemsAssociatedWithActiveContent(
                    GenerateColumnsTemplate(PopulationCohortItemDisplayObject.DataGridColumnsToBeDisplayed()),
                    _finalisedDataGridRowsOfPopulationCohortItemDisplayObjects);

            #endregion

            #region transcribe block of printed results

            var blockSb = new StringBuilder();

            blockSb.AppendLine(FormatColumnsHeaderRow(SpacerBetweenColumnsInPrintVersions,
                relevantColumnSpecificationItems));

            blockSb.AppendLine(FormatUnderliningRowForColumnsHeaderRow(SpacerBetweenColumnsInPrintVersions,
                relevantColumnSpecificationItems));

            var lineItems = _finalisedDataGridRowsOfPopulationCohortItemDisplayObjects
                .Where(z => z != null)
                .ToArray();

            var prettyRowsOfText = await JghParallel.SelectAsParallelWorkStealingAsync(
                lineItems,
                z =>
                    PrintRowAsPrettyConcatenatedString(z, relevantColumnSpecificationItems, SpacerBetweenColumnsInPrintVersions, false),
                500);


            foreach (var prettyLineOfText in prettyRowsOfText)
                blockSb.AppendLine(prettyLineOfText);

            blockSb.AppendLine(string.Empty);

            blockSb.AppendLine(string.Empty);

            webPageSb.Append(blockSb);

            webPageSb.AppendLine(
                $"Timestamp {DateTime.Now.ToString(JghDateTime.Iso8601Pattern)}");

            #endregion

            #region package webpage in HTML format

            var webpageHeadTabTitle = isJustForSingleEvent
                ? _parentEventProfile.Label
                : _parentSeriesProfile.Label;

            var htmlSourceComment = $"Timestamp {DateTime.Now.DayOfWeek} {DateTime.Now:f}";

            var webpageAsHtml = JghString.WrapTextInHtmlPreambleAsWebpage(webpageHeadTabTitle, htmlSourceComment,
                webPageSb.ToString());

            #endregion

            return webpageAsHtml;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static async Task<XElement> GenerateParticipantHubItemDisplayObjectsAsSimpleFlatXmlFileAsync(ParticipantHubItemDisplayObject[] displayObject,
        ColumnSpecificationItem[] columnSpecificationItems,
        string xAttribute1,
        string xAttribute2,
        string xAttribute3,
        string nameToBeUsedForParentXe)
    {
        const string failure = "Unable to compose xml data.";
        const string locus = "[SerialiseResultsAndWrapIntoAParentArrayOfResultXe]";

        try
        {
            #region null checks

            displayObject ??= Array.Empty<ParticipantHubItemDisplayObject>();

            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();

            #endregion

            #region compose inventory of relevant column specifications

            var relevantColumnSpecificationItems =
                ExtractColumnSpecificationItemsAssociatedWithActiveContent(columnSpecificationItems,
                    displayObject);

            #endregion

            #region Transform each displayobject into XElement by one

            var displayObjectArray = displayObject
                .Where(z => z != null).ToArray();

            var collectionOfXe = await JghParallel.SelectAsParallelWorkStealingAsync(
                displayObjectArray,
                z =>
                    TransformDataGridRowItemIntoXElement(z, relevantColumnSpecificationItems,
                        ParticipantHubItemDto.XeParticipant),
                500);

            // be sure to delete any duplicates that inadvertently creep in (which they do!)

            foreach (var xE in collectionOfXe)
            {
                var xNames = xE.Elements().Select(child => child.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(xName => xE.Elements(xName).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);
            }

            // Handle the infuriating eccentricities of the Excel import wizard.
            // Rid problematic XElement values of characters or combinations of characters
            // that discombobulate or flummox imports of specific columns into Excel such
            // as a field that Excel stupidly interprets as a date because it starts with a digit
            // and contains a slash whereas is fact it's meant to be interpreted as
            // an uncomplicated string that intentionally begins with up to four digits followed by a slash/.
            // As time goes by you might need to change other troublesome characters in other columns
            // as new versions of Excel materialise and parse fields too cleverly by half in different ways on import.
            // Also, the only way to reliably get Xml data into Excel is to load it into Access and then export from Access
            // as an Excel file. Second best is to enable the 'Developer' tab in the Excel ribbon, select it and click 'Import'
            // To do this go File>Options>Customise Ribbon>main Tabs>Developer>OK then Developer>Import

            foreach (var xE in collectionOfXe)
            {
                // Sui generis: reformat any fields that require reformatting to comply with peculiarities of Excel for this type of object

                //foreach (var xElement in resultXe.Elements())
                //{
                //    if (xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceInRaceInNumeratorOverDenominatorFormat
                //        && xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceBySexInNumeratorOverDenominatorFormat
                //        && xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat) continue;

                //    var valueAsString = xElement.Value;

                //    var modifiedValue = valueAsString
                //        .Replace(@"/", " out of ");

                //    xElement.Value = modifiedValue;
                //}


                var xNames = xE.Elements().Select(z => z.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(z => xE.Elements(z).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);

                foreach (var element in xE.Descendants()) element.Value = JghString.ToTrimmedString(element.Value);
            }

            #endregion

            #region make a filetimestamp

            var fileTimestamp = DateTime.Now.ToString(JghDateTime.Iso8601Pattern);

            var hopefullyValidTimestamp =
                JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters(
                    '-', fileTimestamp);

            if (
                !JghFilePathValidator.IsValidNtfsFileOrFolderName(hopefullyValidTimestamp,
                    // ReSharper disable once SuggestVarOrType_BuiltInTypes
                    // ReSharper disable once UnusedVariable
                    out var errorMessage))
                throw new JghInvalidValueException(
                    $"Unable to create valid NTFS file name. Sorry. Timestamp is invalid.'{fileTimestamp}'");

            #endregion

            #region wrap child elements into parentXe with attributes hopefully describing series, event etc

            var parentXe =
                JghXElementHelpers.WrapCollectionOfXElementsInNewParent(nameToBeUsedForParentXe,
                    collectionOfXe);

            parentXe.ReplaceAttributes(
                new XAttribute("timestamp", hopefullyValidTimestamp),
                new XAttribute("title3", xAttribute3 ?? ""),
                new XAttribute("title2", xAttribute2 ?? ""),
                new XAttribute("title1", xAttribute1 ?? ""));

            // In retrospect I learn that by inserting attributes we flummox the ability of Excel to import the xml file because it doesn't have a xml schema to work with.
            // I hate deleting the attributes we have so lovingly created and which make such a nice description to do this. Hopefully in a future life we won't need to remove the attributess.

            parentXe.RemoveAttributes(); // wish i didn't need to remove the attributes. oh well.

            #endregion

            return parentXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static async Task<XElement> GenerateTimeStampHubItemDisplayObjectsAsSimpleFlatXmlFileAsync(TimeStampHubItemDisplayObject[] displayObject,
        ColumnSpecificationItem[] columnSpecificationItems,
        string xAttribute1,
        string xAttribute2,
        string xAttribute3,
        string nameToBeUsedForParentXe)
    {
        const string failure = "Unable to compose xml data.";
        const string locus = "[SerialiseResultsAndWrapIntoAParentArrayOfResultXe]";

        try
        {
            #region null checks

            displayObject ??= Array.Empty<TimeStampHubItemDisplayObject>();

            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();

            #endregion

            #region compose inventory of relevant column specifications

            var relevantColumnSpecificationItems =
                ExtractColumnSpecificationItemsAssociatedWithActiveContent(columnSpecificationItems,
                    displayObject);

            #endregion

            #region Transform each displayobject into XElement by one

            var displayObjectArray = displayObject
                .Where(z => z != null).ToArray();

            var collectionOfXe = await JghParallel.SelectAsParallelWorkStealingAsync(
                displayObjectArray,
                z =>
                    TransformDataGridRowItemIntoXElement(z, relevantColumnSpecificationItems,
                        TimeStampHubItemDto.XeTimeStamp),
                500);

            // be sure to delete any duplicates that inadvertently creep in (which they do!)

            foreach (var xE in collectionOfXe)
            {
                var xNames = xE.Elements().Select(child => child.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(xName => xE.Elements(xName).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);
            }

            // Handle the infuriating eccentricities of the Excel import wizard.
            // Rid problematic XElement values of characters or combinations of characters
            // that discombobulate or flummox imports of specific columns into Excel such
            // as a field that Excel stupidly interprets as a date because it starts with a digit
            // and contains a slash whereas is fact it's meant to be interpreted as
            // an uncomplicated string that intentionally begins with up to four digits followed by a slash/.
            // As time goes by you might need to change other troublesome characters in other columns
            // as new versions of Excel materialise and parse fields too cleverly by half in different ways on import.
            // Also, the only way to reliably get Xml data into Excel is to load it into Access and then export from Access
            // as an Excel file. Second best is to enable the 'Developer' tab in the Excel ribbon, select it and click 'Import'
            // To do this go File>Options>Customise Ribbon>main Tabs>Developer>OK then Developer>Import

            foreach (var xE in collectionOfXe)
            {
                // Sui generis: reformat any fields that require reformatting to comply with peculiarities of Excel for this type of object

                //foreach (var xElement in resultXe.Elements())
                //{
                //    if (xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceInRaceInNumeratorOverDenominatorFormat
                //        && xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceBySexInNumeratorOverDenominatorFormat
                //        && xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat) continue;

                //    var valueAsString = xElement.Value;

                //    var modifiedValue = valueAsString
                //        .Replace(@"/", " out of ");

                //    xElement.Value = modifiedValue;
                //}


                var xNames = xE.Elements().Select(z => z.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(z => xE.Elements(z).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);

                foreach (var element in xE.Descendants()) element.Value = JghString.ToTrimmedString(element.Value);
            }

            #endregion

            #region make a filetimestamp

            var fileTimestamp = DateTime.Now.ToString(JghDateTime.Iso8601Pattern);

            var hopefullyValidTimestamp =
                JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters(
                    '-', fileTimestamp);

            if (
                !JghFilePathValidator.IsValidNtfsFileOrFolderName(hopefullyValidTimestamp,
                    // ReSharper disable once SuggestVarOrType_BuiltInTypes
                    // ReSharper disable once UnusedVariable
                    out var errorMessage))
                throw new JghInvalidValueException(
                    $"Unable to create valid NTFS file name. Sorry. Timestamp is invalid.'{fileTimestamp}'");

            #endregion

            #region wrap child elements into parentXe with attributes hopefully describing series, event etc

            var parentXe =
                JghXElementHelpers.WrapCollectionOfXElementsInNewParent(nameToBeUsedForParentXe,
                    collectionOfXe);

            parentXe.ReplaceAttributes(
                new XAttribute("timestamp", hopefullyValidTimestamp),
                new XAttribute("title3", xAttribute3 ?? ""),
                new XAttribute("title2", xAttribute2 ?? ""),
                new XAttribute("title1", xAttribute1 ?? ""));

            // In retrospect I learn that by inserting attributes we flummox the ability of Excel to import the xml file because it doesn't have a xml schema to work with.
            // I hate deleting the attributes we have so lovingly created and which make such a nice description to do this. Hopefully in a future life we won't need to remove the attributess.

            parentXe.RemoveAttributes(); // wish i didn't need to remove the attributes. oh well.

            #endregion

            return parentXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private static async Task<XElement> GenerateSplitIntervalsForParticipantDisplayObjectsAsSimpleFlatXmlFileAsync(SplitIntervalConsolidationForParticipantDisplayObject[] displayObject,
        ColumnSpecificationItem[] columnSpecificationItems,
        string xAttribute1,
        string xAttribute2,
        string xAttribute3,
        string nameToBeUsedForParentXe)
    {
        const string failure = "Unable to compose xml data.";
        const string locus = "[SerialiseResultsAndWrapIntoAParentArrayOfResultXe]";

        try
        {
            #region null checks

            displayObject ??= Array.Empty<SplitIntervalConsolidationForParticipantDisplayObject>();

            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();

            #endregion

            #region compose inventory of relevant column specifications

            var relevantColumnSpecificationItems =
                ExtractColumnSpecificationItemsAssociatedWithActiveContent(columnSpecificationItems,
                    displayObject);

            #endregion

            #region Transform each displayobject into XElement by one

            var displayObjectArray = displayObject
                .Where(z => z != null).ToArray();

            var collectionOfXe = await JghParallel.SelectAsParallelWorkStealingAsync(
                displayObjectArray,
                z =>
                    TransformDataGridRowItemIntoXElement(z, relevantColumnSpecificationItems, XeSplitIntervalsForParticipantItem),
                500);

            // be sure to delete any duplicates that inadvertently creep in (which they do!)

            foreach (var xE in collectionOfXe)
            {
                var xNames = xE.Elements().Select(child => child.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(xName => xE.Elements(xName).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);
            }

            // Handle the infuriating eccentricities of the Excel import wizard.
            // Rid problematic XElement values of characters or combinations of characters
            // that discombobulate or flummox imports of specific columns into Excel such
            // as a field that Excel stupidly interprets as a date because it starts with a digit
            // and contains a slash whereas is fact it's meant to be interpreted as
            // an uncomplicated string that intentionally begins with up to four digits followed by a slash/.
            // As time goes by you might need to change other troublesome characters in other columns
            // as new versions of Excel materialise and parse fields too cleverly by half in different ways on import.
            // Also, the only way to reliably get Xml data into Excel is to load it into Access and then export from Access
            // as an Excel file. Second best is to enable the 'Developer' tab in the Excel ribbon, select it and click 'Import'
            // To do this go File>Options>Customise Ribbon>main Tabs>Developer>OK then Developer>Import

            foreach (var xE in collectionOfXe)
            {
                // Sui generis: reformat any fields that require reformatting to comply with peculiarities of Excel for this type of object

                //foreach (var xElement in resultXe.Elements())
                //{
                //    if (xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceInRaceInNumeratorOverDenominatorFormat
                //        && xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceBySexInNumeratorOverDenominatorFormat
                //        && xElement.Name != ResultItemSerialiserNames
                //            .FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat) continue;

                //    var valueAsString = xElement.Value;

                //    var modifiedValue = valueAsString
                //        .Replace(@"/", " out of ");

                //    xElement.Value = modifiedValue;
                //}


                var xNames = xE.Elements().Select(z => z.Name);

                xNames = xNames.Distinct().ToArray();

                var newChildren = xNames.Select(z => xE.Elements(z).FirstOrDefault()).ToList();

                xE.RemoveAll();

                xE.Add(newChildren);

                foreach (var element in xE.Descendants()) element.Value = JghString.ToTrimmedString(element.Value);
            }

            #endregion

            #region make a filetimestamp

            var fileTimestamp = DateTime.Now.ToString(JghDateTime.Iso8601Pattern);

            var hopefullyValidTimestamp =
                JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters(
                    '-', fileTimestamp);

            if (
                !JghFilePathValidator.IsValidNtfsFileOrFolderName(hopefullyValidTimestamp,
                    // ReSharper disable once SuggestVarOrType_BuiltInTypes
                    // ReSharper disable once UnusedVariable
                    out var errorMessage))
                throw new JghInvalidValueException(
                    $"Unable to create valid NTFS file name. Sorry. Timestamp is invalid.'{fileTimestamp}'");

            #endregion

            #region wrap child elements into parentXe with attributes hopefully describing series, event etc

            var parentXe =
                JghXElementHelpers.WrapCollectionOfXElementsInNewParent(nameToBeUsedForParentXe,
                    collectionOfXe);

            parentXe.ReplaceAttributes(
                new XAttribute("timestamp", hopefullyValidTimestamp),
                new XAttribute("title3", xAttribute3 ?? ""),
                new XAttribute("title2", xAttribute2 ?? ""),
                new XAttribute("title1", xAttribute1 ?? ""));

            // In retrospect I learn that by inserting attributes we flummox the ability of Excel to import the xml file because it doesn't have a xml schema to work with.
            // I hate deleting the attributes we have so lovingly created and which make such a nice description to do this. Hopefully in a future life we won't need to remove the attributess.

            parentXe.RemoveAttributes(); // wish i didn't need to remove the attributes. oh well.

            #endregion

            return parentXe;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region formatting rows

    private static string FormatColumnsHeaderRow(string spacerBetweenColumns,
        IList<ColumnSpecificationItem> columnSpecificationItems)
    {
        var numberOfColumns = columnSpecificationItems.Count;

        var headers = new string[numberOfColumns];

        for (var i = 0; i < numberOfColumns; i++)
        {
            var locusOfPossibleFormattingError =
                $"The header-text property of ColumnSpecificationItem #{i} of {numberOfColumns}.";

            headers[i] = FormatTextAsPerSpecifications(columnSpecificationItems[i],
                columnSpecificationItems[i].ColumnHeaderText, locusOfPossibleFormattingError, true);
        }

        var headersRow = string.Join(spacerBetweenColumns, headers);

        return headersRow;
    }

    private static string FormatUnderliningRowForColumnsHeaderRow(string spacerBetweenColumns,
        IList<ColumnSpecificationItem> columnSpecificationItems)
    {
        var numberOfColumns = columnSpecificationItems.Count;

        var underlinings = new string[numberOfColumns];

        for (var i = 0; i < numberOfColumns; i++)
            underlinings[i] = new string('=', columnSpecificationItems[i].ColumnWidthChars);

        var underliningsRow = string.Join(spacerBetweenColumns, underlinings);

        return underliningsRow;
    }

    private static string PrintRowAsPrettyConcatenatedString<T>(
        T rowItemObject,
        IEnumerable<ColumnSpecificationItem> columnSpecificationItems,
        string separatorBetweenColumns,
        bool isHeaderRow) where T : new()
    {
        const string failure =
            "Unable to map result into its representation as a single userItem-friendly concatenated string.";
        const string locus = "[PrintRowAsPrettyConcatenatedString]";

        try
        {
            #region null checks

            rowItemObject ??= new T();
            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();
            separatorBetweenColumns ??= " ";

            #endregion

            var propertyValuesAsText = from column in columnSpecificationItems
                where !string.IsNullOrWhiteSpace(column.NameOfAssociatedPropertyInXamlBindingSyntax)
                let propertyAsObject =
                    JghReflectionHelpers.GetDescendentProperty(column.NameOfAssociatedPropertyInXamlBindingSyntax,
                        rowItemObject)
                let textInCell = propertyAsObject?.ToString() ?? string.Empty
                let locusOfCurrentCellForErrMsg =
                    $"The value of the property being formatted is {textInCell}. It is earmarked for the column named {column.ColumnHeaderText}."
                select FormatTextAsPerSpecifications(column, textInCell, locusOfCurrentCellForErrMsg, isHeaderRow);

            var answer = string.Join(separatorBetweenColumns, propertyValuesAsText);

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region miscellaneous

    private static string FormatRaceTitleBlockForPrintVersions(string blockTitleString)
    {
        const string failure = "Unable to format document titles.";
        const string locus = "[FormatRaceTitlesForPrettyHtmPageInLeaderboardFormat]";

        try
        {
            if (string.IsNullOrWhiteSpace(blockTitleString)) blockTitleString = "title not specified";

            var sb = new StringBuilder();

            var title = blockTitleString.Trim().ToUpper();

            var titleWidth = title.Length;

            const string prefix = "Race: ";

            var prefixWidth = prefix.Length;

            sb.AppendLine(prefix + title);

            sb.AppendLine(new string('=', prefixWidth + titleWidth));

            var answer = sb.ToString();

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    private static XElement TransformDataGridRowItemIntoXElement<T>(
        T typeOfRowItem,
        ColumnSpecificationItem[] columnSpecificationItems,
        string nameToBeUsedForElement) where T : class, new()
    {
        const string failure =
            "Unable to map row item into a corresponding xml equivalent of the specified collection of columns.";
        const string locus = "[TransformDataGridRowItemIntoXElement]";

        try
        {
            #region null checks

            typeOfRowItem ??= new T();
            columnSpecificationItems ??= Array.Empty<ColumnSpecificationItem>();
            nameToBeUsedForElement ??= "DummyName";

            #endregion

            var answer = new XElement(nameToBeUsedForElement);

            foreach (var column in columnSpecificationItems)
            {
                var newChildElement = new XElement(
                    column.CellXElementName,
                    JghReflectionHelpers.GetDescendentProperty(column.NameOfAssociatedPropertyInXamlBindingSyntax,
                        typeOfRowItem));

                answer.Add(newChildElement);
            }

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public static ColumnSpecificationItem DoTextFormattingSpecifications(
        ColumnSpecificationItem columnSpecificationItem,
        string[] labelsOfColumnSpecificationItemsToBeLeftAligned,
        string[] labelsOfColumnSpecificationItemsToBeUpperCase,
        string[] labelsOfColumnSpecificationItemsToBeNonSpecificCase)
    {
        columnSpecificationItem ??= new ColumnSpecificationItem();

        labelsOfColumnSpecificationItemsToBeLeftAligned ??= Array.Empty<string>();

        labelsOfColumnSpecificationItemsToBeUpperCase ??= Array.Empty<string>();

        labelsOfColumnSpecificationItemsToBeNonSpecificCase ??= Array.Empty<string>();

        var columnSpecInProgress = columnSpecificationItem.ShallowMemberwiseCloneCopy;

        // alignment

        columnSpecInProgress.TextAlignmentEnum =
            labelsOfColumnSpecificationItemsToBeLeftAligned.Contains(columnSpecInProgress.CellXElementName)
                ? EnumStrings.LeftAlign
                : EnumStrings.RightAlign;

        // casing

        if (labelsOfColumnSpecificationItemsToBeUpperCase.Contains(columnSpecInProgress.CellXElementName))
        {
            columnSpecInProgress.CaseOfHeaderTextEnum = EnumStrings.UpperCase;
            columnSpecInProgress.CaseOfLineItemTextEnum = EnumStrings.UpperCase;
        }
        else if (labelsOfColumnSpecificationItemsToBeNonSpecificCase.Contains(columnSpecInProgress.CellXElementName))
        {
            columnSpecInProgress.CaseOfHeaderTextEnum = EnumStrings.NonSpecificCase;
            columnSpecInProgress.CaseOfLineItemTextEnum = EnumStrings.NonSpecificCase;
        }
        else
        {
            columnSpecInProgress.CaseOfHeaderTextEnum = EnumStrings.LowerCase;
            columnSpecInProgress.CaseOfLineItemTextEnum = EnumStrings.LowerCase;
        }

        return columnSpecInProgress;
    }

    #endregion

    #endregion
}