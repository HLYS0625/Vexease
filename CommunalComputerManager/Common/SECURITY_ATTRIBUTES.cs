﻿using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace CommunalComputerManager.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public uint nLength;
        public UIntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }
}
