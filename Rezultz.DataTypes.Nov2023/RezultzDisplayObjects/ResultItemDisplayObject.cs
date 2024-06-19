using System;
using System.Collections.Generic;
using System.Linq;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

// ReSharper disable InconsistentNaming

namespace Rezultz.DataTypes.Nov2023.RezultzDisplayObjects
{
    public class ResultItemDisplayObject : BindableBase, IHasRace, IHasGender, IHasAgeGroup, IHasCity, IHasUtilityClassification, IHasTeam,
        IHasFirstName, IHasLastName, IHasMiddleInitial, IHasFullName, IHasBib, IHasCollectionLineItemPropertiesV2
    {
        public const string XeBib= "bib";
        public const string XeFullName = "fullname";
        public const string XePlaceOverall = "place-overall";
        public const string XeTotalDuration = "total-duration"; 
        public const string XeGapTimespanString = "gap"; 
        public const string XeGapCxStyle = "gap-cxStyle"; 
        public const string XeFractionalPlaceInRaceInNumeratorOverDenominatorFormat = "fractional-points-in-race"; 
        public const string XeFractionalPlaceBySexInNumeratorOverDenominatorFormat = "fractional-points-in-sex";
        public const string XeFractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat = "fractional-points-in-sex-and-age-group"; 
        public const string XePointsCalculated = "points-calculated"; 


        #region field

        private ResultItem _sourceItem;

        #endregion

        #region properties

        //Note:  do not under any circumstances refactor any property names without identically refactoring the magic strings in MakeDataGridColumnSpecificationsForResultsLeaderboard() here below

        #region features of a person

        public string Bib { get; set; } = string.Empty;

        public string Rfid { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string MiddleInitial { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => JghString.ConcatAsSentences(FirstName, MiddleInitial, LastName);

        public Tuple<string, string, string, string> Identifier => new(LastName, FirstName, MiddleInitial, Bib); // used as equality comparer

        public string Gender { get; set; } = string.Empty;

        public string Age { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string AgeGroup { get; set; } = string.Empty;

        public string UtilityClassification { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;

        public string RaceGroup { get; set; } = string.Empty; // emanates from what we call Race in the Rezultz timing module but RaceSpecificationItem prior to 2019

        public string DnxString { get; set; } = string.Empty;

        public string T01 { get; set; } = string.Empty;

        public string T02 { get; set; } = string.Empty;

        public string T03 { get; set; } = string.Empty;

        public string T04 { get; set; } = string.Empty;

        public string T05 { get; set; } = string.Empty;

        public string T06 { get; set; } = string.Empty;

        public string T07 { get; set; } = string.Empty;

        public string T08 { get; set; } = string.Empty;

        public string T09 { get; set; } = string.Empty;

        public string T10 { get; set; } = string.Empty;

        public string T11 { get; set; } = string.Empty;

        public string T12 { get; set; } = string.Empty;

        public string T13 { get; set; } = string.Empty;

        public string T14 { get; set; } = string.Empty;

        public string T15 { get; set; } = string.Empty;

        public string Guid { get; set; } = string.Empty;

        public int ID { get; set; }

        public string Label { get; set; } = string.Empty;

        public string EnumString { get; set; } = string.Empty;

        #endregion

        #region mapped across from derived data

        public string PlaceCalculatedOverallAsString { get; set; }

        public string TotalDurationInDisplayFormat { get; set; } = string.Empty;

        public string SpeedKph { get; set; } = string.Empty;

        public string GapTimespanString { get; set; } = string.Empty;

        public string SplitsBehindWinnerOfRace { get; set; } = string.Empty;

        public string CalculatedNumOfSplitsCompleted { get; set; }

        public string GapCxTimespanThenSplitsString { get; set; } = string.Empty;

        public string FractionalPlaceInRaceInNumeratorOverDenominatorFormat { get; set; } = string.Empty;

        public string FractionalPlaceBySexInNumeratorOverDenominatorFormat { get; set; } = string.Empty;

        public string FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat { get; set; } = string.Empty;

        public string PointsCalculatedAsString { get; set; } = string.Empty;

        #endregion

        #region attribute IDs & calculated fields, used for filtering and ordering, not display columns

        public double PointsCalculatedRank { get; set; }

        public double TotalDurationFromAlgorithmInSecondsRank { get; set; }

        #endregion

        #endregion

        #region methods

        public static ResultItemDisplayObject FromModel(ResultItem model)
        {
            const string failure = "Populating ResultItemViewModel.";
            const string locus = "[FromModel]";

            const int decimalRoundingPlaces = 1;
            const string decimalPlacesFormatSpecifier = JghFormatSpecifiers.DecimalFormat1Dp;
            const string roundedSecondsAsTimeSpanFormat = JghTimeSpan.DdhhmmssfPattern;

            try
            {
                model ??= new ResultItem();

                var displayObject = new ResultItemDisplayObject
                {
                    ID = model.ID,
                    Rfid = model.Rfid,
                    Bib = JghString.RightAlign(JghString.TmLr(model.Bib), 4, ' '),
                    FirstName = JghString.TmLr(model.FirstName),
                    LastName = JghString.TmLr(model.LastName),
                    MiddleInitial = JghString.TmLr(model.MiddleInitial),
                    Gender = JghString.TmLr(model.Gender),
                    Age = JghString.PadLeftOrBlankIfZero(model.Age, 4, ' '),
                    AgeGroup = JghString.TmLr(model.AgeGroup),
                    City = JghString.TmLr(model.City),
                    Team = JghString.TmLr(model.Team),
                    RaceGroup = JghString.TmLr(model.RaceGroup),
                    UtilityClassification = JghString.TmLr(model.UtilityClassification), // maybe this shouldn't be here?
                    DnxString = model.DnxString,
                    Guid = model.Guid,
                    _sourceItem = model
                };


                #region the Txx fields can contain anything as a string - integers, doubles, timespans, strings and so forth depending on whether result or points or tour duration or what have you. we can't make oversimple assumptions about formatting them here hence the formatting method we use

                displayObject.T01 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T01, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T02 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T02, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T03 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T03, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T04 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T04, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T05 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T05, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T06 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T06, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T07 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T07, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T08 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T08, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T09 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T09, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T10 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T10, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T11 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T11, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T12 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T12, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T13 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T13, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T14 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T14, decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                displayObject.T15 = JghString.ToBestGuessStringOrBlankIfNearZero(model.T15, decimalRoundingPlaces, decimalPlacesFormatSpecifier);

                #endregion

                if (model.DerivedData is null) return displayObject;

                #region format fields that will be populated unconditionally from derived data

                //viewModel.IsValidDnx = model.DerivedData.IsValidDnx;

                displayObject.DnxString = model.DerivedData.DnxStringFromAlgorithm;

                //viewModel.IsValidDuration = model.DerivedData.IsValidDuration;

                displayObject.CalculatedNumOfSplitsCompleted = JghString.PadLeftOrBlankIfZero(model.DerivedData.CalculatedNumOfSplitsCompleted, 4, ' ');

                #endregion

                #region handle TotalDurationInDisplayFormat according to whether Result is Dnx and whether is or is not valid duration

                // initialise TotalDurationInDisplayFormat with a blank or with the value

                if (!model.DerivedData.IsValidDuration)
                {
                    //viewModel.TotalDurationFromAlgorithmInSeconds = 0.0;

                    displayObject.TotalDurationInDisplayFormat = string.Empty;
                }
                else
                {
                    //src.TotalDurationFromAlgorithmInSeconds = Math.Round(src.DerivedData.TotalDurationFromAlgorithmInSeconds, 2);

                    var theTimeSpan = JghTimeSpan.ToTimeSpanFromSeconds(model.DerivedData.TotalDurationFromAlgorithmInSeconds, decimalRoundingPlaces);

                    displayObject.TotalDurationInDisplayFormat = JghString.ToBestGuessStringOrBlankIfNearZero(theTimeSpan.ToString(), decimalRoundingPlaces, decimalPlacesFormatSpecifier);
                }

                // overwrite if Dnx. a valid Dnx trumps whatever is in the TotalDurationInDisplayFormat

                if (model.DerivedData.IsValidDnx) displayObject.TotalDurationInDisplayFormat = model.DerivedData?.DnxStringFromAlgorithm?.ToUpper();

                #endregion

                #region handle fields in derived data that subtend from IsValidDuration = true. but only do this if not a dnx

                // as long as it's not a Dnx, if it's a valid duration it is possible to
                // calculate and in turn display all the calculated fields - add them in.  

                if (model.DerivedData is not null && model.DerivedData.IsValidDuration & !model.DerivedData.IsValidDnx)
                {
                    displayObject.PlaceCalculatedOverallAsString = JghString.PadLeftOrBlankIfZero(model.DerivedData.PlaceCalculatedOverallInt, 5, ' ');
                    //viewModel.PlaceCalculatedOverallAsString = JghConvert.ToRightAlignedOrBlank(model.DerivedData.PlaceCalculatedOverallInt);

                    displayObject.SpeedKph = JghString.RightAlign(Math.Round(model.DerivedData.SpeedKph, 1).ToString("N1"), 5, ' ');

                    displayObject.FractionalPlaceInRaceInNumeratorOverDenominatorFormat =
                        JghString.RightAlign(FormatRelativeRankInSubsetAsNumeratorOverDenominator(model.DerivedData.TotalFinishersInRace, model.DerivedData.PlaceCalculatedOverallInt), 9, ' ');

                    displayObject.FractionalPlaceBySexInNumeratorOverDenominatorFormat =
                        JghString.RightAlign(FormatRelativeRankInSubsetAsNumeratorOverDenominator(model.DerivedData.TotalFinishersInSubsetOfSexWithinRace, model.DerivedData.CalculatedRankInSubsetOfSexWithinRace), 9, ' ');

                    displayObject.FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat =
                        JghString.RightAlign(FormatRelativeRankInSubsetAsNumeratorOverDenominator(model.DerivedData.TotalFinishersInSubsetOfAgeGroupWithinSexWithinRace, model.DerivedData.CalculatedRankInSubsetOfAgeGroupWithinSexWithinRace), 9,
                            ' ');

                    displayObject.PointsCalculatedAsString = JghString.RightAlign(Math.Round(model.DerivedData.PointsCalculated, 1).ToString("N1"), 7, ' ');

                    displayObject.GapTimespanString = JghTimeSpan.ToDurationOrBlankIfNearZero(JghTimeSpan.ToTimeSpanFromSeconds(model.DerivedData.TimeGapBehindWinnerOfRaceInSeconds, decimalRoundingPlaces), decimalRoundingPlaces,
                        roundedSecondsAsTimeSpanFormat);

                    var xx = JghString.ToBestGuessStringOrBlankIfNearZero(JghTimeSpan.ToTimeSpanFromSeconds(model.DerivedData.TimeGapBehindWinnerOfRaceInSeconds, decimalRoundingPlaces).ToString(), decimalRoundingPlaces,
                        decimalPlacesFormatSpecifier);

                    displayObject.GapTimespanString = xx;

                    displayObject.GapCxTimespanThenSplitsString = FormatGapCxTimespanThenSplitsString(model.DerivedData.SplitsBehindWinnerOfRace, xx);

                    displayObject.SplitsBehindWinnerOfRace = JghString.ToStringOrBlankIfZero(model.DerivedData.SplitsBehindWinnerOfRace);
                }

                #endregion

                return displayObject;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static ResultItemDisplayObject[] FromModel(ResultItem[] model)
        {
            const string failure = "Populating ResultItemViewModel.";
            const string locus = "[FromModel]";

            try
            {
                if (model is null)
                    return [];

                var answer = model.Select(FromModel).Where(z => z is not null).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static ResultItem ObtainSourceItem(ResultItemDisplayObject displayObject)
        {
            return displayObject?._sourceItem;
        }

        public static ResultItemDisplayObject FromItemToSeriesPointsFormat(SequenceContainerItem sequenceContainerItem, int scoresToCountForPoints)
        {
            const string failure = "Mapping cumulative individual points.";

            const string locus = "[FromItemToSeriesPointsFormat]";

            if (sequenceContainerItem is null) return null;

            const int roundDecimalTo0Place = 0;
            const int roundDecimalTo1Place = 1;

            try
            {
                var viewModel = FromModel(sequenceContainerItem.MostRecentResultItemToWhichThisSequenceApplies); // clever trick to copy over the bulk of fields other than those unique to the sequence container
                //var viewModel = ResultItem.DataGridRowDisplayFormat(sequenceContainerItem.Result); // clever trick to copy over the bulk of fields other than those unique to the sequence container

                // overwrite pertinent fields with SequenceContainerItem data

                viewModel.PlaceCalculatedOverallAsString = JghString.PadLeftOrBlankIfZero(sequenceContainerItem.SequenceTotalRankInt, 5, ' ');

                viewModel.FractionalPlaceInRaceInNumeratorOverDenominatorFormat = sequenceContainerItem.FractionalRankInRaceInNumeratorOverDenominatorFormat;
                viewModel.FractionalPlaceBySexInNumeratorOverDenominatorFormat = sequenceContainerItem.FractionalRankBySexInNumeratorOverDenominatorFormat;
                viewModel.FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat = sequenceContainerItem.FractionalRankBySexPlusCategoryInNumeratorOverDenominatorFormat;
                viewModel.PointsCalculatedAsString = JghString.RightAlign(Math.Round(sequenceContainerItem.SequenceTotal, roundDecimalTo1Place).ToString("N1"), 7, ' ');
                //viewModel.PointsCalculatedAsString = JghString.ToStringOrBlankIfNearZero(sequenceContainerItem.SequenceTotal, roundDecimalTo1Place, JghFormatSpecifiers.DecimalFormat1Dp);
                viewModel.PointsCalculatedRank = sequenceContainerItem.SequenceTotal; // we need this field later on. they come in handy for reordering things in the RezultzPublisher and Leaderboard and Favorites datagrids etc

                var pointsDictionary = AddFlagToNhighestDatapoints(sequenceContainerItem.SequenceOfSourceData, scoresToCountForPoints);

                static string PrettyPoints(Dictionary<int, PointsDataPoint> pointsDictionary, int eventNum)
                {
                    // NB the Sequence of source data is a 0 based dictionary. so event number 1 in fact has a key of zero in the dictionary. this is handled in the GetDataPoint method inside this method
                    return GetDataPointAsTypeDoubleAsStringOrBlankIfNearZeroOrBracketedIfNotOneOfTheBest(pointsDictionary, eventNum, roundDecimalTo0Place);
                }

                viewModel.T01 = PrettyPoints(pointsDictionary, 1);
                viewModel.T02 = PrettyPoints(pointsDictionary, 2);
                viewModel.T03 = PrettyPoints(pointsDictionary, 3);
                viewModel.T04 = PrettyPoints(pointsDictionary, 4);
                viewModel.T05 = PrettyPoints(pointsDictionary, 5);
                viewModel.T06 = PrettyPoints(pointsDictionary, 6);
                viewModel.T07 = PrettyPoints(pointsDictionary, 7);
                viewModel.T08 = PrettyPoints(pointsDictionary, 8);
                viewModel.T09 = PrettyPoints(pointsDictionary, 9);
                viewModel.T10 = PrettyPoints(pointsDictionary, 10);
                viewModel.T11 = PrettyPoints(pointsDictionary, 11);
                viewModel.T12 = PrettyPoints(pointsDictionary, 12);
                viewModel.T13 = PrettyPoints(pointsDictionary, 13);
                viewModel.T14 = PrettyPoints(pointsDictionary, 14);
                viewModel.T15 = PrettyPoints(pointsDictionary, 15);

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static ResultItemDisplayObject[] FromItemToSeriesPointsFormat(SequenceContainerItem[] sequenceContainerItem, int scoresToCountForPoints)
        {
            const string failure = "Mapping cumulative individual points.";
            const string locus = "[FromItemToSeriesPointsFormat]";

            try
            {
                if (sequenceContainerItem is null)
                    return [];

                var answer = (from item in sequenceContainerItem where item!.MostRecentResultItemToWhichThisSequenceApplies is not null select FromItemToSeriesPointsFormat(item, scoresToCountForPoints)).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static ResultItemDisplayObject FromItemToSeriesTourDurationFormat(SequenceContainerItem sequenceContainerItem)
        {
            const string failure = "Mapping cumulative individual tour duration.";
            const string locus = "[ToSeasonTourDurationResultDisplayFormat]";

            if (sequenceContainerItem is null) return null;

            const int roundDecimalTo1Place = 1;
            const string format = JghTimeSpan.HhmmssfPattern;

            try
            {
                var viewModel = FromModel(sequenceContainerItem.MostRecentResultItemToWhichThisSequenceApplies); // clever trick to copy over the bulk of fields other than those unique to the sequence container

                // overwrite pertinent fields with SequenceContainerItem data

                viewModel.PlaceCalculatedOverallAsString = JghString.PadLeftOrBlankIfZero(sequenceContainerItem.SequenceTotalRankInt, 5, ' ');
                viewModel.FractionalPlaceInRaceInNumeratorOverDenominatorFormat = sequenceContainerItem.FractionalRankInRaceInNumeratorOverDenominatorFormat;
                viewModel.FractionalPlaceBySexInNumeratorOverDenominatorFormat = sequenceContainerItem.FractionalRankBySexInNumeratorOverDenominatorFormat;
                viewModel.FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat = sequenceContainerItem.FractionalRankBySexPlusCategoryInNumeratorOverDenominatorFormat;
                viewModel.TotalDurationFromAlgorithmInSecondsRank =
                    sequenceContainerItem.SequenceTotal; // we need this field later on. they come in handy for reordering things in the RezultzPublisher and Leaderboard and Favorites datagrids etc
                viewModel.TotalDurationInDisplayFormat = JghTimeSpan.ToDurationOrBlankIfNearZero(JghTimeSpan.ToTimeSpanFromSeconds(sequenceContainerItem.SequenceTotal, roundDecimalTo1Place), roundDecimalTo1Place, format);
                viewModel.GapTimespanString = JghTimeSpan.ToDurationOrBlankIfNearZero(JghTimeSpan.ToTimeSpanFromSeconds(sequenceContainerItem.GapBehindBestInRaceInPrevailingUnitsOfSequence, roundDecimalTo1Place), roundDecimalTo1Place, format);

                static string PrettyDuration(Dictionary<int, double> sequence, int index)
                {
                    var prettyTimespan = JghTimeSpan.ToDurationOrBlankIfNearZero(JghTimeSpan.ToTimeSpanFromSeconds(GetDataPoint(sequence, index), roundDecimalTo1Place), roundDecimalTo1Place, format);
                    return prettyTimespan;
                }

                viewModel.T01 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 1);
                viewModel.T02 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 2);
                viewModel.T03 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 3);
                viewModel.T04 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 4);
                viewModel.T05 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 5);
                viewModel.T06 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 6);
                viewModel.T07 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 7);
                viewModel.T08 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 8);
                viewModel.T09 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 9);
                viewModel.T10 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 10);
                viewModel.T11 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 11);
                viewModel.T12 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 12);
                viewModel.T13 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 13);
                viewModel.T14 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 14);
                viewModel.T15 = PrettyDuration(sequenceContainerItem.SequenceOfSourceData, 15);

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static ResultItemDisplayObject[] FromItemToSeriesTourDurationFormat(SequenceContainerItem[] sequenceContainerItem)
        {
            const string failure = "Mapping cumulative individual tour duration.";
            const string locus = "[FromItemToSeriesTourDurationFormat]";


            try
            {
                if (sequenceContainerItem is null)
                    return [];

                var answer = sequenceContainerItem.Where(z => z!.MostRecentResultItemToWhichThisSequenceApplies is not null).Select(FromItemToSeriesTourDurationFormat).ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForResultsLeaderboard(string parentColumnFormatEnum, Dictionary<int, string> parentDictionaryOfTxxColumnHeaders)
        {
            var template = new List<ColumnSpecificationItem>();

            switch (parentColumnFormatEnum)
            {
                case EnumStrings.SingleEventResultsColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceInRaceInNumeratorOverDenominatorFormat,
                            "rank",
                            "FractionalPlaceInRaceInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexInNumeratorOverDenominatorFormat,
                            "gender",
                            "FractionalPlaceBySexInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat,
                            "cat",
                            "FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));

                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));


                    template.Add(
                        new ColumnSpecificationItem(
                            XeGapCxStyle, "gap",
                            "GapCxTimespanThenSplitsString"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeRace, "race", "RaceGroup"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));



                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));

                    template.Add(new ColumnSpecificationItem(XePointsCalculated, "points",
                        "PointsCalculatedAsString"));

                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders));

                    #endregion

                    break;

                case EnumStrings.AverageSplitTimesColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceInRaceInNumeratorOverDenominatorFormat,
                            "rank",
                            "FractionalPlaceInRaceInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexInNumeratorOverDenominatorFormat,
                            "gender",
                            "FractionalPlaceBySexInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat,
                            "cat",
                            "FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));


                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeGapCxStyle, "gap",
                            "GapCxTimespanThenSplitsString"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeRace, "race",
                        "UtilityClassification")); //note shift

                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));


                    // no Txx

                    #endregion

                    break;

                case EnumStrings.SeriesTotalPointsColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceInRaceInNumeratorOverDenominatorFormat,
                            "rank",
                            "FractionalPlaceInRaceInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexInNumeratorOverDenominatorFormat,
                            "gender",
                            "FractionalPlaceBySexInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat,
                            "cat",
                            "FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));


                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XePointsCalculated, "points",
                        "PointsCalculatedAsString"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeRace, "race", "RaceGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders)); // always Txx

                    #endregion

                    break;

                case EnumStrings.SeriesTourDurationColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceInRaceInNumeratorOverDenominatorFormat,
                            "rank",
                            "FractionalPlaceInRaceInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexInNumeratorOverDenominatorFormat,
                            "gender",
                            "FractionalPlaceBySexInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat,
                            "cat",
                            "FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));


                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(new ColumnSpecificationItem(XeGapTimespanString, "gap",
                        "GapTimespanString"));



                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeRace, "race", "RaceGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders)); // always Txx

                    #endregion

                    break;
            }

            return template;
        }

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForAbridgedLeaderboard(string parentColumnFormatEnum, Dictionary<int, string> parentDictionaryOfTxxColumnHeaders)
        {
            var template = new List<ColumnSpecificationItem>();

            switch (parentColumnFormatEnum)
            {
                case EnumStrings.SingleEventResultsColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));


                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeGapCxStyle, "gap",
                            "GapCxTimespanThenSplitsString"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));

                    template.Add(new ColumnSpecificationItem(XePointsCalculated, "points",
                        "PointsCalculatedAsString"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders));

                    #endregion

                    break;

                case EnumStrings.AverageSplitTimesColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexInNumeratorOverDenominatorFormat,
                            "gender",
                            "FractionalPlaceBySexInNumeratorOverDenominatorFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeFractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat,
                            "cat",
                            "FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));

                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeGapCxStyle, "gap",
                            "GapCxTimespanThenSplitsString"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeRace, "race",
                        "UtilityClassification")); //note shift

                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));

                    // no Txx

                    #endregion

                    break;

                case EnumStrings.SeriesTotalPointsColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));


                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XePointsCalculated, "points",
                        "PointsCalculatedAsString"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders)); // always Txx

                    #endregion

                    break;

                case EnumStrings.SeriesTourDurationColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));


                    template.Add(new ColumnSpecificationItem(XeGapTimespanString, "gap",
                        "GapTimespanString"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));

                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));


                    template.Add(new ColumnSpecificationItem(ResultDto.XeSex, "g", "Gender"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAge, "age",
                        "Age"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeAgeGroup, "cat",
                        "AgeGroup"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeCity, "city", "City"));

                    template.Add(new ColumnSpecificationItem(ResultDto.XeTeam, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders)); // always Txx

                    #endregion

                    break;

                    //default:
                    //    throw new JghInvalidValueException(
                    //        "The desired column format for the table of results is unspecified or unrecognised. Sorry. Coding error.");
            }

            return template;
        }

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForConciseLeaderboard(string parentColumnFormatEnum, Dictionary<int, string> parentDictionaryOfTxxColumnHeaders)
        {
            var template = new List<ColumnSpecificationItem>();

            switch (parentColumnFormatEnum)
            {
                case EnumStrings.SingleEventResultsColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));

                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeGapCxStyle, "gap",
                            "GapCxTimespanThenSplitsString"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Sex, "g", "Gender"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Age, "age",
                    //    "Age"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.AgeGroup, "cat",
                    //    "AgeGroup"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.City, "city", "City"));

                    template.Add(new ColumnSpecificationItem(XePointsCalculated, "points",
                        "PointsCalculatedAsString"));


                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Team, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders));

                    #endregion

                    break;

                case EnumStrings.AverageSplitTimesColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));

                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(
                        new ColumnSpecificationItem(
                            XeGapCxStyle, "gap",
                            "GapCxTimespanThenSplitsString"));


                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Sex, "g", "Gender"));

                    //template.Add(
                    //    new ColumnSpecificationItem(
                    //        ResultItemSerialiserNames.FractionalPlaceBySexInNumeratorOverDenominatorFormat,
                    //        "rank",
                    //        "FractionalPlaceBySexInNumeratorOverDenominatorFormat"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.AgeGroup, "cat",
                    //    "AgeGroup"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Race, "race",
                    //    "UtilityClassification")); //note shift

                    //template.Add(
                    //    new ColumnSpecificationItem(
                    //        ResultItemSerialiserNames
                    //            .FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat,
                    //        "rank",
                    //        "FractionalPlaceBySexPlusAgeGroupInNumeratorOverDenominatorFormat"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Age, "age",
                    //    "Age"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.City, "city", "City"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Team, "team", "Team"));

                    // no Txx

                    #endregion

                    break;

                case EnumStrings.SeriesTotalPointsColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));


                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));

                    template.Add(new ColumnSpecificationItem(XePointsCalculated, "points",
                        "PointsCalculatedAsString"));


                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Sex, "g", "Gender"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Age, "age",
                    //    "Age"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.AgeGroup, "cat",
                    //    "AgeGroup"));


                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.City, "city", "City"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Team, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders)); // always Txx

                    #endregion

                    break;

                case EnumStrings.SeriesTourDurationColumnFormat:

                    #region regular columns

                    template.Add(new ColumnSpecificationItem(XePlaceOverall,
                        "pos", "PlaceCalculatedOverallAsString"));


                    template.Add(new ColumnSpecificationItem(XeGapTimespanString, "gap",
                        "GapTimespanString"));

                    template.Add(new ColumnSpecificationItem(XeTotalDuration,
                        "time", "TotalDurationInDisplayFormat"));

                    template.Add(new ColumnSpecificationItem(XeBib, "ID", "Bib"));

                    template.Add(new ColumnSpecificationItem(XeFullName, "name",
                        "FullName"));


                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Sex, "g", "Gender"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Age, "age",
                    //    "Age"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.AgeGroup, "cat",
                    //    "AgeGroup"));


                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.City, "city", "City"));

                    //template.Add(new ColumnSpecificationItem(ResultItemSerialiserNames.Team, "team", "Team"));


                    template.AddRange(MakeDataGridColumnSpecificationsForLeaderboardTxxItems(parentDictionaryOfTxxColumnHeaders)); // always Txx

                    #endregion

                    break;

                    //default:
                    //    throw new JghInvalidValueException(
                    //        "The desired column format for the table of results is unspecified or unrecognised. Sorry. Coding error.");
            }

            return template;
        }

        public static List<ColumnSpecificationItem> MakeDataGridColumnSpecificationsForLeaderboardTxxItems(Dictionary<int, string> parentDictionaryOfTxxColumnHeaders)
        {
            var template = new List<ColumnSpecificationItem>
            {
                new(
                    ResultDto.XeT01,
                    JghDictionaryHelpers.LookUpValueSafely(1, parentDictionaryOfTxxColumnHeaders),
                    "T01"),
                new(
                    ResultDto.XeT02,
                    JghDictionaryHelpers.LookUpValueSafely(2, parentDictionaryOfTxxColumnHeaders),
                    "T02"),
                new(
                    ResultDto.XeT03,
                    JghDictionaryHelpers.LookUpValueSafely(3, parentDictionaryOfTxxColumnHeaders),
                    "T03"),
                new(
                    ResultDto.XeT04,
                    JghDictionaryHelpers.LookUpValueSafely(4, parentDictionaryOfTxxColumnHeaders),
                    "T04"),
                new(
                    ResultDto.XeT05,
                    JghDictionaryHelpers.LookUpValueSafely(5, parentDictionaryOfTxxColumnHeaders),
                    "T05"),
                new(
                    ResultDto.XeT06,
                    JghDictionaryHelpers.LookUpValueSafely(6, parentDictionaryOfTxxColumnHeaders),
                    "T06"),
                new(
                    ResultDto.XeT07,
                    JghDictionaryHelpers.LookUpValueSafely(7, parentDictionaryOfTxxColumnHeaders),
                    "T07"),
                new(
                    ResultDto.XeT08,
                    JghDictionaryHelpers.LookUpValueSafely(8, parentDictionaryOfTxxColumnHeaders),
                    "T08"),
                new(
                    ResultDto.XeT09,
                    JghDictionaryHelpers.LookUpValueSafely(9, parentDictionaryOfTxxColumnHeaders),
                    "T09"),
                new(
                    ResultDto.XeT10,
                    JghDictionaryHelpers.LookUpValueSafely(10, parentDictionaryOfTxxColumnHeaders),
                    "T10"),
                new(
                    ResultDto.XeT11,
                    JghDictionaryHelpers.LookUpValueSafely(11, parentDictionaryOfTxxColumnHeaders),
                    "T11"),
                new(
                    ResultDto.XeT12,
                    JghDictionaryHelpers.LookUpValueSafely(12, parentDictionaryOfTxxColumnHeaders),
                    "T12"),
                new(
                    ResultDto.XeT13,
                    JghDictionaryHelpers.LookUpValueSafely(13, parentDictionaryOfTxxColumnHeaders),
                    "T13"),
                new(
                    ResultDto.XeT14,
                    JghDictionaryHelpers.LookUpValueSafely(14, parentDictionaryOfTxxColumnHeaders),
                    "T14"),
                new(
                    ResultDto.XeT15,
                    JghDictionaryHelpers.LookUpValueSafely(15, parentDictionaryOfTxxColumnHeaders),
                    "T15")
            };
            return template;
        }

        public override string ToString()
        {
            return JghString.ConcatAsSentences(PlaceCalculatedOverallAsString, FirstName, MiddleInitial, LastName,
                RaceGroup, AgeGroup, Bib, Rfid);
        }

        #endregion

        #region helpers

        private static string FormatRelativeRankInSubsetAsNumeratorOverDenominator(int numInSubset, int rankInSubsetInt)
        {
            if (numInSubset == 0)
                return Symbols.SymbolQuestionMark;

            return $"{rankInSubsetInt}/{numInSubset}";
        }

        private static string FormatGapCxTimespanThenSplitsString(int splitsBehindWinnerOfRace, string formattedTimeSpanAsString)
        {
            if (splitsBehindWinnerOfRace <= 0)
                return formattedTimeSpanAsString;
            return splitsBehindWinnerOfRace == 1 ? "1 split" : $"{splitsBehindWinnerOfRace} splits";
        }

        private static Dictionary<int, PointsDataPoint> AddFlagToNhighestDatapoints(Dictionary<int, double> sequenceOfSourceData, int scoresToCountForPoints)
        {
            var sequenceOfSourceDataAsPointsDataPointDictionary = (from pointsKvp in sequenceOfSourceData
                let dataPoint = new PointsDataPoint
                {
                    Key = pointsKvp.Key,
                    PointsAsDouble = pointsKvp.Value,
                    IsOneOfTheBestNdatapoints = false
                }
                select dataPoint).ToDictionary(z => z.Key, z => z);

            var bestN = sequenceOfSourceDataAsPointsDataPointDictionary.OrderByDescending(z => z.Value.PointsAsDouble)
                .Take(scoresToCountForPoints).ToArray();


            foreach (var best in bestN)
                sequenceOfSourceDataAsPointsDataPointDictionary[best.Key].IsOneOfTheBestNdatapoints = true;

            return sequenceOfSourceDataAsPointsDataPointDictionary;
        }

        private static string GetDataPointAsTypeDoubleAsStringOrBlankIfNearZeroOrBracketedIfNotOneOfTheBest(Dictionary<int, PointsDataPoint> pointsDictionary, int eventNum, int roundDecimalToNthPlace)
        {
            if (pointsDictionary is null)
                return string.Empty;

            var dataPoint = GetDataPoint(pointsDictionary, eventNum);

            if (dataPoint is null)
                return string.Empty;

            if (dataPoint.IsOneOfTheBestNdatapoints)
                return JghString.ToStringOrBlankIfNearZero(dataPoint.PointsAsDouble, roundDecimalToNthPlace,
                    JghFormatSpecifiers.DecimalFormat0Dp);

            return JghString.ToBracketedStringOrBlankIfNearZero(dataPoint.PointsAsDouble, roundDecimalToNthPlace, JghFormatSpecifiers.DecimalFormat0Dp);
        }

        private static double GetDataPoint(Dictionary<int, double> pointsDictionary, int eventNum)
        {
            if (pointsDictionary is null)
                return 0;

            return pointsDictionary.ContainsKey(eventNum) ? pointsDictionary[eventNum] : 0.0;
        }

        private static PointsDataPoint GetDataPoint(Dictionary<int, PointsDataPoint> pointsDictionary, int eventNum)
        {
            if (pointsDictionary is null)
                return null;

            //var key = eventNum;

            return pointsDictionary.ContainsKey(eventNum) ? pointsDictionary[eventNum] : null;
        }

        #endregion
    }
}