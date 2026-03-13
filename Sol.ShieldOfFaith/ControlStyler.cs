using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sol.ShieldOfFaith
{
    public class ControlStyler : Dictionary<ControlSelectorCode, Dictionary<ControlPropertyCode, object>>
    {
        public enum SpecialValue
        {
            Inherit,
            Auto,
            None,

            /// <summary>
            /// is replaced with original value before assignment procedures, except when no original value has been collected yet
            /// </summary>
            Original
        }

        const string
            inst_select = "target",
            inst_select_properties = "property",
            inst_non_special_value = "value";

        public void Parse(string instructions)
        {
            using (var r = new StringReader(instructions)) Parse(r);
        }
        public void Parse(TextReader r)
        {
            IEnumerable<string> ead()
            {
                for (; ; )
                {
                    var l = r.ReadLine();
                    if (l == null) yield break;
                    yield return l;
                }
            }
            Parse(ead());
        }
        public void Parse(IEnumerable<string> instructions)
        {
            var ctl = new ControlSelectorCode[] { ControlSelectorCode.Tier0 };
            var kwrx = new System.Text.RegularExpressions.Regex(@"^\s*(\w+)\s+\S", System.Text.RegularExpressions.RegexOptions.Compiled);
            var listrx = new System.Text.RegularExpressions.Regex(@"\W+", System.Text.RegularExpressions.RegexOptions.Compiled);

            using (var line = instructions.GetEnumerator())
                while (line.MoveNext())
                {
                    var m = kwrx.Match(line.Current);
                    if (m.Success)
                    {
                        var lc = false;
                        var kw = m.Groups[1].Value;
                    lctry:
                        switch (kw)
                        {
                            default:
                                if (lc) break;
                                lc = true;
                                kw = kw.ToLowerInvariant();
                                goto lctry;
                            case inst_select:
                                {
                                    var list = listrx.Split(line.Current.Substring(m.Index + m.Length - 1));
                                    var c = list.Length;
                                    if (c > 0)
                                    {
                                        ctl = new ControlSelectorCode[c];
                                        while (--c > -1) ctl[c] = (ControlSelectorCode)Enum.Parse(typeof(ControlSelectorCode), list[c]);
                                    }
                                    else ctl = new ControlSelectorCode[] { ControlSelectorCode.Tier0 };
                                }
                                break;
                            case inst_select_properties:
                                {
                                    var list = listrx.Split(line.Current.Substring(m.Index + m.Length - 1));
                                    ControlPropertyCode[] prp;
                                    {
                                        var c = list.Length;

                                        if (c == 0)
                                            throw new ArgumentException("Empty property list.");

                                        prp = new ControlPropertyCode[c];
                                        while (--c > -1) prp[c] = (ControlPropertyCode)Enum.Parse(typeof(ControlPropertyCode), list[c]);
                                    }
                                    if (!line.MoveNext())
                                        throw new ArgumentException("End of input without specifying value for property set.");

                                    m = kwrx.Match(line.Current);
                                    {
                                        string str;
                                        var nsp = m.Success && string.Equals(m.Groups[1].Value, inst_non_special_value, StringComparison.OrdinalIgnoreCase);
                                        if (nsp) str = line.Current.Substring(m.Index + inst_non_special_value.Length + 1);
                                        else str = line.Current.Trim();

                                        Add(ctl, prp, str, nsp);
                                    }
                                }
                                break;
                        }
                    }
                }
        }
        public string Encode()
        {
            var str = new StringBuilder();
            var expr_grouper = new Dictionary<string, List<ControlSelectorCode>>();
            {
                var val_grouper = new Dictionary<string, List<ControlPropertyCode>>();
                foreach (var c in this)
                {
                    val_grouper.Clear();
                    foreach (var v in c.Value.OrderBy(e => e.Key))
                    {
                        if (v.Value is SpecialValue)
                            str.Append(v.Value);
                        else str.Append(inst_non_special_value).Append(" ").Append(v.Value);
                        if (!val_grouper.TryGetValue(str.ToString(), out var l))
                        {
                            l = new List<ControlPropertyCode>(1);
                            val_grouper.Add(str.ToString(), l);
                        }
                        l.Add(v.Key);
                        str.Clear();
                    }
                    foreach (var g in val_grouper)
                    {
                        str.Append(inst_select_properties);
                        foreach (var p in g.Value) str.Append(" ").Append(p);
                        str.AppendLine();
                        str.AppendLine(g.Key);
                        if (!expr_grouper.TryGetValue(str.ToString(), out var l))
                        {
                            l = new List<ControlSelectorCode>(1);
                            expr_grouper.Add(str.ToString(), l);
                        }
                        l.Add(c.Key);
                        str.Clear();
                    }
                }
            }
            {
                var selection_grouper = new Dictionary<List<ControlSelectorCode>, List< string >> (new OrderSensitiveElementComparer<List<ControlSelectorCode>, ControlSelectorCode>());
                foreach (var g in expr_grouper)
                {
                    if (!selection_grouper.TryGetValue(g.Value, out var l))
                    {
                        l = new List<string>(1);
                        selection_grouper.Add(g.Value, l);
                    }
                    l.Add(g.Key);
                }

                foreach (var g in selection_grouper)
                {
                    str.Append(inst_select);
                    foreach (var c in g.Key)
                        str.Append(" ").Append(c);
                    str.AppendLine();
                    foreach (var xpr in g.Value)
                        str.Append(xpr);
                }
            }
            return str.ToString();
        }

        public void Add(ControlSelectorCode ctl, ControlPropertyCode prp, string objstr, bool explicitlyNotSpecialValue = false)
            => Add(new ControlSelectorCode[] { ctl }, new ControlPropertyCode[] { prp }, objstr, explicitlyNotSpecialValue);
        public void Add(IEnumerable<ControlSelectorCode> ctl, IEnumerable<ControlPropertyCode> prplist, string objstr, bool explicitlyNotSpecialValue = false)
        {
            foreach (var prp in prplist)
            {
                object objval;

                if (!explicitlyNotSpecialValue)
                { if (Enum.TryParse<SpecialValue>(objstr, out var sv)) { objval = sv; goto valset; } }

                switch (prp)
                {
                    case ControlPropertyCode.BackgroundImage:
                        throw new NotImplementedException();

                    case ControlPropertyCode.FontStyle:
                        objval = Enum.Parse(typeof(FontStyle), objstr);
                        break;

                    case ControlPropertyCode.ShowAlpha:
                    case ControlPropertyCode.ShowRGB:
                    case ControlPropertyCode.ShowBackButton:
                    case ControlPropertyCode.ShowSelector:
                    case ControlPropertyCode.Visible:
                    case ControlPropertyCode.AutoSize:
                        {
                            objval = string.Equals("yes", objstr, StringComparison.OrdinalIgnoreCase)
                            || string.Equals("true", objstr, StringComparison.OrdinalIgnoreCase)
                            || (long.TryParse(objstr, out var x) && x != 0);
                        }
                        break;

                    case ControlPropertyCode.Cursor:
                        objval = new CursorConverter().ConvertFromInvariantString(objstr);
                        break;

                    case ControlPropertyCode.FontSize:
                    case ControlPropertyCode.Scale:
                        objval = float.Parse(objstr);
                        break;

                    case ControlPropertyCode.BigIncrement:
                    case ControlPropertyCode.Increment:
                    case ControlPropertyCode.Height:
                    case ControlPropertyCode.Width:
                        objval = int.Parse(objstr);
                        break;

                    case ControlPropertyCode.AlternateForeColour:
                    case ControlPropertyCode.LowValueColour:
                    case ControlPropertyCode.HighValueColour:
                    case ControlPropertyCode.HoverColour:
                    case ControlPropertyCode.Foreground:
                    case ControlPropertyCode.Background:
                    case ControlPropertyCode.ToggledBackground:
                    case ControlPropertyCode.ToggledForeground:
                        objstr = objstr.Trim();
                        if (objstr.StartsWith("0x"))
                        {
                            objstr = objstr.Substring(2);
                            goto hex;
                        }
                        if (objstr.StartsWith("#"))
                        {
                            objstr = objstr.Substring(1);
                            goto hex;
                        }
                        {
                            var c = Color.FromName(objstr);
                            if (c.IsKnownColor)
                            {
                                objval = c;
                                goto valset;
                            }
                        }
                        {
                            var split = new System.Text.RegularExpressions.Regex(@"\D+", System.Text.RegularExpressions.RegexOptions.Compiled);
                            var c = split.Split(objstr);
                            switch (c.Length)
                            {
                                case 3:
                                    objval = Color.FromArgb(byte.Parse(c[0]), byte.Parse(c[1]), byte.Parse(c[2]));
                                    goto valset;
                                case 4:
                                    objval = Color.FromArgb(byte.Parse(c[0]), byte.Parse(c[1]), byte.Parse(c[2]), byte.Parse(c[3]));
                                    goto valset;
                            }
                        }
                    cfail:
                        throw new ArgumentException("No known colour or numeric colour representation: " + objstr);
                    hex:
                        {
                            switch (objstr.Length)
                            {
                                case 3:
                                    objval = Color.FromArgb(
                                        0x11 * byte.Parse(objstr[0].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier),
                                        0x11 * byte.Parse(objstr[1].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier),
                                        0x11 * byte.Parse(objstr[2].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier));
                                    goto valset;
                                case 6:
                                    objval = Color.FromArgb(
                                        byte.Parse(objstr.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                                        byte.Parse(objstr.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                                        byte.Parse(objstr.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                                    goto valset;
                                case 8:
                                    objval = Color.FromArgb(
                                        byte.Parse(objstr.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                                        byte.Parse(objstr.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                                        byte.Parse(objstr.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                                        byte.Parse(objstr.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                                    goto valset;
                            }
                        }
                        goto cfail;

                    default:
                        objval = objstr;
                        break;
                }

            valset:
                foreach (var c in ctl)
                {
                    if (!TryGetValue(c, out var d))
                    {
                        d = new Dictionary<ControlPropertyCode, object>();
                        Add(c, d);
                    }
                    d[prp] = objval;
                }
            }
        }


        public Func<Control, ControlSelectorCode, ControlSelectorCode>
            ControlTypeResolver;

        public IDictionary<Control, IDictionary<ControlPropertyCode, object>>
            original_property_control = new ConcurrentWeakKeyDictionary<Control, IDictionary<ControlPropertyCode, object>>();

        List<Dictionary<ControlPropertyCode, object>> GetPrioritisedSpecifications(ControlSelectorCode controlType)
        {
            var l = new List<Dictionary<ControlPropertyCode, object>>();

            for (; ; )
                switch (controlType)
                {
                    case ControlSelectorCode.None:
                        return l;
                    case ControlSelectorCode.Tier0:
                        if (TryGetValue(ControlSelectorCode.Tier0, out var d))
                            l.Add(d);
                        return l;
                    default:
                        if (TryGetValue(controlType, out d)) l.Add(d);

                        if ((ControlSelectorCode.Tierless & controlType) != 0)
                            return l;
                        if ((controlType & (ControlSelectorCode)0xff) == controlType)
                        {
                            if ((0xf0 & (int)controlType) == 0)
                            {
                                if ((controlType & ControlSelectorCode.Tier0) != 0 && TryGetValue(ControlSelectorCode.Tier0, out d))
                                    l.Add(d);
                                return l;
                            }
                            controlType = ((ControlSelectorCode)0xf) & controlType;
                            break;
                        }
                        else
                            controlType = (ControlSelectorCode)((int)controlType >> 8);
                        break;
                }             
        }


        public void ApplyTo(Control target)
        {
            target.SuspendLayout();
            try { ApplyTo(target, 0); }
            finally { target.ResumeLayout(); }
        }
        void ApplyTo(Control target, ControlSelectorCode context)
        {
            if (context == 0)
                target.SuspendLayout();
            
            var ct = ControlTypeResolver?.Invoke(target, context) ?? ControlSelectorCode.ResolveAutomatically;

            switch (ct)
            {
                case ControlSelectorCode.None:
                    return;
                case ControlSelectorCode.ResolveAutomatically:
                    switch (context)
                    {
                        default:
                            if (Enum.TryParse(target.GetType().Name, out ct))
                                switch (ct)
                                {
                                    case ControlSelectorCode.ComboBox:
                                        switch (((ComboBox)target).DropDownStyle)
                                        {
                                            case ComboBoxStyle.DropDownList:
                                                ct = ControlSelectorCode.ListCombo;
                                                break;
                                            case ComboBoxStyle.Simple:
                                                ct = ControlSelectorCode.SimpleCombo;
                                                break;
                                            default:
                                                ct = ControlSelectorCode.DropDownCombo;
                                                break;
                                        }
                                        break;
                                    case ControlSelectorCode.TextBox:
                                        ct = ((TextBox)target).ReadOnly ? ControlSelectorCode.ReadonlyTextBox : ControlSelectorCode.WriteableTextBox;
                                        break;
                                    case ControlSelectorCode.RichTextBox:
                                        ct = ((RichTextBox)target).ReadOnly ? ControlSelectorCode.ReadonlyRichTextBox : ControlSelectorCode.WriteableRichTextBox;
                                        break;
                                }
                            else if (target is TrackBox.Composite) ct = ControlSelectorCode.CompositeTrackBox;
                            else if (target is Button) ct = target is ButtonToggle ? ControlSelectorCode.ExtendedButtonToggle : ControlSelectorCode.ExtendedButton;
                            else if (target is CheckBox) ct = ControlSelectorCode.ExtendedCheckBox;
                            else if (target is ComboBox) ct = ControlSelectorCode.ExtendedComboBox;
                            else if (target is Form) ct = ControlSelectorCode.ExtendedForm;
                            else if (target is GroupBox) ct = ControlSelectorCode.ExtendedGroupBox;
                            else if (target is Label) ct = ControlSelectorCode.ExtendedLabel;
                            else if (target is ListBox) ct = ControlSelectorCode.ExtendedListBox;
                            else if (target is ListView) ct = ControlSelectorCode.ExtendedListView;
                            else if (target is Panel) ct = ControlSelectorCode.ExtendedPanel;
                            else if (target is RadioButton) ct = ControlSelectorCode.ExtendedRadioButton;
                            else if (target is RichTextBox) ct = ControlSelectorCode.ExtendedRichTextBox;
                            else if (target is SplitContainer) ct = ControlSelectorCode.ExtendedSplitContainer;
                            else if (target is TextBox) ct = ControlSelectorCode.ExtendedTextBox;
                            else ct = ControlSelectorCode.Other;
                            break;
                    }
                    break;
            }

            var pspec = GetPrioritisedSpecifications(ct);

            Action recorder = null;

            Action<IDictionary<ControlPropertyCode, object>> TryAdd(ControlPropertyCode key, Func<Control, object> value)
            {
                return tryadd =>
                {
                    if (!tryadd.ContainsKey(key))
                        tryadd.Add(key, value(target));
                };
            }

            Action SetRecorder(params ControlPropertyCode[] code)
            {
                var action_list = new List<Action<IDictionary<ControlPropertyCode, object>>>(code.Length);
                var rerun = new Dictionary<ControlPropertyCode, object>();
                foreach (var c in code)
                {
                    switch (c)
                    {
                        case ControlPropertyCode.NoProperty:
                            break;

                        case ControlPropertyCode.AutoSize:
                            action_list.Add(TryAdd(c, ctl => ctl.AutoSize));
                            break;
                        case ControlPropertyCode.Background:
                            action_list.Add(TryAdd(c, ctl => ctl.BackColor));
                            break;
                        case ControlPropertyCode.BackgroundImage:
                            action_list.Add(TryAdd(c, ctl => ctl.BackgroundImage));
                            break;
                        case ControlPropertyCode.Cursor:
                            action_list.Add(TryAdd(c, ctl => ctl.Cursor));
                            break;

                        case ControlPropertyCode.Foreground:
                            action_list.Add(TryAdd(c, ctl => ctl.ForeColor));
                            break;

                        case ControlPropertyCode.FontSize:
                        case ControlPropertyCode.FontFamily:
                        case ControlPropertyCode.FontStyle:
                            rerun[ControlPropertyCode.Font] = null;
                            break;

                        case ControlPropertyCode.Height:
                            action_list.Add(TryAdd(c, ctl => ctl.Height));
                            break;
                        case ControlPropertyCode.Scale:
                            // defaults to 1
                            break;

                        case ControlPropertyCode.ShowAlpha:
                            if (target is ColourManager)
                                action_list.Add(TryAdd(c, ctl => ((ColourManager)ctl).DisplayAlpha));
                            break;
                        case ControlPropertyCode.ShowRGB:
                            if (target is ColourManager)
                                action_list.Add(TryAdd(c, ctl => ((ColourManager)ctl).DisplayRGB));
                            break;
                        case ControlPropertyCode.ShowSelector:
                            if (target is ColourManager)
                                action_list.Add(TryAdd(c, ctl => ((ColourManager)ctl).DisplayColourPick));
                            break;
                        case ControlPropertyCode.ShowBackButton:
                            if (target is ColourManager)
                                action_list.Add(TryAdd(c, ctl => ((ColourManager)ctl).DisplayPreviousPick));
                            break;

                        case ControlPropertyCode.SubcontrolIterator:
                            break;

                        case ControlPropertyCode.ToggledBackground:
                            if (target is ButtonToggle)
                                action_list.Add(TryAdd(c, ctl => ((ButtonToggle)ctl).ToggledBackColour));
                            break;
                        case ControlPropertyCode.ToggledForeground:
                            if (target is ButtonToggle)
                                action_list.Add(TryAdd(c, ctl => ((ButtonToggle)ctl).ToggledForeColour));
                            break;

                        case ControlPropertyCode.Visible:
                            action_list.Add(TryAdd(c, ctl => ctl.Visible));
                            break;
                        case ControlPropertyCode.Width:
                            action_list.Add(TryAdd(c, ctl => ctl.Width));
                            break;

                        case ControlPropertyCode.Increment:
                            if (target is TrackBox.Composite)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox.Composite)ctl).SmallIncrement));
                            else if (target is TrackBox)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox)ctl).SmallIncrement));
                            else if (target is NumericUpDown)
                                action_list.Add(TryAdd(c, ctl => ((NumericUpDown)ctl).Increment));
                            break;

                        case ControlPropertyCode.BigIncrement:
                            if (target is TrackBox.Composite)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox.Composite)ctl).BigIncrement));
                            else if (target is TrackBox)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox)ctl).BigIncrement));
                            break;

                        case ControlPropertyCode.HoverColour:
                            if (target is TrackBox.Composite)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox.Composite)ctl).HoveredValueColor));
                            else if (target is TrackBox)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox)ctl).HoveredValueColor));
                            break;

                        case ControlPropertyCode.LowValueColour:
                            if (target is TrackBox.Composite)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox.Composite)ctl).LowValueColor));
                            else if (target is TrackBox)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox)ctl).LowValueColor));
                            break;

                        case ControlPropertyCode.HighValueColour:
                            if (target is TrackBox.Composite)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox.Composite)ctl).HighValueColor));
                            else if (target is TrackBox)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox)ctl).HighValueColor));
                            break;

                        case ControlPropertyCode.AlternateForeColour:
                            if (target is TrackBox.Composite)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox.Composite)ctl).AlternateTextColor));
                            else if (target is TrackBox)
                                action_list.Add(TryAdd(c, ctl => ((TrackBox)ctl).AlternateTextColor));
                            break;



                        default:
                                    rerun[c] = null;
                                    break;
                                }
                }

                foreach (var c in rerun.Keys)
                {
                    switch (c)
                    {
                        case ControlPropertyCode.Font:
                            action_list.Add(TryAdd(c, ctl => ctl.Font));
                            break;
                    }
                }

                if (action_list.Count == 0)
                    return () => { };

                return SetRecorderActions(action_list.ToArray());
            }
            Action SetRecorderActions(params Action<IDictionary<ControlPropertyCode, object>>[] actions)
            { 
                return () =>
                {
                    if (!original_property_control.TryGetValue(target, out var pl))
                    {
                        pl = new Dictionary<ControlPropertyCode, object>();
                        original_property_control.Add(target, pl);
                    }
                    foreach (var a in actions) a(pl);
                };
            }

            IEnumerable<Dictionary<ControlPropertyCode, object>> Get(params ControlPropertyCode[] cpc)
            {
                var level = new Dictionary<ControlPropertyCode, object>();
                var rec = true;
                if (!original_property_control.TryGetValue(target, out var originals))
                    originals = null;

                foreach (var spec in pspec)
                {
                    for (int i = 0; i < cpc.Length; i++)
                    {
                        var x = cpc[i];
                        if (x == ControlPropertyCode.NoProperty)
                            continue;
                        if (spec.TryGetValue(x, out var v))
                        {
                            if (v as SpecialValue? == SpecialValue.Original)
                            {
                                if (originals != null && !originals.TryGetValue(x, out v))
                                    v = SpecialValue.Original;
                                cpc[i] = ControlPropertyCode.NoProperty;
                            }
                            level.Add(x, v);
                        }
                    }
                    if (level.Count > 0)
                    {
                        if (rec) { SetRecorder(cpc); rec = false; }
                        yield return level;
                        level = new Dictionary<ControlPropertyCode, object>();
                    }
                }

                foreach (var x in cpc) if (x != ControlPropertyCode.NoProperty) level.Add(x, SpecialValue.Auto);
            }
            IEnumerable<object> GetValue(ControlPropertyCode cpc)
            {
                var rec = true;
                foreach (var spec in pspec)
                    if (spec.TryGetValue(cpc, out var v))
                    {
                        if (rec) { SetRecorder(cpc); rec = false; }
                        if (v as SpecialValue? == SpecialValue.Original) 
                        {
                            if (original_property_control.TryGetValue(target, out var d))
                                yield return d.TryGetValue(cpc, out v) ? v : SpecialValue.Original;
                            yield break;
                        }
                        yield return v;
                    }
                yield return SpecialValue.Auto;
            }

            void ResolveColour(Action<Color> assign_direct, Func<Color> getCurrent, Func<Color> getContext, IEnumerable<object> value, Func<Color> auto = null)
            {
                Action<Color> assign = c => { recorder(); assign_direct(c); };
                foreach (var v in value)
                {
                    if (v is Color c)
                        switch (c.A)
                        {
                            case 255:
                                assign(c);
                                return;
                            case 0:
                                assign(getContext());
                                return;
                            default:
                                assign(FormsUtil.Mix(Color.FromArgb(255, c), getContext(), c.A / 255.0));
                                return;
                        }
                    if (v is SpecialValue s)
                        switch (s)
                        {
                            case SpecialValue.Auto:
                                assign((auto ?? getContext).Invoke());
                                return;
                            case SpecialValue.Inherit:
                                assign(getContext());
                                return;
                            case SpecialValue.None:
                                assign(Color.Transparent);
                                return;

                            // original isn't passed to this function unless the control property is unchanged
                            case SpecialValue.Original:
                                return;
                        }
                }
            }
            void Resolve<T>(Action<T> assign_direct, Func<T> getCurrent, Func<T> getContext, IEnumerable<object> value, Func<T> auto = null)
            {
                Action<T> assign = c => { recorder(); assign_direct(c); };
                foreach (var v in value)
                {
                    if (v is T c)
                        assign(c);
                    if (v is SpecialValue s)
                        switch (s)
                        {
                            case SpecialValue.Inherit:
                                assign(getContext());
                                return;
                            case SpecialValue.Auto:
                                assign((auto ?? getContext).Invoke());
                                return;
                            case SpecialValue.None:
                                assign(default(T));
                                return;

                            // original isn't passed to this function unless the control property is unchanged
                            case SpecialValue.Original:
                                return;
                        }
                }
            }

            ResolveColour(c => target.ForeColor = c, () => target.ForeColor, () => target.Parent?.ForeColor ?? Color.Khaki, GetValue(ControlPropertyCode.Foreground));
            ResolveColour(c => target.BackColor = c, () => target.BackColor, () => target.Parent?.BackColor ?? Color.FromArgb(64, 64, 64), GetValue(ControlPropertyCode.Background));
            Resolve(c => target.BackgroundImage = c, () => target.BackgroundImage, () => null, GetValue(ControlPropertyCode.BackgroundImage));
            Resolve(c => target.Cursor = c, () => target.Cursor, () => Cursors.Arrow, GetValue(ControlPropertyCode.Cursor));
            Resolve(c => target.Visible = c, () => target.Visible, () => true, GetValue(ControlPropertyCode.Visible));

            {
                if (target is ButtonToggle t)
                {
                    ResolveColour(c => t.ToggledBackColour = c, () => t.ToggledBackColour, () => Color.Transparent, GetValue(ControlPropertyCode.ToggledBackground));
                    ResolveColour(c => t.ToggledForeColour = c, () => t.ToggledForeColour, () => Color.Transparent, GetValue(ControlPropertyCode.ToggledForeground));
                }
            }
            {
                if (target is TrackBox.Composite b)
                {
                    Resolve(c => b.SmallIncrement = c, () => b.SmallIncrement, () => 1, GetValue(ControlPropertyCode.Increment));
                    Resolve(c => b.BigIncrement = c, () => b.BigIncrement, () => 0, GetValue(ControlPropertyCode.BigIncrement));
                    Resolve(c => b.HoveredValueColor = c, () => b.HoveredValueColor, () => Color.Transparent, GetValue(ControlPropertyCode.HoverColour));
                    Resolve(c => b.LowValueColor = c, () => b.LowValueColor, () => Color.FromArgb(50, b.ForeColor), GetValue(ControlPropertyCode.LowValueColour));
                    Resolve(c => b.HighValueColor = c, () => b.HighValueColor, () => Color.FromArgb(100, b.ForeColor), GetValue(ControlPropertyCode.HighValueColour));
                    Resolve(c => b.AlternateTextColor = c, () => b.AlternateTextColor, () =>
                    {
                        var alt = b.HighValueColor;
                        switch (alt.A)
                        {
                            case 255:
                                break;
                            case 0:
                                alt = b.BackColor;
                                break;
                            default:
                                alt = FormsUtil.Mix(Color.FromArgb(255, alt), b.BackColor, alt.A / 255.0);
                                break;
                        }
                        var x = (alt.R + alt.G + alt.B) / 3;

                        if (x < 128)
                        {
                            x += 100;
                            return Color.FromArgb(255,
                                (alt.R / 2 + alt.G + alt.B) / 30 + x,
                                (alt.G / 2 + alt.R + alt.B) / 30 + x,
                                (alt.B / 2 + alt.R + alt.G) / 30 + x
                                );
                        }

                        x -= 100;
                        return Color.FromArgb(255,
                            x - (alt.R * 2 + alt.G + alt.B) / 40,
                            x - (alt.G * 2 + alt.R + alt.B) / 40,
                            x - (alt.B * 2 + alt.R + alt.B) / 40
                            );
                    }, GetValue(ControlPropertyCode.AlternateForeColour));
                }
            }

            {
                if (target is ColourManager m)
                {
                    Resolve(c => m.DisplayAlpha = c, () => m.DisplayAlpha, () => context == ControlSelectorCode.ShieldOfFaithPanel ?
                    true : m.DisplayAlpha, GetValue(ControlPropertyCode.ShowAlpha));
                    Resolve(c => m.DisplayRGB = c, () => m.DisplayRGB, () => context != ControlSelectorCode.ShieldOfFaithPanel, GetValue(ControlPropertyCode.ShowRGB));
                    Resolve(c => m.DisplayColourPick= c, () => m.DisplayColourPick, () => context != ControlSelectorCode.ShieldOfFaithPanel, GetValue(ControlPropertyCode.ShowSelector));
                    Resolve(c => m.DisplayPreviousPick = c, () => m.DisplayPreviousPick, () => context != ControlSelectorCode.ShieldOfFaithPanel, GetValue(ControlPropertyCode.ShowBackButton));
                }
            }
            
            {
                object ffam = null, fem = null, fst = null;
                Font template = null, original_parent_font = null;
                foreach (var font in Get(ControlPropertyCode.Font, ControlPropertyCode.FontStyle, ControlPropertyCode.FontSize, ControlPropertyCode.FontFamily))
                {
                    if (ffam == null && font.TryGetValue(ControlPropertyCode.FontFamily, out var v))
                        ffam = v;
                    if (fem == null && font.TryGetValue(ControlPropertyCode.FontSize, out v))
                        fem = v;
                    if (fst == null && font.TryGetValue(ControlPropertyCode.FontStyle, out v))
                        fst = v;
                    if (font.TryGetValue(ControlPropertyCode.Font, out v))
                    {
                        recorder();
                        if (v is Font f)
                            target.Font = f;
                        else
                            switch (v as SpecialValue?)
                            {
                                case SpecialValue.Inherit:
                                    if (target.Parent != null)
                                        target.Font = target.Parent.Font;
                                    break;
                                case SpecialValue.Auto:
                                    if (ffam == null)
                                        ffam = SpecialValue.Auto;
                                    if (fem == null)
                                        fem = SpecialValue.Auto;
                                    if (fst == null)
                                        fst = SpecialValue.Auto;
                                    if (original_property_control.TryGetValue(target, out var ctl) && ctl.TryGetValue(ControlPropertyCode.Font, out var fnt))
                                        target.Font = (Font)fnt;
                                    if (target.Parent != null)
                                    {
                                        if (original_property_control.TryGetValue(target.Parent, out ctl) && ctl.TryGetValue(ControlPropertyCode.Font, out fnt))
                                            original_parent_font = (Font)fnt;
                                        else original_parent_font = target.Parent.Font;
                                    }
                                    break;
                            }
                        template = target.Font;
                        break;
                    }
                }
                {
                    if (ffam is object || fem is object || fst is object)
                    {
                        recorder();
                        if (template == null) template = (Font)original_property_control[target][ControlPropertyCode.Font];
                        var inh = target.Parent == null ? template : target.Parent.Font;

                        FontFamily set_ffam;

                        switch (ffam as SpecialValue?)
                        {
                            case SpecialValue.Auto:
                                if (original_parent_font == null)
                                    set_ffam = template.FontFamily;
                                else if (original_parent_font.FontFamily == template.FontFamily)
                                    set_ffam = target.Parent.Font.FontFamily;
                                else set_ffam = template.FontFamily;
                                break;
                            case SpecialValue.Inherit:
                                        set_ffam = inh.FontFamily;
                                        break;
                                    default:
                                        set_ffam = ffam is string ?
                                            new FontFamily((string)ffam) :
                                            ffam is FontFamily ? (FontFamily)ffam : template.FontFamily;
                                        break;
                                    }

                        float set_fem;

                        switch (fem as SpecialValue?)
                        {
                            case SpecialValue.Auto:
                                if (original_parent_font == null)
                                    set_fem = template.SizeInPoints;
                                else
                                    set_fem = target.Parent.Font.SizeInPoints * template.SizeInPoints / original_parent_font.SizeInPoints;
                                break;
                            case SpecialValue.Inherit:
                                set_fem = inh.SizeInPoints;
                                break;
                            default:
                                set_fem = fem as float? ?? template.SizeInPoints;
                                break;
                        }

                        FontStyle set_fst;

                        switch (fst as SpecialValue?)
                        {
                            case SpecialValue.Auto:
                                if (original_parent_font == null)
                                    set_fst = template.Style;
                                else
                                {
                                    FontStyle rst = original_parent_font.Style, tst = template.Style, inhst = target.Parent.Font.Style, mk = 0;

                                    foreach (FontStyle f in Enum.GetValues(typeof(FontStyle)))
                                        if ((((rst & f) == (tst & f) ? inhst : tst) & f) != 0) mk |= f;

                                    set_fst = mk;
                                }
                                break;
                            case SpecialValue.Inherit:
                                set_fst = inh.Style;
                                break;
                            default:
                                set_fst = fst as FontStyle? ?? template.Style;
                                break;
                        }
                        
                        target.Font = new Font(set_ffam, set_fem, set_fst, GraphicsUnit.Point, template.GdiCharSet, template.GdiVerticalFont);
                    }
                }
            }

            {
                object w = null, h = null;
                foreach (var size in Get(ControlPropertyCode.AutoSize, ControlPropertyCode.Width, ControlPropertyCode.Height))
                {
                    if (w != null)
                    {
                        if (size.TryGetValue(ControlPropertyCode.Height, out var v))
                        {
                            h = v;
                            break;
                        }
                    }
                    else if (h != null)
                    {
                        if (size.TryGetValue(ControlPropertyCode.Width, out var v))
                        {
                            w = v;
                            break;
                        }
                    }
                    else
                    {
                        if (size.TryGetValue(ControlPropertyCode.Width, out var v))
                        {
                            w = v;
                            if (size.TryGetValue(ControlPropertyCode.Height, out v))
                            {
                                h = v;
                                break;
                            }
                        }
                        else if (size.TryGetValue(ControlPropertyCode.Height, out v))
                            h = v;
                        else if (size.TryGetValue(ControlPropertyCode.AutoSize, out v))
                        {
                            recorder();
                            { if (v is bool b) target.AutoSize = b; }
                            break;
                        }
                    }
                }
                if (w is int|| h is int)
                {
                    recorder();
                    { if (w is int a) target.Width = a; }
                    { if (h is int a) target.Height = a; }
                }
            }



            {
                IEnumerable<Control> iterator;

                foreach (var c in GetValue(ControlPropertyCode.SubcontrolIterator))
                {
                    {
                        if (c is Func<Control, IEnumerable<Control>> f)
                        {
                            iterator = f(target);
                            goto customchild;
                        }
                    }
                    {
                        if (c is Func<Control, Control.ControlCollection> f)
                        {
                            iterator = f(target).Cast<Control>();
                            goto customchild;
                        }
                    }
                    {
                        if (c is Func<Control, Control> f)
                        {
                            var found = f(target);
                            iterator = found == null ? new Control[0] : new Control[] { found };
                            goto customchild;
                        }
                    }
                    {
                        if (c is SpecialValue f)
                            switch (f)
                            {
                                case SpecialValue.Auto:
                                    goto autochild;
                                case SpecialValue.None:
                                    iterator = new Control[0];
                                    goto customchild;
                            }
                    }
                }
            autochild:
                foreach (Control c in target.Controls)
                    ApplyTo(c, ct);
                return;
            customchild:
                foreach (var c in iterator)
                    ApplyTo(c, ct);
            }
        }

        void Set(ControlSelectorCode ctl, ControlPropertyCode prp, object value)
        {
            if (!TryGetValue(ctl, out var d))
            {
                d = new Dictionary<ControlPropertyCode, object>();
                Add(ctl, d);
            }
            d[prp] = value;
        }

        public void SetControlChildIterator(ControlSelectorCode ctl, bool iterate_child_controls)
        => Set(ctl, ControlPropertyCode.SubcontrolIterator, iterate_child_controls ? SpecialValue.Auto : SpecialValue.None);

        public void SetControlChildIterator(ControlSelectorCode ctl, Func<Control, IEnumerable<Control>> f)
        => Set(ctl, ControlPropertyCode.SubcontrolIterator, f);
        public void SetControlChildIterator(ControlSelectorCode ctl, Func<Control, Control> f)
        => Set(ctl, ControlPropertyCode.SubcontrolIterator, f);
        public void SetControlChildIterator(ControlSelectorCode ctl, Func<Control, Control.ControlCollection> f)
        => Set(ctl, ControlPropertyCode.SubcontrolIterator, f);
    }

    public enum ControlPropertyCode
    {
        NoProperty,

        Foreground,
        Background,
        BackgroundImage,

        FontFamily,
        FontSize,
        FontStyle,
        Font,

        ToggledBackground,
        ToggledForeground,

        Scale,
        Height,
        Width,
        AutoSize,

        Cursor,
        Visible,

        ShowRGB,
        ShowAlpha,
        ShowSelector,
        ShowBackButton,

        Increment,
        BigIncrement,
        HoverColour,
        LowValueColour,
        HighValueColour,
        AlternateForeColour,

        SubcontrolIterator
    }

    public enum ControlSelectorCode
    {
        None,
        ResolveAutomatically = -1,
        
        // tier 0 alternative roots
        ValueControl = 0x1,
        Misc = 0x2,

        // general tier 0 root
        Tier0 = 0x8,
        Tierless = unchecked((int)0x80000000),

        // tier 1 under general tier 0 mask 0x7 | Tier0 (0x8)
        // tier 0 without tier 0 mask 0x7
        // tier 1.5 mask 0x7f (extending any tier 0 base or tier 1)

        // tier 1 (0x & 1.5 under general tier 0 root mask 0x7f (0x80 bit could push to tierless)
        FalseBorderElement = 0x1 | Tier0,
        BorderCloseButton = 0x10 | FalseBorderElement,
        BorderMinimizeButton = 0x20 | FalseBorderElement,
        BorderTitlePanel = 0x30 | FalseBorderElement,
        BorderTitle = 0x40 | FalseBorderElement,

        Container = 0x2 | Tier0,
        Button = 0x3 | Tier0, ExtendedButton = 0xff | (Button << 8),
        Label = 0x4 | Tier0, ExtendedLabel = 0xff | (Label << 8),
        CheckOrRadio = 0x5 | Tier0,
        ComplexControl = 0x6 | Tier0,
        Other = 0x7 | Tier0,

        // tier 1.5 can not use the 1x80 bit because it could push to Tierless in tier 4

        GroupBox = 0x10 | Container, ExtendedGroupBox = 0xff | (GroupBox << 8),
        Panel = 0x20 | Container, ExtendedPanel = 0xff | (Panel << 8),

        CheckBox = 0x10 | CheckOrRadio, ExtendedCheckBox = 0xff | (CheckBox << 8),
        RadioButton = 0x20 | CheckOrRadio, ExtendedRadioButton = 0xff | (RadioButton << 8),

        ButtonToggle = 0xff | (ExtendedButton << 8),

        SplitContainer = 0x30 | Container, ExtendedSplitContainer = 0xff | (SplitContainer << 8),
        SplitterPanel = 0x01 | (SplitContainer << 8),

        TextInput = 0x10 | ValueControl,
        NumberInput = 0x20 | ValueControl,
        DateInput = 0x30 | ValueControl,
        ListSelector = 0x40 | ValueControl,
        TrackBox = 0x50 | ValueControl,
        CompositeTrackBox = 0x01 | (TrackBox << 8),
        TrackBar = 0x60 | ValueControl,

        ColourManager = 0x10 | ComplexControl,

        ComboBox = 0x01 | (ListSelector << 8), ExtendedComboBox = 0xff | (ComboBox << 8),

        DropDownCombo = 0x01 | (ComboBox << 8),
        ListCombo = 0x02 | (ComboBox << 8),
        SimpleCombo = 0x03 | (ComboBox << 8),

        ListBox = 0x02 | (ListSelector << 8), ExtendedListBox = 0xff | (ListBox << 8),
        ListView = 0x03 | (ListSelector << 8), ExtendedListView = 0xff | (ListView << 8),

        TextBox = 0x01 | (TextInput << 8), ExtendedTextBox = 0xff | (TextBox << 8),
        RichTextBox = 0x02 | (TextInput << 8), ExtendedRichTextBox = 0xff | (RichTextBox << 8),

        ReadonlyTextBox = 0x01 | (TextBox << 8),
        WriteableTextBox = 0x02 | (TextBox << 8),
        ReadonlyRichTextBox = 0x01 | (RichTextBox << 8),
        WriteableRichTextBox = 0x02 | (RichTextBox << 8),

        Form = 0x01 | (ComplexControl << 8), ExtendedForm = 0xff | (Form << 8),
        ShieldOfFaithButton = 0x1 | (ButtonToggle << 8),
        ShaderGlassButton = 0x2 | (ButtonToggle << 8),
        ExtendedButtonToggle = 0xff | (ButtonToggle << 8),

        ShieldOfFaithPanel = 0x1 | (ExtendedPanel << 8),
        ShaderGlassPanel = 0x2 | (ExtendedPanel << 8)
    }

}
