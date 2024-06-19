using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.ViewModels01.April2022.CollectionBases;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace NetStd.ViewModels01.April2022.Collections
{
    //IHasGuid, IHasItemID, IHasEnumString, IHasLabel
    public class
		DataGridViewModel<T> : DataGridViewModelBase<T> where T : class, INotifyPropertyChanged, IHasCollectionLineItemPropertiesV2, new()
	{
		private const string Locus2 = "[DataGridViewModel]";
		private const string Locus3 = "[NetStd.ViewModels01.April2022]";

		#region ctor

		public DataGridViewModel(string caption, Action onSelectionChangedExecuteAction,
			Func<bool> onSelectionChangedCanExecuteFunc) : base(caption, onSelectionChangedExecuteAction, onSelectionChangedCanExecuteFunc)
		{

			SeriesProfileToWhichThisPresenterRefers = null; // NB = null not new()

			EventProfileToWhichThisPresenterRefers = null; // NB = null not new()

			ArrayOfSexFilter = [];

			PresentationServiceInstanceEnum = string.Empty;
		}


		#endregion

		#region properties

		#region SeriesToWhichThisPresenterRefers

		private SeriesProfileItem _backingstoreSeriesProfileToWhichThisPresenterRefers;

		public SeriesProfileItem SeriesProfileToWhichThisPresenterRefers
		{
			get => _backingstoreSeriesProfileToWhichThisPresenterRefers;
			set => SetProperty(ref _backingstoreSeriesProfileToWhichThisPresenterRefers, value);
		}

		#endregion

		#region EventToWhichThisPresenterRefers

		private EventProfileItem _backingstoreEventProfileToWhichThisPresenterRefers;

		public EventProfileItem EventProfileToWhichThisPresenterRefers
		{
			get => _backingstoreEventProfileToWhichThisPresenterRefers;
			set => SetProperty(ref _backingstoreEventProfileToWhichThisPresenterRefers, value);
		}

		#endregion

		#region ArrayOfSexFilter 

		private CboLookupItem[] _backingstoreArrayOfSexFilter;

		public CboLookupItem[] ArrayOfSexFilter
		{
			get => _backingstoreArrayOfSexFilter ??= [];
			set => SetProperty(ref _backingstoreArrayOfSexFilter, value);
		}

		#endregion

		#region PresentationServiceInstanceEnum

		private string _backingstorePresentationServiceInstanceEnum;

		public string PresentationServiceInstanceEnum
		{
			get => _backingstorePresentationServiceInstanceEnum ??= string.Empty;
			set => SetProperty(ref _backingstorePresentationServiceInstanceEnum, value);
		}

		#endregion

		#endregion

		#region methods

		public new async Task<bool> ZeroiseAsync()
		{
			await base.ZeroiseAsync();

			SeriesProfileToWhichThisPresenterRefers = null; // NB = null not new()

			EventProfileToWhichThisPresenterRefers = null; // NB = null not new()

			ArrayOfSexFilter = [];

			PresentationServiceInstanceEnum = string.Empty;

			return true;
		}

		public async Task<bool> PopulatePresenterAsync(SeriesProfileItem seriesProfileToWhichThisPresenterRefers,
            EventProfileItem eventProfileToWhichThisPresenterRefers,
            string[] titlesAndSubtitles,
            CboLookupItem[] arrayOfSexFilter,
            string rowGroupingEnum,
            string columnFormatEnum,
            string leaderboardStyleOrFavoritesStyleEnum,
            T[] rowCollection)
		{
			const string failure = "Unable to populate presenter.";
			const string locus = "[PopulatePresenterAsync]";

			try
			{
				IsAuthorisedToOperate = false;

				IsVisible = false; // NB
				// this presenter is used for both the Leaderboard and Favorites. Because the Favorites datagrid is positioned above the Leaderboard 
				// and steals an varying amount of space on the page there depending on its size, it must not be simultaneously visible with
				// the Leaderboard during Layout or else a Windows.UI.Xaml.LayoutCycleException will be thrown. you have no alternative other than
				// to ensure IsVisible = false during layout. this causes screen flicker when you return to previously rendered page. oh well.

				HeadersVm.Populate(titlesAndSubtitles);

				SeriesProfileToWhichThisPresenterRefers = seriesProfileToWhichThisPresenterRefers ?? new SeriesProfileItem();
				EventProfileToWhichThisPresenterRefers = eventProfileToWhichThisPresenterRefers ?? new EventProfileItem();

				ArrayOfSexFilter = arrayOfSexFilter ?? [];

				RowGroupingFormatEnum = rowGroupingEnum ?? string.Empty;
				ColumnFormatEnum = columnFormatEnum ?? string.Empty;

				PresentationServiceInstanceEnum = leaderboardStyleOrFavoritesStyleEnum ?? string.Empty;

                await RefillItemsSourceAsync(rowCollection);

				IsAuthorisedToOperate = true;
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}

			return true;
		}

        public override async Task<bool> RefillItemsSourceAsync(IEnumerable<T> rowCollection)
        {
            await base.RefillItemsSourceAsync(rowCollection);

            UpdateHeadingRhsLineItemTotalAsync();

            HeadingRhsTextVm.IsVisible = true;

			UpdateDataPagerVisibility();

			return true;
        }

        private void UpdateHeadingRhsLineItemTotalAsync()
        {
            HeadingRhsTextVm.IsVisible = true;

            if (ItemsSource is null || ArrayOfSexFilter is null)
            {
                HeadingRhsTextVm.Text = string.Empty;

                return;
            }

            HeadingRhsTextVm.Text = $"total {ItemsSource.Count} rows";
        }

		private void UpdateDataPagerVisibility()
		{

			if (ItemsSource is null)
			{
				DataPagerIsVisible = false;

                return;
            }

			DataPagerIsVisible =
				ItemsSource.Count >= DataPagerPageSize; // 
		}

        #endregion

    }
}