using System;
using System.Globalization;
using System.Linq;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;

namespace NetStd.DataTypes.Mar2024
{
    public class UriItem
    {
        #region properties

        public string SourceUriString { get; set; } = string.Empty;
        public string ReferenceUriString { get; set; } = "https://dummy"; // this is mandatory. don't delete it. critical for newing up a uri string
        public string BlobName { get; set; } = string.Empty;
        public int DisplayRank { get; set; }

        #endregion

        #region methods

        public override string ToString()
        {
            return JghString.ConcatAsSentences(BlobName, ReferenceUriString, SourceUriString, DisplayRank.ToString(CultureInfo.InvariantCulture));
        }

        public static UriItem FromDataTransferObject(UriDto dataTransferObject)
        {
            const string failure = "Populating UriItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dataTransferObject ?? new UriDto();

                var answer = new UriItem
                {
                    BlobName = x.BlobName,
                    DisplayRank = x.DisplayRank,
                    SourceUriString = x.SourceUriString,
                    ReferenceUriString = x.ReferenceUriString,
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
        public static UriItem[] FromDataTransferObject(UriDto[] dataTransferObject)
        {
            const string failure = "Populating UriItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                if (dataTransferObject == null)
                    return Array.Empty<UriItem>();

                var answer = dataTransferObject.Select(FromDataTransferObject).Where(z => z != null).ToArray();
                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static UriDto ToDataTransferObject(UriItem item)
        {
            const string failure = "Populating UriItemDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new UriItem();

                var answer = new UriDto
                {
                    DisplayRank = x.DisplayRank,
                    SourceUriString = x.SourceUriString,
                    ReferenceUriString = x.ReferenceUriString,
                    BlobName = x.BlobName
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
        public static UriDto[] ToDataTransferObject(UriItem[] item)
        {
            const string failure = "Populating UriItemDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                if (item == null)
                    return Array.Empty<UriDto>();

                var answer = item.Select(ToDataTransferObject).Where(z => z != null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        #endregion
    }
}