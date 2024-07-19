using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces03.Apr2022;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.DataGridDesigners;
using Rezultz.Library02.Mar2024.DataGridInterfaces;
using Rezultz.Library02.Mar2024.Strings;
using Rezultz.Library02.Mar2024.ValidationViewModels;

namespace Rezultz.Library02.Mar2024.PageViewModelBases
{
    public abstract class BasePopulationCohortsStylePageViewModel : BaseViewViewModel
    {
        private const string Locus2 = nameof(BasePopulationCohortsStylePageViewModel);
        private const string Locus3 = "[Rezultz.Library02.Mar2024]";

        #region ctor

        protected BasePopulationCohortsStylePageViewModel(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
            ISessionState sessionState,
            ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationVm)
        {
            #region assign ctor IOC injections

            SessionState = sessionState;

            GlobalSeasonProfileAndIdentityValidationVm = globalSeasonProfileAndIdentityValidationVm;

            _leaderboardResultsSvcAgent = leaderboardResultsSvcAgent;

            #endregion

            #region instantiate ButtonVms

            LoadSourceDataButtonVm = new ButtonControlViewModel(LoadSourceDataButtonOnClickExecuteAsync, LoadSourceDataButtonOnClickCanExecute);

            ApplyFiltersButtonVm = new ButtonControlViewModel(ApplyFiltersButtonOnClickExecuteAsync, ApplyFiltersButtonOnClickCanExecute);

            SavePopulationCohortsToDocumentsAsHtmlWebpageButtonVm = new ButtonControlViewModel(() => { }, () => false);

            #endregion

            #region instantiate Cbos

            CboLookupKindOfCohortVm = new IndexDrivenCollectionViewModel<MoreInformationItemDisplayObject>(
                Strings2017.Other_cohorts,
                CboLookupKindOfCohortOnSelectionChangedExecuteAsync,
                CboLookupKindOfCohortOnSelectionChangedCanExecute);

            #endregion

            #region instantiate AllDataGridLineItemDisplayObjects

            AllDataGridLineItemDisplayObjects = [];

            #endregion

            #region instantiate DataGridVms

            PopulationCohortsDataGridVm = new DataGridViewModel<PopulationCohortItemDisplayObject>(string.Empty, () => { }, () => true);

            #endregion
        }

        #endregion

        #region OnPageLoaded method

        public async Task<string> BeInitialisedFromPageCodeBehindOrchestrateAsync()
        {
            var failure = Strings2017.Unable_to_complete_computations_and_calculations_to_load_page_;
            const string locus = "[OnPageLoadViewModelOrchestrateAsync]";

            try
            {
                if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                    return string.Empty;

                DeadenGui();
                var messageOk = await LoadSourceDataButtonOnClickAsync();

                EnlivenGui();

                SaveGenesisOfThisViewModelAsLastKnownGood();

                ThisViewModelIsInitialised = true;

                return messageOk;
            }

            #region try catch handling

            catch (Exception ex)
            {
                var isHarmless = JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex)
                                 || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghResultsData404Exception>(ex);

                ThisViewModelIsInitialised = isHarmless;

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region fields

        private IHasCohorts _currentRepositoryWithCohorts;

        private static IPopulationCohortsDataGridPresentationService PopulationCohortsDataGridPresentationServiceInstance
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IPopulationCohortsDataGridPresentationService>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(Strings2017.Unable_to_retrieve_instance,
                            "<ISingleEventPopulationCohortsDataGridPresentationService>");

                    const string locus = "Property getter of <SingleEventPopulationCohortsDataGridPresentationServiceInstance]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        #endregion

        #region global props

        protected readonly ISessionState SessionState;

        protected readonly ISeasonProfileAndIdentityValidationViewModel GlobalSeasonProfileAndIdentityValidationVm;

        protected static IAlertMessageService AlertMessageService
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IAlertMessageService>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(Strings2017.Unable_to_retrieve_instance,
                            $"'{nameof(IAlertMessageService)}");

                    var locus = $"Property getter of '{nameof(AlertMessageService)}]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        protected static IProgressIndicatorViewModel GlobalProgressIndicatorVm
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IProgressIndicatorViewModel>();
                }
                catch (Exception ex)
                {
                    var msg = JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, $"[{nameof(IProgressIndicatorViewModel)}]");

                    const string locus = StringsForXamlPages.PropertyGetterOf + $"[{nameof(GlobalProgressIndicatorVm)}]";

                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        private readonly ILeaderboardResultsSvcAgent _leaderboardResultsSvcAgent;

        #endregion

        #region props

        public bool ThisViewModelIsInitialised { get; protected set; }
        public HeaderOrFooterViewModel HeadersVm { get; } = new();
        public HeaderOrFooterViewModel FootersVm { get; } = new();

        protected PopulationCohortItemDisplayObject[] AllDataGridLineItemDisplayObjects { get; set; }

        #region formatting enums

        public string CohortColumnFormatEnum => EnumStrings.PopulationCohortItemColumnFormat;

        #endregion

        #region Buttons

        public ButtonControlViewModel LoadSourceDataButtonVm { get; }

        public ButtonControlViewModel ApplyFiltersButtonVm { get; }

        public ButtonControlViewModel SavePopulationCohortsToDocumentsAsHtmlWebpageButtonVm { get; }

        #endregion

        #region Cbos

        public IndexDrivenCollectionViewModel<MoreInformationItemDisplayObject> CboLookupKindOfCohortVm { get; protected set; }

        #endregion

        #region DataGridVms

        public DataGridViewModel<PopulationCohortItemDisplayObject> PopulationCohortsDataGridVm { get; }

        #endregion

        #region DataGridDesigners

        public DataGridDesigner PopulationCohortsDataGridDesigner { get; } = new();

        #endregion

        #region ImageUriVms

        public IndexDrivenCollectionViewModel<UriItemDisplayObject> PageImagesInSkyscraperRightVm { get; } = new("Skyscraper right image album", () => { }, () => true);

        #endregion

        #region SocialMediaConnectionsVm

        public SocialMediaConnectionsViewModel SocialMediaConnectionsVm { get; } = new();

        #endregion

        #endregion

        #region commands

        #region LoadSourceDataButtonOnClickAsync - heap powerful

        private bool LoadSourceDataButtonOnClickCanExecute()
        {
            return LoadSourceDataButtonVm.IsAuthorisedToOperate;
        }

        private async void LoadSourceDataButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[LoadSourceDataButtonOnClickExecuteAsync]";

            try
            {
                if (!LoadSourceDataButtonOnClickCanExecute())
                    return;


                GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

                DeadenGui();

                var messageOk = await LoadSourceDataButtonOnClickAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowOkAsync(messageOk);
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                    Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        /// <summary>
        ///     Throws three exceptions that happen all the time in perfectly ordinary happenstances. The calling
        ///     method must filter these and treat them as benign however it so chooses. If there is no internet
        ///     connection, innermost exception will be a JghCommunicationFailureException.
        ///     If no blob posted yet, or event was cancelled, innermost exception will be a JghResultsData404Exception.
        ///     To convey a harmless informative message, a message that must be deemed a mere footnote
        ///     to a successful outcome, innermost exception will be a JghAlertMessageException.
        ///     All other exceptions are totally unanticipated and should be deemed showstoppers.
        /// </summary>
        /// <returns></returns>
        public async Task<string> LoadSourceDataButtonOnClickAsync()
        {
            var failure = Strings2017.Unable_to_complete_computations_and_calculations_to_load_page_;
            const string locus = "[LoadSourceDataButtonOnClickAsync]";

            try
            {
                #region do work

                #region begin

                await ZeroiseAsync();

                ThrowExceptionIfAnyFundamentalNullCheckFails();

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

                var currentEvent = EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem);

                #endregion

                #region page headings/footer and social media connections

                HeadersVm.Populate(MakeOrganiserTitle(), MakeSeriesTitle(), MakeEventTitle(), MakeMoreInfoTitle());
                HeadersVm.SaveAsLastKnownGood();
                HeadersVm.IsVisible = true;

                FootersVm.Populate(SessionState.CurrentAmbientPageFooterMessage);
                FootersVm.SaveAsLastKnownGood();
                FootersVm.IsVisible = true;

                SocialMediaConnectionsVm.PopulateConnections(currentEvent.EventSettingsItem.OrganizerEmailAddress,
                    currentEvent.EventSettingsItem.OrganizerFacebookUri, currentEvent.EventSettingsItem.OrganizerInstagramUri,
                    currentEvent.EventSettingsItem.OrganizerTwitterUri);

                #endregion

                #region save last known good - 1st kick

                SaveGenesisOfThisViewModelAsLastKnownGood();

                #endregion

                #region save session state - 1st kick

                SessionState.CurrentAmbientPageFooterMessage = currentEvent.EventSettingsItem.OneLinerContactMessage ?? string.Empty;

                #endregion

                #region populate CboListOfMoreInfo a.k.a sundry kinds of cohort analysis

                var manyKindsOfCohorts = currentEvent.EventSettingsItem.EventAnalysisItems;

                manyKindsOfCohorts = manyKindsOfCohorts
                    .Where(z => z is not null)
                    .OrderBy(z => z.DisplayRank)
                    .ThenBy(z => z.Label)
                    .ToArray();

                await CboLookupKindOfCohortVm.RefillItemsSourceAsync(MoreInformationItemDisplayObject.FromModel(manyKindsOfCohorts));

                CboLookupKindOfCohortVm.MakeVisibleIfItemsSourceIsGreaterThanOne();

                CboLookupKindOfCohortVm.IsDropDownOpen = false;

                CboLookupKindOfCohortVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

                await CboLookupKindOfCohortVm.ChangeSelectedIndexAsync(0); // default - first item on the list

                CboLookupKindOfCohortVm.SaveSelectedIndexAsLastKnownGood();

                #endregion

                #region save genesis as last known good

                SaveGenesisOfThisViewModelAsLastKnownGood();

                CboLookupKindOfCohortVm.SaveSelectedIndexAsLastKnownGood();

                #endregion

                #region before proceding check that CboLookupKindOfCohortVm.CurrentItem is legit

                var selectedCohortEnumString = CboLookupKindOfCohortVm.CurrentItem?.EnumString;

                if (selectedCohortEnumString is not (EnumStrings.HistogramForEachRace or EnumStrings.HistogramForEachSex or EnumStrings.HistogramForEachAgeGroup or EnumStrings.HistogramForEachSex or EnumStrings.HistogramForEachCity))
                {
                    var dataError =
                        Strings2017.Data_error__event_stats_enum_not_recognised;

                    var culprit = $"<{CboLookupKindOfCohortVm?.CurrentItem?.EnumString}]";

                    var theProblem = JghString.ConcatAsSentences(dataError, culprit, null, null);

                    var errorDetails = JghString.ConcatAsParagraphs(failure, theProblem,
                        JghString.ConcatAsSentences(locus, Locus2, null, null), null);

                    throw new JghInvalidValueException(errorDetails); //bale
                }

                #endregion

                #region load repository of cohorts

                _currentRepositoryWithCohorts = await ObtainRepositoryWithCohortsAsync(_leaderboardResultsSvcAgent, currentSeries, currentEvent);

                #endregion

                #region materialise PageImagesInSkyscraperRightVm

                PopulateSourceUriStringOfAllImageUriItems(currentEvent);

                await PageImagesInSkyscraperRightVm.RefillItemsSourceAsync(UriItemDisplayObject.FromModel(currentEvent.EventSettingsItem.UriItems));

                PageImagesInSkyscraperRightVm.MakeVisibleIfItemsSourceIsAny();

                #endregion

                #endregion


                var messageOk = await ApplyFiltersButtonOnClickAsync(); // drill down

                return messageOk;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region ApplyFiltersButtonOnClickAsync - heap powerful

        private bool ApplyFiltersButtonOnClickCanExecute()
        {
            return ApplyFiltersButtonVm.IsAuthorisedToOperate;
        }

        private async void ApplyFiltersButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[ApplyFiltersButtonOnClickExecuteAsync]";

            try
            {
                if (!ApplyFiltersButtonOnClickCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____formatting);

                DeadenGui();

                var messageOk = await ApplyFiltersButtonOnClickAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowOkAsync(messageOk);
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                    Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        protected async Task<string> ApplyFiltersButtonOnClickAsync()
        {
            // end of the line in the drill down sequence from series to event to race to filter results by selected category

            const string failure = "Unable to filter and display or redisplay list of results fully.";
            const string locus = "[ApplyFiltersButtonOnClickAsync]";

            try
            {
                #region do work

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

                var currentEvent = EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem);

                #region update page headers - 4th and final kick

                HeadersVm.Populate(PopulateTableHeadings());
                HeadersVm.SaveAsLastKnownGood();
                HeadersVm.IsVisible = true;

                #endregion

                #region obtain row collection destined to become the datagrid itemssource for selected analysis

                var selectedCohortEnumString = CboLookupKindOfCohortVm.CurrentItem?.EnumString;

                if (_currentRepositoryWithCohorts is null)
                    return string.Empty;

                var tableOfCohorts = await PopulateTableOfCohortLineItems(_currentRepositoryWithCohorts, selectedCohortEnumString);

                #endregion

                #region Key step. Assign AllDataGridLineItemDisplayObjects

                AllDataGridLineItemDisplayObjects = PopulationCohortItemDisplayObject.FromModel(tableOfCohorts.ToArray());

                #endregion

                #region create DataGridDesigner to obtain array of column specification items for RadDataGrid control in a PresentationService

                PopulationCohortsDataGridDesigner.InitialiseDesigner(
                    currentSeries,
                    currentEvent,
                    CohortColumnFormatEnum, AllDataGridLineItemDisplayObjects, PopulateDataGridTitleAndBlurb(), MoreInformationItemDisplayObject.ObtainSourceModel(CboLookupKindOfCohortVm.CurrentItem));

                var columnSpecificationItems = PopulationCohortsDataGridDesigner
                    .GetNonEmptyColumnSpecificationItemsForPopulationCohortItemDisplayObjects();

                #endregion

                #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

                await PopulationCohortsDataGridPresentationServiceInstance.GenerateDataGridColumnCollectionManuallyAsync(
                    columnSpecificationItems);

                #endregion

                #region create a Presenter to provide a datacontext and hence row collection and datacontext for the PresentationService

                await PopulationCohortsDataGridVm.PopulatePresenterAsync(
                    currentSeries,
                    currentEvent,
                    PopulateTableHeadings(),
                    null,
                    EnumStrings.RowsAreUnGrouped,
                    CohortColumnFormatEnum,
                    null, AllDataGridLineItemDisplayObjects);

                #endregion

                #endregion

                var lineCount = AllDataGridLineItemDisplayObjects.Length;

                // because the line items include the totals line, subtract 1

                var messageOk = lineCount - 1 == 1 ? "1 cohort." : $"{lineCount - 1} cohorts.";

                return messageOk;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region CboLookupKindOfCohortOnSelectionChangedAsync

        public virtual bool CboLookupKindOfCohortOnSelectionChangedCanExecute()
        {
            return CboLookupKindOfCohortVm.IsAuthorisedToOperate;
        }

        public async void CboLookupKindOfCohortOnSelectionChangedExecuteAsync()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[CboLookupKindOfCohortOnSelectionChangedExecuteAsync]";

            try
            {
                if (!CboLookupKindOfCohortOnSelectionChangedCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

                DeadenGui();

                await CboLookupKindOfCohortOnSelectionChangedAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (!JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                {
                    EvaluateGui();
                }
                else
                {
                    await PopulationCohortsDataGridVm.ZeroiseAsync();

                    EvaluateGui();
                }

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        public async Task<bool> CboLookupKindOfCohortOnSelectionChangedAsync()
        {
            const string failure = "Unable to execute selection changed command.";
            const string locus = "[CboLookupKindOfCohortOnSelectionChangedAsync]";

            try
            {
                return await Task.FromResult(true);
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #endregion

        #region make titles

        protected string MakeOrganiserTitle()
        {
            return GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem.Organizer?.Title ?? string.Empty;
        }

        protected string MakeSeriesTitle()
        {
            return GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.Title ?? string.Empty;
        }

        protected abstract string MakeEventTitle();

        protected string MakeMoreInfoTitle()
        {
            return CboLookupKindOfCohortVm?.CurrentItem?.Title ?? string.Empty;
        }

        #endregion

        #region make labels

        protected string MakeSeriesLabel()
        {
            return GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.Label ?? string.Empty;
        }

        protected string MakeEventLabel()
        {
            return GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem?.Label ?? string.Empty;
        }

        protected string MakeMoreInfoLabel()
        {
            return CboLookupKindOfCohortVm?.CurrentItem?.Label ?? string.Empty;
        }

        #endregion

        #region helpers

        protected abstract string[] PopulateTableHeadings();

        protected abstract MoreInformationItem PopulateDataGridTitleAndBlurb();

        protected async Task<PopulationCohortItem[]> PopulateTableOfCohortLineItems(IHasCohorts repository, string desiredCohortEnumString)
        {
            var cohortAnalysisLineItems = Array.Empty<PopulationCohortItem>();

            // null repository is a perfectly legitimate outcome for all those occasions where data hasn't been upload yet or event is in the future

            if (repository is not null)
                cohortAnalysisLineItems = desiredCohortEnumString switch
                {
                    EnumStrings.HistogramForEachRace => await repository.GetRaceCohortsFoundAsync(),
                    EnumStrings.HistogramForEachSex => await repository.GetGenderCohortsFoundAsync(),
                    EnumStrings.HistogramForEachAgeGroup => await repository.GetAgeGroupCohortsFoundAsync(),
                    EnumStrings.HistogramForEachCity => await repository.GetCityCohortsFoundAsync(),
                    _ => []
                };

            cohortAnalysisLineItems ??= [];

            var rowCollection = cohortAnalysisLineItems
                .Where(z => z is not null)
                .OrderBy(z => z.NameOfCohort).ToList();


            var totalSexMale = rowCollection.Sum(z => z.SexMaleCount);
            var totalSexFemale = rowCollection.Sum(z => z.SexFemaleCount);
            var totalSexOther = rowCollection.Sum(z => z.SexOtherCount);

            var totalPopulationInTable = totalSexMale + totalSexFemale + totalSexOther;

            var totalsLineItem = new PopulationCohortItem
            {
                NameOfCohort = "Total",
                SexMaleCount = totalSexMale,
                SexFemaleCount = totalSexFemale,
                SexOtherCount = totalSexOther,
                TotalCount = totalPopulationInTable,
                Percent = 0
            };

            var tableOfCohorts = rowCollection.Append(totalsLineItem);

            return tableOfCohorts.ToArray();
        }

        protected bool PopulateSourceUriStringOfAllImageUriItems(EventProfileItem currentEventProfile)
        {
            if (!JghFilePathValidator.IsValidContainerLocationSpecification(
                    GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DatabaseAccountName,
                    GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DataContainerName,
                    out var _))
                return true;

            if (currentEventProfile?.EventSettingsItem?.UriItems is null) return true;

            foreach (var uriItem in currentEventProfile.EventSettingsItem.UriItems.Where(z => z is not null))
            {
                if (!JghFilePathValidator.IsValidBlobName(uriItem.BlobName, out var _))
                    continue;

                uriItem.SourceUriString = JghFilePathValidator
                    .MakeAzureAndRezultzCompliantBlobUriHttps(
                        GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DatabaseAccountName,
                        GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DataContainerName,
                        uriItem.BlobName).ToString();
            }

            return true;
        }

        /// <summary>
        ///     Loads the repository of results failing noisily.
        ///     if all goes well, there might be some or other JghAlertMessageException
        ///     if there is no internet connection, innermost exception will be a JghCommunicationFailureException. a perfectly
        ///     foreseeable happenstance.
        ///     throws if no blob posted yet, or event was cancelled. a perfectly legitimate happenstance. innermost exception will
        ///     be a JghResultsData404Exception
        ///     totally normal outcome to come back with zero results
        /// </summary>
        /// <param name="leaderboardResultsSvcAgent"></param>
        /// <param name="seriesProfileItem"></param>
        /// <param name="eventProfileItem"></param>
        /// <returns></returns>
        protected abstract Task<IHasCohorts> ObtainRepositoryWithCohortsAsync(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent, SeriesProfileItem seriesProfileItem, EventProfileItem eventProfileItem);

        protected void ThrowExceptionIfAnyFundamentalNullCheckFails()
        {
            if (!GlobalSeasonProfileAndIdentityValidationVm.ThisViewModelIsInitialised)
                throw new JghAlertMessageException("Season profile not initialised. Please enter a valid ID.");

            if (GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is null)
                throw new JghAlertMessageException("Season profile not loaded. Please enter a valid ID.");

            if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm is null)
                throw new JghAlertMessageException("Season profile loaded but list of series is null.");


            if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem is null)
                throw new JghAlertMessageException("Season profile loaded but series not yet selected.");

            if (!GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.ItemsSource.Any())
                throw new JghInvalidValueException(Strings2017.List_of_events_is_empty);

            if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem is null)
                throw new JghAlertMessageException("Season profile loaded but event not selected.");
        }

        #endregion

        #region zeroisation stuff

        protected async Task<bool> ZeroiseAsync()
        {
            AllDataGridLineItemDisplayObjects = [];

            HeadersVm.Zeroise();
            FootersVm.Zeroise();

            await ZeroisePopulationCohortItemDataGridPresenterAndPrinterAsync();

            await ZeroiseItemsSourcesOfCboPresentersAsync();

            AllGuiControlsThatTouchDataAreAuthorisedToOperate(false);

            //PopulationCohortsDataGridVm.IsVisible = false;

            return true;
        }

        protected async Task<bool> ZeroiseItemsSourcesOfCboPresentersAsync()
        {
            await CboLookupKindOfCohortVm.ZeroiseItemsSourceAsync();

            return true;
        }

        private async Task ZeroisePopulationCohortItemDataGridPresenterAndPrinterAsync()
        {
            await PopulationCohortsDataGridVm.ZeroiseAsync();

            PopulationCohortsDataGridDesigner.ZeroiseDesigner();
        }

        #endregion

        #region Gui stuff

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
        {
            var answer = new List<object>();

            AddToCollectionIfIHasIsAuthorisedToOperate(answer, LoadSourceDataButtonVm);
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, SavePopulationCohortsToDocumentsAsHtmlWebpageButtonVm);
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupKindOfCohortVm);

            return answer;
        }

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
        {
            LoadSourceDataButtonVm.IsAuthorisedToOperate = true;
            ApplyFiltersButtonVm.IsAuthorisedToOperate = true;
            SavePopulationCohortsToDocumentsAsHtmlWebpageButtonVm.IsAuthorisedToOperate = PopulationCohortsDataGridVm.ItemsSource.Any();

            CboLookupKindOfCohortVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
        }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
        {
            #region toggle according to makeVisible

            PopulationCohortsDataGridVm.IsVisible = makeVisible;

            List<object> MakeListOfAllButtonsThatTouchData()
            {
                var answer = new List<object>();

                AddToCollectionIfIHasIsVisible(answer, LoadSourceDataButtonVm);
                AddToCollectionIfIHasIsVisible(answer, ApplyFiltersButtonVm);
                AddToCollectionIfIHasIsVisible(answer, SavePopulationCohortsToDocumentsAsHtmlWebpageButtonVm);

                return answer;
            }

            foreach (var buttonVm in MakeListOfAllButtonsThatTouchData())
                if (buttonVm is IHasIsVisible xx)
                    xx.IsVisible = makeVisible;

            #endregion

            #region toggle according to embellished makeVisible

            switch (makeVisible)
            {
                case true:
                    SavePopulationCohortsToDocumentsAsHtmlWebpageButtonVm.IsVisible = PopulationCohortsDataGridVm.ItemsSource.Any();
                    PopulationCohortsDataGridVm.IsVisible = PopulationCohortsDataGridVm.ItemsSource.Any();
                    break;
                default:
                    PopulationCohortsDataGridVm.IsVisible = false;
                    break;
            }

            #endregion

            #region final kick at the cat

            HeadersVm.IsVisible = true;
            FootersVm.IsVisible = true;

            LoadSourceDataButtonVm.IsVisible = true;

            #endregion
        }

        #endregion

        #region GenesisAsLastKnownGood

        private SeasonProfileItem _lastKnownGoodSeasonProfileItem;

        private SeriesItemDisplayObject _lastKnownGoodSeriesItemVm;

        private EventItemDisplayObject _lastKnownGoodEventItemVm;

        protected void SaveGenesisOfThisViewModelAsLastKnownGood()
        {
            _lastKnownGoodSeasonProfileItem = GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem;
            _lastKnownGoodSeriesItemVm = GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem;
            _lastKnownGoodEventItemVm = GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem;

            //_lastKnownGoodKindOfCohort = MoreInformationItemDisplayObject.ObtainSourceModel(CboLookupKindOfCohortVm.CurrentItem);
        }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
        {
            if (_lastKnownGoodSeasonProfileItem != GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem)
                return true;

            if (_lastKnownGoodSeriesItemVm != GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem)
                return true;

            if (_lastKnownGoodEventItemVm != GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem)
                return true;

            //if (_lastKnownGoodKindOfCohort != MoreInformationItemDisplayObject.ObtainSourceModel(CboLookupKindOfCohortVm.CurrentItem))
            //    return true;


            return false;
        }

        #endregion
    }
}