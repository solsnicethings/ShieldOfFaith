using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Sol.ShieldOfFaith
{
    [DefaultEvent("ValueChanged")]
    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            BorderStyle = BorderStyle.None;
            TextAlign = HorizontalAlignment.Center;
            Max = 255;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (AllowFractions)
            {
                if (TryGetTextValue(out double result) && result >= Min && result <= Max)
                    FloatingPointValue = result;
            }
            else
            {
                if (TryGetTextValue(out long result) && result >= Min && result <= Max)
                    IntegerValue = result;
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var s = base.GetPreferredSize(proposedSize);
            return new Size(s.Width, s.Height + 2 + s.Height / 10);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Oemplus:
                case Keys.Up:
                    if (AllowFractions)
                        FloatingPointValue = PreliminaryFloatingPointValue + 1;
                    else
                        IntegerValue = PreliminaryIntegerValue + 1;
                    return true;
                case Keys.OemMinus:
                case Keys.Down:
                    if (AllowFractions)
                        FloatingPointValue = PreliminaryFloatingPointValue - 1;
                    else
                        IntegerValue = PreliminaryIntegerValue - 1;
                    return true;
                case Keys.Enter:
                    if (AllowFractions)
                        FloatingPointValue = PreliminaryFloatingPointValue;
                    else
                        IntegerValue = PreliminaryIntegerValue;
                    return true;
            }
            return base.ProcessCmdKey(ref m, keyData);
        }

        double f;
        long l;

        public bool AllowFractions { get; set; }
        public double FloatingPointValue { get => f; set
            {
                if (value > Max)
                    FloatingPointValue = Max;
                else if (value < Min)
                    FloatingPointValue = Min;
                else if (value != f)
                {
                    f = value;
                    l = (long)f;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    if (!(TryGetTextValue(out double text) && text == value))
                        Text = f.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }
        public long IntegerValue { get => l; set
            {
                if (value > Max)
                    IntegerValue = (long)Max;
                else if (value < Min)
                    IntegerValue = (long)Math.Ceiling(Min);
                else if (value != l)
                {
                    f = value;
                    l = value;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    if (!(TryGetTextValue(out long text) && text == value))
                    {
                        var s = Text;
                        if (s.StartsWith("#"))
                        {
                            s = "#";
                            goto hex;
                        }
                        else if (s.StartsWith("0x"))
                        {
                            s = "0x";
                            goto hex;
                        }
                        Text = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        return;
                    hex:
                        Text = s + value.ToString("X");
                    }
                }
            }
        }

        public bool TryGetTextValue(out double result)
        {
            return double.TryParse(Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result)
                    || double.TryParse(Text, out result);
        }
        public bool TryGetTextValue(out long result)
        {
            var s = Text;

            if (s.StartsWith("#"))
            {
                s = s.Substring(1);
                goto hex;
            }
            else if (s.StartsWith("0x"))
            {
                s = s.Substring(2);
                goto hex;
            }
            return long.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        hex:
            return long.TryParse(s, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out result);
        }

        public double PreliminaryFloatingPointValue => TryGetTextValue(out double result) ? result : f;

        public long PreliminaryIntegerValue => TryGetTextValue(out long result) ? result : l;

        public event EventHandler ValueChanged;

        public double Min { get; set; }
        public double Max { get; set; }

    }
}
