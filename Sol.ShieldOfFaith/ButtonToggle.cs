using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sol.ShieldOfFaith
{
    public class EventArgsColourSelect  :EventArgs
    {
        public Color
            SelectForeground,
            SelectBackground;
    }

    public delegate void StateColourReevaluate(object sender, EventArgsColourSelect select);

    [DefaultEvent("Toggled")]
    public class ButtonToggle : Panel
    {
        class Buttoff : Button
        {
            new public Size DefaultSize => base.DefaultSize;
            new public Cursor DefaultCursor => base.DefaultCursor;
            new public ImeMode DefaultImeMode => base.DefaultImeMode;
            new public Padding DefaultMargin => base.DefaultMargin;
            new public Size DefaultMaximumSize => base.DefaultMaximumSize;
            new public Size DefaultMinimumSize => base.DefaultMinimumSize;
            new public Padding DefaultPadding => base.DefaultPadding;
        }

        void ReevaluateToggleColour()
        {
            var e = new EventArgsColourSelect();
            if (ToggledOn)
            {
                switch (tForeground.A)
                {
                    case 255:
                        e.SelectForeground = tForeground;
                        break;
                    case 0:
                        e.SelectForeground = BackColor;
                        break;
                    default:
                        e.SelectForeground = FormsUtil.Mix(Color.FromArgb(255, tForeground), BackColor, tForeground.A / 255.0);
                        break;
                }
                switch (tBackground.A)
                {
                    case 255:
                        e.SelectBackground = tBackground;
                        break;
                    case 0:
                        e.SelectBackground = ForeColor;
                        break;
                    default:
                        e.SelectBackground = FormsUtil.Mix(Color.FromArgb(255, tBackground), ForeColor, tBackground.A / 255.0);
                        break;
                }
            }
            else
            {
                e.SelectForeground = ForeColor;
                e.SelectBackground = BackColor;
            }
            ToggleColourReevaluate?.Invoke(this, e);
            buttoff.ForeColor = e.SelectForeground;
            buttoff.BackColor = e.SelectBackground;
        }

        Color tForeground = Color.Transparent, tBackground = Color.Transparent;

        public Color ToggledForeColour
        {
            get => tForeground;
            set
            {
                if (value == tForeground)
                    return;
                tForeground = value;
                if (ToggledOn)
                    ReevaluateToggleColour();
            }
        }
        public Color ToggledBackColour
        {
            get => tBackground;
            set
            {
                if (value == tBackground)
                    return;
                tBackground = value;
                if (ToggledOn)
                    ReevaluateToggleColour();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            ReevaluateToggleColour();
        }

        public event StateColourReevaluate ToggleColourReevaluate;
        public event EventHandler Toggled;
        readonly Buttoff buttoff = new Buttoff();
        bool button;

        public ButtonToggle()
        {
            buttoff.Click += Buttoff_Click;
            buttoff.Dock = DockStyle.Fill;
            Controls.Add(buttoff);

            base.BorderStyle = BorderStyle.None;
        }

        public bool AutoEllipsis { get => buttoff.AutoEllipsis; set => buttoff.AutoEllipsis = value; }
        public DialogResult DialogResult { get => buttoff.DialogResult; set => buttoff.DialogResult = value; }
        public FlatStyle FlatStyle { get => buttoff.FlatStyle;set=>buttoff.FlatStyle = value; }

        public Image Image { get => buttoff.Image; set => buttoff.Image = value; }
        public ContentAlignment ImageAlign { get => buttoff.ImageAlign;set=> buttoff.ImageAlign = value; }

        public int ImageIndex { get => buttoff.ImageIndex; set => buttoff.ImageIndex = value; }
        public string ImageKey { get => buttoff.ImageKey; set => buttoff.ImageKey = value; }
        public ImageList ImageList { get => buttoff.ImageList;set=>buttoff.ImageList = value; }

        public ContentAlignment TextAlign
        {
            get => buttoff.TextAlign;
            set => buttoff.TextAlign = value;
        }

        public bool UseVisualStyleBackColor
        {
            get => buttoff.UseVisualStyleBackColor;
            set => buttoff.UseVisualStyleBackColor = value;
        }
        public TextImageRelation TextImageRelation
        {
            get => buttoff.TextImageRelation;
            set => buttoff.TextImageRelation = value;
        }


        public bool ToggledOn
        {
            get => button;
            set
            {
                if (value == button)
                    return;
                button = value;
                Toggled?.Invoke(this, EventArgs.Empty);
                ReevaluateToggleColour();
            }
        }

        private void Buttoff_Click(object sender, EventArgs e)
        {
            ToggledOn = !ToggledOn;
        }

        public override Color BackColor { get => base.BackColor; set { base.BackColor = value; ReevaluateToggleColour(); ; } }
        public override Color ForeColor { get => base.ForeColor; set { base.ForeColor = value; ReevaluateToggleColour(); ; } }

        Color disabledColor = Color.FromArgb(172, 108, 108, 108);
        public Color DisabledColorBlend
        {
            get => disabledColor; set
            {
                if (disabledColor == value) return;
                disabledColor = value;
                ReevaluateDisabledColour();
            }
        }

        bool disabledColourOff = true;
        void ReevaluateDisabledColour()
        {
            var v = Enabled | disabledColor.A == 0;
            if (v == disabledColourOff)
                return;
            disabledColourOff = v;
            if (v) buttoff.Paint -= Buttoff_Paint;
            else buttoff.Paint += Buttoff_Paint;
            if (Visible) Invalidate();
        }

        private void Buttoff_Paint(object sender, PaintEventArgs e)
        {
            using (var b = new SolidBrush(DisabledColorBlend))
            {
                e.Graphics.FillRectangle(b, e.ClipRectangle);
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            ReevaluateDisabledColour();
        }

        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Enabled)
                base.OnPaint(e);
            else
            {
                var d = DisabledColorBlend;
                switch (d.A)
                {
                    case 0:
                        base.OnPaint(e);
                        return;
                    case 255:
                        using (var b = new SolidBrush(d))
                            e.Graphics.FillRectangle(b, e.ClipRectangle);
                        return;
                }

                var mix = d.A / 255.0;
                var w = e.ClipRectangle.Width;
                var h = e.ClipRectangle.Height;
                
                if (w == 0 || h == 0)
                {
                    w = Width;
                    h = Height;
                }


                using (var intermediate = 
                    e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0 ?
                    new Bitmap(Width, Height) : new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height))
                {
                    using (var g = Graphics.FromImage(intermediate))
                    using (var a = new PaintEventArgs(g, e.ClipRectangle))
                        base.OnPaint(a);

                    for (int y = e.ClipRectangle.Top; y<e.ClipRectangle.Bottom; y++)
                    for (int x = e.ClipRectangle.Left; x < e.ClipRectangle.Right; x++)
                            intermediate.SetPixel(x, y, FormsUtil.Mix(d, intermediate.GetPixel(x, y), mix));

                    e.Graphics.DrawImage(intermediate, 0, 0);
                }
            }
        }
        */

        [Browsable(true)]
        public override string Text { get => buttoff.Text; set { base.Text = value; buttoff.Text = value; } }

        public override Image BackgroundImage { get => buttoff.BackgroundImage; set => buttoff.BackgroundImage = value; }
        public override ImageLayout BackgroundImageLayout { get => buttoff.BackgroundImageLayout; set => buttoff.BackgroundImageLayout = value; }
        public override Cursor Cursor { get => base.Cursor; set { buttoff.Cursor = value; base.Cursor = value; } }
        
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            buttoff.Padding = Padding;
        }

        protected override Padding DefaultPadding => buttoff.DefaultPadding;
        protected override Cursor DefaultCursor => buttoff.DefaultCursor;
        protected override ImeMode DefaultImeMode => buttoff.DefaultImeMode;
        protected override Padding DefaultMargin => buttoff.DefaultMargin;
        protected override Size DefaultMaximumSize => buttoff.DefaultMaximumSize;
        protected override Size DefaultMinimumSize => buttoff.DefaultMinimumSize;
        protected override Size DefaultSize => buttoff.DefaultSize;
    }
}
