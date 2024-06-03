using System;
using System.Collections.Generic;
using System.Linq;

namespace NetStd.Goodies.Mar2022
{
    public static class JghCollectionHelpers
    {
        /// <summary>
        ///     Determines if two collections contain an identical set of items on the basis of their default equality
        ///     comparer.
        /// </summary>
        /// <typeparam name="T">Nullable reference type.</typeparam>
        /// <param name="collectionA">First collection.</param>
        /// <param name="collectionB">Second collection.</param>
        /// <returns>False if either collection is null or any item in either collection isn't contained in the other.</returns>
        public static bool AreTheSame<T>(IEnumerable<T> collectionA, IEnumerable<T> collectionB)
        {
            if (collectionA == null) return false;
            if (collectionB == null) return false;

            var copyA = collectionA.ToArray();
            var copyB = collectionB.ToArray();

            if (copyA.Any(item => !copyB.Contains(item)))
                return false;

            if (copyB.Any(item => !copyA.Contains(item)))
                return false;

            return true;
        }

        /// <summary>Determines if two collections contain an identical set of items on the basis of a specified equality comparer.</summary>
        /// <typeparam name="T">Nullable reference type.</typeparam>
        /// <param name="collectionA">First collection.</param>
        /// <param name="collectionB">Second collection.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>False if either collection is null or any item in either collection isn't contained in the other.</returns>
        public static bool IsCopyOf<T>(IEnumerable<T> collectionA, IEnumerable<T> collectionB, IEqualityComparer<T> comparer)
        {
            if (collectionA == null) return false;
            if (collectionB == null) return false;

            var copyA = collectionA.ToArray();
            var copyB = collectionB.ToArray();

            if (copyA.Any(item => !copyB.Contains(item, comparer)))
                return false;

            if (copyB.Any(item => !copyA.Contains(item, comparer)))
                return false;

            return true;
        }

        public static T[] TakeEnd<T>(int amountToTake, IEnumerable<T> items)
        {

	        // Note: this here is the Linq way of doing it.
	        // but if the list gets very long it will get slow.
	        // time = O(n)x2. so only use Linq for short lists
	        //var answer = longList.Reverse().Take(DesiredLengthOfShortListOfLastItems).Reverse().ToList();


	        if (items == null)
	        {
		        return [];
	        }

	        var longList = items.Where(z => z != null).ToArray();

	        if (!longList.Any())
	        {
		        return [];
	        }

	        var i = longList.Length - 1;

	        var j = 0;

	        var shortQueue = new Queue<T>();

	        while (i > -1 && j < amountToTake)
	        {
		        shortQueue.Enqueue(longList[i]);

		        i -= 1;
		        j += 1;
	        }

	        return shortQueue.ToArray(); // 0(n) i.e. time = amount to take(n)
        }

    }
}