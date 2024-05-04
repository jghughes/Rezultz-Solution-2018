using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

/* NB !!! remove the Task.Delays() throughout these methods at your utmost peril.
 * Prism trick. This method might be being invoked by a Command that is potentially recursive.  
* In Silverlight, WPF, UWP, and Xamarin, changing the selected index (in the case of a ComboBox or Picker control)
* or the selected item (in the case of a ListView/CollectionView/TelerikRadDataGrid )
* causes the control to raise the SelectionChanged event. In MVVM, this precipitates an unintended infinite loop
* in the execution of the SelectionChangedCommandExecute. Likewise in the case of a button, a ButttonClickCommandExecute method.
* Why? Because in MVVM it is invariably from within a CommandExecute method that we programmatically
* drive changes to a SalectedItem/Index. The instant the programmer changes the selected item/index
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
	public class IndexDrivenCollectionViewModelBase<T> : BindableBase, IHasIsAuthorisedToOperate 
        where T : class, INotifyPropertyChanged, new()
	{
		#region ctor

		public IndexDrivenCollectionViewModelBase(string label, Action onSelectionChangedExecuteAction,
			Func<bool> onSelectionChangedCanExecuteFunc)
		{
			Label = label;

			ItemsSource = Array.Empty<T>();

			LastKnownGoodSelectedIndex = -1;

			SelectedIndex = -1;

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

        internal int LastKnownGoodSelectedIndex;

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

        private T[] _backingstoreItemsSource;

		public T[] ItemsSource
		{
			// N.B. private setter. 
			// intent is that it is allowed to be assigned in the ctor and nowhere else. 

			get => _backingstoreItemsSource ??= Array.Empty<T>();
			private set => SetProperty(ref _backingstoreItemsSource, value);
		}

		#endregion

		#region SelectedIndex - setter sets CurrentItem

		private int _backingstoreSelectedIndex;

		[DataMember]
		public int SelectedIndex
		{
			get => _backingstoreSelectedIndex;
			set
			{
			
				SetProperty(ref _backingstoreSelectedIndex, value);

				CurrentItem = FindCurrentItem();
			}
		}

		#endregion

		#region CurrentItem - NB no public setter - driven by setter of SelectedIndex

		private T _backingstoreCurrentItem;

		[DataMember] public T CurrentItem
		{
			get => _backingstoreCurrentItem;
			private set => SetProperty(ref _backingstoreCurrentItem, value);
		}

		#endregion

		#region OnSelectionChangedCommand

		private DelegateCommand _backingstoreOnSelectionChangedCommand;

		public DelegateCommand OnSelectionChangedCommand
		{
			// N.B. private setter. 
			// intent is that it is allowed to be assigned in the ctor and nowhere else. 
			// Internally it can be swapped in and out as a consequence 
			// of ExtinguishOnSelectionChangedCommand(), and RestoreInstantiatedOnSelectionChangedCommand() 
			// - both of which are private. this is a clever defensive approach to prevent infinite looping

			get => _backingstoreOnSelectionChangedCommand;
			private set => SetProperty(ref _backingstoreOnSelectionChangedCommand, value);
		}

		#endregion

		#endregion

		#region methods

		#region IsVisible

		public void MakeVisibleIfItemsSourceIsAny()
		{
			if (_backingstoreItemsSource == null)
			{
				IsVisible = false;
				return;
			}

			IsVisible = _backingstoreItemsSource.Any();
		}

		public void MakeVisibleIfItemsSourceIsGreaterThanOne()
		{
			if (_backingstoreItemsSource == null)
			{
				IsVisible = false;
				return;
			}

			IsVisible = _backingstoreItemsSource.Length > 1;
		}

		public void MakeVisibleIfItemsSourceIsGreaterThanTwo()
		{
			if (_backingstoreItemsSource == null)
			{
				IsVisible = false;
				return;
			}

			IsVisible = _backingstoreItemsSource.Length > 2;
		}

		#endregion

		#region this viewmodel IsAuthorisedToOperate

		public void CaptureIsAuthorisedToOperateValue()
		{
			_capturedIsAuthorisedToOperateValue = _backingstoreIsAuthorisedToOperate;
		}

		public void RestoreCapturedIsAuthorisedToOperateValue()
		{
			IsAuthorisedToOperate = _capturedIsAuthorisedToOperateValue;
		}

        public void MakeAuthorisedToOperateIfItemsSourceIsAny()
        {
            if (_backingstoreItemsSource == null)
            {
                IsAuthorisedToOperate = false;
                return;
            }

            IsAuthorisedToOperate = _backingstoreItemsSource.Any();
        }

        public void MakeAuthorisedToOperateIfItemsSourceIsMoreThanOne()
        {
            if (_backingstoreItemsSource == null)
            {
                IsAuthorisedToOperate = false;
                return;
            }

            IsAuthorisedToOperate = _backingstoreItemsSource.Length > 1;
        }

		#endregion

        #region Modify contents of ItemsSource

        public async Task<bool> AddItemToItemsSourceAsync(T item)
		{
			if (item == null) return true;

			ExtinguishOnSelectionChangedCommand();

			var temp = ItemsSource.ToList();

			temp.Add(item);

			ItemsSource = temp.ToArray();

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> DeleteItemFromItemsSourceAsync(T item)
		{
			if (item == null) return true;

			ExtinguishOnSelectionChangedCommand();

			var temp = ItemsSource.ToList();

			temp.Remove(item);

			ItemsSource = temp.ToArray();

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> AddRangeToItemsSourceAsync(IEnumerable<T> items)
		{
			ExtinguishOnSelectionChangedCommand();

			if (ItemsSource == null)
				ItemsSource = Array.Empty<T>();

			var temp = ItemsSource.ToList();

			if (items != null) temp.AddRange(items.Where(z => z != null));

			ItemsSource = temp.ToArray();

			SelectedIndex = -1;

			LastKnownGoodSelectedIndex = -1;

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> RefillItemsSourceAsync(IEnumerable<T> items)
		{
			ExtinguishOnSelectionChangedCommand();

			T[] temp = Array.Empty<T>();

			if (items != null)
				temp = items.Where(z => z != null).ToArray();

			ItemsSource = temp;

			SelectedIndex = -1;

			LastKnownGoodSelectedIndex = -1;

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

        public void SaveSelectedIndexAsLastKnownGood()
		{
			LastKnownGoodSelectedIndex = SelectedIndex;
		}

		public bool SelectedIndexIsTheSameAsLastKnownGood()
		{
			// N.B. for better or worse, i am choosing to interpret -1 as a null value, which means that equality is impossible to determine. default to false

			if (SelectedIndex == -1 || LastKnownGoodSelectedIndex == -1)
				return false;

			return SelectedIndex == LastKnownGoodSelectedIndex;
		}

		public async Task<bool> RestoreSelectedIndexToLastKnownGoodAsync()
		{
			ExtinguishOnSelectionChangedCommand();

			var theReplacementIndex = JghGuardAgainstInvalidItemsSourceSelectedIndex.GetSafeIndex(LastKnownGoodSelectedIndex, ItemsSource);
			
			SelectedIndex = theReplacementIndex; // beware. target user control raises SelectionChanged event. recursive

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> ChangeSelectedIndexAsync(int candidateIndex)
		{
			if (ItemsSource == null)
				return true;

			ExtinguishOnSelectionChangedCommand();

			var theReplacementIndex = JghGuardAgainstInvalidItemsSourceSelectedIndex.GetSafeIndex(candidateIndex,
				ItemsSource);

			SelectedIndex = theReplacementIndex; // beware. target user control raises SelectionChanged event. recursive

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		public async Task<bool> ChangeSelectedIndexToNullAsync()
		{
			if (ItemsSource == null)
				return true;

			ExtinguishOnSelectionChangedCommand();

			SelectedIndex = -1; // beware. target user control raises SelectionChanged event. recursive

			await Task.Delay(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

			RestoreInstantiatedOnSelectionChangedCommand();

			return true;
		}

		#endregion

		#region Item operations

		public T FindCurrentItem()
		{
			var answer = JghArrayHelpers.SelectItemFromArrayByArrayIndex(ItemsSource,
				SelectedIndex);

			return ItemsSource == null
				? null
				: answer;
		}

		#endregion

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

        #region helpers

        public async Task<bool> ZeroiseAsync()
		{

            Label = string.Empty;
            IsVisible = false;
            IsDropDownOpen = false;

            ExtinguishOnSelectionChangedCommand();

			ItemsSource = Array.Empty<T>();

			SelectedIndex = -1;

			LastKnownGoodSelectedIndex = -1;

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
			Debug.Write("DummyCommandCanExecuteFuncThatIsAlwaysFalse");
			return false;
		}

		#endregion
	}
}