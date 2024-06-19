using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NetStd.Interfaces02.July2018.Interfaces;

/* this class cut and pasted from SmartHotel360 Xamarin/.Net Core reference project by Microsoft on GitHub
 Many classes are cut and pasted from microsoft's reference sample SmartHotel360. Repo is on GitHub at: - 
 https://github.com/Microsoft/SmartHotel360-Mobile For boiler plating and infrastructure, go to : -
https://github.com/Microsoft/SmartHotel360-Mobile/tree/master/Source/SmartHotel.Clients/SmartHotel.Clients
For file handling, also read https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/files?tabs=windows
 */


namespace NetStd.OnBoardServices01.July2018.Persistence
{
    public class FileServiceFromSmartHotel360 : IFileServiceFromSmartHotel360
    {
        public List<string> GetEmbeddedResourceNames()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(FileServiceFromSmartHotel360)).Assembly;
            var resourceNamesList = new List<string>();

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                resourceNamesList.Add(resourceName);
            }

            return resourceNamesList;
        }

        public string ReadStringFromAssemblyEmbeddedResource(string resourceName)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(FileServiceFromSmartHotel360)).Assembly;

            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream is null) return string.Empty;

            using var reader = new StreamReader(stream);

            var resourceContent = reader.ReadToEnd();

            return resourceContent;
        }

        public string ReadStringFromLocalAppDataFolder(string fileName)
        {
            var fullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);

            if (!File.Exists(fullFileName))
            {
                return null;
            }

            var resourceContent = string.Empty;
            try
            {
                resourceContent = File.ReadAllText(fullFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading file from {fullFileName}, reason: {ex.Message}");
            }

            return resourceContent;
        }

        public bool WriteStringToLocalAppDataFolder(string fileName, string textToWrite)
        {
            var fullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);

            try
            {
                File.WriteAllText(fullFileName, textToWrite);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing file to {fullFileName}, reason: {ex.Message}");
                return false;
            }

            return true;
        }

        public bool ExistsInLocalAppDataFolder(string fileName)
        {
            var fullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);

            return File.Exists(fullFileName);
        }

    }
}