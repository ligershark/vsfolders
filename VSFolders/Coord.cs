// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coord.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   Coord.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    public struct Coord
    {
        [FieldOffset(0)] public ushort X;

        [FieldOffset(2)] public ushort Y;
    }
}