using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace NetStd.Goodies.Mar2022
{
	/// <summary>
	/// At time of writing in Nov 2020, ASP.NET Core standardises on GZIP compression for HTTP, using the .gz extension and GZipStream.
	/// https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream?view=net-5.0
	/// 
	/// For ZIP FILE handling on the desktop use the .zip extension and System.IO.Compression.ZipArchive.
	/// https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive?view=net-5.0
	/// </summary>
    public static class JghCompression
    {
        public static async Task<byte[]> CompressAsync(byte[] bytes)
        {
            const string failure = "Unable to compress byte array.";
            const string locus = "[JghCompression.CompressAsync(byte[] bytes)]";

            MemoryStream inputStream = null;

            MemoryStream compressedOutputStream;

            try
            {
                if (bytes == null)
                    throw new ArgumentNullException(nameof(bytes));

                inputStream = new MemoryStream(bytes);

                compressedOutputStream = new MemoryStream();

                using var gzipStream = new GZipStream(compressedOutputStream, CompressionMode.Compress);

                await inputStream.CopyToAsync(gzipStream);
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw new Exception(JghString.ConcatAsSentences(ex.Message, failure, locus));
            }

            #endregion

            finally
            {
                inputStream?.Dispose();
            }

            //query - should the output stream be closed prior to return or not? if not why not?

            return compressedOutputStream.ToArray();
        }

        public static async Task<byte[]> DecompressAsync(byte[] bytes)
        {
            const string failure = "Unable to decompress byte array.";
            const string locus = "[JghCompression.DecompressAsync(byte[] bytes)]";

            MemoryStream decompressedOutputStream;

            try
            {
                if (bytes == null)
                    throw new ArgumentNullException(nameof(bytes));

                var compressedStream = new MemoryStream(bytes);

                using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
                decompressedOutputStream = new MemoryStream();
                await gzipStream.CopyToAsync(decompressedOutputStream);
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw new Exception(JghString.ConcatAsSentences(ex.Message, failure, locus));
            }

            #endregion

            return decompressedOutputStream.ToArray();
        }
    }
}