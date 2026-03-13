using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace Sol.ShieldOfFaith
{
    public partial class ProtectorPanel : Panel
    {
        public ProtectorPanel()
        {
            Header = new Label();
            Header.Dock = DockStyle.Top;
            Header.AutoSize = false;
            Controls.Add(Header);
        }

        bool twinkling_controls;
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (twinkling_controls)
                return;
            twinkling_controls = true;
            try { Header.SendToBack(); }
            finally { twinkling_controls = false; }
        }

        bool autoheaderheight, recalculateheader;
        public bool AdjustHeaderHeightToText
        {
            get => autoheaderheight;
            set
            {
                if (autoheaderheight == value)
                    return;
                autoheaderheight = value;
                recalculateheader = false;
                if (value)
                {
                    Header.TextChanged += Header_TextChanged;
                    Header.FontChanged += Header_TextChanged;
                    Header_TextChanged(null, EventArgs.Empty);
                }
            }
        }

        private void Header_TextChanged(object sender, EventArgs e)
        {
            if (recalculateheader)
                return;
            recalculateheader = true;
            Header.Paint += Header_Paint;
            if (Header.Visible) Header.Invalidate();
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var h = Header;
            h.Paint -= Header_Paint;

            if (!recalculateheader) return;

            recalculateheader = false;

            if (autoheaderheight)
            {
                var dim = (int)Math.Ceiling(e.Graphics.MeasureString(h.Text, h.Font).Height + h.Padding.Top + h.Padding.Bottom);
                if (h.Height != dim)
                    h.Height = dim;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Label Header { get; private set; }

        Image headerImage;
        public Image HeaderImage
        {
            get => headerImage;
            set
            {
                if (headerImage == value) return;

                if (value == null)
                {
                    Header.Paint -= HeaderLabel_Paint;
                }
                else if (headerImage == null)
                {
                    Header.Paint += HeaderLabel_Paint;
                }

                headerImage = value;

                if (Header.Visible)
                    Header.Invalidate();
            }
        }

        private void HeaderLabel_Paint(object sender, PaintEventArgs e)
        {
            var h = Header.Height;
            var w = Header.Width;
            var p = Header.Padding;

            try
            {
                float iw = headerImage.Width;
                float ih = headerImage.Height;

                if (ih > h)
                {
                    iw *= h / ih;
                    ih = h;
                }

                float t = p.Top;
                if (t + ih > h)
                    t = h - ih;

                e.Graphics.DrawImage(headerImage, new RectangleF(w - p.Right - iw, t, iw, ih));
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
        }
    }
}
