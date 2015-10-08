// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidList.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   GuidList.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System;

    internal static class GuidList
    {
        public const string GuidVsFoldersPkgString = "bfddff91-960d-410f-9edf-9a53f100d7fa";

        public const string GuidVsFoldersCmdSetString = "39720495-37c2-4c45-9822-5f04b1800e06";

        public const string GuidToolWindowFoldersPersistanceString = "c65ed792-6d5d-4fe6-8332-454b2d65e4d6";

        public const string GuidToolWindowConsolePersistanceString = "3BB2E1FA-9ADE-4B04-99CA-C3BB37276220";

        public static readonly Guid GuidVsFoldersCmdSet = new Guid(GuidList.GuidVsFoldersCmdSetString);
    };
}