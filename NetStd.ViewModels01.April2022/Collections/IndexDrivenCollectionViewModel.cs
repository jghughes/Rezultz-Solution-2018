using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.ViewModels01.April2022.CollectionBases;

namespace NetStd.ViewModels01.April2022.Collections
{
    [DataContract]
	public class IndexDrivenCollectionViewModel<T> : IndexDrivenCollectionViewModelBase<T>
		where T : class, INotifyPropertyChanged, IHasItemID, IHasLabel, IHasEnumString, new()
	{
		#region ctor

		public IndexDrivenCollectionViewModel(string caption, Action onSelectionChangedExecuteAction,
			Func<bool> onSelectionChangedCanExecuteFunc) : base(caption, onSelectionChangedExecuteAction,
			onSelectionChangedCanExecuteFunc)
		{
		}

		#endregion

		#region methods

		public int GetItemIndexByItemId(int searchParameter)
		{
			var answer = JghArrayHelpers.SelectArrayIndexOfItemInArrayByItemId(ItemsSource, searchParameter);

			return ItemsSource == null
				? -1
				: answer;
		}

		public T GetItemByItemEnumString(string searchParameter)
		{
			var answer = JghArrayHelpers.SelectItemFromArrayByItemEnumString(ItemsSource,
				searchParameter);

			return ItemsSource == null
				? null
				: answer;
		}

		public T GetItemByItemLabel(string searchParameter)
		{
			var answer = JghArrayHelpers.SelectItemFromArrayByItemLabel(ItemsSource,
				searchParameter);

			return ItemsSource == null
				? null
				: answer;
		}

		public async Task<bool> ChangeSelectedIndexToMatchItemIdAsync(int searchParameter)
		{
			var candidateIndex = JghArrayHelpers.SelectArrayIndexOfItemInArrayByItemId(
				ItemsSource,
				searchParameter);

			await ChangeSelectedIndexAsync(candidateIndex);

			return true;
		}

		public async Task<bool> ChangeSelectedIndexToMatchItemLabelAsync(string searchParameter)
		{
			var candidateIndex = JghArrayHelpers.SelectArrayIndexOfItemInArrayByItemLabel(
				ItemsSource,
				searchParameter);

			await ChangeSelectedIndexAsync(candidateIndex);

			return true;
		}

		public async Task<bool> ChangeSelectedIndexToMatchItemEnumStringAsync(string searchParameter)
		{
			var candidateIndex = JghArrayHelpers.SelectArrayIndexOfItemInArrayByItemEnumString(
				ItemsSource,
				searchParameter);

			await ChangeSelectedIndexAsync(candidateIndex);

			return true;
		}

		#endregion
	}
}