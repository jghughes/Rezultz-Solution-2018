using System;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class MoreInformationItem
    {

        #region properties

        public string Title { get; set; } = string.Empty; 

        public string Label { get; set; } = string.Empty; 

        public string EnumString { get; set; } = string.Empty; 

        public string Blurb { get; set; } = string.Empty; 

        public int DisplayRank { get; set; } 


        #endregion

        #region methods

        public static MoreInformationItem FromDataTransferObject(MoreInformationDto dto)
        {
            const string failure = "Populating MoreInformationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new MoreInformationDto();

                var answer = new MoreInformationItem
                {
                    Title = x.Title,
                    Blurb = x.Blurb,
                    //AdvertisedDateAsString = x.AdvertisedDateAsString,
                    DisplayRank = x.DisplayRank,
                    //ID = x.ID,
                    Label = x.Label,
                    EnumString = x.EnumString,
                    //Guid = string.Empty,
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

        public static MoreInformationItem[] FromDataTransferObject(MoreInformationDto[] dto)
        {
            const string failure = "Populating MoreInformationItem.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                if (dto == null)
                    return [];

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

        public static MoreInformationDto ToDataTransferObject(MoreInformationItem item)
        {
            const string failure = "Populating CboLookupItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new MoreInformationItem();

                var answer = new MoreInformationDto
                {
                    Title = x.Title,
                    Blurb = x.Blurb,
                    //AdvertisedDateAsString = x.AdvertisedDateAsString,
                    DisplayRank = x.DisplayRank,
                    //ID = x.ID,
                    Label = x.Label,
                    EnumString = x.EnumString
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

        public static MoreInformationDto[] ToDataTransferObject(MoreInformationItem[] item)
        {
            const string failure = "Populating CboLookupItemDataTransferObject.";
            const string locus = "[ToDataTransferObject]";

            try
            {
                if (item == null)
                    return [];

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

        public override string ToString()
        {
            return JghString.ConcatAsSentences(Title, Label, EnumString);
        }

        #endregion

    }
}