using System.Threading.Tasks;
using NetStd.Interfaces01.July2018.HasProperty;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface ILocalStorageServiceCustomMethods
    {
        Task<string> SaveSerialisableObjectAsync<T>(string folder, string fileName, T theTypedObject);

        Task<T> ReadSerializedObjectAsync<T>(string folder, string fileName);

        Task<string> WriteFreeformIdentitiesOfNamedItemsAsync<T>(string directoryPath, string fileName, T[] namedItems)
            where T : class, IHasFirstName, IHasLastName, IHasMiddleInitial,
            IHasFullName; // loose coupling - use this rather than SaveSerialisableObjectAsync

        Task<T[]> ReadFreeformIdentitiesOfNamedItemsAsync<T>(string directoryPath, string fileName)
            where T : class, IHasFirstName, IHasLastName, IHasMiddleInitial, IHasFullName, new(
            ); // loose coupling - use this rather than ReadSerializedObjectAsync
    }
}