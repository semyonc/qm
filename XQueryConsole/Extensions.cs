using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Data;

namespace XQueryConsole
{
    /// <summary>
    /// See http://efreedom.com/Question/1-315164/Use-FolderBrowserDialog-WPF-Application 
    /// </summary>
    public static class Extensions
    {
        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly IntPtr _handle;

            public OldWindow(IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cxTopHeight;
            public int cxBottomHeight;
        }

        [DllImport("DwmApi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        public static Margins GetDpiAdjustedMargins(IntPtr hWnd, double left, double right, double top, double bottom)
        {
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(hWnd);
            float desktopDpiX = g.DpiX;
            float desktopDpiY = g.DpiY;
            
            Margins margins = new Margins();
            margins.cxLeftWidth = Convert.ToInt32(left * (desktopDpiX / 96));
            margins.cxRightWidth = Convert.ToInt32(right * (desktopDpiX / 96));
            margins.cxTopHeight = Convert.ToInt32(top * (desktopDpiY / 96));
            margins.cxBottomHeight = Convert.ToInt32(bottom * (desktopDpiY / 96));

            return margins;
        }        

        public static System.Drawing.Image GetImageFromResourceUri(string uri)
        {
            System.Windows.Resources.StreamResourceInfo sri = System.Windows.Application.GetResourceStream(
                new Uri(uri, UriKind.Relative));
            System.Drawing.Image image = System.Drawing.Image.FromStream(sri.Stream);
            return image;
        }

        public static string GetAppDataPath()
        {
            string dir = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "WmHelp\\SQLEngine");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static void ShellExecute(string command)
        {
            try
            {
                Process.Start(command);
            }
            catch
            {
            }
        }

        public static void SetTreeViewTheme(IntPtr handle)
        {
            try
            {
                SetWindowTheme(handle, "explorer", null);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        //
        // http://stackoverflow.com/questions/665719/wpf-animate-listbox-scrollviewer-horizontaloffset/665798#665798
        //
        public static childItem FindVisualChild<childItem>(DependencyObject obj)
           where childItem : DependencyObject
        {
            // Search immediate children first (breadth-first)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// http://stackoverflow.com/questions/397556/wpf-how-to-bind-radiobuttons-to-an-enum
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.Equals(false))
                return Binding.DoNothing;
            else
                return parameter;
        }
    }
}
