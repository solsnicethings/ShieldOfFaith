using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            Min,
            Max,
            None
        }

        public Func<Control, ControlSelectorCode, ControlSelectorCode>
            ControlTypeResolver;

        public List<Dictionary<ControlPropertyCode, object>> GetPrioritisedSpecifications(ControlSelectorCode controlType)
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

            IEnumerable<Dictionary<ControlPropertyCode, object>> Get(params ControlPropertyCode[] cpc)
            {
                var level = new Dictionary<ControlPropertyCode, object>();
                foreach (var spec in pspec)
                {
                    foreach (var x in cpc)
                        if (spec.TryGetValue(x, out var v))
                            level.Add(x, v);
                    if (level.Count > 0)
                    {
                        yield return level;
                        level = new Dictionary<ControlPropertyCode, object>();
                    }
                }
            }
            IEnumerable<object> GetValue(ControlPropertyCode cpc)
            {
                foreach (var spec in pspec)
                    if (spec.TryGetValue(cpc, out var v))
                        yield return v;
            }

            void ResolveColour(Action<Color> assign, Func<Color> getContext, IEnumerable<object> value)
            {
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
                            case SpecialValue.Inherit:
                                assign(getContext());
                                return;
                        }
                }
                assign(getContext());
            }

            ResolveColour(c => target.ForeColor = c, () => target.Parent?.ForeColor ?? Color.Khaki, GetValue(ControlPropertyCode.Foreground));
            ResolveColour(c => target.BackColor = c, () => target.Parent?.BackColor ?? Color.FromArgb(64, 64, 64), GetValue(ControlPropertyCode.Background));


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

    }
    public enum ControlPropertyCode
    {
        Foreground,
        Background,
        BackgroundImage,

        FontFamily,
        FontSize,
        FontWeight,
        Font,

        ToggledBackground,
        ToggleForeground,

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
        ExtendedButtonToggle = 0xff | (ButtonToggle << 8)
    }

}
