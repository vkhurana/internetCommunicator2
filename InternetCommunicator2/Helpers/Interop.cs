/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

namespace InterComm.Helpers
{
    class Interop
    {
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        /// <summary>
        /// The margins struct has to be defined in C#, maintain T, L, R, B info
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;

            public Margins(Thickness t)
            {
                Left = (int)t.Left;
                Right = (int)t.Right;
                Top = (int)t.Top;
                Bottom = (int)t.Bottom;
            }
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();
    }
}
*/