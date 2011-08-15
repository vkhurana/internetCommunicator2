/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace InterComm.Helpers
{
    class GlassHelper
    {
        /// <summary>
        /// Gets whether dwmapi.dll is present and DWM functions can be used
        /// </summary>
        public static bool IsDwmCompositionAvailable
        {
            get
            {
                // Vista is version 6.  Don't do aero stuff if not >= Vista because dwmapi.dll won't exist
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        /// <summary>
        /// Gets whether DWM is enabled
        /// </summary>
        public static bool IsDwmCompositionEnabled
        {
            get
            {
                // Make sure dwmapi.dll is present.  If not, calling DwmIsCompositionEnabled will throw an exception
                if (!IsDwmCompositionAvailable)
                    return false;
                return Interop.DwmIsCompositionEnabled();
            }
        }

        /// <summary>
        /// Extends the glass frame of a window
        /// </summary>
        public static bool ExtendGlassFrame(Window window, Thickness margin)
        {
            if (!IsDwmCompositionEnabled)
                return false;

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("The Window must be shown before extending glass.");

            HwndSource source = HwndSource.FromHwnd(hwnd);

            // Set the background to transparent from both the WPF and Win32 perspectives
            window.Background = Brushes.Transparent;
            source.CompositionTarget.BackgroundColor = Colors.Transparent;

            Interop.Margins margins = new Interop.Margins(margin);
            Interop.DwmExtendFrameIntoClientArea(hwnd, ref margins);
            return true;
        }

    }
}
*/