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
    public class TrackBox : Control
    {
        static Color Mix(Color a, Color b, double keep = 0.5) => FormsUtil.Mix(a, b, keep);

        public TrackBox()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Selectable , true);
            SmallIncrement = 1;
        }

        int min = 0, max = 255, value = 128, hovervalue = -1;
        Color tmpcolor = Color.Transparent, lowcolor = Color.FromArgb(100, Color.DarkCyan), highcolor =
            Color.FromArgb(180, Color.Blue), alttxt = Color.Transparent;

        [DefaultValue(1)]
        public int SmallIncrement { get; set; }

        [DefaultValue(0)]
        public int BigIncrement { get; set; }


        bool fs_text;
        public override string Text
        {
            get => base.Text;
            set
            {
                if (value == Text)
                    return;
                base.Text = value;
                if (string.IsNullOrEmpty(value))
                    return;
                try
                {
                    fs_text = string.Format(value, 0) != value;
                }
                catch (FormatException) { fs_text = false; }
                Invalidate();
            }
        }

        void set<T>(ref T var, T val, ref EventHandler eventHandler, bool invalidate = true)
        {
            if (EqualityComparer<T>.Default.Equals(var, val))
                return;
            var = val;
            eventHandler?.Invoke(this, EventArgs.Empty);
            if (invalidate) Invalidate();
        }


        public event EventHandler
            LimitsChanged,
            ValueChanged,
            BarColorsChanged,
            FormatChanged;

        Func<int, string> _ValueToStringOverride;
        public Func<int, string> ValueToStringOverride
        {
            get => _ValueToStringOverride;
            set => set(ref _ValueToStringOverride, value, ref FormatChanged);
        }

        [DefaultValue(0)]
        public int Min { get => min; set => set(ref min, value, ref LimitsChanged) ; }
        [DefaultValue(255)]
        public int Max { get => max; set => set(ref max, value, ref LimitsChanged); }
        [DefaultValue(128)]
        public int Value { get => value; set => set(ref this.value, value, ref ValueChanged); }

        int calcbiginc()
        {
            int v = BigIncrement;
            if (v != 0) return v;
            v = SmallIncrement * 3 + (max - min) / 41;
            return v;
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int v;
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Right:
                    v = SmallIncrement;
                    break;
                case Keys.Left:
                case Keys.Down:
                    v = -SmallIncrement;
                    break;
                case Keys.PageUp:
                    v = calcbiginc();
                    break;
                case Keys.PageDown:
                    v = -calcbiginc();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            v += value;
            if (v < min) Value = min;
            else if (v > max) Value = max;
            else Value = v;
            return true;
        }

        //[DefaultValue(typeof(Color), "0xaaaa00")]
        public Color HoveredValueColor { get => tmpcolor; set => set(ref tmpcolor, value, ref BarColorsChanged); }
        //[DefaultValue(typeof(Color), "0x112255")]
        public Color LowValueColor { get => lowcolor; set => set(ref lowcolor, value, ref BarColorsChanged); }
        //[DefaultValue(typeof(Color), "0x0000aa")]
        public Color HighValueColor { get => highcolor; set => set(ref highcolor, value, ref BarColorsChanged); }
        
        public Color AlternateTextColor { get => alttxt; set => set(ref alttxt, value, ref BarColorsChanged); }


        protected override void OnMouseEnter(EventArgs e)
        {
            hovervalue = PointToClient(Cursor.Position).X;
            if (HoveredValueColor.A > 0)
                Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (hovervalue != -1)
            {
                hovervalue = PointToClient(Cursor.Position).X;
                if (HoveredValueColor.A > 0)
                    Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            hovervalue = -1;
            if (HoveredValueColor.A > 0)
                Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var v = PointToClient(Cursor.Position).X;
            Value = min + (v * (max - min)) / Width;
            Focus();
            base.OnMouseClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var v = e.Delta > 0 ? SmallIncrement : e.Delta < 0 ? -SmallIncrement : 0;
            v += value;
            if (v < min) v = min;
            else if (v > max) v = max;
            Value = v;
            base.OnMouseWheel(e);
        }

        bool isPainted;

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Brush Make(Color c)
                {
                    switch (c.A)
                    {
                        case 0:
                            return new SolidBrush(BackColor);
                        case 255:
                            return new SolidBrush(c);
                    }
                    return new SolidBrush(Mix(c, BackColor, c.A / 255.0));
                }
                int v;
                {

                    int progress = 0;
                    void Fill(int to, Color with)
                    {
                        int w = to - progress;
                        if (w < 1) return;
                        using (var b = Make(with))
                            e.Graphics.FillRectangle(b, new Rectangle(progress, 0, w, Height));
                        progress = to;
                    }

                    if (max <= min)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, Width, Height));
                        RenderText(ForeColor);
                        return;
                    }

                    v = value;
                    if (v < min) v = 0;
                    else if (v > max) v = max - min;
                    else v -= min;

                    Color cv = Mix(highcolor, lowcolor, (double)(v - min) / (double)(max - min));

                    v = (v * Width) / max;

                    int h = hovervalue;
                    if (h < 0 || HoveredValueColor.A == 0) goto no_hover;

                    if (h > v)
                    {
                        Fill(v, cv);
                        Fill(h, tmpcolor);
                    }
                    else
                    {
                        Fill(h, cv);
                        Fill(v, Mix(cv, tmpcolor, 0.7));
                    }
                    goto fill;

                no_hover:
                    Fill(v, cv);
                fill:
                    Fill(Width, BackColor);
                }

                void RenderText(Color c)
                {
                    var t = Text;
                    {
                        if (string.IsNullOrEmpty(t))
                            t = ValueToStringOverride == null ? value.ToString() : ValueToStringOverride(value);
                        else if (fs_text)
                            t = string.Format(t, ValueToStringOverride == null ? (object)value : ValueToStringOverride(value));
                        else if (ValueToStringOverride == null)
                            t += value;
                        else
                            t += ValueToStringOverride(value);
                    }
                    var f = Font;
                    var m = e.Graphics.MeasureString(t, f);
                    using (var b = Make(c))
                    {
                        var l = Padding.Left;
                        var w = Width - Padding.Right - l;
                        var u = Padding.Top;
                        var h = Height - Padding.Bottom - u;
                        e.Graphics.DrawString(t, f, b, l + (w - m.Width) / 2, u + (h - m.Height) / 2);
                    }
                }
                {
                    var c = AlternateTextColor;
                    if (c.A == 0)
                        RenderText(ForeColor);
                    else
                    {
                        var f = ForeColor;
                        c = Mix(Color.FromArgb(f.A, c), f, c.A / 255.0);
                        var g = e.Graphics.BeginContainer();
                        e.Graphics.IntersectClip(new Rectangle(0, 0, v, Height));
                        RenderText(c);
                        e.Graphics.EndContainer(g);
                        g = e.Graphics.BeginContainer();
                        e.Graphics.IntersectClip(new Rectangle(v, 0, Width - v, Height));
                        RenderText(f);
                        e.Graphics.EndContainer(g);
                    }
                }

            }
            finally
            {
                if (!isPainted)
                {
                    Application.Idle += Application_Idle;
                }
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= Application_Idle;
            isPainted = true;
            Invalidate();
        }

        public override Size MinimumSize
        {
            get
            {
                var m = base.MinimumSize;
                return new Size(m.Width < 40 ? 40 : m.Width, m.Height < 20 ? 20 : m.Height);
            }
            set => base.MinimumSize = value;
        }

        public class Composite : Panel
        {
            TrackBox trackbox = new TrackBox();
            Button left = new Button(), right = new Button();

            public Composite()
            {
                trackbox.Dock = DockStyle.Fill;
                left.Dock = DockStyle.Left;
                right.Dock = DockStyle.Right;

                left.Width = 10 + Width / 100;
                right.Width = 10 + Width / 100;

                Controls.Add(trackbox);
                Controls.Add(left);
                Controls.Add(right);

                left.Click += Left_Click;
                right.Click += Right_Click;

                trackbox.LimitsChanged += Trackbox_LimitsChanged;
                trackbox.ValueChanged += Trackbox_ValueChanged;
                trackbox.BarColorsChanged += Trackbox_BarColorsChanged;
                trackbox.FormatChanged += Trackbox_FormatChanged;
            }

            private void Trackbox_FormatChanged(object sender, EventArgs e)
            {
                FormatChanged?.Invoke(this, e);
            }

            private void Trackbox_BarColorsChanged(object sender, EventArgs e)
            {
                BarColorsChanged?.Invoke(this, e);
            }

            private void Trackbox_ValueChanged(object sender, EventArgs e)
            {
                ValueChanged?.Invoke(this, e);
            }

            private void Trackbox_LimitsChanged(object sender, EventArgs e)
            {
                LimitsChanged?.Invoke(this, e);
            }

            public event EventHandler
                LimitsChanged,
                ValueChanged,
                BarColorsChanged,
                FormatChanged;

            private void Right_Click(object sender, EventArgs e)
            {
                Value = Math.Min( Value + trackbox.calcbiginc(), Max);
            }

            private void Left_Click(object sender, EventArgs e)
            {
                Value = Math.Max(Value - trackbox.calcbiginc(), Min);
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                left.Width = 10 + Width / 100;
                right.Width = 10 + Width / 100;
            }


            [DefaultValue(1)]
            public int SmallIncrement { get => trackbox.SmallIncrement; set => trackbox.SmallIncrement = value; }

            [DefaultValue(0)]
            public int BigIncrement { get => trackbox.BigIncrement; set => trackbox.BigIncrement = value; }


            [DefaultValue(0)]
            public int Min { get => trackbox.Min; set => trackbox.Min = value; }
            [DefaultValue(255)]
            public int Max { get => trackbox.Max; set => trackbox.Max = value; }
            [DefaultValue(128)]
            public int Value { get => trackbox.Value; set => trackbox.Value = value; }


            public Color HoveredValueColor { get => trackbox.HoveredValueColor; set => trackbox.HoveredValueColor = value; }
            public Color LowValueColor { get => trackbox.LowValueColor; set { trackbox.LowValueColor = value; left.BackColor = value; } }
            public Color HighValueColor { get => trackbox.HighValueColor; set { trackbox.HighValueColor = value; right.BackColor = value; } }
            public Color AlternateTextColor { get => trackbox.AlternateTextColor; set => trackbox.AlternateTextColor = value; }

            public Func<int, string> ValueToStringOverride { get => trackbox.ValueToStringOverride; set => trackbox.ValueToStringOverride = value; }

            [Browsable(true)]
            public override string Text { get => trackbox.Text; set => trackbox.Text = value; }

            public override Size MinimumSize { get
                {
                    var m = base.MinimumSize;
                    return new Size(m.Width < 40 ? 40 : m.Width, m.Height < 20 ? 20 : m.Height);
                }
                set => base.MinimumSize = value; }

            public override Size GetPreferredSize(Size proposedSize)
            {
                var s = trackbox.PreferredSize;
                return new Size(s.Width + left.Width + right.Width, s.Height);
            }
        }
    }
}
