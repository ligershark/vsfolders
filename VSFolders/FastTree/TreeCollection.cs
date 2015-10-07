using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VSFolders.FastTree
{
    [TypeDescriptionProvider(typeof(TreeNodeTypeDescriptionProvider))]
    public class TreeCollection<T> : IEnumerable<TreeNode<T>>, ITreeCollection
    {
        private IComparer<T> _comparer;
        private readonly List<TreeNode<T>> _allItems;
        private readonly List<T> _allItemsRaw;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();
        private readonly TreeCollection<T> _owner;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private TaskCompletionSource<bool> _filterComplete = new TaskCompletionSource<bool>();
        private bool _useFiltering;
        private bool _expandOnFilterMatch;

        public TreeCollection()
        {
            _allItems = new List<TreeNode<T>>();
            _allItemsRaw = new List<T>();
            Root = this;
            _filterComplete.SetResult(true);
        }

        public void RefreshAll()
        {
            Root.OnPropertyChanged("__DISPLAY_PROPERTY_REFRESH__");
        }

        public IComparer<T> Comparer
        {
            get { return Root._comparer; }
            set { Root._comparer = value; }
        }

        public Func<T, IEnumerable<T>> LoadChildren
        {
            get { return Root._loadChildren; }
            set { Root._loadChildren = value; }
        }

        public bool IsSuspendingUpdates { get; set; }

        public bool ExpandOnFilterMatch
        {
            get { return Root._expandOnFilterMatch; }
            set
            {
                var oldVal = Root._expandOnFilterMatch;
                Root._expandOnFilterMatch = value;

                if (oldVal ^ value)
                {
                    Root.OnPropertyChanged();
                }
            }
        }

        public bool IsSearching
        {
            get { return !_filterComplete.Task.IsCompleted; }
        }

        protected TreeCollection(TreeCollection<T> owner)
        {
            _owner = owner;
            Root = owner.Root;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        protected IEnumerable<TreeNode<T>> ChildValues
        {
            get { return _children; }
        }

        public int Count { get { return _children.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool IsRoot
        {
            get { return ReferenceEquals(_owner, null); }
        }

        public TreeCollection<T> Root { get; private set; }

        public TreeNode<T> Add(T item)
        {
            var wrapper = new TreeNode<T>(this, item);
            AddInternal(wrapper);

            if (Comparer != null)
            {
                int i;
                for (i = 0; i < _children.Count && Comparer.Compare(_children[i].Value, item) <= 0; ++i) ;
                _children.Insert(i, wrapper);
            }
            else
            {
                _children.Add(wrapper);
            }

            OnCollectionAdd(wrapper);
            return wrapper;
        }

        public IEnumerable<Tuple<T, TreeNode<T>>> AddRange(IEnumerable<T> items)
        {
            var result = new List<Tuple<T, TreeNode<T>>>();
            var wrappers = new List<TreeNode<T>>();

            foreach (var item in items)
            {
                var wrapper = new TreeNode<T>(this, item);
                AddInternal(wrapper);

                if (Comparer != null)
                {
                    int i;
                    for (i = 0; i < _children.Count && Comparer.Compare(_children[i].Value, item) <= 0; ++i) ;
                    _children.Insert(i, wrapper);
                }
                else
                {
                    _children.Add(wrapper);
                }

                result.Add(new Tuple<T, TreeNode<T>>(item, wrapper));
                wrappers.Add(wrapper);
            }

            OnCollectionAdd(wrappers);
            return result;
        }

        private bool _filterApplicationComplete;
        private Func<T, IEnumerable<T>> _loadChildren;

        public bool FilterApplicationComplete
        {
            get { return _filterApplicationComplete; }
            set
            {
                _filterApplicationComplete = value;
                OnPropertyChanged();
                OnPropertyChanged("IsSearching");
            }
        }

        public void ApplyFilter(Func<TreeNode<T>, bool> filter)
        {
            Task.Run(() =>
            {
                //My cancellation token
                var cts = new CancellationTokenSource();
                //Their cancellation token
                var token = Interlocked.Exchange(ref _cancellationTokenSource, cts);
                //Cancel theirs
                token.Cancel(false);

                //My completion token
                var tcs = new TaskCompletionSource<bool>();
                var cancelled = false;

                try
                {
                    //Their completion token
                    var complete = Interlocked.Exchange(ref _filterComplete, tcs);
                    
                    if (!complete.Task.IsCompleted)
                    {
                        //Wait for their completion
                        complete.Task.Wait();
                    }

                    OnPropertyChanged("IsSearching");

                    Parallel.ForEach(_allItems, new ParallelOptions {CancellationToken = cts.Token}, x =>
                    {
                        if (x.MatchesFilterInternal ^ filter(x))
                        {
                            x.MatchesFilter = !x.MatchesFilterInternal;
                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    cancelled = true;
                }
                catch
                {
                    
                }
                finally
                {
                    tcs.TrySetResult(true);
                    FilterApplicationComplete = !cancelled;
                }
            });
        }

        public void Clear()
        {
            if (_children.Count == 0)
            {
                return;
            }

            OnCollectionRemove(_children);

            if (!IsRoot)
            {
                foreach (var item in _children)
                {
                    RemoveInternal(item);
                }
            }

            foreach (var child in _children)
            {
                child.Clear();
            }

            if (IsRoot)
            {
                _allItems.Clear();
            }

            _children.Clear();
        }

        public bool UseFiltering
        {
            get
            {
                return Root._useFiltering;
            }
            set
            {
                var oldValue = Root._useFiltering;
                Root._useFiltering = value;

                if (oldValue ^ Root._useFiltering)
                {
                    OnPropertyChanged();

                    foreach (var item in _allItems)
                    {
                        item.OnPropertyChanged();
                    }
                }
            }
        }

        public bool Contains(TreeNode<T> item)
        {
            return _children.Contains(item);
        }

        IEnumerator<ITreeNode> IEnumerable<ITreeNode>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TreeNode<T> item)
        {
            RemoveInternal(item);

            if (_children.Remove(item))
            {
                OnCollectionRemove(item);
                return true;
            }

            return false;
        }

        public void RemoveWhere(Func<TreeNode<T>, bool> predicate)
        {
            var matches = _children.Where(predicate).ToList();

            if (matches.Count > 0)
            {
                OnCollectionRemove(matches);
            }

            foreach (var match in matches)
            {
                RemoveInternal(match);
                _children.Remove(match);
            }
        }

        protected virtual void OnCollectionAdd(params TreeNode<T>[] newItems)
        {
            OnCollectionAdd((IList<TreeNode<T>>)newItems);
        }

        protected virtual void OnCollectionRemove(params TreeNode<T>[] removedItems)
        {
            OnCollectionRemove((IList<TreeNode<T>>)removedItems);
        }

        private void AddInternal(TreeNode<T> item)
        {
            Root._allItems.Add(item);
            Root._allItemsRaw.Add(item.Value);

            if (LoadChildren != null)
            {
                item.AddRange(LoadChildren(item.Value));
            }
        }

        private void OnCollectionAdd(IList<TreeNode<T>> newItems)
        {
            if (IsSuspendingUpdates)
            {
                return;
            }

            var handler = CollectionChanged;

            if (handler != null)
            {
                foreach (var item in newItems)
                {
                    handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _children.IndexOf(item)));
                }
            }
        }

        private void OnCollectionRemove(IList<TreeNode<T>> removedItems)
        {
            if (IsSuspendingUpdates)
            {
                return;
            }

            var handler = CollectionChanged;

            if (handler != null)
            {
                foreach (var item in removedItems)
                {
                    handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, _children.IndexOf(item)));
                }
            }
        }

        private void RemoveInternal(TreeNode<T> item)
        {
            Root._allItems.Remove(item);
            Root._allItemsRaw.Remove(item.Value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
