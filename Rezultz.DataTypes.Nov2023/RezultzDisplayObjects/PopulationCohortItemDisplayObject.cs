using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.DataTypes.Nov2023.RezultzDisplayObjects
{
    public class PopulationCohortItemDisplayObject : BindableBase, IHasCollectionLineItemPropertiesV2
    {
        #region ColumnSpecificationItem CellXElementName props

        public const string XeNameOfCohort = "cohort";
        public const string XeMale = "Male";
        public const string XeFemale = "Female";
        public const string XeOther = "Other";
        public const string XeTotal = "Total";
        public const string XePercent = "Percent";


        #endregion
        #region props

        public string NameOfCohort { get; set; }

        public string FinishersCount { get; set; }

        public string SexMaleCount { get; set; }

        public string SexFemaleCount { get; set; }

        public string SexOtherCount { get; set; }

        //public int Other { get; set; }

        public string DnfCount { get; set; }

        public string DnsCount { get; set; }

        public string DqCount { get; set; }

        public string TotalCount { get; set; }

        public string Percent { get; set; }

        public int ID { get; set; }

        public string Label { get; set; }

        public string EnumString { get; set; }

        public string Guid { get; set; }

        #endregion

        #region methods

        public static PopulationCohortItemDisplayObject FromModel(PopulationCohortItem model)
        {
            const string failure = "Populating PopulationCohortItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new PopulationCohortItem();

                var displayObject = new PopulationCohortItemDisplayObject
                {
                    NameOfCohort = model.NameOfCohort,
                    FinishersCount = JghString.PadLeftOrBlankIfZero(model.FinishersCount, 5, ' '),
                    SexMaleCount = JghString.PadLeftOrBlankIfZero(model.SexMaleCount, 5, ' '),
                    SexFemaleCount = JghString.PadLeftOrBlankIfZero(model.SexFemaleCount, 5, ' '),
                    SexOtherCount = JghString.PadLeftOrBlankIfZero(model.SexOtherCount, 5, ' '),
                    //Other = model.Other,
                    DnfCount = JghString.PadLeftOrBlankIfZero(model.DnfCount, 5, ' '),
                    DnsCount = JghString.PadLeftOrBlankIfZero(model.DnsCount, 5, ' '),
                    DqCount = JghString.PadLeftOrBlankIfZero(model.DqCount, 5, ' '),
                    TotalCount = JghString.PadLeftOrBlankIfZero(model.TotalCount, 5, ' '),
                    Percent = JghString.RightAlign(model.Percent == 0 ? "" : Math.Round(model.Percent, 1).ToString("N1"), 5, ' '),
                    ID = model.ID,
                    Label = model.Label,
                    EnumString = model.EnumString
                    //Guid = src.Guid
                };

                return displayObject;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static PopulationCohortItemDisplayObject[] FromModel(PopulationCohortItem[] model)
        {
            const string failure = "Populating PopulationCohortItemDisplayObject.";
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

        public static List<ColumnSpecificationItem> DataGridColumnsToBeDisplayed()
        {
            var template = new List<ColumnSpecificationItem>
            {
                new(XeNameOfCohort, "cohort", "NameOfCohort"),
                new(XeMale, "male", "SexMaleCount"),
                new(XeFemale, "female", "SexFemaleCount"),
                new(XeOther, "x", "SexOtherCount"),
                new(XeTotal, "total", "TotalCount"),
                new(XePercent, "total %", "Percent")
            };
            return template;
        }

        #endregion
    }
}