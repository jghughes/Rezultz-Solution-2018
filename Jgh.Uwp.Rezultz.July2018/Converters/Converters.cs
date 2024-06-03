using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023.PortalDisplayObjects;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;

namespace Jgh.Uwp.Rezultz.July2018.Converters
{
    // Note If there is an error in any conversion, do not throw an exception. Instead, return DependencyProperty.UnsetValue, which will stop the data transfer.

    #region Rezultz DisplayObject converters

    public sealed class SeasonItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is SeasonItemDisplayObject ? value : new SeasonItemDisplayObject { Label = "Converter warning: binding is null or of wrong type" };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class SeasonItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<SeasonItemDisplayObject>)
            return value;

        return new[]
        {
            new SeasonItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type",
                Title = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class SeriesItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is SeriesItemDisplayObject ? value : new SeriesItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class SeriesItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<SeriesItemDisplayObject>)
            return value;

        return new[]
        {
            new SeriesItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type",
                Title = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class EventItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is EventItemDisplayObject ? value : new EventItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class EventItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<EventItemDisplayObject>)
            return value;

        return new[]
        {
            new EventItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type",
                Title = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class MoreInformationItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is MoreInformationItemDisplayObject ? value : new MoreInformationItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class MoreInformationItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<MoreInformationItemDisplayObject>)
            return value;

        return new[]
        {
            new MoreInformationItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type",
                Title = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class RaceSpecificationItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is RaceSpecificationItemDisplayObject ? value : new RaceSpecificationItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class RaceSpecificationItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<RaceSpecificationItemDisplayObject>)
            return value;

        return new[]
        {
            new RaceSpecificationItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type",
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class CboLookupItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is CboLookupItemDisplayObject ? value : new CboLookupItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class CboLookupItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<CboLookupItemDisplayObject>)
            return value;

        return new[]
        {
            new CboLookupItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class CboLookupItemDisplayObjectDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is CboLookupItemDisplayObject ? value : null;
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    }

    public sealed class UriItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<UriItemDisplayObject>)
            return value;

        return new[]
        {
            new UriItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class ResultItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is CboLookupItemDisplayObject ? value : new CboLookupItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class ResultItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<ResultItemDisplayObject>)
            return value;

        return new[]
        {
            new ResultItemDisplayObject
            {
                FirstName = " Converter warning: binding is null or of wrong type",
                LastName = " Converter warning: binding is null or of wrong type",
                Bib = " Converter warning: binding is null or of wrong type",
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class ResultItemDisplayObjectDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is ResultItemDisplayObject ? value : null;
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    }

    public sealed class CohortAnalysisDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is PopulationCohortItemDisplayObject ? value : new PopulationCohortItemDisplayObject {Label = "Converter warning: binding is null or of wrong type", NameOfCohort = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class CohortAnalysisItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<PopulationCohortItemDisplayObject>)
            return value;

        return new[]
        {
            new PopulationCohortItemDisplayObject
            {
                NameOfCohort = "Converter warning: binding is null or of wrong type",
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class EntityLocationItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<EntityLocationItemDisplayObject>)
            return value;

        return new[]
        {
            new EntityLocationItemDisplayObject
            {
                DatabaseAccountName = "Converter warning: binding is null or of wrong type",
                DataContainerName = "Converter warning: binding is null or of wrong type",
                DataItemName = "Converter warning: binding is null or of wrong type",
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class TimeStampHubItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is TimeStampHubItemDisplayObject ? value : new TimeStampHubItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class TimeStampHubItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<TimeStampHubItemDisplayObject>)
            return value;

        return new[]
        {
            new TimeStampHubItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class TimeStampHubItemDisplayObjectDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is TimeStampHubItemDisplayObject ? value : null;
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    }

    public sealed class TimeStampHubItemDisplayObjectCollectionDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is IEnumerable<TimeStampHubItemDisplayObject> ? value : null;
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    }

    public sealed class SplitIntervalsForParticipantDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<SplitIntervalConsolidationForParticipantDisplayObject>)
            return value;

        return new[]
        {
            new SplitIntervalConsolidationForParticipantDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class SplitIntervalsForParticipantDisplayObjectDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is not SplitIntervalConsolidationForParticipantDisplayObject ? null : value;
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    }

    public sealed class ParticipantHubItemDisplayObjectNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is ParticipantHubItemDisplayObject ? value : new ParticipantHubItemDisplayObject {Label = "Converter warning: binding is null or of wrong type"};
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class ParticipantHubItemDisplayObjectCollectionNullConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is IEnumerable<ParticipantHubItemDisplayObject>)
            return value;

        return new[]
        {
            new ParticipantHubItemDisplayObject
            {
                Label = "Converter warning: binding is null or of wrong type"
            }
        };
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }

    
    }

    public sealed class ParticipantHubItemDisplayObjectDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is ParticipantHubItemDisplayObject ? value : null;
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    }

    public sealed class ParticipantHubItemDisplayObjectCollectionDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is IEnumerable<ParticipantHubItemDisplayObject> ? value : null;
    }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    }

    #endregion
}