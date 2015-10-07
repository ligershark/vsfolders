using System.ComponentModel;
using System.Linq;

namespace Microsoft.VSFolders.FastTree
{
    public class TreeNodeTypeDescriptor : CustomTypeDescriptor
    {
        private readonly ITreeNode _instance;
        private PropertyDescriptorCollection _allProps;

        public TreeNodeTypeDescriptor(ITreeNode instance)
        {
            _instance = instance;
            ComputeAllProps();
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            var prop = (CustomPropertyDescriptor) pd;

            if (prop.IsValueProperty)
            {
                return _instance.Value;
            }

            return _instance;
        }

        private void ComputeAllProps()
        {
            if (_instance == null)
            {
                _allProps = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
                return;
            }

            var instType = _instance.GetType();

            var allProps = instType.GetProperties()
                                   .Select(x => new CustomPropertyDescriptor(false, _instance, x))
                                   .Union(
                                       _instance.ElementType.GetProperties()
                                                .Select(x => new CustomPropertyDescriptor(true, _instance, x))
                )
                                   .ToArray<PropertyDescriptor>();
            _allProps = new PropertyDescriptorCollection(allProps);
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return _allProps;
        }
    }
}