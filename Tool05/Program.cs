using System.IO;
using System.Xml.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051

namespace Tool05;

internal class Program
{
    private const string Description =
        "This program is used to do four things." +
        " 1. be a test bed for new publisher-profile files authored in XML for future publisher modules." +
        " 2. ensure that the authored profile-files deserialise correctly using the system serialiser at the client end of the remote svc wire." +
        " 3. maintain a handwritten free-form deserialiser that can deserialise profile-files (used for exception fallback at the remote svc end). " +
        " 4. be a test bed for a handwritten XML fragment reader to extract custom information out of the profile-files for sole use at the remote svc end." +
        " A minimally valid profile-file includes a <ThisFileNameFragment> element and <CSharpModuleCodeName> element." +
        " These paired elements tie the specifications in a publisher profile file to the functions of a paired C# code module." +
        " The program is used to debug new xml profiles used for serialisation as a serialised [PublisherModuleProfileItemDto]." +
        " The beauty is that the XML is designed to be extensible, it can be free-form to an extent. Each C# module can have its" +
        " own customised profile that contains settings, parameters, and data that is specific to the code module." +
        " At minimum, the profile must populate all the members of a valid [PublisherModuleProfileItemDto]." +
        " Beyond that, additional elements are optional and customised. The problem we are circumventing with a " +
        " handwritten deserialiser is that it caters for the extensibilty. We deserialise the profile file" +
        " in two steps: first we use the system deserialiser to create a [PublisherModuleProfileItemDto] object." +
        " This gets us the essential elements of the file. The we use a totally simple little custom manual de-serialiser" +
        " to fish out custom XElements manually. Once debugged, copy and paste the perfected methods into your module." +
        " For 3. cut and paste the method perfected here into private [IPublisher.PublisherBase.ManuallyDeserialisePublisherProfileDtoFromXml()]" +
        " For 4. cut and paste the method perfected here into abstract [IPublisher.PublisherXXXXX.ExtractCustomElementsFromAssociatedProfile()]";

    #region variables

    private static readonly List<FileItem> FileItemsToBeProcessed = [];

    #endregion


    private static Task Main()
    {
        JghConsoleHelper.WriteLineWrappedInTwo("Welcome.");
        JghConsoleHelper.WriteLineFollowedByOne(Description);
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Input folder", LhsWidth)} : {InputFolder}");
        JghConsoleHelper.WriteLine($"{JghString.LeftAlign("Output folder", LhsWidth)} : {OutputFolder}");
        JghConsoleHelper.WriteLineWrappedInOne("Press enter to go. When you see FINISH you're done.");
        JghConsoleHelper.ReadLine();
        JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

        try
        {
            #region get ready

            #region confirm existence of folders

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(InputFolder);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + InputFolder);
                return Task.CompletedTask;
            }

            try
            {
                // If this directory does not exist, a DirectoryNotFoundException is thrown when attempting to set the current directory.
                Directory.SetCurrentDirectory(OutputFolder);
            }
            catch (DirectoryNotFoundException)
            {
                JghConsoleHelper.WriteLine("Directory not found: " + OutputFolder);
                return Task.CompletedTask;
            }

            #endregion

            #region confirm existence of files of input data

            var di = new DirectoryInfo(InputFolder); // Create a reference to the input directory.

            var arrayOfInputFileInfo = di.GetFiles(); // Create an array representing the files in the current directory.

            #endregion

            #region confirm existence of directories and files for output data (we don't do any output files for now. placeholder

            try
            {
                foreach (var fileInfo in arrayOfInputFileInfo)
                {
                    var pathOfFile = Path.Combine(OutputFolder, Path.ChangeExtension(fileInfo.Name, StandardFileTypeSuffix.Json));

                    File.WriteAllTextAsync(pathOfFile, "<element>dummy</element>");
                }
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLine($"Failed to access a designated output file. {e.Message}");

                return Task.CompletedTask;
            }

            #endregion

            #region Ingest the filenames to be processed, interpreting the contents as raw strings at the outset

            try
            {
                foreach (var fileInfo in arrayOfInputFileInfo)
                {
                    if (!fileInfo.Name.EndsWith(RequiredFileExtension))
                    {
                        JghConsoleHelper.WriteLine($"Skipping {fileInfo.Name} because it's not a {RequiredFileExtension} file as you appear to require. Does this make sense?");
                        continue;
                    }

                    var fileToBeProcessed = new FileItem
                    {
                        FileInfo = fileInfo,
                        FileContentsAsLines = [],
                        OutputFileName = fileInfo.Name
                    };

                    FileItemsToBeProcessed.Add(fileToBeProcessed);
                    JghConsoleHelper.WriteLine($"File added to list to be processed:  {fileToBeProcessed.FileInfo.Name}.");
                }

                JghConsoleHelper.WriteLine();


                try
                {
                    foreach (var item in FileItemsToBeProcessed)
                    {
                        JghConsoleHelper.WriteLine($"Looking for {item.FileInfo.Name}.");

                        var fullInputPath = item.FileInfo.FullName;

                        var rawInputAsText = File.ReadAllText(fullInputPath);
                        var rawInputAsLines = File.ReadAllLines(fullInputPath);

                        item.FileContentsAsText = rawInputAsText;
                        item.FileContentsAsLines = rawInputAsLines.ToList();

                        JghConsoleHelper.WriteLine($"Found {item.FileInfo.Name}.");
                    }
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLine(e.Message);
                    throw new Exception(e.InnerException?.Message);
                }

                if (FileItemsToBeProcessed.Count == 0)
                    throw new Exception("Found not even a single file with the designated file extension in the input folder.");
            }
            catch (Exception e)
            {
                JghConsoleHelper.WriteLineFollowedByOne($"Exception thrown: failed to import list of files of input data. {e.Message}");
                return Task.CompletedTask;
            }

            JghConsoleHelper.WriteLine();

            #endregion

            #endregion

            Main06(FileItemsToBeProcessed);
            //Main05(FileItemsToBeProcessed);
            //Main04(FileItemsToBeProcessed);
            //Main03(FileItemsToBeProcessed);
            //Main02(FileItemsToBeProcessed);
            //Main01(FileItemsToBeProcessed);
        }
        catch (Exception e)
        {
            JghConsoleHelper.WriteLineWrappedInOne($"{e.Message}");
        }

        return Task.CompletedTask;
    }

    #region local class

    internal class FileItem
    {
        public FileInfo FileInfo { get; set; } = new("DummyFileName.txt");

        public string FileContentsAsText { get; set; } = string.Empty;

        public List<string> FileContentsAsLines { get; set; } = [];

        public string OutputFileName { get; set; } = string.Empty;
    }

    #endregion

    #region constants

    private const int LhsWidth = 30;
    private const string RequiredFileExtension = ".xml"; // or .json as case may be
    private const string InputFolder = @"C:\Users\johng\holding pen\Xml-old\";
    private const string OutputFolder = @"C:\Users\johng\holding pen\Xml-new\";

    #endregion

    #region main methods

    private static void Main06(List<FileItem> fileItems)
    {
        if (!fileItems.Any())
        {
            JghConsoleHelper.WriteLine("No files of data to process.");
            return;
        }

        foreach (var fileItem in fileItems)
            try
            {
                #region step 1 - deserialise contents of this file. This should normally succeed.

                var publisherModuleProfileDtoFromXml = new PublisherModuleProfileItemDto();

                try
                {
                    publisherModuleProfileDtoFromXml = JghSerialisation.ToObjectFromXml<PublisherModuleProfileItemDto>(fileItem.FileContentsAsText, [typeof(PublisherModuleProfileItemDto)]);

                    JghConsoleHelper.WriteLine($"KICK-FOR-TOUCH WAS UNEVENTFUL: System.Xml de-serialised contents of file safely. [{fileItem.FileInfo.Name}]");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"KICK-FOR-TOUCH BLEW UP: System.Xml blew up when it tried to de-serialise contents of file. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLineFollowedByTwo(e.Message);
                }

                #endregion

                #region step 3 round trip and print to view

                try
                {
                    var publishingProfileDtoBackToXml = JghSerialisation.ToXmlFromObject(publisherModuleProfileDtoFromXml, [typeof(PublisherModuleProfileItemDto)]);

                    JghConsoleHelper.WriteLine();
                    JghConsoleHelper.WriteLineFollowedByOne($"FINISHED WITH THIS FILE. TEST MAY HAVE " +
                                                            $"SUCCEEDED. System.Xml did not blow up re-serialising PublisherModuleProfileItemDto to xml [{fileItem.FileInfo.Name}] ");
                    JghConsoleHelper.WriteLineFollowedByTwo("ooo000ooo");
                    JghConsoleHelper.WriteLineFollowedByOne("DISPLAYING: Round-tripped PublisherModuleProfileItemDto");

                    var objectAsString = publishingProfileDtoBackToXml;

                    JghConsoleHelper.WriteLine(objectAsString);

                    var pathOfFile = Path.Combine(OutputFolder, Path.ChangeExtension(fileItem.OutputFileName, StandardFileTypeSuffix.Xml));
                    File.WriteAllText(pathOfFile, objectAsString);

                    JghConsoleHelper.WriteLineWrappedInOne("ooo000ooo");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineWrappedInOne($"Test failure: Serialiser blew up trying to re-serialise object PublisherModuleProfileItemDto. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLine(e.Message);
                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST FAILED. [{fileItem.FileInfo.Name}] ");
                }

                #endregion
            }
            catch (Exception ex)
            {
                JghConsoleHelper.WriteLine($"{Path.GetFileName(fileItem.FileInfo.FullName)} not found? {ex.Message}");
            }
    }

    private static void Main05(List<FileItem> fileItems)
    {
        if (!fileItems.Any())
        {
            JghConsoleHelper.WriteLine("No files of data to process.");
            return;
        }

        foreach (var fileItem in fileItems)
            try
            {
                #region step 1 - deserialise contents of this file with Newtonsoft deserialiser. This should normally succeed.

                var seriesProfileDtoFromJson = new SeriesProfileDto();

                try
                {
                    seriesProfileDtoFromJson = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(fileItem.FileContentsAsText);

                    JghConsoleHelper.WriteLine($"KICK-FOR-TOUCH WAS UNEVENTFUL: Newtonsoft de-serialised contents of file safely. [{fileItem.FileInfo.Name}]");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"KICK-FOR-TOUCH BLEW UP: Newtonsoft blew up when it tried to de-serialise contents of file. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLineFollowedByTwo(e.Message);
                }

                #endregion

                #region reset all EventSettings to null (as opposed to empty)

                foreach (var eventProfileDto in seriesProfileDtoFromJson.EventProfileCollection)
                {
                    eventProfileDto.EventSettings = null;
                    eventProfileDto.PublishedResultsForEvent = null;
                }

                #endregion

                #region step 3 round trip and print to view

                try
                {
                    var seriesProfileDtoBackToJson = JghSerialisation.ToJsonFromObject(seriesProfileDtoFromJson);

                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST MAY HAVE " +
                                                           $"SUCCEEDED. Newtonsoft did not blow up re-serialising SeriesProfileDto to Json [{fileItem.FileInfo.Name}] ");
                    JghConsoleHelper.WriteLineFollowedByTwo("ooo000ooo");
                    JghConsoleHelper.WriteLineFollowedByOne("DISPLAYING: Round-tripped SeriesProfileDto");

                    var objectAsString = seriesProfileDtoBackToJson;

                    JghConsoleHelper.WriteLine(objectAsString);

                    var pathOfFile = Path.Combine(OutputFolder, Path.ChangeExtension(fileItem.OutputFileName, StandardFileTypeSuffix.Json));

                    File.WriteAllText(pathOfFile, objectAsString);


                    JghConsoleHelper.WriteLineWrappedInOne("ooo000ooo");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineWrappedInOne($"Test failure: Serialiser blew up trying to re-serialise object SeriesProfileDto. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLine(e.Message);
                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST FAILED. [{fileItem.FileInfo.Name}] ");
                }

                #endregion
            }
            catch (Exception ex)
            {
                JghConsoleHelper.WriteLine($"{Path.GetFileName(fileItem.FileInfo.FullName)} not found? {ex.Message}");
            }
    }

    private static void Main04(List<FileItem> fileItems)
    {
        if (!fileItems.Any())
        {
            JghConsoleHelper.WriteLine("No files of data to process.");
            return;
        }

        foreach (var fileItem in fileItems)
            try
            {
                #region step 1 - deserialise contents of this file with Newtonsoft deserialiser. This should normally succeed.

                var computerProfileDtoFromXml = new PublisherModuleProfileItemDto();

                try
                {
                    computerProfileDtoFromXml = JghSerialisation.ToObjectFromXml<PublisherModuleProfileItemDto>(fileItem.FileContentsAsText, [typeof(PublisherModuleProfileItemDto)]);

                    JghConsoleHelper.WriteLine($"KICK-FOR-TOUCH WAS UNEVENTFUL: System de-serialised contents of file safely. [{fileItem.FileInfo.Name}]");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"KICK-FOR-TOUCH BLEW UP: system blew up when it tried to de-serialise contents of file. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLineFollowedByTwo(e.Message);
                }

                #endregion

                #region step 2 round trip and print to view

                try
                {
                    var computerProfileDtoBackToXml = JghSerialisation.ToXmlFromObject(computerProfileDtoFromXml, [typeof(PublisherModuleProfileItemDto)]);

                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST MAY HAVE " +
                                                           $"SUCCEEDED. System did not blow up re-serialising ComputerProfileDto to XML [{fileItem.FileInfo.Name}] ");
                    JghConsoleHelper.WriteLineFollowedByOne("ooo000ooo");
                    JghConsoleHelper.WriteLineWrappedInOne("DISPLAYING: Round-tripped ComputerProfileDto");

                    var objectAsString = computerProfileDtoBackToXml;
                    JghConsoleHelper.WriteLine(objectAsString);

                    var pathOfFile = Path.Combine(OutputFolder, Path.ChangeExtension(fileItem.OutputFileName, StandardFileTypeSuffix.Xml));

                    File.WriteAllText(pathOfFile, objectAsString);

                    JghConsoleHelper.WriteLineWrappedInOne("ooo000ooo");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineWrappedInOne($"Test failure: Serialiser blew up trying to re-serialise object ComputerProfileDto. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLine(e.Message);
                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST FAILED. [{fileItem.FileInfo.Name}] ");
                }

                #endregion
            }
            catch (Exception ex)
            {
                JghConsoleHelper.WriteLine($"{Path.GetFileName(fileItem.FileInfo.FullName)} not found? {ex.Message}");
            }
    }

    private static void Main03(List<FileItem> fileItems)
    {
        if (!fileItems.Any())
        {
            JghConsoleHelper.WriteLine("No files of data to process.");
            return;
        }

        foreach (var fileItem in fileItems)
            try
            {
                #region step 1 - deserialise contents of this file with Newtonsoft deserialiser. This should normally succeed.

                var seasonProfileDtoFromJson = new SeasonProfileDto();

                try
                {
                    seasonProfileDtoFromJson = JghSerialisation.ToObjectFromJson<SeasonProfileDto>(fileItem.FileContentsAsText);

                    JghConsoleHelper.WriteLine($"KICK-FOR-TOUCH WAS UNEVENTFUL: Newtonsoft de-serialised contents of file safely. [{fileItem.FileInfo.Name}]");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"KICK-FOR-TOUCH BLEW UP: Newtonsoft blew up when it tried to de-serialise contents of file. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLineFollowedByTwo(e.Message);
                }

                #endregion

                #region step 2 round trip and print to view

                try
                {
                    var seasonProfileDtoBackToJson = JghSerialisation.ToJsonFromObject(seasonProfileDtoFromJson);

                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST MAY HAVE " +
                                                           $"SUCCEEDED. Newtonsoft did not blow up re-serialising SeasonProfileDto to Json [{fileItem.FileInfo.Name}] ");
                    JghConsoleHelper.WriteLineFollowedByTwo("ooo000ooo");
                    JghConsoleHelper.WriteLineFollowedByOne("DISPLAYING: Round-tripped SeasonProfileDto");

                    var objectAsString = seasonProfileDtoBackToJson;
                    JghConsoleHelper.WriteLine(objectAsString);

                    var pathOfFile = Path.Combine(OutputFolder, Path.ChangeExtension(fileItem.OutputFileName, StandardFileTypeSuffix.Json));

                    File.WriteAllText(pathOfFile, objectAsString);

                    JghConsoleHelper.WriteLineWrappedInOne("ooo000ooo");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineWrappedInOne($"Test failure: Serialiser blew up trying to re-serialise object SeasonProfileDto. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLine(e.Message);
                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST FAILED. [{fileItem.FileInfo.Name}] ");
                }

                #endregion
            }
            catch (Exception ex)
            {
                JghConsoleHelper.WriteLine($"{Path.GetFileName(fileItem.FileInfo.FullName)} not found? {ex.Message}");
            }
    }

    private static void Main02(List<FileItem> fileItems)
    {
        if (!fileItems.Any())
        {
            JghConsoleHelper.WriteLine("No files of data to process.");
            return;
        }

        foreach (var fileItem in fileItems)
            try
            {
                #region step 1 - deserialise contents of this file with Newtonsoft deserialiser. This should normally succeed.

                var seriesProfileDtoFromJson = new SeriesProfileDto();

                try
                {
                    seriesProfileDtoFromJson = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(fileItem.FileContentsAsText);

                    JghConsoleHelper.WriteLine($"KICK-FOR-TOUCH WAS UNEVENTFUL: Newtonsoft de-serialised contents of file safely. [{fileItem.FileInfo.Name}]");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"KICK-FOR-TOUCH BLEW UP: Newtonsoft blew up when it tried to de-serialise contents of file. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLineFollowedByTwo(e.Message);
                }

                #endregion

                #region step 2 round trip and print to view

                try
                {
                    var seriesProfileDtoBackToJson = JghSerialisation.ToJsonFromObject(seriesProfileDtoFromJson);

                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST MAY HAVE " +
                                                           $"SUCCEEDED. Newtonsoft did not blow up re-serialising SeriesProfileDto to Json [{fileItem.FileInfo.Name}] ");
                    JghConsoleHelper.WriteLineFollowedByOne("ooo000ooo");
                    JghConsoleHelper.WriteLineWrappedInOne("DISPLAYING: Round-tripped SeriesProfileDto");

                    var objectAsString = seriesProfileDtoBackToJson;
                    JghConsoleHelper.WriteLine(objectAsString);

                    var pathOfFile = Path.Combine(OutputFolder, Path.ChangeExtension(fileItem.OutputFileName, StandardFileTypeSuffix.Json));


                    File.WriteAllText(pathOfFile, objectAsString);

                    JghConsoleHelper.WriteLineWrappedInOne("ooo000ooo");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"Test failure: Serialiser blew up trying to re-serialise object SeriesProfileDto. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLine(e.Message);
                    JghConsoleHelper.WriteLineFollowedByOne($"FINISHED WITH THIS FILE. TEST FAILED. [{fileItem.FileInfo.Name}] ");
                }

                #endregion
            }
            catch (Exception ex)
            {
                JghConsoleHelper.WriteLine($"{Path.GetFileName(fileItem.FileInfo.FullName)} not found? {ex.Message}");
            }
    }

    private static void Main01(List<FileItem> fileItems)
    {
        if (!fileItems.Any())
        {
            JghConsoleHelper.WriteLine("No files of data to process.");
            return;
        }

        foreach (var fileItem in fileItems)
            try
            {
                #region step 1 - just for fun, attempt to deserialise contents of this file with system deserialiser. This will often/normally fail.

                try
                {
                    JghSerialisation.ToObjectFromXml<PublisherModuleProfileItemDto>(fileItem.FileContentsAsText, [typeof(PublisherModuleProfileItemDto)]);
                    JghConsoleHelper.WriteLine($"KICK-FOR-TOUCH WAS UNEVENTFUL: system XML serialiser de-serialised contents of file safely. [{fileItem.FileInfo.Name}]");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"KICK-FOR-TOUCH BLEW UP: system serialiser blew up when it tried to de-serialise contents of file. THIS IS TO BE EXPECTED. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLineFollowedByTwo(e.Message);
                }

                #endregion

                #region step 2 de-serialise using hand-written deserialiser

                try
                {
                    var fileContentsAsDto = ToComputerProfileFromXml(fileItem.FileContentsAsText);

                    JghConsoleHelper.WriteLineWrappedInOne($"FINISHED WITH THIS FILE. TEST MAY HAVE " +
                                                           $"SUCCEEDED. Hand-written de-serialiser did not blow up parsing the contents of this file to ComputerProfileDto [{fileItem.FileInfo.Name}] ");
                    JghConsoleHelper.WriteLineFollowedByOne("ooo000ooo");
                    JghConsoleHelper.WriteLineWrappedInOne("DISPLAYING: Round-tripped ComputerProfileDto");

                    var xmlFromObject = JghSerialisation.ToXmlFromObject(fileContentsAsDto, [typeof(PublisherModuleProfileItemDto)]);

                    var xmlFromObjectAsString = xmlFromObject;

                    JghConsoleHelper.WriteLine(xmlFromObjectAsString);
                    JghConsoleHelper.WriteLineWrappedInOne("ooo000ooo");
                }
                catch (Exception e)
                {
                    JghConsoleHelper.WriteLineFollowedByOne($"Test failure: Hand-written de-serialiser blew up trying to parse contents of this file to ComputerProfileDto. [{fileItem.FileInfo.Name}]");
                    JghConsoleHelper.WriteLineFollowedByOne(e.Message);
                    JghConsoleHelper.WriteLineFollowedByOne($"FINISHED WITH THIS FILE. TEST FAILED. [{fileItem.FileInfo.Name}] ");
                }

                #endregion
            }
            catch (Exception ex)
            {
                JghConsoleHelper.WriteLine($"{Path.GetFileName(fileItem.FileInfo.FullName)} not found? {ex.Message}");
            }
    }

    #endregion

    #region THE MEAT helper methods

    // THE MEAT: this is the method to cut and paste into each computer module in the remote computer Svc 
    private static PublisherModuleProfileItemDto ToComputerProfileFromXml(string freeFormComputerProfileXmlFile)

    {
        const string failure = "Unable to convert string data into ComputerProfileDto.";
        const string locus = "[ToComputerProfileFromXmlV01]";

        try
        {
            XElement parentXElement;

            try
            {
                parentXElement = ParsePlainTextIntoXElement(freeFormComputerProfileXmlFile);
            }
            catch (Exception ex)
            {
                throw new JghPublisherServiceFaultException("System.Xml.Linq.XElement.Parse() blew up when it tried to parse contents of this file as a single top-level XElement. File contents are unexpected.", ex);
            }

            PublisherModuleProfileItemDto answer = new()
            {
                FragmentInNameOfThisFile = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeThisFileNameFragment),
                CaptionForModule = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeCaption),
                DescriptionOfModule = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeDescription),
                OverviewOfModule = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeOverview),
                CSharpModuleCodeName = GetChildElementValue(parentXElement, PublisherModuleProfileItemDto.XeCSharpModuleId)
            };

            var portalButtonArrayXe = GetChildElement(parentXElement, PublisherModuleProfileItemDto.XeGuiButtonsForPullingDatasetsFromPortalHub);
            var browseButtonArrayXe = GetChildElement(parentXElement, PublisherModuleProfileItemDto.XeGuiButtonsForBrowsingFileSystemForDatasets);

            var portalButtons = GetComputerGuiButtonProfileDtos(portalButtonArrayXe);
            var browseButtons = GetComputerGuiButtonProfileDtos(browseButtonArrayXe);

            answer.GuiButtonProfilesForPullingDatasetsFromPortalHub = portalButtons;
            answer.GuiButtonProfilesForBrowsingFileSystemForDatasets = browseButtons;

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion

        #region helpers

        static XElement ParsePlainTextIntoXElement(string inputText)
        {
            var failure = "Unable to parse text into xml.";
            const string locus = "[ParsePlainTextIntoXElement]";

            try
            {
                if (inputText == null)
                    throw new ArgumentNullException(nameof(inputText));

                return XElement.Parse(inputText); // automatically throws if invalid
            }

            #region try-catch

            catch (Exception ex)
            {
                failure = JghString.ConcatAsParagraphs(failure, "Parsing the contents of this file into a XElement failed.",
                    "(Unfortunately even the tiniest error in format, syntax and/or content causes failure.)");

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        static string GetChildElementValue(XElement parentXe, string nameOfChild)
        {
            var childXe = parentXe.Element(nameOfChild);

            if (childXe == null)
                return string.Empty;

            var value = childXe.Value; // empirically this blows up when the Value itself contains xml : for that use our cunning alternative method: GetChildElementValueVerbatim()

            return value.Trim();
        }

#pragma warning disable CS8321 // Local function is declared but never used
        static string GetChildElementValueVerbatim(XElement? parentXe, string nameOfChild)
#pragma warning restore CS8321 // Local function is declared but never used
        {
            var childXe = parentXe?.Element(nameOfChild);

            if (childXe == null)
                return string.Empty;

            var value = childXe.ToString(); // this works

            return value.Trim();

            //var rubbish1 = childXe.Value; // Note. empirically this alternative fails when the Value itself contains xml
            //var rubbish2 = (string) childXe; // Note. empirically this alternative fails when the Value itself contains xml
        }

        static XElement? GetChildElement(XElement parentXe, string nameOfChild)
        {
            var childXe = parentXe.Element(nameOfChild);

            return childXe;
        }

        static XElement[]? GetChildElements(XElement? parentXe, string nameOfChild)
        {
            var childrenXe = parentXe?.Elements(nameOfChild).ToArray();

            return childrenXe;
        }

        // ReSharper disable once IdentifierTypo
        static PublisherButtonProfileItemDto[] GetComputerGuiButtonProfileDtos(XElement? parentXe)
        {
            if (parentXe == null)
                return [];

            var buttonXElements = GetChildElements(parentXe, PublisherButtonProfileItemDto.XeGuiButtonProfile);

            if (buttonXElements == null || !buttonXElements.Any())
                return [];

            return buttonXElements.Select(thisButtonXe => new PublisherButtonProfileItemDto
            {
                ShortDescriptionOfAssociatedDataset = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeDatasetDescription),
                IdentifierOfAssociatedDataset = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeDatasetIdentifier),
                GuiButtonContent = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeGuiButtonText),
                FileNameExtensionFiltersForBrowsingHardDrive = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeAllowableFileNameExtensions),
                FileNameOfExampleOfAssociatedDataset = GetChildElementValue(thisButtonXe, PublisherButtonProfileItemDto.XeDatasetExampleFileName)
            }).ToArray();
        }

        #endregion
    }

#pragma warning disable CS8321 // Local function is declared but never used
    private static void SaveWorkToHardDriveAsXml(PublisherModuleProfileItemDto profileItem, FileItem fileItem)
#pragma warning restore CS8321 // Local function is declared but never used
    {
        var fileContents = JghSerialisation.ToXmlFromObject(profileItem, [typeof(PublisherModuleProfileItemDto)]);

        var pathOfFile = Path.Combine(OutputFolder, Path.ChangeExtension(fileItem.OutputFileName, StandardFileTypeSuffix.Xml));

        File.WriteAllText(pathOfFile, fileContents);

        JghConsoleHelper.WriteLine($"Round-tripped profile saved to {pathOfFile}");
    }

    #endregion
}