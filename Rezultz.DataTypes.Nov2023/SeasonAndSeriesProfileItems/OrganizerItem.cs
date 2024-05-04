using System;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    public class OrganizerItem
    {
        #region properties

        public string Title { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty; 

        #endregion

        #region methods

        public override string ToString()
        {
            return JghString.ConcatAsSentences(Label, Title);
        }

        public static OrganizerItem FromDataTransferObject(OrganizerDto dto)
        {
            const string failure = "Populating Organizer.";
            const string locus = "[FromDataTransferObject]";

            try
            {
                var x = dto ?? new OrganizerDto();

                var answer = new OrganizerItem
                {
                    //ID = x.ID,
                    Label = x.Label,
                    //EnumString = x.EnumString,
                    //Guid = string.Empty,
                    Title = x.Title,
                    //Blurb = x.Blurb,
                    //AdvertisedDateAsString = x.AdvertisedDateAsString,
                    //DisplayRank = x.DisplayRank,
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

        public static OrganizerDto ToDataTransferObject(OrganizerItem item)
        {
            const string failure = "Populating OrganizerDto.";
            const string locus = "[ToDataTransferObject]";


            try
            {
                var x = item ?? new OrganizerItem();

                var answer = new OrganizerDto
                {
                    //ID = x.ID,
                    Label = x.Label,
                    //EnumString = x.EnumString,
                    Title = x.Title,
                    //Blurb = x.Blurb,
                    //AdvertisedDateAsString = x.AdvertisedDateAsString,
                    //DisplayRank = x.DisplayRank,
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

        #endregion
    }
}