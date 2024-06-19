using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using Newtonsoft.Json;

// ReSharper disable IdentifierTypo

namespace NetStd.Goodies.Mar2022
{
    /// <summary>
    ///     Provides methods for serialising and deserialising Json and XML. The classes employed are :
    ///     - Newtonsoft.Json
    ///     - System.Runtime.Serialization.DataContractSerializer
    ///     The Json serialisers are recommended in preference to the xml serialiser in all applications because they
    ///     are more robust and will default intelligently in edge-conditions where the xml serialiser will throw an exception.
    ///		This is strongly true for deserialisation. You have been warned.
    /// </summary>
    public static class JghSerialisation
    {
        private const string Locus2 = nameof(JghSerialisation);
        private const string Locus3 = "[NetStd.Goodies.Mar2022]";

		#region Json - Newtonsoft.Json

		/// <summary>
		///     Returns Json.
		///     Returns null as Json if object itself is null.
		///     Uses Newtonsoft.Json
		/// </summary>
		/// <param name="inputObject"></param>
		/// <returns></returns>
		public static string ToJsonFromObject(object inputObject)
		{
			var failure = "Unable to serialise object to JSON.";
			const string locus = "[ToJsonFromObject]";

			try
			{
				if (inputObject is null)
					return JsonConvert.SerializeObject(null, MakeSerialiserSettings());

				var answer = JsonConvert.SerializeObject(inputObject, MakeSerialiserSettings());

				return answer;
			}
			catch (Exception ex)
			{
				failure = JghString.ConcatAsSentences(failure, locus, Locus2, Locus3,
					$"The type of object for serialisation to JSON is [{inputObject?.GetType()}].", ex.Message);

				var exx = new FormatException(failure);

				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);
			}
		}

		/// <summary>
		///     Uses Newtonsoft.Json
		///     Returns default(T) if input string is null or empty.
		///     Returns null if Newtonsoft fails deserialisation.
		///     Cannot handle Dictionaries in UWP.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static T ToObjectFromJson<T>(string inputString)
		{
			var failure = "Unable to deserialise JSON to object.";
			const string locus = "[ToObjectFromJson]";

			try
			{
				if (string.IsNullOrWhiteSpace(inputString))
				{
					var defaultAnswer = default(T);

					return defaultAnswer;
				}

				T answer = JsonConvert.DeserializeObject<T>(inputString, MakeSerialiserSettings());
				// bizarre! During testing i empirically determined that this method can handle a Dictionary (or Dictionary somewhere in the tree) when running in a WPF app, but not when running in a UWP app! 

				return answer;
			}
			catch (Exception ex)
			{
				failure = JghString.ConcatAsParagraphs(failure, locus, Locus2, Locus3, $"The target type of object is [{typeof(T)}].", JghExceptionHelpers.FindInnermostException(ex).Message,
					$"The input string for deserialisation from JSON is [{inputString}].");

				var exx = new FormatException(failure);

				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);
			}
		}

		private static JsonSerializerSettings MakeSerialiserSettings()
		{
			// never ever change any of these settings. these are cast in stone. full stop.
			// this serialiser is used on the server side in a multitude of REST services. it's used on the client side of REST services.
			// it's used on the server side of wcf services. it's used on the client side of wcf services.
			// it's used for storing and retrieving long standing settings.
			// it's used in my WCF apps, UWP apps, and Xamarin apps.
			// change will lead to disaster. guaranteed.

			var serializerSettings = new JsonSerializerSettings
			{
				//NullValueHandling = NullValueHandling.Ignore,
				//ContractResolver = new CamelCasePropertyNamesContractResolver(), // DON'T DO THIS. IT REDUCES THE INITIAL CAPITAL LETTER OF A PROPERTY NAME TO LOWER CASE FOR A DICTIONARY KEY, THUS DEFEATING A ROUNDTRIP
				//DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				//Formatting = Formatting.Indented, // never do this! messes up serialisation/deserialisation in REST services
			};

			return serializerSettings;
		}

		#endregion

        #region xml - System.Runtime.Serialization.DataContractSerializer

        private const string XmlFormatErrorMsg =
	        "Unable to interpret data as Xml. Among other reasons, this can occur if the data is not in xml format or if the xml text contains one or more format errors.";

        /// <summary>
        ///     Returns empty XElmemt entitled 'null' if object itself is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputObject"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
        public static XElement ToXElementFromObject<T>(T inputObject, Type[] knownTypes)
        {
	        const string failure =
		        "Unable to serialise object as xml XElement using System.Runtime.Serialization.DataContractSerializer.";
	        const string locus = "[ToXElementFromObject]";

	        try
	        {
		        if (inputObject is null) return new XElement("null");
		        //if (inputObject is null) throw new ArgumentNullException(nameof(inputObject));

		        XElement xElement;

		        var serialiser = new DataContractSerializer(typeof(T), knownTypes);

		        using var memstream = new MemoryStream();

		        serialiser.WriteObject(memstream, inputObject);

		        memstream.Position = 0;

		        try
		        {
			        xElement = XElement.Load(memstream, LoadOptions.PreserveWhitespace);

			        var attributes = xElement.Attributes().Where(z => z.IsNamespaceDeclaration);

			        attributes.Remove();

		        }
		        catch (Exception)
		        {
			        throw new Exception(XmlFormatErrorMsg);
		        }

		        return xElement;
	        }
	        catch (Exception ex)
	        {
		        throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
	        }
        }

        /// <summary>
        ///     Returns empty XElmemt entitled 'null' if object itself is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputObject"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
        public static XElement ToXElementWithoutWhiteSpaceFromObject<T>(T inputObject, Type[] knownTypes)
        {
	        const string failure =
		        "Unable to serialise object as xml XElement using System.Runtime.Serialization.DataContractSerializer.";
	        const string locus = "[ToXElementFromObject]";

	        try
	        {
		        if (inputObject is null) return new XElement("null");
		        //if (inputObject is null) throw new ArgumentNullException(nameof(inputObject));

		        XElement xElement;

		        var serialiser = new DataContractSerializer(typeof(T), knownTypes);

		        using var memstream = new MemoryStream();

		        serialiser.WriteObject(memstream, inputObject);

		        memstream.Position = 0;

		        try
		        {
			        xElement = XElement.Load(memstream, LoadOptions.None);

			        var attributes = xElement.Attributes().Where(z => z.IsNamespaceDeclaration);

			        attributes.Remove();

		        }
		        catch (Exception)
		        {
			        throw new Exception(XmlFormatErrorMsg);
		        }

		        return xElement;
	        }
	        catch (Exception ex)
	        {
		        throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
	        }
        }

		public static string ToXmlFromObject<T>(T inputObject, Type[] knownTypes)
		{
			const string failure =
				"Unable to serialise object to string using System.Runtime.Serialization.DataContractSerializer.";
			const string locus = "[ToXmlFromObject]";

			try
			{
				if (inputObject is null) return string.Empty;
				//if (inputObject is null) throw new ArgumentNullException(nameof(inputObject));

				var xx = ToXElementFromObject(inputObject, knownTypes);

				return xx.ToString(SaveOptions.None);
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}
		}

		/// <summary>
		///     Returns default(T) if input string is null or empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inputString"></param>
		/// <param name="knownTypes"></param>
		/// <returns></returns>
		public static T ToObjectFromXml<T>(string inputString, Type[] knownTypes)
		{
			const string failure =
				"Unable to de-serialise string back into object using System.Runtime.Serialization.DataContractSerializer";
			const string locus = "[ToObjectFromXml]";

			try
			{
				if (string.IsNullOrWhiteSpace(inputString)) return default;
				//if (inputString is null) throw new ArgumentNullException(nameof(inputString));

				var inputAsByteArray = JghConvert.ToBytesUtf8FromString(inputString);

                var mySerializer = new DataContractSerializer(typeof(T), knownTypes);

                using var memStream = new MemoryStream(inputAsByteArray);

                memStream.Position = 0;

                var answerAsGenericObject = (T)mySerializer.ReadObject(memStream);

                return answerAsGenericObject;
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}
		}

		/// <summary>
		///     Returns default(T) if input string is null or empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inputXElement"></param>
		/// <param name="knownTypes"></param>
		/// <returns></returns>
		public static T ToObjectFromXml<T>(XElement inputXElement, Type[] knownTypes)
		{
			const string failure =
				"Unable to deserialise XML data back into object using System.Runtime.Serialization.DataContractSerializer.";
			const string locus = "[ToObjectFromXml]";

			try
			{
				if (inputXElement is null) return default;

				var serialiser = new DataContractSerializer(typeof(T), knownTypes);

                using Stream stream = new MemoryStream();

                var bytes = JghConvert.ToBytesUtf8FromString(inputXElement.ToString());

                stream.Write(bytes, 0, bytes.Length);

                stream.Position = 0;

                var answerAsGenericObject = (T)serialiser.ReadObject(stream);

                return answerAsGenericObject;
			}
			catch (Exception ex)
			{
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
			}
		}

		#endregion

	}

}