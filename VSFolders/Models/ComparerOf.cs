using System;
using System.Collections.Generic;

namespace Microsoft.VSFolders.Models
{
    public class ComparerOf<T> : IComparer<T>
    {
        private readonly List<Func<T, T, int>> _comparisonSteps = new List<Func<T, T, int>>();

        public int Compare(T x, T y)
        {
            foreach (var step in _comparisonSteps)
            {
                var stepResult = step(x, y);

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
            _comparisonSteps.Add((x, y) => on(x).CompareTo(on(y)));
            return this;
        }

        public ComparerOf<T> OnDescending<TValue>(Func<T, TValue> on)
            where TValue : IComparable<TValue>
        {
            _comparisonSteps.Add((x, y) => -on(x).CompareTo(on(y)));
            return this;
        }
    }
}