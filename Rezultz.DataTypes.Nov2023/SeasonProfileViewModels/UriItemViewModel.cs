namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
}

//public class UriItemViewModel : BindableBase, IHasCollectionLineItemPropertiesV2
//{

//    #region properties

//    public string SourceUriString { get; set; } = string.Empty;

//    public string ReferenceUriString { get; set; } = "https://dummy"; // this is mandatory. don't delete it. critical for newing up album items 

//    public string BlobName { get; set; } = string.Empty;

//    public string EnumString { get; set; } = string.Empty;

//    public int DisplayRank { get; set; }

//    public string Guid { get; set; } = string.Empty;

//    public int ID { get; set; }

//    public string Label { get; set; } = string.Empty;

//    #endregion

//    #region methods

//    public static UriItemViewModel FromModel(UriItem model)
//    {
//        const string failure = "Populating UriItemViewModel.";
//        const string locus = "[FromModel]";

//        try
//        {
//            model ??= new UriItem();

//            var viewModel = new UriItemViewModel
//            {
//                Guid = string.Empty,
//                BlobName = model.BlobName,
//                DisplayRank = model.DisplayRank,
//                SourceUriString = model.SourceUriString,
//                ReferenceUriString = model.ReferenceUriString,
//                //BlobSpecificationItem = BlobSpecificationItemViewModel.FromModel(model.BlobSpecificationItem),
//            };

//            return viewModel;
//        }

//        #region trycatch

//        catch (Exception ex)
//        {
//            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
//        }

//        #endregion
//    }

//    public static UriItemViewModel[] FromModel(UriItem[] model)
//    {
//        const string failure = "Populating UriItemViewModel.";
//        const string locus = "[FromModel]";

//        try
//        {
//            if (model is null)
//                return Array.Empty<UriItemViewModel>();

//            var viewModel = model.Select(FromModel).Where(z => z is not null).ToArray();

//            return viewModel;
//        }

//        #region trycatch

//        catch (Exception ex)
//        {
//            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
//        }

//        #endregion
//    }

//    public override string ToString()
//    {
//        return JghString.ConcatAsSentences(BlobName, ReferenceUriString, DisplayRank.ToString(CultureInfo.InvariantCulture));
//    }

//    #endregion

//}