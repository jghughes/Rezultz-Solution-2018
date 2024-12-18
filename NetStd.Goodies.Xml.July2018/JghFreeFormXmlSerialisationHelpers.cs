﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;

namespace NetStd.Goodies.Xml.July2018
{
    public static class JghFreeFormXmlSerialisationHelpers
    {
        private const string Locus2 = nameof(JghFreeFormXmlSerialisationHelpers);
        private const string Locus3 = "[NetStd.Goodies.Xml.July2018]";

        public const string XeFirstName = "first";
        public const string XeMiddleInitial = "middle";
        public const string XeLastName = "last";
        public const string XeFullName = "fullname";

        public static string ConvertNamedThingsIntoSimpleIdentitiesAsFreeFormXml<T>(T[] itemsIdentifiedByTheirNames)
            where T : class, IHasFirstName, IHasLastName, IHasMiddleInitial, IHasFullName
        {
            const string failure = "Unable to convert list of named things into a concise list of their identities.";
            const string locus = "[ConvertNamedThingsIntoSimpleIdentitiesAsFreeFormXml]";

            try
            {
                if (itemsIdentifiedByTheirNames is null)
                    throw new ArgumentNullException(nameof(itemsIdentifiedByTheirNames));

                var childElements = new List<XElement>();

                foreach (var person in itemsIdentifiedByTheirNames)
                {
                    var xE = new XElement(
                        "Thing",
                        //new XElement(XmlNames.Bib, JghString.TmLr(person.Bib)), // don't use bib. people can change their bib during the course of a season
                        new XElement(XeFirstName, JghString.TmLr(person.FirstName)),
                        new XElement(XeLastName, JghString.TmLr(person.LastName)),
                        new XElement(XeMiddleInitial, JghString.TmLr(person.MiddleInitial)),
                        new XElement(XeFullName, JghString.TmLr(person.FullName))
                    );

                    childElements.Add(xE);
                }

                var rootElement = new XElement(
                    "root",
                    new XAttribute("generated", DateTime.UtcNow));

                rootElement.Add(childElements);

                var answer = rootElement.ToString(SaveOptions.DisableFormatting);

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public static T[] ConvertCollectionOfSimpleIdentitiesAsFreeFormXmlBackIntoNamedThings<T>(string parentXElementAsString)
            where T : class, IHasFirstName, IHasLastName, IHasMiddleInitial, IHasFullName, new()
        {
            const string failure = "Unable to convert string data into a recognisable list of people type things.";
            const string locus = "[ConvertCollectionOfSimpleIdentitiesAsFreeFormXmlBackIntoNamedThings]";

            try
            {
                if (parentXElementAsString is null)
                    return [];

                XElement parentXElement;

                using (var sr = new StringReader(parentXElementAsString))
                {
                    parentXElement =
                        XElement.Load(sr,
                            LoadOptions.PreserveWhitespace); // preserve merely for ease of inspection during debugging
                }

                var individuals = new List<T>();

                foreach (var child in parentXElement.Elements())
                {
                    var person = new T
                    {
                        //Bib = JghString.TmLr(JghXElementHelpers.AsTrimmedString(child.Element(XmlNames.Bib))), // don't use bib. people can change their bib during the course of a season
                        FirstName = JghString.TmLr(
                            JghXElementHelpers.AsTrimmedString(child.Element(XeFirstName))),
                        LastName = JghString.TmLr(
                            JghXElementHelpers.AsTrimmedString(child.Element(XeLastName))),
                        MiddleInitial =
                            JghString.TmLr(
                                JghXElementHelpers.AsTrimmedString(
                                    child.Element(XeMiddleInitial))),
                    };

                    individuals.Add(person);
                }

                var answer = individuals.ToArray();

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }
    }
}