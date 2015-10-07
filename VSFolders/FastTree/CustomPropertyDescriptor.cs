using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.VSFolders.FastTree
{
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private readonly ITreeNode _owner;
        private readonly PropertyInfo _property;
        private readonly bool _isValueProperty;

        public bool IsValueProperty{get { return _isValueProperty; }}

        public CustomPropertyDescriptor(bool isValueProperty, ITreeNode instance, PropertyInfo prop)
            : base(prop.Name, prop.GetCustomAttributes().ToArray())
        {
            _isValueProperty = isValueProperty;
            _owner = instance;
            _property = prop;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            if (_isValueProperty)
            {
                return _property.GetValue(_owner.Value);
            }

            return _property.GetValue(_owner);
        }

        public override void ResetValue(object component)
        {
            throw new NotSupportedException();
        }

        public override void SetValue(object component, object value)
        {
            if (_isValueProperty)
            {
                _property.SetValue(_owner.Value, value);
                return;
            }

            _property.SetValue(_owner, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { throw new NotSupportedException(); }
        }

        public override bool IsReadOnly
        {
            get { return _property.SetMethod == null; }
        }

        public override Type PropertyType
        {
            get { return _property.PropertyType; }
        }

        public override string ToString()
        {
            return _property.ToString();
        }
    }
}