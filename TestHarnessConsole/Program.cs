using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces03.Apr2022;
using NetStd.Objects03.Oct2022;
using Rezultz.DataTypes.Nov2023.RsrezultzItems;
using TestHarnessConsole.ServiceReference1;

//using NetStd.RsRezultz01.July2018;

namespace TestHarnessConsole;

public class Program
{
    #region Main

    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Hellooooooooo. Welcome to my my little console project for testing things");
        Console.ReadLine();

        Console.WriteLine("Are you ready to go?");
        Console.ReadLine();

        var svcClient1 = new TimingSystemDataPreprocessingSvcClient("NetHttpBinding_ITimingSystemDataPreprocessingSvc");

        var xx = svcClient1.GetIfServiceIsAnswering();

        Console.WriteLine(xx);


        Console.ReadLine();

        return 0;
    }

    #endregion

    #region fields

    private static IAzureStorageSvcConnector _svcConnector;

    private static IPublishingSvcConnector _ttConnector;

    #endregion

    #region test data

    private static readonly int _testMetaDataId = 199; // valid
    //static int _testMetaDataId = 101; // nonexistent

    private static readonly string _testTimingTentConverterCodeName = "15m"; // valid

    #endregion

    #region tests

    private static async Task<bool> DoAllAzureStorageHelperTestsAsync()
    {
        var testDescription = "";

        try
        {
            Console.WriteLine("BEGIN DoAllAzureStorageHelperTestsAsync");
            Console.WriteLine();

            #region GetIfServiceIsAnsweringAsync

            testDescription = "TEST : Connector.GetIfServiceIsAnsweringAsync()";

            PrintIntroMessage(testDescription);

            try
            {
                var outcomeAsBool = await _svcConnector.ThrowIfNoServiceConnectionAsync(CancellationToken.None);

                Console.WriteLine($"Answer = {outcomeAsBool}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                PrintErrorMessage(testDescription, ex);
            }


            #region print

            #endregion

            #endregion

            #region GetServiceEndpointsInfoAsync

            testDescription = "TEST : Connector.GetServiceEndpointsInfoAsync()";

            PrintIntroMessage(testDescription);

            try
            {
                var outcomeAsStringArray = await _svcConnector.GetServiceEndpointsInfoAsync(CancellationToken.None);

                PrintLineItems(outcomeAsStringArray);
            }
            catch (Exception ex)
            {
                PrintErrorMessage(testDescription, ex);
            }

            #endregion

            // todo all the other tests

            Console.WriteLine("END DoAllAzureStorageHelperTestsAsync");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            PrintErrorMessage(testDescription, ex);

            return false;
        }

        return true;
    }

    private static async Task<bool> DoAllTimingSystemDataConverterTestsAsync()
    {
        var testDescription = "";

        try
        {
            Console.WriteLine("BEGIN DoAllTimingSystemDataConverterTestsAsync");
            Console.WriteLine();

            #region GetIfServiceIsAnsweringAsync

            testDescription = "TEST : Connector.GetIfServiceIsAnsweringAsync()";

            PrintIntroMessage(testDescription);

            bool outcomeAsBool;

            try
            {
                outcomeAsBool = await _ttConnector.GetIfServiceIsAnsweringAsync(CancellationToken.None);

                Console.WriteLine($"Answer = {outcomeAsBool.ToString()}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                PrintErrorMessage(testDescription, ex);
            }

            #endregion

            #region GetServiceEndpointsInfoAsync

            testDescription = "TEST : Connector.GetServiceEndpointsInfoAsync()";

            PrintIntroMessage(testDescription);

            try
            {
                var answer = await _ttConnector.GetServiceEndpointsInfoAsync(CancellationToken.None);

                PrintLineItems(answer);
            }
            catch (Exception ex)
            {
                PrintErrorMessage(testDescription, ex);
            }

            #endregion

            #region GetListOfPreprocessorsAsync

            //testDescription = "TEST : GetListOfPreprocessorsAsync()";

            //PrintIntroMessage(testDescription);

            //try
            //{
            //    var answer = await _ttConnector.GetListOfPreprocessorsAsync(CancellationToken.None);

            //    PrintLineItems(answer);
            //}
            //catch (Exception ex)
            //{
            //    PrintErrorMessage(testDescription, ex);
            //}

            #endregion

            #region GetIfPreprocessorIdIsRecognisedAsync

            //testDescription = $"TEST : GetIfPreprocessorIdIsRecognisedAsync('{_testTimingTentConverterCodeName}')";

            //PrintIntroMessage(testDescription);

            //try
            //{
            //    var answer = await _ttConnector.GetIfPreprocessorIdIsRecognisedAsync(_testTimingTentConverterCodeName, CancellationToken.None);

            //    Console.WriteLine($"Answer = {answer}");
            //    Console.WriteLine();
            //}
            //catch (Exception ex)
            //{
            //    PrintErrorMessage(testDescription, ex);
            //}

            #endregion

            #region GetInfoAboutPreprocessorAsync

            //testDescription = $"TEST : GetInfoAboutPreprocessorAsync('{_testTimingTentConverterCodeName}')";

            //PrintIntroMessage(testDescription);

            //try
            //{
            //    var answer = await _ttConnector.GetInfoAboutPreprocessorAsync(_testTimingTentConverterCodeName, CancellationToken.None);

            //    if (answer == null)
            //        Console.WriteLine("Method returned null!");
            //    else
            //        Console.WriteLine($"'{answer.String1}'  '{answer.String2}'  '{answer.String3}'");
            //    Console.WriteLine();
            //}
            //catch (Exception ex)
            //{
            //    PrintErrorMessage(testDescription, ex);
            //}

            #endregion

            Console.WriteLine("END DoAllTimingSystemDataConverterTestsAsync");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            PrintErrorMessage(testDescription, ex);

            return false;
        }

        return true;
    }

    #endregion

    #region helpers

    private static void PrintIntroMessage(string description)
    {
        Console.WriteLine(description);
    }

    private static void PrintLineItems(CompositeDataItem[] items)
    {
        if (items == null)
        {
            Console.WriteLine("Method returned null!");
        }
        else
        {
            Console.WriteLine($"Total Number of items = {items.Length}");

            for (var i = 0; i < items.Length; i++)
                Console.WriteLine(
                    $"LineItem # {i}  {items[i].String1} {items[i].String2} {items[i].String3}");
        }

        Console.WriteLine("");
    }

    private static void PrintLineItems(string[] items)
    {
        if (items == null)
        {
            Console.WriteLine("Method returned null!");
        }
        else
        {
            Console.WriteLine($"Total Number of items = {items.Length}");

            for (var i = 0; i < items.Length; i++) Console.WriteLine($"LineItem # {i}  {items[i]}");
        }

        Console.WriteLine("");
    }

    private static void PrintLineItems(RsRezultzLineItem[] items)
    {
        if (items == null)
        {
            Console.WriteLine("Method returned null!");
        }
        else
        {
            Console.WriteLine($"Total Number of items = {items.Length}");

            for (var i = 0; i < items.Length; i++)
                Console.WriteLine($"LineItem # {i}  {items[i].Title}  {items[i].AdvertisedDateTimeOfItem}");
        }

        Console.WriteLine("");
    }

    private static void PrintErrorMessage(string description, Exception ex)
    {
        Console.WriteLine();

        Console.WriteLine(
            $"Exception was thrown by method: '{description}' Message: '{JghExceptionHelpers.FindInnermostException(ex).Message}'");

        Console.WriteLine();
    }

    #endregion
}