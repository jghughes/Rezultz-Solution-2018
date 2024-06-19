using System;
using System.IO;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;

namespace NetStd.Goodies.Mar2022
{
    /// <summary>
    ///     Targeted for all platforms
    /// </summary>
    public static class JghStreamHandlers
    {
        private const string Locus2 = nameof(JghStreamHandlers);
        private const string Locus3 = "[NetStd.Goodies.Mar2022]";

        /// <summary>
        ///     Copies Stream into byte array
        /// </summary>
        /// <param name="inputStream">Any kind of stream</param>
        public static async Task<byte[]> ReadStreamToBytesAsync(Stream inputStream)
        {
            const string failure = "Unable to read stream into array.";
            const string locus = "[ReadStreamToBytesAsync]";

            try
            {
                if (inputStream is null) return [];

                using (inputStream)
                using (var memorystream = new MemoryStream())
                {
                    // belt and braces. i have emprically dertermined that this reset is necessary in some circumstances
                    // system.Io.Streams are not seekable. but memory streams are

                    if (inputStream.CanSeek)
                        inputStream.Position = 0;

                    await inputStream.CopyToAsync(memorystream);
                    //inputStream.CopyTo(memorystream);

                    return memorystream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        /// <summary>
        ///     Reads Stream into text. Defaults to UTF-8.
        ///     Not thread safe;
        /// </summary>
        /// <param name="inputStream">Any kind of stream</param>
        public static async Task<string> ReadStreamToStringAsync(Stream inputStream)
        {
            const string failure = "Unable to read stream into text.";
            const string locus = "[ReadStreamToStringAsync]";

            try
            {
                if (inputStream is null) return string.Empty;

                using (inputStream)

                using (var memstream = new MemoryStream())
                {
                    // belt and braces. i have empirically determined that this reset is necessary in some circumstances
                    // system.Io.Streams are not seekable. but memory streams are
                    if (inputStream.CanSeek)
                        inputStream.Position = 0;

                    await inputStream.CopyToAsync(memstream);

                    memstream.Position = 0;

                    using var reader = new StreamReader(memstream);

                    var answer = await reader.ReadToEndAsync();

                    return answer;
                }
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }
    }
}