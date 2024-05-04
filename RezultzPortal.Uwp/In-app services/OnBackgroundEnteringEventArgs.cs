using System;

namespace RezultzPortal.Uwp.In_app_services
{
    public class OnBackgroundEnteringEventArgs : EventArgs
    {
        public OnBackgroundEnteringEventArgs(SuspensionState suspensionState, Type target)
        {
            SuspensionState = suspensionState;
            Target = target;
        }

        public SuspensionState SuspensionState { get; set; }

        public Type Target { get; }
    }
}
