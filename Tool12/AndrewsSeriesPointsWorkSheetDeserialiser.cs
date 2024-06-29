using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using System.Xml.Linq;

namespace Tool12
{
    public static class AndrewsSeriesPointsWorkSheetDeserialiser
    {
        #region parameters

        public const bool MustProcessXmlUsingSystemXmlSerializerNotJghDeserialiser = false;

        #endregion

        #region mappings of source element Names in Access/Excel worksheets to new dictionary keys for lookups

        private static Dictionary<string, string> SourceElementNameNewKeyPairsForSystemSerializer => new()
    {
        {
            "Participant", ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Expert", ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Intermediate", ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Novice", ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.
        {
            "Sport", ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant
        }, // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything. get this wrong and XML deserialisation will blow up.

        { "POS", ParticipantOnAndrewsPointsSpreadsheetDto.XePos },
        { "Name", ParticipantOnAndrewsPointsSpreadsheetDto.XeFullName },
        { "Product", ParticipantOnAndrewsPointsSpreadsheetDto.XeProduct },
        { "PLATE", ParticipantOnAndrewsPointsSpreadsheetDto.XePlate },
        { "BIBTAG", ParticipantOnAndrewsPointsSpreadsheetDto.XeBibTag },
        { "Date_x0020_of_x0020_Birth", ParticipantOnAndrewsPointsSpreadsheetDto.XeDateOfBirth },
        { "Age", ParticipantOnAndrewsPointsSpreadsheetDto.XeAge },
        { "Sex", ParticipantOnAndrewsPointsSpreadsheetDto.XeSex },
        { "Category", ParticipantOnAndrewsPointsSpreadsheetDto.XeCategory },
        { "Top_x0020_9_x0020_Points", ParticipantOnAndrewsPointsSpreadsheetDto.XePointsTopN },
        { "Total_x0020_Points", ParticipantOnAndrewsPointsSpreadsheetDto.XePointsOverall },
        { "R1", ParticipantOnAndrewsPointsSpreadsheetDto.XeR1 },
        { "R2", ParticipantOnAndrewsPointsSpreadsheetDto.XeR2 },
        { "R3", ParticipantOnAndrewsPointsSpreadsheetDto.XeR3 },
        { "R4", ParticipantOnAndrewsPointsSpreadsheetDto.XeR4 },
        { "R5", ParticipantOnAndrewsPointsSpreadsheetDto.XeR5 },
        { "R6", ParticipantOnAndrewsPointsSpreadsheetDto.XeR6 },
        { "R7", ParticipantOnAndrewsPointsSpreadsheetDto.XeR7 },
        { "R8", ParticipantOnAndrewsPointsSpreadsheetDto.XeR8 },
        { "R9", ParticipantOnAndrewsPointsSpreadsheetDto.XeR9 },
        { "R10", ParticipantOnAndrewsPointsSpreadsheetDto.XeR10 },
        { "R11", ParticipantOnAndrewsPointsSpreadsheetDto.XeR11 },
        { "R12", ParticipantOnAndrewsPointsSpreadsheetDto.XeR12 }
    }; // note: the keys are the source element names from the column headings in the worksheet exported from Access. The values are the new keys to be used in the DTO

        private static KeyValuePair<string, string>[] NewKeySourceElementNamePairsForJghFreeFormDeserialiser =>
        [
            new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant,
            "Participant"), // candidate root element of repeating elements. stems from the name of the corresponding worksheet exported from Access, which could be anything.
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant, "Expert"), // ditto
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant, "Intermediate"), // ditto
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant, "Novice"), // ditto
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeParticipant, "Sport"), // ditto
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XePos, "POS"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeFullName, "Name"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeFullName,
            "Visitor Name"), // note the duplicate XeFullName! a nice example of accommodating different worksheets from Andrew with different/inconsistent column headings
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeFullName, "Visitor_x0020_Name"), //ditto
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeProduct, "Product"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XePlate, "PLATE"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XePlate, "BIB"), // note the duplicate XePlate! a nice example of accommodating different worksheets from Andrew with different/inconsistent column headings
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeBibTag, "BIBTAG"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeDateOfBirth, "Date of Birth"), // Excel export version (csv heading). Caution: presence of the blanks will detonate system xml serializer but not JghMapper
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeDateOfBirth, "Date_x0020_of_x0020_Birth"), // Access export version (XElement name)
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeAge, "Age"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeSex, "Sex"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeCategory, "Category"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsTopN, "Top 9 Points"), // Excel export version (csv heading). Caution: presence of the blanks will detonate system xml serializer but not JghMapper
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsTopN, "Top_x0020_9_x0020_Points"), // Access export version (XElement name)
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsOverall, "Total Points"), // Excel export version (csv heading). Caution: presence of the blank will detonate system xml serializer but not JghMapper
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsOverall, "Total_x0020_Points"), // Access export version (XElement name)
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1, "R1"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR2, "R2"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR3, "R3"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR4, "R4"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR5, "R5"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR6, "R6"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR7, "R7"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR8, "R8"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR9, "R9"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR10, "R10"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR11, "R11"),
        new KeyValuePair<string, string>(ParticipantOnAndrewsPointsSpreadsheetDto.XeR12, "R12")
        ]; // note: the keys are the new keys to be used in the DTO. The source element names are the superset of the column headings in the worksheet exported from Access AND/OR Excel.

        private static Dictionary<string, string> SymbolAndEnumMap => new()
    {
        { "M", "m" },
        { "F", "f" },
        { "Expert", "expert" },
        { "Intermediate", "intermediate" },
        { "Novice", "novice" },
        { "Sport", "sport" }
    };

        #endregion

        #region variables

        private static readonly JghConsoleHelperV2 Console = new();

        #endregion

        #region methods

        public static async Task<List<ParticipantOnAndrewsPointsSpreadsheet>> ProcessXmlFile(string fullInputPath, FileItem fileItem)
        {
            try
            {
                var rawInputAsText = await File.ReadAllTextAsync(fullInputPath);

                fileItem.FileContentsAsText = rawInputAsText;

                try
                {
                    var rawInputAsXElement = XElement.Parse(rawInputAsText);

                    fileItem.FileContentsAsXElement = rawInputAsXElement;
                }
                catch (Exception ex)
                {
                    var msg = $"{fileItem.FileInfo.Name} failed to parse successfully. A valid XML document is expected. Is this a XML document? ({ex.Message})";

                    Console.WriteLine(msg);

                    throw new Exception(ex.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.InnerException?.Message);
            }

            var arrayOfRepeatingXe = fileItem.FileContentsAsXElement.Elements().ToArray();

            if (arrayOfRepeatingXe.Length == 0)
                throw new Exception($"Found not even a single repeating child XElement in file <{fileItem.FileInfo.Name}>.");

            List<ParticipantOnAndrewsPointsSpreadsheet> seriesParticipants = [];

            JghFreeFormDeserialiser deserialiser = new (NewKeySourceElementNamePairsForJghFreeFormDeserialiser);

            foreach (var repeatXe in arrayOfRepeatingXe)
            {
                ParticipantOnAndrewsPointsSpreadsheet item;

                if (MustProcessXmlUsingSystemXmlSerializerNotJghDeserialiser)
                {
                    item = DeserialiseParticipantUsingSystemXmlSerializer(repeatXe); // try out system deserialiser - not recommended, brittle in unexpected circumstances
                }
                else
                {
                    item = DeserialiseParticipantUsingJghFreeFormDeserialiser(repeatXe, deserialiser); // use free-form deserialiser - strongly recommended, more versatile and robust, but very slow unfortunately (not sure why)
                    //item = DeserialiseParticipantUsingJghFreeFormDeserialiser(repeatXe); // use free-form deserialiser - strongly recommended, more versatile and robust, but very slow unfortunately (not sure why)
                }
                seriesParticipants.Add(item);
            }

            return seriesParticipants;
        }

        public static async Task<List<ParticipantOnAndrewsPointsSpreadsheet>> ProcessCsvFile(string fullInputPath, FileItem fileItem)
        {
            var rawInputAsRows = await File.ReadAllLinesAsync(fullInputPath);

            fileItem.FileContentsAsText = string.Join(Environment.NewLine, rawInputAsRows);
            fileItem.FileContentsAsXElement = new XElement("dummy");

            var csvHeaderRow = rawInputAsRows[0];

            List<string> rowsOfRepeatingCsvData = [];

            for (var i = 1; i < rawInputAsRows.Length; i++) rowsOfRepeatingCsvData.Add(rawInputAsRows[i]);

            List<ParticipantOnAndrewsPointsSpreadsheet> seriesParticipants = [];

            JghFreeFormDeserialiser deserialiser = new(csvHeaderRow, NewKeySourceElementNamePairsForJghFreeFormDeserialiser);

            foreach (var row in rowsOfRepeatingCsvData)
            {
                var item = DeserialiseParticipantUsingJghFreeFormDeserialiser(row, deserialiser);

                seriesParticipants.Add(item);
            }

            return seriesParticipants;
        }

        public static string FigureOutRaceGroup(ParticipantHubItemDto participantItem, DateTime dateOfEvent)
        {
            var isTransitionalParticipant = participantItem.RaceGroupBeforeTransition != participantItem.RaceGroupAfterTransition;

            string answerAsRaceGroup;

            if (isTransitionalParticipant)
            {
                if (DateTime.TryParse(participantItem.DateOfRaceGroupTransitionAsString, out var dateOfTransition))
                {
                    var eventIsBeforeRaceGroupTransition = dateOfEvent < dateOfTransition;

                    answerAsRaceGroup = eventIsBeforeRaceGroupTransition ? participantItem.RaceGroupBeforeTransition : participantItem.RaceGroupAfterTransition;

                    return answerAsRaceGroup;
                }

                answerAsRaceGroup = participantItem.RaceGroupBeforeTransition;
            }
            else
            {
                answerAsRaceGroup = participantItem.RaceGroupBeforeTransition;
            }

            return answerAsRaceGroup;
        }


        #endregion

        #region helpers


        private static ParticipantOnAndrewsPointsSpreadsheet DeserialiseParticipantUsingSystemXmlSerializer(XElement repeatXe)
        {
            #region prettify all specified symbol/enum values by cunning means of editing the element as plain text

            var repeatXeAsText = PrettifySymbolsAndEnums(repeatXe.ToString(), SymbolAndEnumMap);

            try
            {
                ParsePlainTextIntoXml(repeatXeAsText); // parse plain text document back into xml to verify the (potentially error-prone) renaming process
            }
            catch (Exception)
            {
                throw new Exception("Error. In the process of renaming XElement values, the format of the data was corrupted. One or more elements no longer parse back into Xml. This is a program error. Please contact the administrator.");
            }

            #endregion

            #region rename/map all specified XElement names in source by cunning means of editing the document as plain text

            repeatXeAsText = ReformatXElementNames(repeatXeAsText, SourceElementNameNewKeyPairsForSystemSerializer);

            try
            {
                // parse plain text document back into xml to verify the (potentially error-prone) renaming process

                ParsePlainTextIntoXml(repeatXeAsText); // blow up?
            }
            catch (Exception)
            {
                throw new Exception("Error. In the process of reformatting XElement values, the format of the data was corrupted. One or more elements no longer parse back into Xml. This is a program error. Please contact the administrator.");
            }

            #endregion

            #region deserialise

            var cleanRepeatXe = ParsePlainTextIntoXml(repeatXeAsText);

            var dto = JghSerialisation.ToObjectFromXml<ParticipantOnAndrewsPointsSpreadsheetDto>(cleanRepeatXe, [typeof(ParticipantOnAndrewsPointsSpreadsheetDto)]);

            var item = ParticipantOnAndrewsPointsSpreadsheet.FromDataTransferObject(dto);

            #endregion

            return item;
        }

        private static ParticipantOnAndrewsPointsSpreadsheet DeserialiseParticipantUsingJghFreeFormDeserialiser(XElement repeatXe, JghFreeFormDeserialiser deserializer)
        {
            #region prettify all specified symbol/enum values by cunning means of editing the element as plain text

            var repeatXeAsText = PrettifySymbolsAndEnums(repeatXe.ToString(), SymbolAndEnumMap);

            XElement cleanRepeatXe;

            try
            {
                cleanRepeatXe = ParsePlainTextIntoXml(repeatXeAsText); // parse plain text document back into xml to verify the (potentially error-prone) renaming process
            }
            catch (Exception)
            {
                throw new Exception("Error. In the process of reformatting XElement values, the format of the data was corrupted. One or more elements no longer parse back into Xml. This is a program error. Please contact the administrator.");
            }

            #endregion

            #region deserialise

            var dummy = deserializer.Deserialise(cleanRepeatXe);

            ParticipantOnAndrewsPointsSpreadsheetDto spreadsheetDto = new()
            {
                Position = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePos),
                FullName = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeFullName),
                Plate = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePlate),
                BibTag = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeBibTag),
                DateOfBirthAsString = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeDateOfBirth),
                Age = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeAge),
                Sex = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeSex),
                Category = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeCategory),
                PointsTopNine = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsTopN),
                PointsOverall = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsOverall),
                R1 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R2 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R3 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R4 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R5 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R6 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R7 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R8 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R9 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R10 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R11 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R12 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                Comment = string.Empty
            };

            var item = ParticipantOnAndrewsPointsSpreadsheet.FromDataTransferObject(spreadsheetDto);

            #endregion

            return item;
        }

        private static ParticipantOnAndrewsPointsSpreadsheet DeserialiseParticipantUsingJghFreeFormDeserialiser(string rowOfCsv, JghFreeFormDeserialiser deserializer)
        {
            #region prettify all specified symbol/enum values by cunning means of editing the element as plain text

            var cleanRowOfCsv = PrettifySymbolsAndEnums(rowOfCsv, SymbolAndEnumMap);

            #endregion

            #region deserialise

            var dummy = deserializer.Deserialise(cleanRowOfCsv);

            ParticipantOnAndrewsPointsSpreadsheetDto spreadsheetDto = new()
            {
                Position = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePos),
                FullName = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeFullName),
                Plate = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePlate),
                BibTag = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeBibTag),
                DateOfBirthAsString = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeDateOfBirth),
                Age = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeAge),
                Sex = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeSex),
                Category = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeCategory),
                PointsTopNine = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsTopN),
                PointsOverall = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XePointsOverall),
                R1 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R2 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R3 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R4 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R5 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R6 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R7 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R8 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R9 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R10 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R11 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                R12 = deserializer.GetFirstOrBlankAsString(ParticipantOnAndrewsPointsSpreadsheetDto.XeR1),
                Comment = string.Empty
            };

            var item = ParticipantOnAndrewsPointsSpreadsheet.FromDataTransferObject(spreadsheetDto);

            #endregion

            return item;
        }

        private static string ReformatXElementNames(string xElementAsPlainText, Dictionary<string, string> mappingDictionary)
        {
            var failure = "Unable to map XElement names from source data in accordance with mapping dictionary.";
            const string locus = "[MapXElementNames]";


            var provisionalCulpritOldXeName = "";
            var provisionalCulpritNewXeName = "";

            var scratchPadText = xElementAsPlainText;

            try
            {
                if (string.IsNullOrWhiteSpace(xElementAsPlainText))
                    throw new ArgumentNullException(nameof(xElementAsPlainText));

                foreach (var entry in mappingDictionary.Where(z => !string.IsNullOrWhiteSpace(z.Key)))
                {
                    provisionalCulpritOldXeName = entry.Key;
                    provisionalCulpritNewXeName = entry.Value;
                    scratchPadText = scratchPadText.Replace($"<{entry.Key}>", $"<{entry.Value}>");
                    scratchPadText = scratchPadText.Replace($"</{entry.Key}>", $"</{entry.Value}>");
                }

                return scratchPadText;
            }

            #region try-catch

            catch (Exception ex)
            {
                var intro = JghString.ConcatAsSentences(
                    "Unable to substitute field names with their specified equivalents.",
                    $"Problem arose whilst replacing occurrences of the field name {provisionalCulpritOldXeName} with {provisionalCulpritNewXeName}.",
                    "Please inspect the data. There could be a programming error here.",
                    "A typical programming error is that a specified equivalent contains a forbidden character.  Angle-brackets, for example, are prohibited in XML field names.");

                failure = JghString.ConcatAsSentences(failure, intro);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        private static string PrettifySymbolsAndEnums(string text, Dictionary<string, string> mappingDictionary)
        {
            var failure = "Unable to map XElement values in accordance with mapping dictionary.";
            const string locus = "[MapXElementSymbolsAndEnums]";


            var provisionalCulpritOldValue = "";
            var provisionalCulpritNewValue = "";

            var scratchPadText = text;

            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    throw new ArgumentNullException(nameof(text));

                foreach (var entry in mappingDictionary.Where(z => !string.IsNullOrWhiteSpace(z.Key)))
                {
                    provisionalCulpritOldValue = entry.Key;
                    provisionalCulpritNewValue = entry.Value;
                    scratchPadText = scratchPadText.Replace($">{entry.Key}<", $">{entry.Value}<");
                }

                return scratchPadText;
            }

            #region try-catch

            catch (Exception ex)
            {
                var intro = JghString.ConcatAsSentences(
                    "Unable to substitute fields with their specified equivalents.",
                    $"Problem arose whilst replacing occurrences of {provisionalCulpritOldValue} with {provisionalCulpritNewValue}.",
                    "Please inspect the data. There could be a programming error here.",
                    "A typical programming error is that a specified equivalent contains a forbidden character.");

                failure = JghString.ConcatAsSentences(failure, intro);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        private static XElement ParsePlainTextIntoXml(string inputText)
        {
            var failure = "Unable to parse text into xml.";
            const string locus = "[ParsePlainTextIntoXml]";

            try
            {
                if (inputText is null)
                    throw new ArgumentNullException(nameof(inputText));

                return XElement.Parse(inputText); // automatically throws if invalid
            }

            #region try-catch

            catch (Exception ex)
            {
                failure = JghString.ConcatAsSentences(
                    $"{failure} Text is not 100% correctly formatted.",
                    "Even the tiniest error in format, syntax and/or content causes disqualification.");

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        #endregion

    }
}
