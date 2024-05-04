using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.ViewModels01.April2022.CollectionBases;

namespace NetStd.ViewModels01.April2022.Collections
{
	[DataContract]
	public class ItemDrivenCollectionViewModel<T> : ItemDrivenCollectionViewModelBase<T>
		where T : class, INotifyPropertyChanged, IHasCollectionLineItemPropertiesV2, new()
	{
		#region ctor

		public ItemDrivenCollectionViewModel(string caption, Action onSelectionChangedExecuteAction,
			Func<bool> onSelectionChangedCanExecuteFunc) : base(caption, onSelectionChangedExecuteAction,
			onSelectionChangedCanExecuteFunc)
		{
		}

		#endregion

		#region methods

		public T GetItemByItemId(int searchParameter)
		{
			if (ItemsSource == null)
				return null;

			return JghArrayHelpers.SelectItemFromArrayByItemId(ItemsSource.ToArray(),
				searchParameter);
		}

		public T GetItemByItemEnumString(string searchParameter)
		{
			if (ItemsSource == null)
				return null;

			return JghArrayHelpers.SelectItemFromArrayByItemEnumString(ItemsSource.ToArray(),
				searchParameter);
		}

		public async Task<bool> ChangeSelectedItemToMatchItemIdAsync(int searchParameter)
		{
			var candidateItem = GetItemByItemId(searchParameter);

			await ChangeSelectedItemAsync(candidateItem);

			return true;
		}

		public async Task<bool> ChangeSelectedItemToMatchItemEnumStringAsync(string searchParameter)
		{
			var candidateItem = GetItemByItemEnumString(searchParameter);

			await ChangeSelectedItemAsync(candidateItem);

			return true;
		}

		#endregion
	}
}