// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortedObservableCollection.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   SortedObservableCollection.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class SortedObservableCollection<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly List<T> _children = new List<T>();

        private readonly IComparer<T> _comparer;

        public SortedObservableCollection(IComparer<T> comparer)
        {
            this._comparer = comparer;
        }

        public SortedObservableCollection(IEnumerable<T> items, IComparer<T> comparer, Func<bool> suspendHandler)
            : this(comparer)
        {
            this.SuspendHandler = suspendHandler;
            this.AddRange(items);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public Func<bool> SuspendHandler { get; set; }

        public bool IsSuspendingUpdates
        {
            get { return this.SuspendUpdates || (this.SuspendHandler != null && this.SuspendHandler()); }
        }

        public bool SuspendUpdates { get; set; }

        public void AddLocal(T item)
        {
            int i;
            for (i = 0; i < this._children.Count && this._comparer.Compare(this._children[i], item) <= 0; ++i)
            {
            }

            this._children.Insert(i, item);
            this.OnCollectionAdd(item, i);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                this.AddLocal(item);
            }
        }

        public bool Contains(T item)
        {
            return this._children.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._children.GetEnumerator();
        }

        public bool RemoveLocal(T item)
        {
            int index = this._children.IndexOf(item);

            if (index != -1)
            {
                this._children.RemoveAt(index);
                this.OnCollectionRemove(item, index);
                return true;
            }

            return false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void OnCollectionAdd(T item, int index)
        {
            if (this.IsSuspendingUpdates)
            {
                return;
            }

            NotifyCollectionChangedEventHandler handler = this.CollectionChanged;

            if (handler != null)
            {
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        private void OnCollectionRemove(T removedItem, int index)
        {
            if (this.IsSuspendingUpdates)
            {
                return;
            }

            NotifyCollectionChangedEventHandler handler = this.CollectionChanged;

            if (handler != null)
            {
                handler(
                    this, 
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
            }
        }
    }
}