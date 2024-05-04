using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;

namespace NetStd.ViewModels01.April2022.CollectionBases
{
    public abstract class DataGridViewModelBase<T> : ItemDrivenCollectionViewModel<T>
	    where T : class, INotifyPropertyChanged, IHasCollectionLineItemPropertiesV2, new()
    {
        private const int DefaultDataPagerPageSize = 500;
        // protection against slow load-times for v large datasets of individual results, don't go much above 1000. i used a DataPager in Silverlight, but at the time of writing i'm not here, so the DataPager functionality is dormant

        #region ctor

        public DataGridViewModelBase(string caption, Action onSelectionChangedExecuteAction,
            Func<bool> onSelectionChangedCanExecuteFunc) : base(caption, onSelectionChangedExecuteAction,
            onSelectionChangedCanExecuteFunc)
        {
            DataPagerPageSize = DefaultDataPagerPageSize;
        }

        #endregion

        #region props

        #region HeadersVm

        public HeaderOrFooterViewModel HeadersVm { get; } = new HeaderOrFooterViewModel();

        #endregion

        #region HeadingRhsTextVm

        public TextBlockControlViewModel HeadingRhsTextVm { get; } = new TextBlockControlViewModel();

        #endregion

        #region DataPagerPageSize

        private int _backingstoreDataPagerPageSize;

        public int DataPagerPageSize
        {
            get => _backingstoreDataPagerPageSize;
            set => SetProperty(ref _backingstoreDataPagerPageSize, value);
        }

        #endregion

        #region DataPagerIsVisible

        private bool _backingstoreDataPagerIsVisible;

        public bool DataPagerIsVisible
        {
            get => _backingstoreDataPagerIsVisible;
            set => SetProperty(ref _backingstoreDataPagerIsVisible, value);
        }

        #endregion

        #region ColumnFormatEnum

        private string _backingstoreColumnFormatEnum;

        public string ColumnFormatEnum
        {
            get => _backingstoreColumnFormatEnum ??= string.Empty;
            set => SetProperty(ref _backingstoreColumnFormatEnum, value);
        }

        #endregion

        #region RowGroupingFormatEnum

        private string _backingstoreRowGroupingFormatEnum;

        public string RowGroupingFormatEnum
        {
            get => _backingstoreRowGroupingFormatEnum ??= string.Empty;
            set => SetProperty(ref _backingstoreRowGroupingFormatEnum, value);
        }

        #endregion

        #endregion

        #region methods

        public new virtual async Task<bool> RefillItemsSourceAsync(IEnumerable<T> rowCollection)
        {
            await base.RefillItemsSourceAsync(rowCollection);

            return true;

        }

        public new async Task<bool> ZeroiseAsync()
        {
            await base.ZeroiseAsync();

            HeadersVm.Zeroise();
            HeadingRhsTextVm.Zeroise();
            DataPagerPageSize = DefaultDataPagerPageSize;
            DataPagerIsVisible = false;
            ColumnFormatEnum = string.Empty;
            RowGroupingFormatEnum = string.Empty;

            return true;
        }

        #endregion
    }
}