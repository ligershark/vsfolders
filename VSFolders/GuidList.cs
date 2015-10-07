// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.VSFolders
{
    static class GuidList
    {
        public const string guidVSFoldersPkgString = "bfddff91-960d-410f-9edf-9a53f100d7fa";
        public const string guidVSFoldersCmdSetString = "39720495-37c2-4c45-9822-5f04b1800e06";
        public const string guidToolWindowFoldersPersistanceString = "c65ed792-6d5d-4fe6-8332-454b2d65e4d6";
        public const string guidToolWindowConsolePersistanceString = "3BB2E1FA-9ADE-4B04-99CA-C3BB37276220";

        public static readonly Guid guidVSFoldersCmdSet = new Guid(guidVSFoldersCmdSetString);
    };
}