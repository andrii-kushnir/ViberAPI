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

        public static void SetRoundedShape(Control control)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            float h2 = control.Height / 2f;
            if (control.Height <= 20)
            {
                path.AddLine(h2, 0, control.Width - h2, 0);
                path.AddArc(control.Width - control.Height, 0, control.Height, control.Height, 270, 180);
                path.AddLine(control.Width - h2, control.Height, h2, control.Height);
                path.AddArc(0, 0, control.Height, control.Height, 90, 180);
            }
            else
            {
                path.AddArc(0, 0, 10, 10, 180, 90);
                path.AddLine(5, 0, control.Width - 5, 0);
                path.AddArc(control.Width - 10, 0, 10, 10, 270, 90);
                path.AddLine(control.Width, 5, control.Width, control.Height - 5);
                path.AddArc(control.Width - 10, control.Height - 10, 10, 10, 0, 90);
                path.AddLine(control.Width - 5, control.Height, 5, control.Height);
                path.AddArc(0, control.Height - 10, 10, 10, 90, 90);
                path.AddLine(0, control.Height - 5, 0, 5);
            }
            control.Region = new Region(path);
        }

        private static string[] MonthNames = { "січня", "лютого", "березня", "квітня", "травня", "червня", "липня", "серпня", "вересня", "жовтня", "листопада", "грудня" };
        private static int TimeHeight = 10;

        public static void ShowDateLabel(Panel parent, DateTime date, ref int posOnPanel)
        {
            var label = new Label()
            {
                Parent = parent,
                Text = $"  {date.Day} {MonthNames[date.Month - 1]} {date.Year}  ",
                Font = new Font("Times New Roman", 9.75F, FontStyle.Italic, GraphicsUnit.Point, ((byte)(204))),
                ForeColor = Color.Black,
                BackColor = Color.Plum,
                AutoSize = true
            };
            label.Location = new Point((parent.Width - label.Width) / 2, posOnPanel - parent.VerticalScroll.Value);
            SetRoundedShape(label);
            posOnPanel += label.Height + 1;
        }

        public static Control ShowMessageTime(Panel parent, Control control, DateTime date)
        {
            var labelTime = new Label()
            {
                Parent = parent,
                Text = date.ToString("HH:mm"),
                Font = new Font("Times New Roman", 6.75F, FontStyle.Italic, GraphicsUnit.Point, ((byte)(204))),
                ForeColor = Color.Black,
                BackColor = Color.White,
                Location = new Point(control.Right + 1, control.Bottom - TimeHeight),
                AutoSize = true
            };
            SetRoundedShape(labelTime);
            return labelTime;
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
