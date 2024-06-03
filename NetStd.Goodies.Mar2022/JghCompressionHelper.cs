using System.Threading.Tasks;

namespace NetStd.Goodies.Mar2022
{
    public class JghCompressionHelper
    {
        public static async Task<T> ConvertXmlAsCompressedBytesToObjectAsync<T>(byte[] myObjectAsXmlAsCompressedBytes)
    {

        var myObjectAsXmlAsBytes =
            await JghCompression.DecompressAsync(myObjectAsXmlAsCompressedBytes);

        var myObjectAsXmlString = JghConvert.ToStringFromUtf8Bytes(myObjectAsXmlAsBytes);

        var myObject = JghSerialisation.ToObjectFromXml<T>(myObjectAsXmlString, [typeof(T)]); 

        return myObject;
    }

        public static async Task<T> ConvertJsonAsCompressedBytesToObjectAsync<T>(byte[] myObjectAsJsonAsCompressedBytes)
    {

        var myObjectAsJsonAsBytes =
            await JghCompression.DecompressAsync(myObjectAsJsonAsCompressedBytes);

        var myObjectAsJsonString = JghConvert.ToStringFromUtf8Bytes(myObjectAsJsonAsBytes);

        var myObject = JghSerialisation.ToObjectFromJson<T>(myObjectAsJsonString); 

        return myObject;
    }

        public static async Task<byte[]> ConvertObjectToXmlAsCompressedBytesAsync<T>(T myObject)
    {

        var myObjectAsXmlString = JghSerialisation.ToXmlFromObject(myObject, [typeof(T)]);

        var myObjectAsXmlAsBytes = JghConvert.ToBytesUtf8FromString(myObjectAsXmlString);

        var myObjectAsXmlAsCompressedBytes = await JghCompression.CompressAsync(myObjectAsXmlAsBytes);

        return myObjectAsXmlAsCompressedBytes;
    }

        public static async Task<byte[]> ConvertObjectToJsonAsCompressedBytesAsync<T>(T myObject)
    {

        var myObjectAsJsonString = JghSerialisation.ToJsonFromObject(myObject);

        var myObjectAsJsonAsBytes = JghConvert.ToBytesUtf8FromString(myObjectAsJsonString);

        var myObjectAsJsonAsCompressedBytes = await JghCompression.CompressAsync(myObjectAsJsonAsBytes);

        return myObjectAsJsonAsCompressedBytes;
    }

    }
}