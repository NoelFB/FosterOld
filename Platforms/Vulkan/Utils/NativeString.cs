using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Foster.Vulkan
{
    // Native UTF-8 String for Vulkan
    internal unsafe class NativeString : NativeValue
    {
        public NativeString(string s) : base(Encoding.UTF8.GetBytes(s))
        {

        }

        private string GetString()
        {
            return Encoding.UTF8.GetString(Pointer, (int)Size);
        }

        public static implicit operator NativeString(string s) => new NativeString(s);
        public static implicit operator string(NativeString utf8String) => utf8String.GetString();
    }
}
