using System;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.ValidationViewModels;

namespace Rezultz.Library02.Mar2024.PageViewModels
{
    public class SeriesPopulationCohortsPageViewModel : BasePopulationCohortsStylePageViewModel
    {
        #region ctor

        public SeriesPopulationCohortsPageViewModel(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
            ISessionState sessionState, ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationVm)
            : base(leaderboardResultsSvcAgent, sessionState, globalSeasonProfileAndIdentityValidationVm)
        {
        }

        #endregion

        #region OnPageLoaded method

        // nothing. use Base

        #endregion

        #region helpers

        protected override string MakeEventTitle()
        {
            return string.Empty;
        }

        protected override string[] PopulateTableHeadings()
        {
            var headings = new[]
            {
                MakeSeriesLabel(), MakeMoreInfoLabel()
            };

            return headings;
        }

        protected override MoreInformationItem PopulateDataGridTitleAndBlurb()
        {
            var answer = new MoreInformationItem
            {
                Title = "Cohorts",
                Blurb = string.Empty
            };

            return answer;
        }

        protected override async Task<IHasCohorts> ObtainRepositoryWithCohortsAsync(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent, SeriesProfileItem seriesProfileItem, EventProfileItem eventProfileItem)
        {
            IRepositoryOfSeriesStandings repositoryOfResults = new RepositoryOfSeriesStandings(leaderboardResultsSvcAgent);

            try
            {
                var locationOfData = seriesProfileItem.ContainerForResultsDataFilesPublished;

                await repositoryOfResults.LoadRepositoryOfSequenceContainersAsync(locationOfData.AccountName, locationOfData.ContainerName, seriesProfileItem);
                // Loads the repository of series totals failing SILENTLY.be sure to verify internet connection and service availability beforehand
            }

            #region try catch

            catch (Exception ex)
            {
                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                {
                    // do nothing. no option but to deem this outcome successful. wish we didn't have to suppress. but we have no other option until a threadsafe, failsafe alert messaging system is invented that doesn't entail exception throwing. which is nowhere on the horizon
                }
                else
                {
                    throw;
                }
            }

            #endregion

            return repositoryOfResults;
        }

        #endregion
    }
}