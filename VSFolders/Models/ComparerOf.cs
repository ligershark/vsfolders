// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparerOf.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   ComparerOf.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using System;
    using System.Collections.Generic;

    public class ComparerOf<T> : IComparer<T>
    {
        private readonly List<Func<T, T, int>> _comparisonSteps = new List<Func<T, T, int>>();

        public int Compare(T x, T y)
        {
            foreach (Func<T, T, int> step in this._comparisonSteps)
            {
                int stepResult = step(x, y);

                if (stepResult > 0)
                {
                    return stepResult;
                }
            }

            return 0;
        }

        public ComparerOf<T> On<TValue>(Func<T, TValue> on)
            where TValue : IComparable<TValue>
        {
            this._comparisonSteps.Add((x, y) => on(x).CompareTo(on(y)));
            return this;
        }

        public ComparerOf<T> OnDescending<TValue>(Func<T, TValue> on)
            where TValue : IComparable<TValue>
        {
            this._comparisonSteps.Add((x, y) => -on(x).CompareTo(on(y)));
            return this;
        }
    }
}