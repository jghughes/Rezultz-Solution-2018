using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Goodies.Mar2022;

namespace Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems
{
    /// <summary>
    /// Note: ColumnSpecificationItems are used for assembling a list of columns for a Telerik data-grid.
    /// See Rezultz.Uwp.UserControls_DataGrid.TelerikRadDataGridUserControlHelpers.ManuallyGenerateDataGridColumns().
    /// Two properties are required for this: HeaderText and NameOfAssociatedPropertyInXamlBindingSyntax.
    /// The other properties are used in Rezultz.Library02.Mar2024.DataGridDesigners.DataGridDesigner to determine and define
    /// column widths and text formatting in row cells. Much of the donkey work is done using row item objects serialised into XML elements
    /// and the CellXElementName property for each column maps through to the XElement Name of the corresponding child element. 
    /// </summary>
    public class ColumnSpecificationItem
    {
        #region ctor 

        public ColumnSpecificationItem()
        {
        }

        public ColumnSpecificationItem(string cellXElementName, string columnHeaderText, string associatedPropertyNameInXamlBindingSyntax)
        {
            CellXElementName = cellXElementName;
            ColumnHeaderText = columnHeaderText;
            NameOfAssociatedPropertyInXamlBindingSyntax = associatedPropertyNameInXamlBindingSyntax;
        }

        #endregion

        #region props

        public string CellXElementName { get; set; } = string.Empty; 

        public string ColumnHeaderText { get; set; } = string.Empty;

        public string NameOfAssociatedPropertyInXamlBindingSyntax { get; set; } = string.Empty;

        public string CaseOfHeaderTextEnum { get; set; } = EnumStrings.NonSpecificCase;

        public string CaseOfLineItemTextEnum { get; set; } = EnumStrings.NonSpecificCase;

        public string TextAlignmentEnum { get; set; } = string.Empty;

        public int ColumnWidthChars { get; set; }

        public ColumnSpecificationItem ShallowMemberwiseCloneCopy
        {
            get
            {
                var other = (ColumnSpecificationItem)MemberwiseClone();

                return other;
            }
        }


        #endregion

        #region methods

        public override string ToString()
        {
            return JghString.ConcatAsSentences(
                ColumnHeaderText,
                NameOfAssociatedPropertyInXamlBindingSyntax,
                ColumnWidthChars.ToString(),
                CaseOfHeaderTextEnum,
                TextAlignmentEnum);
        }

        #endregion

        #region deprecated

        //public string Blurb { get; set; } = string.Empty;

        //public string NameOfAssociatedPropertyAsXElement { get; set; } = "NameUnspecified";

        //public string Title { get; set; } = string.Empty; 

        //public int ID { get; set; } 

        //public string EnumString { get; set; } = string.Empty; 

        //public string Guid { get; set; } = string.Empty; 

        #endregion


    }
}