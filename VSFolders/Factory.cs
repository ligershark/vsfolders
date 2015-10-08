// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factory.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   Factory.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System;
    using System.Collections.Concurrent;

    public static class Factory
    {
        private static readonly ConcurrentDictionary<Type, object> Registrations =
            new ConcurrentDictionary<Type, object>();

        public static void Register<T>(T obj)
        {
            Factory.Registrations[typeof(T)] = obj;
        }

        public static T Resolve<T>()
        {
            return (T)Factory.Registrations.GetOrAdd(
                typeof (T),
                x =>
                {
                    if (x.IsClass)
                    {
                        return Activator.CreateInstance<T>();
                    }

                    throw new Exception();
                });
        }
    }
}