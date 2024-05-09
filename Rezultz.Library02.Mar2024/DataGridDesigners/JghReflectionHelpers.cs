using System;
using System.Reflection;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;

namespace Rezultz.Library02.Mar2024.DataGridDesigners
{
    public static class JghReflectionHelpers
    {
        private const string Locus2 = nameof(JghReflectionHelpers);
        private const string Locus3 = "[Rezultz.Library02.Mar2024]";


        public static bool DescendantPropertyIsNullOrValueIsSystemDefault(string propertyPathInXamlBindingSyntax, object parentObject)
        {
            var failure = "Failure whilst trying to determine value of a property on an object.";
            const string locus = "[DescendantPropertyIsNullOrValueIsSystemDefault]";

            try
            {
                var descendantObject = GetDescendantProperty(propertyPathInXamlBindingSyntax, parentObject);

                if (descendantObject == null)
                    return true;

                if (descendantObject is string)
                    return string.IsNullOrWhiteSpace(descendantObject as string);

                if (descendantObject is int)
                    return descendantObject as int? == 0;

                if (descendantObject is double)
                    return descendantObject as double? < double.Epsilon;

                if (descendantObject is bool)
                    return descendantObject as bool? == false;

                if (descendantObject is DateTime)
                    return descendantObject as DateTime? == DateTime.MinValue;

                return false;
            }

            #region trycatch

            catch (Exception ex)
            {
                failure = JghString.ConcatAsSentences(ex.GetType().ToString(), failure);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }


        public static object GetDescendantProperty(string propertyPathInXamlBindingSyntax, object objectBeingReflected)
        {
            var failureMsg = "Unable to obtain a specified property of an object by means of reflection.";
            string locus = "[GetDescendantProperty]";

            try
            {
                #region null checks

                if (propertyPathInXamlBindingSyntax == null)
                    throw new JghInvalidValueException($"{nameof(propertyPathInXamlBindingSyntax)} is null.");

                if (objectBeingReflected == null)
                    throw new JghInvalidValueException($"{nameof(objectBeingReflected)} is null.");

                #endregion

                var hierarchyOfMemberNames = propertyPathInXamlBindingSyntax.Split('.');

                var objectAtCurrentLevel = objectBeingReflected; // initialise

                foreach (var propertyNameAtThisLevel in hierarchyOfMemberNames)
                {
                    var propertyInfo = objectAtCurrentLevel.GetType().GetRuntimeProperty(propertyNameAtThisLevel);

                    #region throw meaningful exception if not found

                    if (propertyInfo == null)
                    {
                        throw new JghInvalidValueException($"Coding error. Missing or mismatched property path or fragment. Property path is <{propertyPathInXamlBindingSyntax}>. Missing path fragment is <{propertyNameAtThisLevel}>.");
                    }

                    #endregion

                    var childObject = propertyInfo.GetValue(objectAtCurrentLevel, null);

                    objectAtCurrentLevel = childObject;
                }

                return objectAtCurrentLevel;
            }

            #region trycatch

            catch (Exception ex)
            {
                failureMsg = JghString.ConcatAsSentences(ex.GetType().ToString(), failureMsg, ex.Message);

                throw JghExceptionHelpers.ConvertToCarrier(failureMsg, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

    }
}