using System;
using System.Collections.Concurrent;

namespace Microsoft.VSFolders
{
    public static class ServiceContainer
    {
        private static readonly ConcurrentDictionary<Type, object> Registrations = new ConcurrentDictionary<Type, object>();

        public static void Register<T>(T obj)
        {
            Registrations[typeof(T)] = obj;
        }

        public static T Resolve<T>()
            where T : new()
        {
            return (T)Registrations.GetOrAdd(typeof(T), x => new T());
        }
    }
}
