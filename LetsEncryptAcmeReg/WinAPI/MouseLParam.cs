using System;

namespace LetsEncryptAcmeReg.WinAPI
{
    public struct MouseLParam
    {
        public readonly short X;
        public readonly short Y;

        private MouseLParam(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }

        public static explicit operator MouseLParam(IntPtr v)
        {
            return new MouseLParam(
                (short)(ushort)(uint)v.ToInt32(),
                (short)(ushort)((uint)v.ToInt32() >> 16));
        }
    }
}