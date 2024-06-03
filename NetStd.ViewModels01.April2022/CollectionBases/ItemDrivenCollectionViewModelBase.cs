using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

/* NB !!! remove the delays throughout these methods at your utmost peril.
* Prism trick. This method might be being invoked by a Command that is potentially recursive.  
* In Silverlight, WPF, UWP, and Xamarin, changing the selected index (in the case of a ComboBox or Picker control)
* or the selected item (in the case of a ListView/CollectionView/TelerikRadDataGrid )
* causes the control to raise the SelectionChanged event. In MVVM, this precipitates an unintended infinite loop
* in the execution of the SelectionChangedCommandExecute. Likewise in the case of a button, a ButttonClickCommandExecute method.
* Why? Because in MVVM it is invariably from within a CommandExecute method that we programmatically
* drive changes to a SelectedItem/Index. The instant the programmer changes the selected item/index
* (in TwoWay binding mode), he causes event to be raised and the binding engine to relaunch
* the CommandExecute method! Thus, the method recalls itself automatically. And so on add infinitum. Infinite loop!
* To prevent the catastrophe, we use the trick of extinguishing the CommandExecute
* method. We keep it extinguished for long enough for the binding engine to effect the change on the GUI and
* the event to arrive back at the CommandExecute method. Still extinguished, nothing happens. The loop is forestalled.
* Only afterwards do we restore the CommandExecute. The choice of length of delay is fraught. Too short
* and it might not do the job (for one or more of the kinds of controls on different devices with different speeds
* and binding engines); too long and the user might notice the delay or, worse, be scuppered by a mysteriously
* non-functional button or collection control.
*/

namespace NetStd.ViewModels01.April2022.CollectionBases
{
    [DataContract]
	public class ItemDrivenCollectionViewModelBase<T> : BindableBase, IHasIsAuthorisedToOperate
		where T : class, INotifyPropertyChanged, IHasGuid, new()
	{
		#region ctor

		public ItemDrivenCollectionViewModelBase(string label, Action onSelectionChangedExecuteAction,
			Func<bool> onSelectionChangedCanExecuteFunc)
		{
			Label = label ?? string.Empty;

			IsAuthorisedToOperate = false;

            LastKnownGoodSelectedUniqueItemIdString = string.Empty;

			SelectedItem = null; //the default on xaml views where the value is null if nothing selected

			SelectedItems = null; 

			_delegateCommandThatDoesNothing =
				new DelegateCommand(
					DummyCommandExecuteActionThatDoesNothing,
					DummyCommandCanExecuteFuncThatIsAlwaysFalse);

			if (onSelectionChangedExecuteAction == null)
				onSelectionChangedExecuteAction = DummyCommandExecuteActionThatDoesNothing;

			if (onSelectionChangedCanExecuteFunc == null)
				onSelectionChangedCanExecuteFunc = DummyCommandCanExecuteFuncThatIsAlwaysFalse;

			_asInstantiatedDelegateCommand =
				new DelegateCommand(onSelectionChangedExecuteAction, onSelectionChangedCanExecuteFunc);

			OnSelectionChangedCommand = _asInstantiatedDelegateCommand;
		}

        #endregion

        #region constants

        /* on a Dell xps15 a delay of 200 ms has done the job over the years but i have determined empirically
         * that this is too short for even fast new phones. 
         */

        private const int FactorySettingForSafeDelayForBindingEngineMilliSec = 200; // the default

        #endregion

        #region fields

        internal string LastKnownGoodSelectedUniqueItemIdString;

        internal int DangerouslyBriefSafetyMarginForBindingEngineMilliSec = FactorySettingForSafeDelayForBindingEngineMilliSec; // merely a default

        private readonly DelegateCommand _asInstantiatedDelegateCommand;

		private readonly DelegateCommand _delegateCommandThatDoesNothing;

        #endregion

        #region properties

        #region Label

        private string _backingstoreLabel;

        //[DataMember]
        public string Label
        {
            get => _backingstoreLabel ??= string.Empty;
            set => SetProperty(ref _backingstoreLabel, value);
        }

        #endregion

        #region IsVisible

        private bool _backingstoreIsVisible;

        //[DataMember]
        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        #endregion

        #region IsAuthorisedToOperate

        private bool _backingstoreIsAuthorisedToOperate;

		public bool IsAuthorisedToOperate
		{
			get => _backingstoreIsAuthorisedToOperate;
			set => SetProperty(ref _backingstoreIsAuthorisedToOperate, value);
		}

		private bool _capturedIsAuthorisedToOperateValue;

        #endregion

        #region IsDropDownOpen

        private bool _backingstoreIsDropDownOpen;

        //[DataMember]
        public bool IsDropDownOpen
        {
            get => _backingstoreIsDropDownOpen;
            set => SetProperty(ref _backingstoreIsDropDownOpen, value);
        }

        #endregion

        #region ItemsSource

        public ObservableCollection<T> ItemsSource { get; } = [];

		#endregion

		#region CurrentIndex - NB no public setter - driven by setter of SelectedItem

		private int _backingstoreCurrentIndex;

		[DataMember]
		public int CurrentIndex
		{
			get => _backingstoreCurrentIndex;
			private set => SetProperty(ref _backingstoreCurrentIndex, value);
		}

		#endregion

		#region SelectedItem - setter sets CurrentIndex

		private T _backingstoreSelectedItem;

		[DataMember]
		public T SelectedItem
		{
			get => _backingstoreSelectedItem;
			set
			{
				SetProperty(ref _backingstoreSelectedItem, value);

				CurrentIndex = FindIndexOfSelectedItem();
			}
		}

		#endregion

		#region SelectedItems - applicable to xaml controls that allow mutiple selection

		private List<T> _backingstoreSelectedItems;

		[DataMember]
		public List<T> SelectedItems
		{
			get => _backingstoreSelectedItems;
			set => SetProperty(ref _backingstoreSelectedItems, value);
		}

		#endregion

		#region OnSelectionChangedCommand


		private DelegateCommand _backingstoreOnSelectionChangedCommand;

		public DelegateCommand OnSelectionChangedCommand
		{
			// N.B. private _backingstore with a private setter. 
			// slightly risky practice to allow this to be protected as opposed to private. intent is that it is allowed to be 
			// assigned in the ctor and nowhere else. Internally it can be swapped in and out as a consequence 
			// of ExtinguishOnSelectionChangedCommand(), and RestoreInstantiatedOnSelectionChangedCommand() 
			// - both of which are private. this is a clever defensive approach

			get => _backingstoreOnSelectionChangedCommand;
			private set => SetProperty(ref _backingstoreOnSelectionChangedCommand, value);
		}

		#endregion

		#endregion

		#region methods

		#region Visiblity

		public void MakeVisibleIfItemsSourceIsAny()
		{
			if (ItemsSource == null)
			{
				IsVisible = false;
				return;
			}

			IsVisible = ItemsSource.Any();
		}

		public void MakeVisibleIfItemsSourceIsGreaterThanOne()
		{
			if (ItemsSource == null)
			{
				IsVisible = false;
				return;
			}

			IsVisible = ItemsSource.Count > 1;
		}

		public void MakeVisibleIfItemsSourceIsGreaterThanTwo()
		{
			if (ItemsSource == null)
			{
				IsVisible = false;
				return;
			}

			IsVisible = ItemsSource.Count > 2;
		}

		#endregion

		#region IsAuthorisedToOperate

		public void CaptureIsAuthorisedToOperateValue()
		{
			_capturedIsAuthorisedToOperateValue = IsAuthorisedToOperate;
		}

		public void RestoreCapturedIsAuthorisedToOperateValue()
		{
			IsAuthorisedToOperate = _capturedIsAuthorisedToOperateValue;
		}

        public void MakeAuthorisedToOperateIfItemsSourceIsAny()
        {
            if (ItemsSource == null)
            {
                IsAuthorisedToOperate = false;
                return;
            }

            IsAuthorisedToOperate = ItemsSource.Any();
        }

        public void MakeAuthorisedToOperateIfItemsSourceIsMoreThanOne()
        {
            if (ItemsSource == null)
            {
                IsAuthorisedToOperate = false;
                return;
            }

            IsAuthorisedToOperate = ItemsSource.Count > 1;
        }


        #endregion

        #region Modify contents of ItemsSource

        public async Task<bool> AddItemToItemsSourceAsync(T item)
		{
			ExtinguishOnSelectionChangedCommand();

			if (item != null)
				ItemsSource.Add(item);

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> DeleteItemFromItemsSourceAsync(T item)
		{
			ExtinguishOnSelectionChangedCommand();

			if (item != null)
				ItemsSource.Remove(item);

            await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> AddRangeToItemsSourceAsync(IEnumerable<T> items)
		{
			ExtinguishOnSelectionChangedCommand();

			if (items !=null)
				foreach (var item in items)
				{
					if(item != null)
						ItemsSource.Add(item);
				}

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

        public async Task<bool> RefillItemsSourceAsync(IEnumerable<T> items)
        {
            ExtinguishOnSelectionChangedCommand();

            ItemsSource.Clear();

            if (items != null)
                foreach (var item in items)
                {
                    if (item != null)
                        ItemsSource.Add(item);
                }


            LastKnownGoodSelectedUniqueItemIdString = string.Empty;

            SelectedItem = null; // is this required or advantageous or dangerous?

            await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

            RestoreInstantiatedOnSelectionChangedCommand();

            return true;
        }

        public async Task ZeroiseItemsSourceAsync()
        {
            await RefillItemsSourceAsync(Array.Empty<T>());
        }

        #endregion

        #region Index operations

        private int FindIndexOfSelectedItem()
		{
			if (SelectedItem == null)
				return -1;

			var answer = JghArrayHelpers.SelectArrayIndexOfItemInArrayByItemEquality(ItemsSource.ToArray(), SelectedItem);

			return answer;
		}

		#endregion

		#region Item operations

		public T GetItemByItemIndex(int index)
		{
			if (ItemsSource == null)
				return null;

			return JghArrayHelpers.SelectItemFromArrayByArrayIndex(ItemsSource.ToArray(),
				index);
		}

		public void SaveSelectedItemAsLastKnownGood()
		{
			if (SelectedItem == null)
			{
				LastKnownGoodSelectedUniqueItemIdString = string.Empty;

				return;
			}

			LastKnownGoodSelectedUniqueItemIdString = SelectedItem.Guid;
		}

		public bool SelectedItemIsTheSameAsLastKnownGood()
		{
			if (SelectedItem == null)
			{
				return LastKnownGoodSelectedUniqueItemIdString == string.Empty;
			}

			return SelectedItem.Guid == LastKnownGoodSelectedUniqueItemIdString;
		}

		public async Task<bool> RestoreSelectedItemToLastKnownGoodAsync()
		{
			ExtinguishOnSelectionChangedCommand();

			var lastKnownGood = JghArrayHelpers.SelectItemFromArrayByItemGuidString(ItemsSource.ToArray(),
				LastKnownGoodSelectedUniqueItemIdString);

			SelectedItem = ItemsSource.Contains(lastKnownGood) ? lastKnownGood : null;

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> ChangeSelectedItemAsync(T candidateItem)
		{
			ExtinguishOnSelectionChangedCommand();

			if (candidateItem != null && ItemsSource.Contains(candidateItem)) // beware how equality comparer might or might not work for the particular type
			{
				SelectedItem = candidateItem; // beware. target user control raises SelectionChanged event. recursive

				await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);
			}

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> ChangeSelectedItemToNullAsync()
		{
			ExtinguishOnSelectionChangedCommand();

			SelectedItem = null;

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}



        #endregion

        #region Binding engine safe delay

        public void OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(int milliseconds)
        {
            switch (milliseconds)
            {
                case < 0:
                    DangerouslyBriefSafetyMarginForBindingEngineMilliSec = 0;
                    return;
                case > FactorySettingForSafeDelayForBindingEngineMilliSec:
                    DangerouslyBriefSafetyMarginForBindingEngineMilliSec = FactorySettingForSafeDelayForBindingEngineMilliSec;
                    return;
                default:
                    DangerouslyBriefSafetyMarginForBindingEngineMilliSec = milliseconds;
                    break;
            }
        }

        public void RestoreFactorySettingForSafetyMarginForGuiBindingEngine()
        {
            DangerouslyBriefSafetyMarginForBindingEngineMilliSec = FactorySettingForSafeDelayForBindingEngineMilliSec;
        }

        #endregion

        #region OnSelectionChangedCommand operations

        public void ExtinguishOnSelectionChangedCommand()
        {
            OnSelectionChangedCommand = _delegateCommandThatDoesNothing;
        }

        public void RestoreInstantiatedOnSelectionChangedCommand()
        {
            OnSelectionChangedCommand = _asInstantiatedDelegateCommand;
        }

        #endregion

        #endregion

        #region helpers

        public async Task<bool> ZeroiseAsync()
		{
            Label = string.Empty;
            IsVisible = false;
            IsDropDownOpen = false;

            ExtinguishOnSelectionChangedCommand();

			ItemsSource.Clear();

			SelectedItem = null;

			LastKnownGoodSelectedUniqueItemIdString = string.Empty;

			IsAuthorisedToOperate = false;

			RestoreInstantiatedOnSelectionChangedCommand();

			return await Task.FromResult(true);
		}

		private static void DummyCommandExecuteActionThatDoesNothing()
		{
			// do nothing
		}

		private static bool DummyCommandCanExecuteFuncThatIsAlwaysFalse()
		{
			return false;
		}

		#endregion
	}
}