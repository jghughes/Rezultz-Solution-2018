using System.Collections.Generic;

/* this class cut and pasted from SmartHotel360 Xamarin/.Net Core reference project by Microsoft on GitHub
 Many classes are cut and pasted from microsoft's reference sample SmartHotel360. Repo is on GitHub at: - 
 https://github.com/Microsoft/SmartHotel360-Mobile For boiler plating and infrastructure, go to : -
https://github.com/Microsoft/SmartHotel360-Mobile/tree/master/Source/SmartHotel.Clients/SmartHotel.Clients
For file handling, also read https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/files?tabs=windows
 */


namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface IFileServiceFromSmartHotel360
    {
        List<string> GetEmbeddedResourceNames();

        string ReadStringFromAssemblyEmbeddedResource(string path);

        string ReadStringFromLocalAppDataFolder(string fileName);

        bool WriteStringToLocalAppDataFolder(string fileName, string textToWrite);

        bool ExistsInLocalAppDataFolder(string fileName);

    }
}
