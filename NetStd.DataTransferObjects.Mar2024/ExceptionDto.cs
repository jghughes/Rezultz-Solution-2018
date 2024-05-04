using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace NetStd.DataTransferObjects.Mar2024;

[DataContract(Namespace = "", Name = XeException)]
public class ExceptionDto
{
    #region methods

    public static ExceptionDto FromException(Exception ex)
    {
        try
        {
            if (ex == null)
                return null; // important to return null here, in order to make the recursion work and not loop infinitely

            var answer = new ExceptionDto
            {
                ClassName = ex.GetType().ToString(),
                Message = ex.Message,
                InnerException = FromException(ex.InnerException),
                StackTraceString = ex.StackTrace,
                HResult = ex.HResult,
                Source = ex.Source
            };

            var data = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in ex.Data)
            {
                // Convert the key and value to strings

                var key = entry.Key.ToString();

                var value = ConvertLikelyObjectToString(entry.Value);
                //var value = entry.Value?.ToString(); //for complex types, you might need a more sophisticated approach that can handle serialisation of arbitrary types (some of which may not be serialisable)

                data.Add(key, value);
            }

            answer.Data = data;

            return answer;
        }

        #region trycatch

        catch (Exception e)
        {
            return new ExceptionDto
            {
                ClassName = e.GetType().ToString(),
                Message = $"An error occurred while trying to populate the SerialisableExceptionDto. The error was: {e.Message}",
                StackTraceString = e.StackTrace,
                Source = e.Source
            };
        }

        #endregion
    }

    #endregion

    #region helpers

    private static string ConvertLikelyObjectToString(object value)
    //private string ConvertLikelyObjectToString(object value, CultureInfo cultureInfo)
    {
        switch (value)
        {
            case null:
                return "";
            case Enum:
                {
                    var name = Enum.GetName(value.GetType(), value);

                    if (name != null)
                    {
                        var field = value.GetType().GetTypeInfo().GetDeclaredField(name);

                        if (field != null)
                            if (field.GetCustomAttribute(typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute)
                                return attribute.Value ?? name;

                        var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
                        //var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));

                        return converted;
                    }

                    break;
                }
            case bool booleanValue:
                return Convert.ToString(booleanValue).ToLowerInvariant();
            //return Convert.ToString(booleanValue, cultureInfo).ToLowerInvariant();
            case byte[] byteArray:
                return Convert.ToBase64String(byteArray);
            case string[] stringArray:
                return string.Join(",", stringArray);
            default:
                {
                    // this is not a default case, but it is the last case in the switch statement
                    if (value.GetType().IsArray)
                    {
                        var valueArray = (Array)value;
                        var valueTextArray = new string[valueArray.Length];
                        for (var i = 0; i < valueArray.Length; i++) valueTextArray[i] = ConvertLikelyObjectToString(valueArray.GetValue(i));
                        //for (var i = 0; i < valueArray.Length; i++) valueTextArray[i] = ConvertLikelyObjectToString(valueArray.GetValue(i), cultureInfo);
                        return string.Join(",", valueTextArray);
                    }

                    break;
                }
        }

        var result = Convert.ToString(value);
        //var result = Convert.ToString(value, cultureInfo);

        return result;
    }

    #endregion

    #region Names

    private const string XeException = "exception";
    private const string XeClassName = "class-name";
    private const string XeMessage = "message";
    private const string XeHResult = "hresult";
    private const string XeSource = "source";
    private const string XeData = "data";
    private const string XeStackTraceString = "stacktrace-string";
    private const string XeInnerException = "inner-exception";

    #endregion

    #region DataMembers

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1, Name = XeClassName)]
    //[JsonProperty(XeClassName, Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public string ClassName { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2, Name = XeMessage)]
    public string Message { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3, Name = XeHResult)]
    public int HResult { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4, Name = XeSource)]
    public string Source { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5, Name = XeData)]
    public Dictionary<string, string> Data { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6, Name = XeStackTraceString)]
    public string StackTraceString { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7, Name = XeInnerException)]
    public ExceptionDto InnerException { get; set; }

    #endregion
}