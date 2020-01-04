﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    internal abstract unsafe class NativeValue : IDisposable
    {
        private GCHandle handle;

        public byte* Pointer => (byte*)handle.AddrOfPinnedObject().ToPointer();
        public readonly int Size;

        public NativeValue(byte[] data)
        {
            handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Size = data.Length;
        }

        ~NativeValue()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (handle.IsAllocated)
                handle.Free();
        }

        public static implicit operator byte*(NativeValue utf8String) => utf8String.Pointer;
        public static implicit operator IntPtr(NativeValue utf8String) => new IntPtr(utf8String.Pointer);
    }
}
