using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.VSFolders
{
    public class FilteredCollection<T, TItem> : IList<TItem>, INotifyCollectionChanged
        where T: class, INotifyCollectionChanged, ICollection<TItem>
    {
        private readonly Func<TItem, bool> _predicate;
        private readonly T _items;
        private IList<TItem> _filtered;
        private readonly Func<TItem, FilteredCollection<T, TItem>> _childNotify;

        public FilteredCollection(T items, Func<TItem, bool> predicate, Func<TItem, FilteredCollection<T, TItem>> childNotify)
        {
            _items = items;
            _items.CollectionChanged += ItemsOnCollectionChanged;
            _predicate = predicate;
            _childNotify = childNotify;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Refresh();
            OnCollectionChanged();
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return GetFiltered().GetEnumerator();
        }

        private IList<TItem> GetFiltered()
        {
            if (_filtered != null)
            {
                return _filtered;
            }

            return _filtered = (_items != null && _items.Count > 0) ? _items.Where(_predicate).ToList() : new List<TItem>();
        }

        public void Refresh()
        {
            _filtered = null;
            OnCollectionChanged();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItem item)
        {
        }

        public void Clear()
        {
            Refresh();
        }

        public bool Contains(TItem item)
        {
            return GetFiltered().Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            GetFiltered().CopyTo(array, arrayIndex);
        }

        public bool Remove(TItem item)
        {
            return false;
        }

        public int Count { get { return GetFiltered().Count; } }
        
        public bool IsReadOnly { get { return true; } }
        
        public int IndexOf(TItem item)
        {
            return GetFiltered().IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
        }

        public void RemoveAt(int index)
        {
        }

        public TItem this[int index]
        {
            get { return GetFiltered()[index]; }
            set { }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void NotifyInternal(NotifyCollectionChangedEventHandler handler)
        {
            handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (_childNotify != null)
            {
                foreach (var item in GetFiltered())
                {
                    var coll = _childNotify(item);
                    var hndlr = coll.CollectionChanged;

                    if (hndlr != null)
                    {
                        coll.NotifyInternal(hndlr);
                    }
                }
            }
        }

        protected virtual void OnCollectionChanged()
        {
            var handler = CollectionChanged;

            if (handler != null)
            {
                if (!Dispatcher.CurrentDispatcher.CheckAccess())
                {
                    NotifyInternal(handler);
                }
                else
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke((Action) (() => NotifyInternal(handler)));
                }
            }
        }

        public void RefreshNow()
        {
            _filtered = null;
            GetFiltered();
        }

        public void RaiseCollectionChanged()
        {
            OnCollectionChanged();
        }
    }
}
