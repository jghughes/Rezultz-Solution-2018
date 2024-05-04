using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;

namespace NetStd.DataTypes.Mar2024;

public class JghSerialisableException
{
    #region props

    public string ClassName { get; set; }

    public string Message { get; set; }

    public JghSerialisableException InnerException { get; set; }

    public string StackTraceString { get; set; }

    public int HResult { get; set; }

    public string Source { get; set; }

    public Dictionary<string, string> Data { get; set; }

    public string PrettyPrintedSummary
    {
        get
        {
            var theList = new List<string>();

            if (!string.IsNullOrWhiteSpace(Message))
                theList.Add($"Message: {Message}");

            if (!string.IsNullOrWhiteSpace(ClassName))
                theList.Add($"ClassName: {ClassName}");

            if (!string.IsNullOrWhiteSpace(StackTraceString))
                theList.Add($"StackTrace: {StackTraceString}");

            if (HResult != 0)
                theList.Add($"HResult: {HResult}");

            if (!string.IsNullOrWhiteSpace(Source))
                theList.Add($"Source: {Source}");

            if (Data != null)
            {
                theList.Add("Data:");

                theList.AddRange(Data.ToArray().Select(kvp => $"DataItem: {kvp.Key}={kvp.Value}"));
            }

            if (InnerException != null)
            {
                theList.Add("");
                theList.Add($"InnerException:");
                theList.Add($"{InnerException.PrettyPrintedSummary}");
                theList.Add("");
            }

            return JghString.ConcatAsParagraphs(theList.ToArray());
        }
    }

    #endregion

    #region ctor

    public JghSerialisableException()
    {
    }

    public JghSerialisableException(Exception ex)
    {
        ClassName = ex.GetType().ToString();
        Message = ex.Message;
        InnerException = ex.InnerException == null ? null : new JghSerialisableException(ex.InnerException);
        StackTraceString = ex.StackTrace;
    }

    #endregion

    #region methods

    public static JghSerialisableException FromDataTransferObject(ExceptionDto dto)
    {
        const string failure = "Populating SerialisableExceptionItem.";
        const string locus = "[FromDataTransferObject]";

        try
        {
            if (dto == null)
                return null; // important to return null here, not a new instance, in order to make the recursion work properly

            var answer = new JghSerialisableException
            {
                ClassName = dto.ClassName,
                Message = dto.Message,
                InnerException = FromDataTransferObject(dto.InnerException),
                StackTraceString = dto.StackTraceString,
                HResult = dto.HResult,
                Source = dto.Source,
                Data = dto.Data
            };

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    public static JghSerialisableException[] FromDataTransferObject(ExceptionDto[] dto)
    {
        const string failure = "Populating SerialisableExceptionItem[].";
        const string locus = "[FromDataTransferObject]";

        try
        {
            if (dto == null)
                return Array.Empty<JghSerialisableException>();

            var answer = dto.Select(FromDataTransferObject).Where(z => z != null).ToArray();

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        }

        #endregion
    }

    public static JghSerialisableException FindInnermostException(JghSerialisableException ex)
    {
        if (ex == null)
            return null;

        var e = ex;

        while (e.InnerException != null)
            e = e.InnerException;

        return e;
    }

    public override string ToString()
    {
        var theList = new List<string>
        {
            $"Message: {Message}",
            $"ClassName: {ClassName}",
            $"StackTrace: {StackTraceString}",
            $"HResult: {HResult}",
            $"Source: {Source}",
            InnerException == null ? "InnerException: null" : $"InnerException: {InnerException}"
        };

        if (Data != null)
        {
            theList.Add("Data:");

            theList.AddRange(Data.ToArray().Select(kvp => $"DataItem: {kvp.Key}={kvp.Value}"));
        }

        theList.Add(InnerException == null ? "InnerException: null" : $"InnerException: {InnerException}");

        return JghString.ConcatAsParagraphs(theList.ToArray());
    }

    #endregion
}