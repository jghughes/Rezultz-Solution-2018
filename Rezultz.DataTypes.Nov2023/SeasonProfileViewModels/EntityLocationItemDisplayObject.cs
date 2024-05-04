using System;
using System.Linq;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

namespace Rezultz.DataTypes.Nov2023.SeasonProfileViewModels
{
    public class EntityLocationItemDisplayObject : BindableBase, IHasCollectionLineItemPropertiesV1
    {
        #region properties

        public string DatabaseAccountName { get; set; } = string.Empty;

        public string DataContainerName { get; set; } = string.Empty;

        public string DataItemName { get; set; } = string.Empty;

        public int ID { get; set; } // primary key used all over the place

        public string Label { get; set; }

        public string EnumString { get; set; }

        #endregion

        #region methods

        public override string ToString()
        {
            return string.Join("  ", DatabaseAccountName, DataContainerName, DataItemName);
        }

        public static EntityLocationItemDisplayObject FromModel(EntityLocationItem model)
        {
            const string failure = "Populating EntityLocationItemDisplayObject.";
            const string locus = "[FromModel]";

            try
            {
                model ??= new EntityLocationItem();

                var viewModel = new EntityLocationItemDisplayObject
                {
                    DatabaseAccountName = model.AccountName,
                    DataContainerName = model.ContainerName,
                    DataItemName = model.EntityName,
                    Label = model.EntityName,
                };

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        public static EntityLocationItemDisplayObject[] FromModel(EntityLocationItem[] model)
        {
            const string failure = "Populating EntityLocationItemDisplayObject[].";
            const string locus = "[FromModel]";

            try
            {
                if (model == null)
                    return Array.Empty<EntityLocationItemDisplayObject>();

                var viewModel = model.Select(FromModel).Where(z => z != null).ToArray();

                return viewModel;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
            }

            #endregion
        }

        //public static EntityLocation ObtainSourceModel(DatabaseLocationItemViewModel viewModel)
        //{
        //    return viewModel?._sourceItem;
        //}

        //public static EntityLocation[] ObtainSourceModel(DatabaseLocationItemViewModel[] viewModel)
        //{
        //    const string failure = "Populating EntityLocation[]";
        //    const string locus = "[ObtainSourceModel]";

        //    try
        //    {
        //        if (viewModel == null)
        //            return Array.Empty<EntityLocation>();

        //        var answer = viewModel.Select(ObtainSourceModel).Where(z => z != null).ToArray();


        //        return answer;
        //    }

        //    #region trycatch

        //    catch (Exception ex)
        //    {
        //        throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        //    }

        //    #endregion
        //}


        //public static EntityLocation ToModel(DatabaseLocationItemViewModel viewModel)
        //{
        //    const string failure = "Populating EntityLocation.";
        //    const string locus = "[ObtainSourceItem]";


        //    try
        //    {
        //         viewModel ??= new DatabaseLocationItemViewModel();

        //        var model = new EntityLocation
        //        {
        //            AccountName = viewModel.AccountName,
        //            ContainerName = viewModel.ContainerName,
        //            EntityName = viewModel.EntityName,
        //        };

        //        return model;
        //    }

        //    #region trycatch

        //    catch (Exception ex)
        //    {
        //        throw JghExceptionHelpers.ConvertToCarrier(failure, locus, ex);
        //    }

        //    #endregion
        //}

        #endregion

    }
}