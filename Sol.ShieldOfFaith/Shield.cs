using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Sol.ShieldOfFaith
{
    public partial class Shield : Form
    {
        enum GWL : int { ExStyle = -20 }
        enum WS_EX : int
        {
            Transparent = 0x20,
            Layered = 0x80000
        }
        enum LWA : int
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

        const uint
            SWP_NOMOVE = 0x0002,
            SWP_NOSIZE = 0x0001,
            SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowLong(IntPtr hWnd, GWL nIndex);
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hWn, GWL nIndex, IntPtr assign);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(
            IntPtr hWnd,
            int crKey,
            byte alpha,
               LWA dwFlags
            );

        bool loaded;

        private void Shield_Load(object sender, EventArgs e)
        {
            var attr = GetWindowLong(Handle, GWL.ExStyle);
            attr = new IntPtr(attr.ToInt64() | (long)WS_EX.Transparent | (long)WS_EX.Layered);
            SetWindowLong(Handle, GWL.ExStyle, attr);
            SetIntensity();
            loaded = true;
        }

        byte opacity;
        public byte Intensity
        {
            get => opacity;
            set
            {
                opacity = value;
                if (loaded) SetIntensity();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
                SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
        }

        void SetIntensity() => SetLayeredWindowAttributes(Handle, 0, opacity, LWA.Alpha);


        public Shield()
        {
            InitializeComponent();
        }
    }
}
