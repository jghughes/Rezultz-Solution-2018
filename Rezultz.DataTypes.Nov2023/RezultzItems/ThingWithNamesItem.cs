using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;

namespace Rezultz.DataTypes.Nov2023.RezultzItems
{
    // used for storage of list of favorites in settings

    [Serializable] // - because we use this to store Favorites
    public class ThingWithNamesItem : IHasFirstName, IHasMiddleInitial, IHasLastName
    {

        #region props

        [DataMember] public string FirstName { get; set; } = string.Empty;

        [DataMember] public string MiddleInitial { get; set; } = string.Empty;

        [DataMember] public string LastName { get; set; } = string.Empty;

        #endregion

        #region methods

        public static ThingWithNamesItem FromThing<T>(T thingWithNames) where T : class, IHasFirstName, IHasMiddleInitial, IHasLastName
        {
            var answer = new ThingWithNamesItem()
            {
                FirstName = thingWithNames.FirstName,
                MiddleInitial = thingWithNames.MiddleInitial,
                LastName = thingWithNames.LastName,
            };

            return answer;
        }

        public static T ToThing<T>(ThingWithNamesItem thingWithNames) where T : class, IHasFirstName, IHasMiddleInitial, IHasLastName, new()
        {
            var answer = new T()
            {
                FirstName = thingWithNames.FirstName,
                MiddleInitial = thingWithNames.MiddleInitial,
                LastName = thingWithNames.LastName,
            };

            return answer;
        }

        public static TU[] SearchByName<T, TU>(T[] searchList, TU[] populationToBeSearched)
            where T : class, IHasFirstName, IHasMiddleInitial, IHasLastName, new()
            where TU : class, IHasFirstName, IHasMiddleInitial, IHasLastName, new()
        {
            const string failure =
                "Unable to populate array of simple identities of itemsIdentifiedByTheirNames with full details of their newest results.";
            const string locus = "[SearchByName]";

            if (searchList is null) return [];

            if (populationToBeSearched is null || !populationToBeSearched.Any()) return [];

            // arrayOfIdentities are the Favorites for example

            var searchResults = new List<TU>();

            try
            {
                foreach (var identity in searchList)
                {
                    if (identity is null) continue;

                    //step 1. get ready. default assumption is a blank person/item - populated with names alone

                    var assumption = new TU
                    {
                        FirstName = identity.FirstName,
                        MiddleInitial = identity.MiddleInitial,
                        LastName = identity.LastName,
                    };

                    // step 2. supersede the default assumption if a match is found in the populationToBeSearched

                    foreach (var person in populationToBeSearched)
                    {
                        if (!ThingWithNamesItem.AreMatchingNamesInAllProbability(person, identity)) continue;

                        assumption = person;

                        break;
                    }

                    searchResults.Add(assumption);
                }

                var answer = searchResults
                    .OrderBy(xx => xx.FirstName ?? string.Empty)
                    .ThenBy(xx => xx.LastName ?? string.Empty);

                return answer.ToArray();
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static TU[] RemoveItemByName<T, TU>(T nameToBeRemoved, TU[] populationToBeReduced)
            where T : class, IHasFirstName, IHasMiddleInitial, IHasLastName
            where TU : class, IHasFirstName, IHasMiddleInitial, IHasLastName
        {
            var listOfItemsAfterDeletion = (from item in populationToBeReduced
                where item is not null
                where !ThingWithNamesItem.AreMatchingNamesInAllProbability(nameToBeRemoved, item)
                select item).ToArray();

            return listOfItemsAfterDeletion;
        }

        public static bool AreMatchingNamesInAllProbability<T, TU>(T aa, TU bb)
            where T : class, IHasFirstName, IHasMiddleInitial, IHasLastName
            where TU : class, IHasFirstName, IHasMiddleInitial, IHasLastName
        {

            var answer = JghString.AreEqualIgnoreOrdinalCase(aa.LastName, bb.LastName) && JghString.AreEqualIgnoreOrdinalCase(aa.FirstName, bb.FirstName) && JghString.AreEqualIgnoreOrdinalCase(aa.MiddleInitial, bb.MiddleInitial);

            return answer;
        }

        //public static bool AreMatchingLastAndFirstNames<T, TU>(T aa, TU bb)
        //    where T : class, IHasFirstName, IHasLastName
        //    where TU : class, IHasFirstName, IHasLastName
        //{
        //    return JghString.AreEqualIgnoreOrdinalCase(aa.LastName, bb.LastName) && JghString.AreEqualIgnoreOrdinalCase(aa.FirstName, bb.FirstName);
        //}
    
        public bool IsValid()
        {
            if (!string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName))
                return true;

            return false;
        }

        public override string ToString()
        {
            return string.Concat(" ", FirstName, MiddleInitial, LastName);
        }

        #endregion

    }
}