using System;
using System.Collections.Generic;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

namespace Rezultz.Library02.Mar2024.PageViewModelBases
{
    public abstract class BaseViewViewModel : BindableBase
    {

        #region Gui stuff - heap clever - the raison d'etre of this base class

        public void DeadenGui()
        {

            ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.CaptureThenFreeze);

            EvaluateVisibilityOfAllGuiControlsThatTouchData(false);
        }

        public void EnlivenGui()
        {
            ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);

            EvaluateGui();
        }

        public void RestoreGui()
        {
            ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui.Restore);
        }

        public void EvaluateGui()
        {
            EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

            EvaluateVisibilityOfAllGuiControlsThatTouchData(true);
        }

        protected void AllGuiControlsThatTouchDataAreAuthorisedToOperate(bool makeAuthorised)
        {
            foreach (var thing in MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate())
                if (thing is IHasIsAuthorisedToOperate xx)
                    xx.IsAuthorisedToOperate = makeAuthorised;
        }

        private void CaptureIsAuthorisedToOperateValue()
        {
            foreach (var thing in MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate())
                if (thing is IHasIsAuthorisedToOperate xx)
                    xx.CaptureIsAuthorisedToOperateValue();
        }

        private void RestoreCapturedIsAuthorisedToOperateValue()
        {
            foreach (var thing in MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate())
                if (thing is IHasIsAuthorisedToOperate xx)
                    xx.RestoreCapturedIsAuthorisedToOperateValue();
        }

        public void ToggleIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData(EnumsForGui freezeOrRestore)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (freezeOrRestore)
            {
                case EnumsForGui.CaptureThenFreeze:

                    CaptureIsAuthorisedToOperateValue();

                    AllGuiControlsThatTouchDataAreAuthorisedToOperate(false);

                    break;
                case EnumsForGui.Restore:

                    RestoreCapturedIsAuthorisedToOperateValue();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(freezeOrRestore), freezeOrRestore, null);
            }
        }

        protected static void AddToCollectionIfIHasIsAuthorisedToOperate(List<object> theList, object theItem)
        {
            if (theList == null)
                return;

            if (theItem == null)
                return;

            if (theList.Contains(theItem))
                return;

            if (theItem is IHasIsAuthorisedToOperate)
                theList.Add(theItem);
        }

        protected static void AddToCollectionIfIHasIsVisible(List<object> theList, object theItem)
        {
            if (theList == null)
                return;

            if (theItem == null)
                return;

            if (theList.Contains(theItem))
                return;

            if (theItem is IHasIsVisible xx)
                theList.Add(xx);
        }


        public abstract void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

        protected abstract void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible);

        protected abstract List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate();

        #endregion

        #region GenesisAsLastKnownGood

        public abstract bool LastKnownGoodGenesisOfThisViewModelHasChanged();

        public bool LastKnownGoodGenesisOfThisViewModelHasNotChanged()
        {
            return !LastKnownGoodGenesisOfThisViewModelHasChanged();
        }

        #endregion
    }
}