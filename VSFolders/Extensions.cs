// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   Extensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> col, Action<T> act)
        {
            foreach (T obj in col)
            {
                act(obj);
            }
        }

        public static T CheckAs<T>(this object o)
            where T : class
        {
            T obj = o as T;

            if (obj == null)
            {
                throw new ArgumentException($"Argument expected to be of type {typeof (T)}");
            }
            return obj;
        }
    }
}