using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VSFolders.FastTree
{
    [TypeDescriptionProvider(typeof(TreeNodeTypeDescriptionProvider))]
    public class TreeNode<T> : TreeCollection<T>, ITreeNode, IEquatable<TreeNode<T>>
    {
        private T _value;

        internal TreeNode(TreeCollection<T> owner, T value)
            : base(owner)
        {
            Value = value;
            Parent = owner as TreeNode<T>;
            MatchesFilter = true;

            Root.PropertyChanged += RootPropertyChanged;

            var inp = value as INotifyPropertyChanged;

            if (inp != null)
            {
                inp.PropertyChanged += ProxyPropertyChanged;
            }

            var fte = value as IFastTreeEnlightened<T>;

            if (fte != null)
            {
                fte.TreeNode = this;
            }
        }

        private void RootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ExpandOnFilterMatch")
            {
                OnPropertyChanged("IsExpanded");
            }
            else if (e.PropertyName == "__DISPLAY_PROPERTY_REFRESH__")
            {
                OnPropertyChanged("Self");
            }
        }

        private void ProxyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        internal bool MatchesFilterInternal { get; private set; }

        public bool MatchesFilter
        {
            get { return MatchesFilterInternal || this.Any<TreeNode<T>>(x => x.MatchesFilter); }
            set
            {
                var oldValue = MatchesFilterInternal;
                MatchesFilterInternal = value;

                if (UseFiltering && ExpandOnFilterMatch)
                {
                    IsExpanded = MatchesFilter;
                }

                if (!ReferenceEquals(Parent, null))
                {
                    Parent.MatchesFilter = Parent.MatchesFilterInternal;
                    Parent.OnPropertyChanged();
                }

                if (!Equals(oldValue, MatchesFilterInternal))
                {
                    OnPropertyChanged();
                }
            }
        }

        public Type ElementType { get { return typeof (T); } }
        
        public ITreeNode Self { get { return this; } }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            base.OnPropertyChanged("Self");
        }

        public object this[string name]
        {
            get { return typeof (T).GetProperty(name).GetValue(Value); }
            set { typeof (T).GetProperty(name).SetValue(Value, value); }
        }

        public bool IsExpanded
        {
            get
            {
                var decisionBasis = "IsExpanded";

                if (UseFiltering && ExpandOnFilterMatch)
                {
                    decisionBasis = "IsFilterExpanded";
                }

                return (HasProperty(decisionBasis) && (this[decisionBasis] is bool && (bool)this[decisionBasis]));
            }
            set
            {
                var assign = "IsExpanded";

                if (UseFiltering && ExpandOnFilterMatch)
                {
                    assign = "IsFilterExpanded";
                }

                if (HasProperty(assign))
                {
                    this[assign] = value;
                }
            }
        }

        public bool HasProperty(string name)
        {
            return typeof (T).GetProperty(name) != null;
        }

        public TreeNode<T> Parent { get; private set; }

        public T Value
        {
            get { return _value; }
            set
            {
                var oldValue = _value;
                _value = value;

                if (!Equals(oldValue, value))
                {
                    var oldInp = oldValue as INotifyPropertyChanged;

                    if (oldInp != null)
                    {
                        oldInp.PropertyChanged -= ProxyPropertyChanged;
                    }

                    OnPropertyChanged();
                }

                var newInp = value as INotifyPropertyChanged;

                if (newInp != null)
                {
                    newInp.PropertyChanged += ProxyPropertyChanged;
                }

                var fte = value as IFastTreeEnlightened<T>;

                if (fte != null)
                {
                    fte.TreeNode = this;
                }
            }
        }

        object ITreeNode.Value
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        IEnumerable<ITreeNode> ITreeNode.Children
        {
            get { return ChildValues; }
        }

        public void Invalidate()
        {
            Clear();

            if (LoadChildren != null)
            {
                AddRange(LoadChildren(Value));
            }
        }

        public bool Equals(TreeNode<T> other)
        {
            return !ReferenceEquals(other, null) && (ReferenceEquals(other, this) || ReferenceEquals(other.Value, Value) || Equals(Value, other));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TreeNode<T>);
        }

        public override int GetHashCode()
        {
            return ReferenceEquals(Value, null) ? 0 : Value.GetHashCode();
        }

        public override string ToString()
        {
            return !ReferenceEquals(Value, null) ? Value.ToString() : typeof(T).ToString();
        }
    }
}