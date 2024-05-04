using System;
using System.Collections.Generic;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
    public interface IPointsForEventCalculator
    {
        Dictionary<Tuple<string, string, string, string>, double> CalculatePointsForAllCompetitors(
	        ResultItem[] allFinisherResults, EventProfileItem eventProfileToWhichThisRepositoryBelongs);
    }
}