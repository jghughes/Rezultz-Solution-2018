using System;
using Windows.UI.Xaml.Controls;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Goodies.Mar2022;
using NetStd.ServiceLocation.Aug2022;

using RezultzPortal.Uwp.PageViewModels;
using System.Collections.Generic;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Interfaces02.July2018.Interfaces;

//using CommonServiceLocator;


namespace RezultzPortal.Uwp.UserControls
{
    public sealed partial class SearchForSplitIntervalsUserControl
    {
        private const string Locus2 = "[SearchForSplitIntervalsUserControl]";
        private const string Locus3 = "[RezultzPortal.Uwp]";

        #region ctor

        public SearchForSplitIntervalsUserControl()
        {
            InitializeComponent();
        }

        #endregion

        private KeepTimeViewModel ViewModel => DataContext as KeepTimeViewModel;

        #region fields

        private static IAlertMessageService AlertMessageServiceInstance
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
                        JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance,
                            "<IAlertMessageService>");

                    const string locus = "Property getter of <AlertMessageService]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        #endregion

        #region event handlers

        /// <summary>
        ///     This event gets fired anytime the text in the TextBox gets updated.
        ///     It is recommended to check the reason for the text changing by checking against args.Reason
        /// </summary>
        /// <param name="sender">The AutoSuggestBox whose text got changed.</param>
        /// <param name="args">The event arguments.</param>
        private async void MyAutoSuggestBox_OnTextChanged(AutoSuggestBox sender,
            AutoSuggestBoxTextChangedEventArgs args)
        {
            const string failure = "Unable to obtain shortlist of suggestions.";
            const string locus = "[MyAutoSuggestBox_OnTextChanged]";

            try
            {
                if (ViewModel is null)
                    throw new NullReferenceException(
                        "The DataContext is not an object of type KeepTimeViewModel. This is mandatory.");

                // We only want to get results when it was a user typing, otherwise we assume the value got filled in by TextMemberPath 
                // or the handler for SuggestionChosen
                if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;

                var matchingSuggestions =
                    await ViewModel.OnShortlistOfQuerySuggestionsRequestedFromTheSearchUniverseForSplitIntervalItemsAsync(sender.Text);

                List<string> listOfMatchingSuggestions = matchingSuggestions.ToList();

                if (listOfMatchingSuggestions.Count == 0)
                {
                    listOfMatchingSuggestions.Add("Nothing found");
                }

                sender.ItemsSource = listOfMatchingSuggestions;
            }

            #region trycatch

            catch (Exception ex)
            {
                await AlertMessageServiceInstance.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3,
                    ex);
            }

            #endregion
        }

        /// <summary>
        ///     This event is raised as the user keys through the list, or taps on a suggestion.
        ///     The AutoSuggestBox.TextMemberPath property controls what text appears in the TextBox.
        ///     You could use this event to trigger a prefetch of the suggestion.
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">
        ///     The args contain SelectedItem, which contains the data item of the item that is currently
        ///     highlighted.
        /// </param>
        private void MyAutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender,
            AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // nothing much i can think of. i don't need to do anything behind the scenes.
            // Anyhow, here is how you obtain the SelectedItem for whatever purpose you might choose in future

            // ReSharper disable once UnusedVariable
            var theSelectedSearchSuggestion = (string) args.SelectedItem;
        }

        /// <summary>
        ///     This event gets fired when:
        ///     * a user presses Enter while focus is in the TextBox
        ///     * a user clicks or tabs to and invokes the query button (defined using the QueryIcon API)
        ///     * a user presses selects (clicks/taps/presses Enter) a suggestion
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">
        ///     The args contain the QueryText, which is the text in the TextBox,
        ///     and also ChosenSuggestion, which is only non-null when a user selects an item in the list.
        /// </param>
        private async void MyAutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            const string failure = "Unable to respond to submission of search query.";
            const string locus = "[MyAutoSuggestBox_OnSuggestionChosen]";

            try
            {
                if (ViewModel is null)
                    throw new NullReferenceException(
                        "The DataContext is not an object of type KeepTimeViewModel. This is mandatory.");


                if (args.ChosenSuggestion is not null)
                {
                    // user selected an item, take an action on it here
                    await ViewModel.OnFinalSearchQuerySubmittedAsTextForSplitIntervalsAsync((string) args.ChosenSuggestion);
                }
                else
                {
                    // Do a fuzzy search on the query text
                    if (!string.IsNullOrWhiteSpace(args.QueryText))
                        await ViewModel.OnFinalSearchQuerySubmittedAsTextForSplitIntervalsAsync(args.QueryText);
                }
            }

            #region trycatch

            catch (Exception ex)
            {
                await AlertMessageServiceInstance.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3,
                    ex);
            }

            #endregion
        }

        #endregion
    }
}
