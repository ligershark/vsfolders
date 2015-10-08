// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextMenuService.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   ContextMenuService.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using ContextMenu;

    public class ContextMenuService
    {
        private readonly ConcurrentDictionary<Type, List<Func<object, IEnumerable<IContextMenuItem>>>> _Handlers =
            new ConcurrentDictionary<Type, List<Func<object, IEnumerable<IContextMenuItem>>>>();

        public void RegisterHandler<T>(Func<object, IEnumerable<IContextMenuItem>> handler)
        {
            List<Func<object, IEnumerable<IContextMenuItem>>> handlers = this._Handlers.GetOrAdd(
                typeof (T), 
                t => new List<Func<object, IEnumerable<IContextMenuItem>>>());
            handlers.Add(handler);
        }

        public IEnumerable<IContextMenuItem> GetMenuItems(object obj)
        {
            List<Func<object, IEnumerable<IContextMenuItem>>> handlers;
            if (this._Handlers.TryGetValue(obj.GetType(), out handlers))
            {
                return handlers.SelectMany(x => x(obj));
            }

            return new IContextMenuItem[0];
        }
    }
}