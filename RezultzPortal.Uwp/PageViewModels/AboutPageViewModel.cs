using Windows.ApplicationModel;
using NetStd.Prism.July2018;

namespace RezultzPortal.Uwp.PageViewModels
{
    public class AboutPageViewModel : BindableBase
    {
        public string BeInitialised()
        {
            if (_meIsInitialialised)
                return string.Empty;

            AppDisplayName = "Rezultz Portal";

            var version = Package.Current.Id.Version;

            VersionDescription = $"{version.Major}.{version.Minor}.{version.Build}";

            InstalledDate = $"{Package.Current.InstalledDate.Date.ToShortDateString()}";

            _meIsInitialialised = true;

            return string.Empty;

        }

        private string _appDisplayName;
        public string AppDisplayName
        {
            get => _appDisplayName;

            set => SetProperty(ref _appDisplayName, value);
        }

        private string _versionDescription;
        public string VersionDescription
        {
            get => _versionDescription;

            set => SetProperty(ref _versionDescription, value);
        }

        private string _installedDate;
        public string InstalledDate
        {
            get => _installedDate;

            set => SetProperty(ref _installedDate, value);
        }

        private bool _meIsInitialialised;
    }
}
