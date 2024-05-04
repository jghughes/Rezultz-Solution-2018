// ReSharper disable InconsistentNaming
namespace Jgh.SymbolsStringsConstants.Mar2022
{
    public static class StringsForXamlPages
    {

        public const string UnableToRetrieveInstance = "Runtime blunder. Unable to retrieve instance of interface from dependency injection container. Did you forget to register it?";
        public const string DataContextIsNull = "Runtime blunder. The data context for this page translates as null. Did you forget to assign a datacontext in the page Xaml or code-behind? Or did you assign the wrong viewmodel?";
        public const string ExceptionCaughtAtPageLevel = "Exception caught at page level.";
        public const string PropertyGetterOf = "Property getter of ";
        public const string UnableToInitialiseViewmodel = "Unable to initialise viewmodel.";
        public const string DependencyInjectionLocator = "DependencyInjectionLocator"; // must match the resource key in the App.xaml file
    }
}