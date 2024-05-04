﻿using NetStd.Goodies.Mar2022;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Behaviours
{
	/// <summary>
	/// Highlights the color of the text in the entry to Red if the entry is wrong.
	/// Entry must be an integer.
	/// </summary>
	public class ValidationBehaviourAlphaNumericMin3 : ValidationBehavior
	{
		internal override void OnEntryTextChanged(object sender, TextChangedEventArgs args)
		{
			var value = args.NewTextValue;

			var isValid = JghString.IsOnlyLettersOrDigits(value) && !string.IsNullOrWhiteSpace(value) && value.Length >= 3;

			((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
		}
	}
}