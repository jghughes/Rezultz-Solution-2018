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


        public static bool DescendentPropertyIsNullOrValueIsSystemDefault(string propertyPathInXamlBindingSyntax, object parentObject)
        {
            var failure = "Failure whilst trying to determine value of a property on an object.";
            const string locus = "[DescendentPropertyIsNullOrValueIsSystemDefault]";

            try
            {
                var descendentObject = GetDescendentProperty(propertyPathInXamlBindingSyntax, parentObject);

                if (descendentObject == null)
                    return true;

                if (descendentObject is string)
                    return string.IsNullOrWhiteSpace(descendentObject as string);

                if (descendentObject is int)
                    return descendentObject as int? == 0;

                if (descendentObject is double)
                    return descendentObject as double? < double.Epsilon;

                if (descendentObject is bool)
                    return descendentObject as bool? == false;

                if (descendentObject is DateTime)
                    return descendentObject as DateTime? == DateTime.MinValue;

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

        // ReSharper disable once UnusedMember.Local
        //private static bool ValueOfDescendentPropertyAsDateTimeIsDefault(string propertyPathInXamlBindingSyntax, object parentObject)
        //{
        //    var descendentObject = GetDescendentProperty(propertyPathInXamlBindingSyntax, parentObject);

        //    return descendentObject as DateTime? == DateTime.MinValue;
        //}

        // ReSharper disable once UnusedMember.Local
        //private static bool ValueOfDescendentPropertyAsDoubleIsZero(string propertyPathInXamlBindingSyntax, object parentObject)
        //{
        //    var descendentObject = GetDescendentProperty(propertyPathInXamlBindingSyntax, parentObject);

        //    return descendentObject as double? < double.Epsilon;
        //}

        // ReSharper disable once UnusedMember.Local
        //private static bool ValueOfDescendentPropertyAsInt32IsZero(string propertyPathInXamlBindingSyntax, object parentObject)
        //{
        //    var descendentObject = GetDescendentProperty(propertyPathInXamlBindingSyntax, parentObject);

        //    return descendentObject as int? == 0;
        //}

        // ReSharper disable once UnusedMember.Local
        //private static bool ValueOfDescendentPropertyAsStringIsNullOrWhitespace(string propertyPathInXamlBindingSyntax, object parentObject)
        //{
        //    var descendentObject = GetDescendentProperty(propertyPathInXamlBindingSyntax, parentObject);

        //    return string.IsNullOrWhiteSpace(descendentObject as string);
        //}

        public static object GetDescendentProperty(string propertyPathInXamlBindingSyntax, object objectBeingRefelected)
        {
            var failureMsg = "Unable to obtain a specified member of an object by means of reflection.";

            string locus = "[GetDescendentProperty]";

            try
            {
                #region null checks

                if (propertyPathInXamlBindingSyntax == null)
                    throw new JghInvalidValueException($"{nameof(propertyPathInXamlBindingSyntax)} is null.");

                if (objectBeingRefelected == null)
                    throw new JghInvalidValueException($"{nameof(objectBeingRefelected)} is null.");

                #endregion

                var hierarchyOfMemberNames = propertyPathInXamlBindingSyntax.Split('.');

                var objectAtCurrentLevel = objectBeingRefelected; // initialise

                foreach (var propertyNameAtThisLevel in hierarchyOfMemberNames)
                {
                    var propertyInfo = objectAtCurrentLevel.GetType().GetRuntimeProperty(propertyNameAtThisLevel);

                    #region throw meaningful exception if not found

                    if (propertyInfo == null)
                    {
                        throw new JghInvalidValueException($"Coding error. Missing or mismatched property path or fragment. Property path is <{propertyPathInXamlBindingSyntax}>. Missing path fragment is <{propertyNameAtThisLevel}>.");
                        //throw new JghInvalidValueException($"Path name being specified is <{propertyPathInXamlBindingSyntax}>.");
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