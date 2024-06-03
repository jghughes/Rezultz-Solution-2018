using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Goodies.Mar2022;
using Newtonsoft.Json;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;

namespace Tool02
{
    internal class Program
    {
        private const string Description = "This program de-serialises json files, then exports tidied up xml and json files.";

        private static void Main()
    {
        JghConsoleHelper.WriteLineWrappedInOne("Welcome.");
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder", LhsWidth)} : {InputFolder}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder for XML", LhsWidth)} : {OutputFolderForXml}");
        JghConsoleHelper.WriteLineFollowedByOne($"{JghString.LeftAlign("Output folder for JSON", LhsWidth)} : {OutputFolderForJson}");
        JghConsoleHelper.WriteLine($"MustDoWorkForXmlOutput = {MustDoWorkForXmlOutput}");
        JghConsoleHelper.WriteLine($"MustDoWorkForJsonOutput = {MustDoWorkForJsonOutput}");
        JghConsoleHelper.WriteLineWrappedInOne("Are you ready to go? Press enter to continue.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

        try
        {
            #region Step 1 Option A - load a bunch of files (ad hoc)

            //var arrayOfFileNames = new string[]
            //{
            //    @"awake.gif",
            //    @"bios.gif",
            //    @"eload.gif",
            //    @"eska.gif",
            //    @"foxy_logo.gif",
            //    @"mizuno.gif",
            //    @"rr_logo.gif",
            //    @"logo-aug-2013.gif"

            //};

            //FileInfo[] arrayOfInputFileInfo = arrayOfFileNames.Select(z => new FileInfo(InputFolder + z)).ToArray();

            #endregion

            #region Step 1 Option B - load a bunch of files (everything in a folder)

            var di = new DirectoryInfo(InputFolder); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing the files in the current directory.

            #endregion

            #region Step 2 Iterate through the files, doing work, and saving one or more files to the specified output folders

            foreach (var fileInfo in arrayOfInputFileInfo)
                try
                {
                    PublisherModuleProfileItemDto rawInputAsObject;

                    if (!Path.GetFileName(fileInfo.FullName).EndsWith(StandardFileTypeSuffix.Json))
                        continue;


                    #region step 1 read json file from specified input folder

                    var fullInputPath = fileInfo.FullName;

                    var rawInputAsText = File.ReadAllText(fullInputPath);

                    rawInputAsObject = JghSerialisation.ToObjectFromJson<PublisherModuleProfileItemDto>(rawInputAsText);

                    #endregion

                    #region step 2 option - do work on object to OutputFolderForXml if required

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (MustDoWorkForXmlOutput)
#pragma warning disable 162
                    {
                        var knownTypesForSerialization = new[] {typeof(PublisherModuleProfileItemDto), typeof(PublisherButtonProfileItemDto)};

                        var xE = JghSerialisation.ToXElementFromObject(rawInputAsObject,
                            knownTypesForSerialization);

                        var answerAsXml = xE.ToString();

                        // clean up irritating null artifacts of system xml serialiser

                        Tuple<string, string>[] snippets2 =
                        {
                            new("p3:nil=\"true\"", string.Empty),
                            new("xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty)
                        };

                        answerAsXml = snippets2.Aggregate(answerAsXml, (current, snippet) => current.Replace(snippet.Item1, snippet.Item2));

                        var pathOfXmlFile = OutputFolderForXml + @"\" + Path.GetFileNameWithoutExtension(fileInfo.FullName) + "." + StandardFileTypeSuffix.Xml;

                        File.WriteAllText(pathOfXmlFile, answerAsXml);


                        PrintReport(JghConvert.ToBytesUtf8FromString(rawInputAsText), JghConvert.ToBytesUtf8FromString(answerAsXml), pathOfXmlFile);
                    }
#pragma warning restore 162

                    #endregion

                    #region step 2 option - do work on object to OutputFolderForJson if required

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (MustDoWorkForJsonOutput)
                        // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
                    {
                        // on we go
                        var answerAsJson = JsonConvert.SerializeObject(rawInputAsObject, Formatting.None, new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore});

                        var pathOfJsonFile = OutputFolderForJson + @"\" + Path.GetFileNameWithoutExtension(fileInfo.FullName) + "." + StandardFileTypeSuffix.Json;

                        File.WriteAllText(pathOfJsonFile, answerAsJson);

                        PrintReport(JghConvert.ToBytesUtf8FromString(rawInputAsText), JghConvert.ToBytesUtf8FromString(answerAsJson), pathOfJsonFile);
                    }
#pragma warning restore 162

                    #endregion
                }
                catch (Exception ex)
                {
                    JghConsoleHelper.WriteLine($"{Path.GetFileName(fileInfo.FullName)} not found? {ex.Message}");
                }

            #endregion

            JghConsoleHelper.WriteLineWrappedInOne("FINISH. Press Enter to close.");
            JghConsoleHelper.ReadLine();
        }

        #region trycatch

        catch (Exception ex)
        {
            JghConsoleHelper.WriteLineWrappedInOne(ex.Message);
            JghConsoleHelper.WriteLineWrappedInOne("Sorry. Unable to continue. Rectify the error and try again.");
        }

        #endregion
    }

        #region helpers

        private static void PrintReport(byte[] beforeBytes, byte[] afterBytes, string filename)
    {
        var beforeBytesLength = JghConvert.SizeOfBytesInHighestUnitOfMeasure(beforeBytes.Length);
        var afterBytesLength = JghConvert.SizeOfBytesInHighestUnitOfMeasure(afterBytes.Length);
        var differenceInLength = JghConvert.SizeOfBytesInHighestUnitOfMeasure(afterBytes.Length - beforeBytes.Length);

        JghConsoleHelper.WriteLine($"Before: {beforeBytesLength,-10}    After: {afterBytesLength,-10}    Difference: {differenceInLength,-10}    Output file:{filename,-15}");
    }

        #endregion

        #region constants

        private const int LhsWidth = 50;

        private const string InputFolder = @"C:\Users\johng\holding pen\StuffByJohn\Input";
        private const string OutputFolderForXml = @"C:\Users\johng\holding pen\StuffByJohn\Output";
        private const string OutputFolderForJson = @"C:\Users\johng\holding pen\StuffByJohn\Output";

        private const bool MustDoWorkForXmlOutput = true;
        private const bool MustDoWorkForJsonOutput = true;

        #endregion
    }
}