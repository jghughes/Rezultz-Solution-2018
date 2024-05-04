using System.Collections.Generic;
using System.Linq;

namespace NetStd.Goodies.Mar2022
{
    /// <summary>
    ///     Class for ensuring that the selected index to be applied
    ///     to the ItemsSource of an ItemsControl of is not out of range.
    ///     Overrides the suggested selected index if it is out of range.
    ///     return the suggested selected index if valid, or -1 if the suggested index is less
    ///     than -1 or exceeds the zero-based size of the ItemsSource.
    /// </summary>
    public static class JghGuardAgainstInvalidItemsSourceSelectedIndex
    {
        /// <summary>
        ///     Overrides the suggested selected index if it is out of range.
        ///     The suggested selected index or -1 if the suggested index is less
        ///     than -1 or exceeds the zero-based size of the ItemsSource.
        /// </summary>
        /// <param name="suggestedSelectedIndex">Index suggested by the user.</param>
        /// <param name="itemCount">The item count of the underlying ItemsSource.</param>
        /// <returns>
        ///     The suggested selected index or -1 if the suggested index is less than -1 or exceeds the zero-based size of
        ///     the ItemsSource.
        /// </returns>
        public static int GetSafeIndex(int suggestedSelectedIndex, int itemCount)
        {
            var argumentIsOutOfRange = (suggestedSelectedIndex < -1) | (suggestedSelectedIndex > itemCount - 1);

            return argumentIsOutOfRange ? -1 : suggestedSelectedIndex;
        }

        /// <summary>
        ///     Overrides the suggested selected index if it is out of range. The suggested
        ///     selected index or -1 if the suggested index is less than -1 or exceeds
        ///     the zero-based size of the ItemsSource.
        /// </summary>
        /// <typeparam name="T">The type of items in the ItemsSource.</typeparam>
        /// <param name="suggestedSelectedIndex">Index suggested by the user.</param>
        /// <param name="itemsSource">The itemsSource.</param>
        /// <returns>
        ///     The suggested selected index or -1 if the suggested index is less than -1 or exceeds the zero-based size of
        ///     the ItemsSource.
        /// </returns>
        public static int GetSafeIndex<T>(int suggestedSelectedIndex, IEnumerable<T> itemsSource)
        {
            var itemCount = itemsSource.Count();

            return GetSafeIndex(suggestedSelectedIndex, itemCount);
        }
    }
}