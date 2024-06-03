using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

namespace NetStd.ViewModels01.April2022.CollectionBases
{
    public class ItemDrivenCollectionViewModelForRadioButtonsBase : BindableBase, IHasIsAuthorisedToOperate
    
    {
        #region ctor

        public ItemDrivenCollectionViewModelForRadioButtonsBase(string label, Action onSelectionChangedExecuteAction,
            Func<bool> onSelectionChangedCanExecuteFunc)
        {
            Label = label ?? string.Empty;

            IsAuthorisedToOperate = false;

            LastKnownGoodSelectedItem = string.Empty;

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

        #region fields

        internal string LastKnownGoodSelectedItem;

        /* on a Dell xps15 a delay of 200 ms has done the job over the years but i have determined empirically
             * that this is too too short for even fast new phones. so reluctantly i am doubling it to 400
             */
        internal int SafeDelayMilliSec = 200;

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

        #region ItemsSource

        public ObservableCollection<string> ItemsSource { get; } = [];

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

        private string _backingstoreSelectedItem;

        [DataMember]
        public string SelectedItem
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

        private List<string> _backingstoreSelectedItems;

        [DataMember]
        public List<string> SelectedItems
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

        public string GetItemByItemIndex(int index)
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
                LastKnownGoodSelectedItem = string.Empty;

                return;
            }

            LastKnownGoodSelectedItem = SelectedItem;
        }

        public bool SelectedItemIsTheSameAsLastKnownGood()
        {
            if (SelectedItem == null)
            {
                return LastKnownGoodSelectedItem == string.Empty;
            }

            return SelectedItem == LastKnownGoodSelectedItem;
        }

        public async Task<bool> RestoreSelectedItemToLastKnownGoodAsync()
        {
            ExtinguishOnSelectionChangedCommand();

            var lastKnownGood = LastKnownGoodSelectedItem;

            SelectedItem = ItemsSource.Contains(lastKnownGood) ? lastKnownGood : null;

            await Task.Delay(SafeDelayMilliSec);

            RestoreInstantiatedOnSelectionChangedCommand();

            return true;
        }

        public async Task<bool> ChangeSelectedItemAsync(string candidateItem)
        {
            ExtinguishOnSelectionChangedCommand();

            if (candidateItem != null && ItemsSource.Contains(candidateItem)) // beware how equality comparer might or might not work for the particular type
            {
                SelectedItem = candidateItem; // beware. target user control raises SelectionChanged event. recursive

                await Task.Delay(SafeDelayMilliSec);
            }

            RestoreInstantiatedOnSelectionChangedCommand();

            return true;
        }

        //public async Task<bool> ChangeSelectedItemToNullAsync()
        //{
        //    ExtinguishOnSelectionChangedCommand();

        //    SelectedItem = null;

        //    await Task.Delay(SafeDelayMilliSec);

        //    RestoreInstantiatedOnSelectionChangedCommand();

        //    return true;
        //}
    
        #endregion

        #endregion

        #region helpers

        public async Task<bool> ZeroiseAsync()
        {
            Label = string.Empty;
            IsVisible = false;

            ExtinguishOnSelectionChangedCommand();

            ItemsSource.Clear();

            SelectedItem = null;

            LastKnownGoodSelectedItem = string.Empty;

            IsAuthorisedToOperate = false;

            RestoreInstantiatedOnSelectionChangedCommand();

            return await Task.FromResult(true);
        }

        protected void ExtinguishOnSelectionChangedCommand()
        {
            OnSelectionChangedCommand = _delegateCommandThatDoesNothing;
        }

        protected void RestoreInstantiatedOnSelectionChangedCommand()
        {
            OnSelectionChangedCommand = _asInstantiatedDelegateCommand;
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