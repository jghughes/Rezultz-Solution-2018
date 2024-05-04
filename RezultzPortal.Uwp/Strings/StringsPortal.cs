
// ReSharper disable InconsistentNaming

namespace RezultzPortal.Uwp.Strings
{
    internal class StringsPortal
    {
        public const string ServiceUpAndRunning = "Service up and running";

        public const string Target_event = "Target event : ";
        public const string Not_yet_launched = "not yet launched";
        public const string Launch_succeeded = "Launch succeeded. Ready to start work. You may continue.";

        public const string Must_complete_launch_of_work_session = "Please continue to complete launch of work session in order to start work.";

        public const string WorkSessionNotLaunched = "Launch of work session is incomplete. Please launch this session.";
        public const string IdentityNotAuthenticated = "Identity not authenticated. Please authenticate yourself.";
        public const string IdentityNotAuthorisedForWorkRole = "Identity not authorised for this work role. You require suitable authorisation to continue.";

        public const string SelectedSeriesIsNull = "Selected series is null.";
        public const string SelectedEventIsNull = "Selected event is null.";

        public const string PublishingProfileNotFound = "Publishing profile not found. Please enter a valid publishing module ID.";

        public const string SeasonDataNotInitialised = "Season profile not loaded. Please submit a valid ID.";

        public const string NoConnection = "No connection.";

        public const string Unable_to_retrieve_instance = "Unable to retrieve an object from dependency injection container. Item is not registered in the container. Coding error.";

        public const string Welcome__ = "Welcome";

        public const string Dnx_status = "Dnx status";

        public const string Clock_mode = "Clock mode";

        public const string WorkingSaving = "Working .... saving";
        public const string WorkingFormatting = "Working .... formatting";
        public const string GettingReady = "Working .... getting ready";
        public const string WakingServer = "Working .... waking server";
        public const string Working = "Working .... ";

        public const string Working_____looking_for_data = "Working .... looking for data";
        public const string Working_____processing = "Working .... processing";
        public const string Working_____saving = "Working .... saving";
        public const string Working_____downloading = "Working .... downloading";
        public const string Working_____importing = "Working .... importing";

        public const string Working______pulling_timestamps_from_rezultz_hub = "Working .... pulling timestamp data from rezultz hub";
        public const string Working______loading_timestamps_into_repository = "Working ... loading timestamp data into repository";
        public const string Working______consolidating_timestamps_into_consolidated_splitintervals_for_each_participant = "Working ... generating split-intervals for each_participant";

        public const string Working______pulling_participant_profiles_from_rezultz_hub = "Working .... pulling participant data from rezultz hub";
        public const string Working______loading_participants_into_repository = "Working ... loading participant data into repository";
        public const string Working______generating_participant_master_list = "Working ... generating participant masterlist";
        public const string Working______generating_results = "Working ... generating results";
        public const string Working_____converting = "Working .... preprocessing";
        public const string Working______uploading = "Working .... uploading";

        public const string Working_____pushing_data_to_hub = "Working ... pushing data to hub";
        public const string Working_____adding_item_to_cache = "Working ... adding item to cache";
        public const string Working_____abandoning_modification = "Working ... abandoning modification";
        public const string Working_____deleting_all_data = "Working ... deleting all data";
        public const string Working_____committing_data = "Working ... committing data to storage";


        public const string Unable_to_proceed__No_datasets_as_yet = "Unable to proceed. You haven't yet obtained any datasets. You need to import data from the hub and/or your filesystem as required.";
        public const string Unable_to_proceed__Unable_to_deduce_a_blob_name_for_this_upload =
            "Unable to proceed. We are unable to deduce a blob name for this upload.  The <CurrentItem> property of the current <CboPreProcessedDataLocationForEventLookUpPresenter> is null.";
        public const string Unable_to_proceed__Unable_to_deduce_a_destination_for_upload_to_staging = "Unable to proceed. We are unable to deduce a destination for this upload to draft published results. Data error.";
        public const string Unable_to_proceed__Unable_to_deduce_a_destination_for_upload_to_production =
            "Unable to proceed. We are unable to deduce a destination for this upload. Data error. The <DatabaseForResultsPublished> property of the current <SeriesItem> is null.";
        public const string Unable_to_proceed__Unable_to_deduce_a_destination_for_upload = "Unable to proceed. We are unable to deduce a destination for this upload. Coding error. Enumeration value not found.";
        public const string Unable_to_proceed__You_need_to_authenticate_yourself_before_proceeding_ = "Unable to proceed. You need to authenticate yourself before proceeding.";


        public const string Sorry_not_authorised_for_workrole =
            "Sorry. Unable to proceed. Your  authorisation does not include clearance in a work role that allows you to post data.";


        public const string Incomplete__Compute_process_failed_to_run_to_completion = "Incomplete. Compute task failed to run to completion.";
        public const string Incomplete__nothing_saved = "Incomplete. Nothing saved.";
    }
}
