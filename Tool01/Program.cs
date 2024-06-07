using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Prism.July2018;
using System.IO;

namespace Tool01
{
    internal class Program
    {
        private const string Description = "This program is intended to be a dirty little throw-away scratch pad. Do your work in Main01(). Erase it when you are finished.";


        private static void Main()
        {
            JghConsoleHelper.WriteLineWrappedInOne("Welcome.");
            JghConsoleHelper.WriteLineFollowedByOne(Description);
            JghConsoleHelper.WriteLineWrappedInOne("Are you ready to go? Press enter to continue.");
            JghConsoleHelper.ReadLine();
            JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

            try
            {
                Main01();

                JghConsoleHelper.WriteLineWrappedInOne("FINISH. Press enter to close.");
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
        private static void Main01()
        {
            FileInfo fileInfo = new FileInfo(@"C:\Users\johng\holding pen\StuffByJohn\Input\input.json");

            const string  OutputFolderForXml = @"C:\Users\johng\holding pen\rubbish";

            var path = Path.Combine(OutputFolderForXml, Path.ChangeExtension(fileInfo.Name, StandardFileTypeSuffix.Xml));

            DateTime startDate = new DateTime(2023, 9, 19);
            //DateTime endDate = new DateTime(2024, 3, 6);

            //double elapsedPeriodFromStartOfZsunToFourthCategoryChange = (endDate - startDate).TotalDays;

            //int zz = (int) (elapsedPeriodFromStartOfZsunToFourthCategoryChange + 90);

            //var yy = new TimeSpan(zz, 0, 0, 0);

            //DateTime theoreticalCommencementOfLoggingOfPowerDataByZwift = endDate - yy;

            //var altStartDate = endDate.AddDays(-90);

        }

        #region constants

        //private const int LhsWidth = 50;

        //private const string InputFolder = @"C:\Users\johng\holding pen\StuffByJohn\Input";
        //private const string OutputFolderForXml = @"C:\Users\johng\holding pen\StuffByJohn\Output";
        //private const string OutputFolderForJson = @"C:\Users\johng\holding pen\StuffByJohn\Output";

        //private const bool MustDoWorkForXmlOutput = true;
        //private const bool MustDoWorkForJsonOutput = true;

        #endregion

    }
}
