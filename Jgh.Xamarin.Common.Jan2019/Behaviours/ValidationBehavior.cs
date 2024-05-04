using System.Linq;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Behaviours
{
    /// <summary>
    /// Base class for dealing with errors in a Xamarin Entry control by altering the
    /// appearance of the Entry if an error is detected upon a keystroke. Derived classes
    /// must provide an implementation for OnEntryTextChanged(object sender, TextChangedEventArgs args).
    /// </summary>
    public class ValidationBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(ValidationBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static bool GetAttachBehavior(BindableObject view)
        {
            return (bool)view.GetValue(AttachBehaviorProperty);
        }

        public static void SetAttachBehavior(BindableObject view, bool value)
        {
            view.SetValue(AttachBehaviorProperty, value);
        }

        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            if (view is not Entry entry)
            {
                return;
            }

            var attachBehavior = (bool)newValue;

            if (attachBehavior)
            {
                entry.Behaviors.Add(new ValidationBehavior());
            }
            else
            {
                var toRemove = entry.Behaviors.FirstOrDefault(b => b is ValidationBehavior);
                if (toRemove != null)
                {
                    entry.Behaviors.Remove(toRemove);
                }
            }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        internal virtual void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            // do nothing. this method is overwritten in derived classes
        }

    }
}
