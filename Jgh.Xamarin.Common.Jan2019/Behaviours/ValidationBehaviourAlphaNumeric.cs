using NetStd.Goodies.Mar2022;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Behaviours
{
	/// <summary>
	/// Highlights the color of the text in the entry to Red if the entry is wrong.
	/// Entry must be an integer.
	/// </summary>
	public class ValidationBehaviourAlphaNumeric : ValidationBehavior
	{
		internal override void OnEntryTextChanged(object sender, TextChangedEventArgs args)
		{
			var isValid = JghString.IsOnlyLettersOrDigits(args.NewTextValue);

			((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
		}
	}
}