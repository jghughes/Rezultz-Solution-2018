using System;
using System.Collections.Concurrent;

namespace RezultzPortal.Uwp.Helpers
{
    internal static class Singleton<T>
        where T : new()
    {
        private static readonly ConcurrentDictionary<Type, T> Instances = new();

        public static T Instance
        {
            get { return Instances.GetOrAdd(typeof(T), t => new T()); }
        }
    }
}
