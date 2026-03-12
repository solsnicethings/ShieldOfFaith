using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Sol.ShieldOfFaith
{
    /// <summary>
    /// Settings file format
    /// 
    /// Line starting with # (after optional whitespace) does nothing
    /// Other lines add keys and values to this dictionary
    /// The part leading up to the first : character is the key
    ///     Spaces are trimmed
    ///     Ordinal key comparison, not culture sensitive
    /// The part after is the value
    ///     If the value starts with =, the equals sign is removed and the rest of the value is unmodified
    ///     Otherwise, whitespace is removed from the start of the value
    /// 
    /// The same key can be used more than once, adding to a list of values under that key.
    /// A line without a : is a key without a value.
    /// If no value is added to a key, but the key has been listed, the list of values under that key will be null, not an empty list.
    /// 
    /// If a key is empty, just whitespace before :, the previous key is reused.
    /// </summary>
    public class Settings : Dictionary<string, List<string>>
    {
        public string LocationOnDisk => path;

        string path;
        Settings included;

        static Settings getRedirectedTo(Settings at, Dictionary<string, Settings> rtoo)
        {
            for (; ; )
            {
                if (at.included == null || at.path == null)
                    return at;
                if (rtoo.ContainsKey(at.path))
                    return at;
                if (String.IsNullOrEmpty(at.RedirectSettingsFile))
                    return at;
                rtoo.Add(at.path, at);
                at = at.included;
            }
        }

        public Settings RedirectedToSettings
            => string.IsNullOrEmpty(RedirectSettingsFile) ? null : getRedirectedTo(this, new Dictionary<string, Settings>(StringComparer.OrdinalIgnoreCase));

        public bool SaveAndApplyRedirect(string saveAs, string redirectTo)
        {
            Set(K_redirect, redirectTo);
            return Save(saveAs, true) && Load(saveAs);
        }

        public enum StartupWindows
        {
            Main,
            Shield,
            Glass,
            Options
        }

        public enum When
        {
            Never,
            Always,
            Once
        }

        void Set(string key, string value)
        {
            if (value == null)
                this[key] = null;
            else
                this[key] = new List<string>(1) { value };
        }

        void Add(string key, string value)
        {
            if (TryGetValue(key, out var l) && l != null)
            {
                if (!l.Contains(value))
                    l.Add(value);
            }
            else Set(key, value);
        }
        void Remove(string key, string value)
        {
            if (TryGetValue(key, out var l))
            {
                l?.Remove(value);
            }
            else Set(key, null);
        }

        void Set<T>(string key, IReadOnlyCollection<T> values)
        {
            if (values == null || values.Count == 0)
                Set(key, default(T).ToString());
            else
            {
                var l = new List<string>(values.Count);
                foreach (var v in values) l.Add(v.ToString());
                this[key] = l;
            }
        }

        const string
            K_autosave = "SaveSettingsOnClose",
            K_savestate = "SaveWindowStateOnClose",
            K_redirect = "RedirectConfigFile",

            K_extra_settings_file = "IncludeSettingsFile",
            K_optional_settings_file = "OptionalSettingsFile",

            K_visualstyleson = "VisualStylesEnabled",
            K_visualstylesoff = "VisualStylesDisabled",

            K_MainBorder = "WindowBorder",
            K_MainLoc = "WindowLocation",
            K_MainSize = "WindowSize",

            K_StartupWindows = "StartupWindow",

            K_GlassExecutable = "ShaderGlass custom executable",
            K_GlassProfile = "ShaderGlass profile",
            K_GlassCustomShader = "ShaderGlass shader file",

            K_ShieldColour = "Shield colour",
            K_ShieldIntensity = "Shield intensity",
            K_ShieldColourRed = "Shield colour#R",
            K_ShieldColourGreen = "Shield colour#G",
            K_ShieldColourBlue = "Shield colour#B",

            K_ControlStyler = "Appearance",

            K_LoadError = "LastLoadError";

        void CorrectKeys()
        {
            void Correct(string change_from, string change_to)
            {
                if (change_from == change_to)
                    LoadError.Add("PROGRAMMER ERROR: config key correction specifies the same name as right and wrong (" + change_from + ")");
                else if (ContainsKey(change_to))
                    Remove(change_from);
                else if (TryGetValue(change_from, out var v))
                {
                    Remove(change_from);
                    Add(change_to, v);
                }
            }
            Correct("SaveWindowStatOnClose", K_savestate);
        }

        public List<string> LoadError
        {
            get
            {
                if (TryGetValue(K_LoadError, out var l) && l != null)
                    return l;
                l = new List<string>();
                this[K_LoadError] = l;
                return l;
            }
            set => this[K_LoadError] = value;
        }

        public ICollection<string> ControlStyleScript
        {
            get => TryGetValue(K_ControlStyler, out var l) && l.Count > 0 ? l.ToArray() : null;
            set => this[K_ControlStyler] = value == null || value.Count == 0 ? null : new List<string>(value);
        }
        public void SetControlStyleScript(string script)
        {
            var l = new List<string>();
            using (var r = new StringReader(script))
            {
                string ine;
                while ((ine = r.ReadLine()) != null)
                    l.Add(ine);
            }
            this[K_ControlStyler] = l;
        }

        public IEnumerable<Tuple<string, bool>> IncludedSettingsRequiredIfTrue
        {
            get
            {
                if (TryGetValue(K_extra_settings_file, out var l) && l != null)
                    foreach (var required in l)
                        if (!string.IsNullOrWhiteSpace(required))
                            yield return new Tuple<string, bool>(required, true);
                if (TryGetValue(K_optional_settings_file, out l) && l != null)
                    foreach (var optional in l)
                        if (!string.IsNullOrWhiteSpace(optional))
                            yield return new Tuple<string, bool>(optional, false);
            }
        }
        public IEnumerable<Tuple<string, bool, string>> IncludedSettingsRequiredIfTrueWithFullPath
        {
            get
            {
                using (var e = IncludedSettingsRequiredIfTrue.GetEnumerator())
                    if (e.MoveNext())
                    {
                        var path = Path.GetDirectoryName(this.path);
                        do yield return new Tuple<string, bool, string>(e.Current.Item1, e.Current.Item2,
                            Path.GetFullPath(Path.Combine(path, e.Current.Item1))); while (e.MoveNext());
                    }
            }
        }

        /// <summary>
        /// Changes to this setting take effect when you save and then load
        /// </summary>
        public IReadOnlyCollection<string> RequiredSettings
        {
            get
            {
                if (TryGetValue(K_extra_settings_file, out var l) && l != null)
                    return l;
                l = new List<string>();
                this[K_extra_settings_file] = l;
                return l;
            }
            set { this[K_extra_settings_file] = value == null ? null : new List<string>(value); included = null; }
        }
        /// <summary>
        /// Changes to this setting take effect when you save and then load
        /// </summary>
        public IReadOnlyCollection<string> OptionalSettings
        {
            get
            {
                if (TryGetValue(K_optional_settings_file, out var l) && l != null)
                    return l;
                l = new List<string>();
                this[K_optional_settings_file] = l;
                return l;
            }
            set { this[K_optional_settings_file] = value == null ? null : new List<string>(value); included = null; }
        }

        public string RedirectSettingsFile
        {
            get => TryGetValue(K_redirect, out var a) && a != null && a.Count > 0 ? a[0] : null;
            // set => Set(K_redirect, value);
        }

        public When SaveSettingsOnClose
        {
            get => TryGetValue(K_autosave, out var l) ? EnumsFromString<When>(l).FirstOrDefault() : When.Never;
            set => Set(K_autosave, value.ToString());
        }
        public When SaveStateOnClose
        {
            get => TryGetValue(K_savestate, out var l) ? EnumsFromString<When>(l).FirstOrDefault() : When.Never;
            set => Set(K_savestate, value.ToString());
        }

        public bool EnableVisualStyles
        {
            get => ContainsKey(K_visualstyleson);
            set
            {
                if (value)
                {
                    Remove(K_visualstylesoff);
                    this[K_visualstyleson] = null;
                }
                else
                {
                    this[K_visualstylesoff] = null;
                    Remove(K_visualstyleson);
                }
            }
        }
        public FormBorderStyle MainWindowBorderStyle
        {
            get => TryGetValue(K_MainBorder, out var border) ?
                FormBorderStyle.None : border == null || border.Count == 0 ? FormBorderStyle.None
                : Enum.TryParse(border[0], out FormBorderStyle s) ? s : FormBorderStyle.None;
            set => Set(K_MainBorder, value.ToString());
        }

        public Point? MainWindowLocation
        {
            get => TryGetValue(K_MainLoc, out var loc) ?
                PointFromString(loc) : null;
            set => Set(K_MainLoc, value.HasValue ? PointToString(value.Value) : null);
        }
        static Size? SizeFromPoint(Point? point) => point.HasValue ? new Size(point.Value.X, point.Value.Y) : (Size?)null;
        public Size? MainWindowSize
        {
            get => TryGetValue(K_MainSize, out var s) ?
                SizeFromPoint(PointFromString(s)) : null;
            set => Set(K_MainSize, value.HasValue ? PointToString(new Point(value.Value.Width, value.Value.Height)) : null);
        }

        public IReadOnlyCollection<StartupWindows> StartupWindow
        {
            get => TryGetValue(K_StartupWindows, out var v) ?
                EnumsFromString<StartupWindows>(v) : new StartupWindows[] { StartupWindows.Main };
            set => Set(K_StartupWindows, value);
        }

        public string ShaderGlassPath
        {
            get => TryGetValue(K_GlassExecutable, out var v) && v != null && v.Count > 0 ?
                v[0] : null;
            set => Set(K_GlassExecutable, value);
        }
        public string ShaderGlassProfile
        {
            get => TryGetValue(K_GlassProfile, out var v) && v != null && v.Count > 0 ?
                v[0] : null;
            set => Set(K_GlassProfile, value);
        }
        public string ShaderGlassCustomShader
        {
            get => TryGetValue(K_GlassCustomShader, out var v) && v != null && v.Count > 0 ?
                v[0] : null;
            set => Set(K_GlassCustomShader, value);
        }

        public byte ShieldIntensity
        {
            get => ShieldColorAndIntensity.A;
            set { var a = ShieldColorAndIntensity; ShieldColorAndIntensity = Color.FromArgb(value, a.R, a.G, a.B); }
        }
        public byte ShieldRed
        {
            get => ShieldColorAndIntensity.R;
            set { var r = ShieldColorAndIntensity; ShieldColorAndIntensity = Color.FromArgb(r.A, value, r.G,r.B); }
        }
        public byte ShieldBlue
        {
            get => ShieldColorAndIntensity.B;
            set { var b = ShieldColorAndIntensity; ShieldColorAndIntensity = Color.FromArgb(b.A, b.R, b.G, value); }
        }
        public byte ShieldGreen
        {
            get => ShieldColorAndIntensity.G;
            set { var g = ShieldColorAndIntensity; ShieldColorAndIntensity = Color.FromArgb(g.A, g.R, value, g.B); }
        }
        public Color ShieldColor
        {
            get => ShieldColorAndIntensity;
            set => ShieldColorAndIntensity = Color.FromArgb(ShieldColorAndIntensity.A, value);
        }

        public Color ShieldColorAndIntensity
        {
            get
            {
                byte? r = null, g = null, b = null;

                if (TryGetValue(K_ShieldColour, out var l))
                {
                    foreach (var e in ColourComponentFromString(l))
                    {
                        switch (e.Item2)
                        {
                            case 'A':
                                return Color.FromArgb(e.Item1, r.Value, g.Value, b.Value);
                            case 'R':
                                r = e.Item1;
                                break;
                            case 'G':
                                g = e.Item1;
                                break;
                            case 'B':
                                b = e.Item1;
                                break;
                        }
                    }
                }

                byte checkcomponent(string key, byte? current)
                {
                    if (!TryGetValue(key, out l))
                        return current ?? 0;
                    foreach (var n in NumericFromString(l))
                        if ((n & 0xff) == n)
                            return (byte)n;
                    return 0;
                }

                if (r == null || g == null || b == null)
                {
                    r = checkcomponent(K_ShieldColourRed, r);
                    g = checkcomponent(K_ShieldColourRed, g);
                    b = checkcomponent(K_ShieldColourRed, b);
                }
                return Color.FromArgb(checkcomponent(K_ShieldIntensity, 34), r.Value, g.Value, b.Value);
            }
            set
            {
                Set(K_ShieldColour, string.Format("#{0:X2}{1:X2}{2:X2}", value.R, value.G, value.B));
                Set(K_ShieldIntensity, value.A.ToString());
                Remove(K_ShieldColourBlue);
                Remove(K_ShieldColourRed);
                Remove(K_ShieldColourGreen);
            }
        }



        public bool OpenMainOnStartup
        {
            get => StartupWindow.Contains(StartupWindows.Main);
            set
            {
                if (value) Add(K_StartupWindows, StartupWindows.Main.ToString());
                else Remove(K_StartupWindows, StartupWindows.Main.ToString());
            }
        }
        public bool OpenOptionsOnStartup
        {
            get => StartupWindow.Contains(StartupWindows.Options);
            set
            {
                if (value) Add(K_StartupWindows, StartupWindows.Options.ToString());
                else Remove(K_StartupWindows, StartupWindows.Options.ToString());
            }
        }
        public bool OpenShieldOnStartup
        {
            get => StartupWindow.Contains(StartupWindows.Shield);
            set
            {
                if (value) Add(K_StartupWindows, StartupWindows.Shield.ToString());
                else Remove(K_StartupWindows, StartupWindows.Shield.ToString());
            }
        }
        public bool OpenGlassOnStartup
        {
            get => StartupWindow.Contains(StartupWindows.Glass);
            set
            {
                if (value) Add(K_StartupWindows, StartupWindows.Glass.ToString());
                else Remove(K_StartupWindows, StartupWindows.Glass.ToString());
            }
        }

        static T FirstOr<T>(IEnumerable<T> from, T def)
        {
            using (var e = from.GetEnumerator())
                return e.MoveNext() ? e.Current : def;
        }

        static IEnumerable<int> NumericFromString(List<string> s)
        {
            if (s == null) yield break;
            foreach (var l in s)
            {
                if (string.IsNullOrEmpty(l))
                    continue;

                int r;
                if (l[0] == '#' && l.Length > 1)
                {
                    if (int.TryParse(l.Substring(1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out r))
                        yield return r;
                }
                else if (int.TryParse(l, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out r))
                    yield return r;
            }
        }

        static IEnumerable<Tuple<byte, char>> ColourComponentFromString(List<string> s)
        {
            int nextpos = 1;

            char pos_code(int pos = -1)
            {
                if (pos == -1) pos = ++nextpos;
                else nextpos = ++pos;

                switch (pos)
                {
                    case 1:
                        return 'A';
                    case 2:
                        return 'R';
                    case 3:
                        return 'G';
                    case 4:
                        return 'B';
                    default:
                        return (char)('Z' + pos - 4);
                }
            }

            if (s == null) yield break;
            foreach (var l in s)
            {
                if (string.IsNullOrEmpty(l))
                    continue;

                if (nextpos > 3)
                    nextpos = 1;

                if (l[0] == '#')
                {
                    var len = l.Length;
                    if (len < 3 || (len - 1) % 2 != 0)
                        continue;
                    var r = new byte[len / 2];
                    for (int p = 1; p < len; p += 2)
                    {
                        if (byte.TryParse(l.Substring(p, 2), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out var b))
                            r[p / 2] = b;
                        else { len = 0; break; }
                    }
                    if (len > 0)
                        switch (len)
                        {
                            case 3:
                                yield return new Tuple<byte, char>(r[0], pos_code());
                                break;
                            case 7:
                                yield return new Tuple<byte, char>(r[0], pos_code(1));
                                yield return new Tuple<byte, char>(r[1], pos_code(2));
                                yield return new Tuple<byte, char>(r[2], pos_code(3));
                                break;
                            default:
                                if (len > 7) nextpos = 0;
                                foreach (var b in r) yield return new Tuple<byte, char>(r[2], pos_code());
                                break;
                        }
                }
                else
                {
                    if (byte.TryParse(l, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var b))
                        yield return new Tuple<byte, char>(b, pos_code());
                }
            }
        }

        static T[] EnumsFromString<T>(List<string> s) where T : struct
        {
            if (s == null || s.Count == 0)
                return new T[] { };

            var l = new List<T>(s.Count);
            foreach (var e in s)
            {
                if (Enum.TryParse(e, out T val))
                    l.Add(val);
            }
            return l.ToArray();
        }


        static Point? PointFromString(IEnumerable<string> s)
        {
            if (s == null)
                return null;
            int x = 0, y;
            bool bx = false;
            foreach (var e in s)
            {
                var a = e.Split(new char[] { ',' }, 3, StringSplitOptions.RemoveEmptyEntries);
                switch (a.Length)
                {
                    case 2:
                        if (int.TryParse(a[0].Trim(), out x) && int.TryParse(a[1].Trim(), out y))
                            return new Point(x, y);
                        break;
                    case 1:
                        if (int.TryParse(a[0].Trim(), out y))
                        {
                            if (bx) return new Point(x, y);
                            x = y;
                            bx = true;
                        }
                        break;
                }
            }
            return null;
        }
        static string PointToString(Point p) => String.Format("{0},{1}", p.X, p.Y);

        public void Autofill()
        {
            SaveSettingsOnClose = SaveSettingsOnClose;

            EnableVisualStyles = EnableVisualStyles;
            MainWindowBorderStyle = MainWindowBorderStyle;
            MainWindowLocation = MainWindowLocation;
            MainWindowSize = MainWindowSize;

            ShieldColorAndIntensity = ShieldColorAndIntensity;

            ShaderGlassCustomShader = "ShieldOfFaith.slang";
            ShaderGlassProfile = "ShieldOfFaith.sgp";
            ShaderGlassPath = "ShaderGlassForShieldOfFaith.exe";

            if (!ContainsKey(K_StartupWindows))
                StartupWindow = new StartupWindows[] { StartupWindows.Main, StartupWindows.Shield };

            if (!(ContainsKey(K_optional_settings_file) || ContainsKey(K_extra_settings_file)))
                Set(K_optional_settings_file, "defaults.cfg");

        }

        public Settings() : base(StringComparer.OrdinalIgnoreCase) { }
        private Settings(IDictionary<string, List<string>> copy) : base(copy, StringComparer.OrdinalIgnoreCase) { }

        public bool Load(string path , Dictionary<string, Settings> preloaded = null)
        {
            path = Path.GetFullPath(path);

                if (preloaded != null)
                    if (preloaded.TryGetValue(path, out var alt))
                    {
                        Clear();
                        foreach (var e in alt)
                            Add(e.Key, e.Value);
                        this.path = path;
                        return true;
                    }
                    else preloaded.Add(path, this);

            if (Count != 0)
            {
                LoadError.Clear();
                var proxy = new Settings();
                if (proxy.Load(path))
                {
                    Clear();
                    foreach (var p in proxy)
                        Add(p.Key, p.Value);
                    return true;
                }
                LoadError = proxy.LoadError;
                return false;
            }

            LoadError.Clear();

            try
            {
                using (var r = new StreamReader(path))
                {
                    string k = null;
                    for (string l; (l = r.ReadLine()) != null;)
                    {
                        var s = l.Split(new char[] { ':' }, 2);
                        if (s.Length == 0)
                            continue;
                        var e = s[0];
                        if (!string.IsNullOrWhiteSpace(e))
                        {
                            e = e.Trim();
                            if (e[0] == '#')
                                continue;
                            k = e;
                        }

                        if (s.Length == 1)
                        {
                            if (!ContainsKey(k))
                                Add(k, null);
                            continue;
                        }

                        if (TryGetValue(k, out var v) == false || v == null)
                            this[k] = v = new List<string>(1);
                        e = s[1];
                        if (e.Length == 0) e = string.Empty;
                        else if (e[0] == '=') e = e.Substring(1);
                        else e = e.TrimStart();
                        v.Add(e);
                    }
                }
            }
            catch (IOException x) { LoadError.Add(x.Message); return false; }
            catch (ArgumentException x) { LoadError.Add(x.Message); return false; }
            catch (Exception x) { LoadError.Add(x.Message); throw; }

            Remove(K_LoadError + '#');
            CorrectKeys();

            this.path = path;
            included = null;
            using (var inc = IncludedSettingsRequiredIfTrue.GetEnumerator())
            {
                if (inc.MoveNext())
                {
                    path = Path.GetDirectoryName(path);
                    if (preloaded == null)
                    {
                        preloaded = new Dictionary<string, Settings>();
                        preloaded.Add(this.path, this);
                    }
                    if (!TryGetValue(K_LoadError, out var err)) err = null;
                    do
                    {
                        var proxy = new Settings();
                        var loaded = proxy.Load(Path.Combine(path, inc.Current.Item1), preloaded);
                        if (proxy.TryGetValue(K_LoadError, out var perr) && perr != null && perr.Count > 0 && (loaded || inc.Current.Item2))
                        {
                            if (err == null) Add(K_LoadError, perr);
                            else err.AddRange(perr);
                        }
                        if (loaded)
                        {
                            if (included == null) included = proxy;
                            else
                            {
                                if (included.path != null && included == preloaded[included.path])
                                    included = new Settings(included);
                                foreach (var cfg in proxy)
                                {
                                    if (!included.ContainsKey(cfg.Key))
                                        included.Add(cfg.Key, cfg.Value);
                                }
                            }
                        }
                    } while (inc.MoveNext());

                    if (included != null)
                    {
                        included.Remove(K_LoadError);
                        foreach (var cfg in included)
                            if (!ContainsKey(cfg.Key))
                                Add(cfg.Key, cfg.Value);
                    }
                }
            }

            {
                var redirect = RedirectSettingsFile;
                if (!String.IsNullOrEmpty(redirect))
                {
                    if (this.path == path) path = Path.GetDirectoryName(path);

                    if (preloaded == null)
                    {
                        preloaded = new Dictionary<string, Settings>();
                        preloaded.Add(this.path, this);
                    }

                    Dictionary<string, List<string>>
                        keepout = new Dictionary<string, List<string>>(StringComparer.Ordinal);

                    foreach (var c in new string[] {K_redirect, K_extra_settings_file, K_optional_settings_file})
                    {
                        if (TryGetValue(c, out var l))
                        {
                            keepout.Add(c, l);
                            Remove(c);
                        }
                    }

                    try
                    {
                        included = null;

                        Settings r = new Settings();
                        path = Path.Combine(path, redirect);
                        if (r.Load(path, preloaded))
                            included = r;
                        else if (File.Exists(path))
                            LoadError.AddRange(r.LoadError);
                        else
                        {
                            r.RequiredSettings = new string[] { Path.GetDirectoryName( this.path) == Path.GetDirectoryName(path) ? Path.GetFileName(this.path) : this.path };
                            r.SaveSettingsOnClose = When.Always;
                            r.SaveStateOnClose = When.Always;
                            r.LoadError.Clear();
                            r.LoadError.Add("This file did not exist and was pointed to by a redirect, so it was created.");
                            if (r.Save(path) && r.Load(path, preloaded))
                                included = r;
                            else LoadError.Add("Could not create valid redirect file at " + path);
                        }
                    }
                    finally
                    {
                        foreach (var c in keepout)
                            Add(c.Key, c.Value);
                    }
                }
            }

            return true;
        }

        public bool Save(string path, bool force = false)
        {
            if (!(force || string.IsNullOrEmpty(RedirectSettingsFile)))
                throw new InvalidOperationException("Can not save a configuration file that has a redirect in it; these should be edited manually. If RedirectedToSettings is not null after loading, the program should switch to the settings in that property.");

            var saveonclose = SaveSettingsOnClose;
            var stateonclose = SaveStateOnClose;
            if (saveonclose == When.Once)
                SaveSettingsOnClose = When.Never;
            try
            {
                if (stateonclose == When.Once)
                    SaveStateOnClose = When.Never;
                var breather = false;
                using (var w = new StreamWriter(path))
                {
                    foreach (var e in (included == null ? (IEnumerable<KeyValuePair<string, List<string>>>)this : this.Where(e =>
                    {
                        if (included.TryGetValue(e.Key, out var v))
                        {
                            if (ReferenceEquals(v, e.Value))
                                return false;
                            if (v == null || e.Value == null)
                                return true;
                            int c = v.Count;
                            if (c != e.Value.Count)
                                return true;
                            while (--c > -1)
                                if (v[c] != e.Value[c])
                                    return true;
                            return false;
                        }
                        return true;
                    })).OrderBy(e=>
                    {
                        bool leading = true;
                        for (int i = 0; i < e.Key.Length; i++)
                        {
                            if (e.Key[i] == ':')
                                throw new FormatException("key name contains separator symbol");
                            else if (leading)
                            {
                                if (e.Key[i] == '#')
                                    throw new FormatException("key name starts with #, making it a line excluder");
                                if (!char.IsWhiteSpace(e.Key[i]))
                                    leading = false;
                            }
                        }
                        if (leading)
                            throw new FormatException("key is pure whitespace (silly, and conflicting with the key repeat syntax)");
                        return e.Key;
                    }))
                    {
                        var k = e.Key;

                        if (k == K_LoadError)
                        {
                            if (e.Value == null || e.Value.Count == 0)
                                continue;
                            k += '#';
                        }

                        if (e.Value == null)
                        {
                            if (breather) w.WriteLine();
                            w.WriteLine(k);
                            breather = true;
                        }
                        else
                        {
                            var wrote = false;
                            foreach (var l in e.Value)
                            {
                                if (breather)
                                {
                                    w.WriteLine();
                                    breather = false;
                                }
                                wrote = true;
                                w.Write(k + ":");
                                if (l.Length == 0 || (char.IsWhiteSpace(l[0]) || l[0] == '='))
                                    w.WriteLine("=" + l);
                                else w.WriteLine(" " + l);
                                k = " ";
                            }
                            if (wrote) breather = true;
                        }
                    }
                }
            }
            catch (IOException) { return false; }
            catch (ArgumentException) { return false; }
            finally { SaveSettingsOnClose = saveonclose; SaveStateOnClose = stateonclose; }

            this.path = path;
            return true;
        }
        public bool Save() => Save(path);
    }
}
