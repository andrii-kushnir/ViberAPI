using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Arsenium
{
    public static class Infrastructure
    {
        private static bool connect = true;
        public static Image GetIconFromWebImage(this string fromUrl)
        {
            if (!connect || String.IsNullOrWhiteSpace(fromUrl))
                return null;
            using (var client = new WebClient())
                try
                {
                    using (var stream = client.OpenRead(new Uri(fromUrl)))
                    {
                        var image = Image.FromStream(stream, true);
                        return image;
                        //using (var img = (Bitmap)Image.FromStream(stream, true))
                        //{
                        //    return Icon.FromHandle(img.GetHicon());
                        //}
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ConnectFailure)
                        connect = false;
                    return null;
                }
        }
    }

    public static class InvokeHelper
    {
        public static void SafeInvoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
                return;
            }
            action();
        }

        public static void SafeInvoke<T>(this Control control, Action<T> action, T obj)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action, obj);
                return;
            }
            action(obj);
        }

        public static void SafeInvoke(this Control control, Action<object, object, object> action, object obj1, object obj2, object obj3)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action, obj1, obj2, obj3);
                return;
            }
            action(obj1, obj2, obj3);
        }

        public static void SafeAsyncInvoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
                return;
            }
            action();
        }

        public static void SafeAsyncInvoke(this Control control, Action<object> action, object obj)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action, obj);
                return;
            }
            action(obj);
        }

        public static void SafeAsyncInvoke(this Control control, Action<object, object> action, object obj1, object obj2)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action, obj1, obj2);
                return;
            }
            action(obj1, obj2);
        }

        public static void SafeAsyncInvoke(this Control control, Action<object, object, object> action, object obj1, object obj2, object obj3)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action, obj1, obj2, obj3);
                return;
            }
            action(obj1, obj2, obj3);
        }
    }
}
