using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Behaviours
{
	/// <summary>
	/// Highlights the color of the text in the entry to Red if the entry is wrong.
	/// Entry must be an integer or a decimal.
	/// </summary>
	public class ValidationBehaviourNumeric : ValidationBehavior
	{
		internal override void OnEntryTextChanged(object sender, TextChangedEventArgs args)
		{
			var isValid = decimal.TryParse(args.NewTextValue, out var _);

			((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
		}
	}
}