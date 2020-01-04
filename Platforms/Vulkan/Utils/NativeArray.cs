using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    internal unsafe class NativeArray<T> : IDisposable where T : NativeValue
    {
        public readonly uint Length;
        public readonly IntPtr Pointer;

        private readonly T[] array;
        private bool disposed;

        public NativeArray(int length)
        {
            Length = (uint)length;
            Pointer = Marshal.AllocHGlobal(sizeof(IntPtr) * length);
            array = new T[length];
        }

        ~NativeArray()
        {
            Dispose();
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException();

                return array[index];
            }

            set
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException();

                array[index] = value;
                ((byte**)Pointer)[index] = value;
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Marshal.FreeHGlobal(Pointer);
            }
        }

        public static implicit operator byte**(NativeArray<T> arr) => (byte**)arr.Pointer;
    }
}
