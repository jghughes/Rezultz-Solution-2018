using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Tool07;

internal class Program
{
    private const string Description = "This program reads a folder of files of published results and replaces all the XElement names to lower case.";

    private static void Main()
    {
        #region Preamble

        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder for old Results files", LhsWidth)} : {InputFolder}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for new Results files", LhsWidth)} : {OutputFolder}");
        JghConsoleHelper.WriteLineWrappedInOne("Press enter to go. When you see FINISH you're done.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

        #endregion

        #region confirm existence of folders

        try
        {
            // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
            Directory.SetCurrentDirectory(InputFolder);
        }
        catch (DirectoryNotFoundException)
        {
            JghConsoleHelper.WriteLine("Directory not found: " + InputFolder);
            return;
        }

        try
        {
            // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
            Directory.SetCurrentDirectory(OutputFolder);
        }
        catch (DirectoryNotFoundException)
        {
            JghConsoleHelper.WriteLine("Directory not found: " + OutputFolder);
            return;
        }

        #endregion

        try
        {
            #region Step 1 - load a bunch of files (everything in a folder)

            var di = new DirectoryInfo(InputFolder); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing the files in the current directory.

            #endregion

            #region Step 2 - first of all clean out old files in designated output location

            DeleteFilesInThisFolder(OutputFolder);

            #endregion

            #region Step 3 - iIterate through the files, doing work, an saving them in designated output location

            var numberOfFilesProcessed = 0;

            foreach (var fileInfo in arrayOfInputFileInfo)
            {
                if (!Path.GetFileName(fileInfo.FullName).EndsWith(".xml")) continue; // we are only interested in xml files here

                RenameXElements(fileInfo);

                numberOfFilesProcessed++;
            }

            Console.WriteLine($"Total Number of files processed: {numberOfFilesProcessed}");

            #endregion
        }

        #region trycatch

        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            Console.WriteLine($"{ex.InnerException?.Message}");
            Console.WriteLine();
            Console.WriteLine("Sorry. Unable to continue. Rectify the error and try again.");
        }

        #endregion

        #region wrap up

        Console.WriteLine();
        Console.WriteLine("FINISH. Press Enter to close.");
        Console.ReadLine();

        #endregion
    }

    #region constants

    private const int LhsWidth = 30;

    private const string InputFolder = @"C:\Users\johng\holding pen\results-old\";
    private const string OutputFolder = @"C:\Users\johng\holding pen\results-new\";

    public const string XeArrayOfResult = "ArrayOf"+$"{XeResult}";
    public const string XeResult = "Result";
    public const string XeFirst = "First";
    public const string XeMiddle = "Middle";
    public const string XeLast = "Last";
    public const string XeSex = "Sex"; // NB
    public const string XeAge = "Age";
    public const string XeIsSeries = "IsSeries";
    public const string XeRace = "Race";
    public const string XeAgeGroup = "AgeGroup";
    public const string XeTeam = "Team";
    public const string XeCity = "City";
    public const string XeT01 = "T01";
    public const string XeT02 = "T02";
    public const string XeT03 = "T03";
    public const string XeT04 = "T04";
    public const string XeT05 = "T05";
    public const string XeT06 = "T06";
    public const string XeT07 = "T07";
    public const string XeT08 = "T08";
    public const string XeT09 = "T09";
    public const string XeT10 = "T10";
    public const string XeT11 = "T11";
    public const string XeT12 = "T12";
    public const string XeT13 = "T13";
    public const string XeT14 = "T14";
    public const string XeT15 = "T15";
    public const string XeDnxString = "Dnx";
    public const string XeBib = "Bib";
    public const string XeFirstName = "First";
    public const string XeMiddleInitial = "Middle";
    public const string XeLastName = "Last";
    public const string XeFullName = "FullName";

    private static readonly List<string> ListOfXeNames = new()
    {
        XeArrayOfResult,
        XeResult,
        XeFirst,
        XeMiddle,
        XeLast,
        XeSex,
        XeAge,
        XeIsSeries,
        XeRace,
        XeAgeGroup,
        XeTeam,
        XeCity,
        XeT01,
        XeT02,
        XeT03,
        XeT04,
        XeT05,
        XeT06,
        XeT07,
        XeT08,
        XeT09,
        XeT10,
        XeT11,
        XeT12,
        XeT13,
        XeT14,
        XeT15,
        XeDnxString,
        XeBib,
        XeFirstName,
        XeMiddleInitial,
        XeLastName,
        XeFullName
    };

    private static readonly List<string> ListOfTextToBeDeleted = new()
    {
        "xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\"",
        "xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"",
        "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"",
        "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"",
        "p3:nil=\"true\" xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" ",
    };

    #endregion

    #region helpers

    private static void RenameXElements(FileInfo fileInfo)
    {
        #region get the text

        var fullInputPath = fileInfo.FullName;

        var oldText = File.ReadAllText(fullInputPath);

        #endregion

        #region first of all delete unwanted text that might mess-up the replacements

        var textAfterDeletions = ListOfTextToBeDeleted.Aggregate(oldText, (current, textToBeDeleted) => current.Replace(textToBeDeleted, string.Empty));

        #endregion

        #region get list of old/new text replacements

        var listOfTextSubstitutionPairs = ComposeTextSubstitutionPairs();

        #endregion

        #region do the text replacements

        var newText =  listOfTextSubstitutionPairs.Aggregate(textAfterDeletions, (current, pair) => current.Replace(pair.Item1, pair.Item2));

        newText = newText.Replace("arrayofresult", "ArrayOfresult");

        #endregion

        #region double-check the new XML text - does it parse as an XML document and deserialise correctly ?

        var myXml = ParsePlainTextIntoXml(newText);

        var dummyRows = ExtractListOfIndividualResults(myXml, XeResult.ToLower()); // blow up?

        var arrayOfResultDto = ConvertArrayOfXElementsToArrayOfResultItemDataTransferObjects(dummyRows);

        var yy = JghSerialisation.ToXmlFromObject(arrayOfResultDto, new[] {typeof(ResultDto)});

        var zz = ListOfTextToBeDeleted.Aggregate(yy, (current, textToBeDeleted) => current.Replace(textToBeDeleted, string.Empty));

        #endregion

        #region use same filename and save the new text in designated output location

        var legitNewFileName = fileInfo.Name;

        var pathOfOutputFile = OutputFolder + @"\" + legitNewFileName;

        File.WriteAllText(pathOfOutputFile, zz);
        //File.WriteAllText(pathOfOutputFile, newText);

        #endregion

        PrintReport(pathOfOutputFile);
    }


    private static XElement ParsePlainTextIntoXml(string inputText)
    {
        var failure = "Unable to parse text into xml.";
        const string locus = "[ParsePlainTextIntoXml]";

        try
        {
            if (inputText == null)
                throw new ArgumentNullException(nameof(inputText));

            return XElement.Parse(inputText);
        }

        #region try-catch

        catch (Exception ex)
        {
            failure = JghString.ConcatAsSentences(
                $"{failure} Text is not 100% correctly formatted.",
                "Even the tiniest error in format, syntax and/or content causes disqualification.");

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, "", "", ex);
        }

        #endregion
    }

    private static XElement[] ExtractListOfIndividualResults(XContainer parentXContainer,
    string nameOfRepeatingChildElement)
    {
        var failure = "Unable to extract child elements from parent Xml document.";
        const string locus = "[ExtractListOfIndividualResults]";

        try
        {
            if (parentXContainer == null)
                throw new ArgumentNullException(nameof(parentXContainer));

            if (string.IsNullOrWhiteSpace(nameOfRepeatingChildElement))
                throw new ArgumentException($"Error. Null or blank argument. <{nameOfRepeatingChildElement}>");

            var repeaters = parentXContainer.Elements(nameOfRepeatingChildElement).ToArray();

            if (!repeaters.Any())
                throw new Exception("No rows of data found.");

            return repeaters;
        }

        #region try-catch

        catch (Exception ex)
        {
            var intro = JghString.ConcatAsSentences(
                "Unable to see or retrieve multiple repeating records in your data. This might be because there aren\'t any. It might be because the items are invisible.",
                "If your data is exported from Access or Excel, be aware that row titles are generated automatically by the export wizard.",
                "The wizard takes them from the name of their containing worksheet or table or table query output as the case may be.",
                $"The problematic row title seems to be <{nameOfRepeatingChildElement}>.",
                "This needs to be the same as the title in your dataset.");

            failure = JghString.ConcatAsSentences(failure, intro);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, "Locus2", "Locus3", ex);
        }

        #endregion
    }

    private static ResultDto[] ConvertArrayOfXElementsToArrayOfResultItemDataTransferObjects(XElement[] arrayOfIndividualResultXes)
    {
        List<ResultDto> answer = new();

        foreach (var element in arrayOfIndividualResultXes)
        {
            var resultItem = JghSerialisation.ToObjectFromXml<ResultDto>(element.ToString(), new[]{typeof(ResultDto)});

            answer.Add(resultItem);
        }

        return answer.ToArray();
    }

    private static List<Tuple<string, string>> ComposeTextSubstitutionPairs()
    {
        var textSubstitutionPairs = new List<Tuple<string, string>>();

        foreach (var xeName in ListOfXeNames)
        {
            var oldOpeningTag = $"<{xeName}>";
            var newOpeningTag = $"<{xeName.ToLower()}>";

            var oldClosingTag = $"</{xeName}>";
            var newClosingTag = $"</{xeName.ToLower()}>";

            var oldEmptyTag = $"<{xeName} />";
            var newEmptyTag = $"<{xeName.ToLower()} />";

            textSubstitutionPairs.Add(new Tuple<string, string>(oldOpeningTag, newOpeningTag));
            textSubstitutionPairs.Add(new Tuple<string, string>(oldClosingTag, newClosingTag));
            textSubstitutionPairs.Add(new Tuple<string, string>(oldEmptyTag, newEmptyTag));
        }

        return textSubstitutionPairs;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static string[] DeleteFilesInThisFolder(string folderPath)
    {
        var filePaths =
            Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).ToArray();

        foreach (var filePath in filePaths)
        {
            var fi = new FileInfo(filePath);

            fi.Delete();
        }

        return filePaths;
    }

    private static void PrintReport(string filename)
    {
        Console.WriteLine($"Output file:{filename}");
    }

    #endregion
}