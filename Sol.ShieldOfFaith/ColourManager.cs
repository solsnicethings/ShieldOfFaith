using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sol.ShieldOfFaith
{
    public partial class ColourManager : UserControl
    {
        public ColourManager()
        {
            InitializeComponent();
            Value = Color.Gray;
            buttonPrevC.BackColor = BackColor;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int w = 0, h = 0;

            void account(params Control[] ctl)
            {
                foreach (var c in ctl)
                    if (c.Visible)
                    {
                        h += c.Height;
                        var s = c.GetPreferredSize(proposedSize);
                        if (s.Width > w) w = s.Width;
                    }
            }
            account(trackB, trackR, trackG, trackBoxAlpha);

            if (panelColour.Visible)
            {
                int ph;
                if (panelPrevious.Visible) ph = panelPrevious.Top;
                else ph = 0;
                var s = buttonPickColour.GetPreferredSize(proposedSize);
                ph += s.Height;
                w += s.Width;

                if (ph > h) h = ph;
            }

            if (h < proposedSize.Height && h < 256)
                h = 256;
            if (w < proposedSize.Width)
                w = proposedSize.Width;

            return new Size(w, h);
        }

        Func<Font, Font> bar_font = f => new Font(f, FontStyle.Bold);

        bool no_reenter;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Func<Font,Font> BarFont
        {
            get => bar_font;
            set
            {
                if (bar_font != value)
                {
                    bar_font = value;
                    if (no_reenter) return;
                    OnFontChanged(EventArgs.Empty);
                }
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (bar_font != null && !no_reenter && !DesignMode)
            {
                no_reenter = true;
                try
                {
                    var f = bar_font(Font);
                    trackB.Font = f;
                    trackG.Font = f;
                    trackR.Font = f;
                    trackBoxAlpha.Font = f;
                }
                finally { no_reenter = false; }
            }
        }

        Color value ;
        bool updatingUI, currentIsPicked = true;

        public event EventHandler ValueChanged;

        public override Color BackColor { get => base.BackColor; set { base.BackColor = value; if (buttonPrevC != null) buttonPrevC.BackColor = value; } }

        public Color Value
        {
            get => value;
            set
            {
                if (Value == value)
                    return;
                updatingUI = true;
                try
                {
                    if (currentIsPicked)
                    {
                        panelPrevious.BackColor = Color.FromArgb(255, this.value);
                        currentIsPicked = false;
                    }
                    trackB.Value = value.B;
                    trackR.Value = value.R;
                    trackG.Value = value.G;
                    trackBoxAlpha.Value = value.A;
                    panelColour.BackColor = Color.FromArgb(255, value);
                    this.value = value;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
                finally { updatingUI = false; }
            }
        }

        private void buttonPrevC_Click(object sender, EventArgs e)
        {
            if (panelPrevious.BackColor != Value)
            {
                currentIsPicked = true;
                Value = Color.FromArgb(trackBoxAlpha.Value, panelPrevious.BackColor);
                currentIsPicked = true;
            }
        }

        private void track_ValueChanged(object sender, EventArgs e)
        {
            if (updatingUI)
                return;
            Value = Color.FromArgb(trackBoxAlpha.Value, trackR.Value, trackG.Value, trackB.Value);
        }


        private void buttonPickColour_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog(FindForm()) == DialogResult.OK && colorDialog1.Color != Value)
            {
                currentIsPicked = true;
                Value = Color.FromArgb(trackBoxAlpha.Value, colorDialog1.Color);
                currentIsPicked = true;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TrackBox.Composite BarRed => trackR;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TrackBox.Composite BarGreen => trackG;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TrackBox.Composite BarBlue => trackB;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TrackBox.Composite BarAlpha => trackBoxAlpha;

        public bool DisplayPreviousPick
        {
            get => panelPrevious.Visible ;
            set =>panelPrevious.Visible = value;
        }
        public bool DisplayColourPick
        {
            get => panelColour.Visible;
            set => panelColour.Visible = value;
        }
        public bool DisplayAlpha
        {
            get => trackBoxAlpha.Visible;
            set => trackBoxAlpha.Visible = value;
        }
        public bool DisplayRGB
        {
            get => trackB.Visible;
            set
            {
                trackB.Visible = value;
                trackG.Visible = value;
                trackR.Visible = value;
            }
        }
    }
}
