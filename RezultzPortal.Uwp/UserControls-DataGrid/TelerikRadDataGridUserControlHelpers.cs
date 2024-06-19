using System;
using System.Collections.Generic;
using System.Linq;
using NetStd.Exceptions.Mar2024.Helpers;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.DataGridDesigners;
using Telerik.UI.Xaml.Controls.Grid;

namespace RezultzPortal.Uwp.UserControls_DataGrid
{
    public static class TelerikRadDataGridUserControlHelpers
    {
        private const string Locus2 = nameof(TelerikRadDataGridUserControlHelpers);
        private const string Locus3 = "[RezultzPortal.Uwp]";

        #region methods

        public static void ManuallyGenerateDataGridColumns(IEnumerable<ColumnSpecificationItem> columnSpecificationItems, RadDataGrid theDataGrid)
        {
            const string failure = "Unable to generate column collection.";
            const string locus = "[ManuallyGenerateDataGridColumns]";

            try
            {
                if (theDataGrid is null) return;

                theDataGrid.AutoGenerateColumns = false; // NB

                //be sure to zeroise the column collection, this might not be the first time thru
                while (theDataGrid.Columns.Any())
                    theDataGrid.Columns.RemoveAt(0);

                foreach (var columnSpecificationItem in columnSpecificationItems)
                {
                    if (columnSpecificationItem?.NameOfAssociatedPropertyInXamlBindingSyntax is null)
                        continue; // cautionary measure

                    if (columnSpecificationItem.NameOfAssociatedPropertyInXamlBindingSyntax.Length <= DataGridDesigner.LengthOfPrefixForViewModelChildPropertyNames)
                        continue; // cautionary measure

                    var newColumn =
                        new DataGridTextColumn
                        {
                            Header = columnSpecificationItem.ColumnHeaderText,
                            PropertyName = columnSpecificationItem.NameOfAssociatedPropertyInXamlBindingSyntax.Remove(0, DataGridDesigner.LengthOfPrefixForViewModelChildPropertyNames),
                            SizeMode = DataGridColumnSizeMode.Auto
                        };
                    // Note: InformationPrinter generates a collection of ColumnSpecificationItem which has a property named Model
                    // and that property is of type Result, or whatever. Each NameOfAssociatedPropertyInXamlBindingSyntax generated
                    // by InformationPrinter therefore is prefixed by the string "Model." Here, we need to delete it.

                    theDataGrid.Columns.Add(newColumn);
                }

            }
            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }
        
        #endregion

    }
}
