namespace HotTotemAssets.EpicMenu.DPI {
    class EpicMenuWindowsDPIHandler {
#if UNITY_EDITOR_WIN
        [System.Runtime.InteropServices.DllImport ("gdi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps (System.IntPtr hDC, int nIndex);

        [System.Runtime.InteropServices.DllImport ("user32.dll")]
        static extern int GetDpiForWindow (System.IntPtr hWnd);

        public enum DeviceCap {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,
        }

        public static float GetDPI () {
#if !UNITY_2018_2_OR_NEWER
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(System.IntPtr.Zero);
            System.IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            g.ReleaseHdc();
            return screenScalingFactor;
#else
            return 1f;
#endif
        }
#endif
    }
}