using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sol.ShieldOfFaith
{
    static class FormsUtil
    {
        static public Rectangle UsedContentRegion(this Control c)
        {
            bool any = false;
            int t = 0, b = 0, l = 0, r = 0;

            foreach (Control child in c.Controls)
            {
                if (!child.Visible) continue;
                var bounds = child.Bounds;
                if (any)
                {
                    if (t > child.Top)
                        t = child.Top;
                    if (b < child.Bottom)
                        b = child.Bottom;
                    if (l > child.Left)
                        l = child.Left;
                    if (r < child.Right)
                        r = child.Right;
                }
                else
                {
                    any = true;
                    t = bounds.Top;
                    b = bounds.Bottom;
                    l = bounds.Left;
                    r = bounds.Right;
                }
            }

            return new Rectangle(l, t, r - l, b - t);
        }

        static public Color Mix(Color a, Color b, double keep = 0.5)
        {
            double blend = 1.0 - keep;
            return Color.FromArgb((int)(a.A * keep + b.A * blend),
                (int)(a.R * keep + b.R * blend),
                (int)(a.G * keep + b.G * blend),
                (int)(a.B * keep + b.B * blend));
        }

        #region https://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        // http://www.codeproject.com/KB/cs/csharpmovewindow.aspx
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        static public void FormOrControl_MouseDown_LikeMovableTitleBar(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Control c = (Control)sender;
            if (!(c is Form)) c = c.FindForm();
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(c.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

    }
}
