using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.VSFolders
{
    public class ContextMenuService
    {
        private ConcurrentDictionary<Type, List<Func<object, IEnumerable<IContextMenuItem>>>> _Handlers = new ConcurrentDictionary<Type, List<Func<object, IEnumerable<IContextMenuItem>>>>();
        public void RegisterHandler<T>(Func<object, IEnumerable<IContextMenuItem>> handler)
        {
            var handlers = _Handlers.GetOrAdd(typeof(T), t => new List<Func<object, IEnumerable<IContextMenuItem>>>());
            handlers.Add(handler);
        }

        public IEnumerable<IContextMenuItem> GetMenuItems(object obj)
        {
            List<Func<object, IEnumerable<IContextMenuItem>>> handlers;
            if (_Handlers.TryGetValue(obj.GetType(), out handlers))
                return handlers.SelectMany(x => x(obj));

            return new IContextMenuItem[0];
        }

        public Func<FrameworkElement> ContextMenuFrameworkElement { get; set; }
    }
}
