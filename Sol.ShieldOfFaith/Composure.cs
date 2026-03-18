using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sol.ShieldOfFaith
{
    public partial class Composure : Form
    {
        public event EventHandler ShaderChanged, ShaderProfileChanged;

        [Flags]
        enum ConfigFunction
        {
            Undefined,

            File = 0x1,
            Directory = 0x2,
            Default = 0x4,
            Active = 0x8,

            Missing = unchecked((int)0x80000000),
            Not = 0x40000000,

            ExplicitReference = 0x10,
            Optional = 0x20,
            Required = 0x40,
            Inherited = 0x80,

            DataDirectory = 0x100| Directory,
            Config = 0x200 | File,
            Shader = 0x400 | File,
            ShaderProfile = 0x800 | File,
            ShaderApp = 0x1000 | File,

            DirectoryContents = 0x2000
        }

        void ResetSessionInfo()
        {
            outputSessionDetails.Rtf = string.Empty;

            Output("Session details", null);
            if (Program.args.Length == 0)
                Output(null, "No command line arguments");
            else
                Output("Command line arguments", string.Join(" ", Program.args));
            
            Output("Active settings file", Program.Settings.LocationOnDisk);
            Output("Application data container", Program.AppDataLocation);

            using (var p = System.Diagnostics.Process.GetCurrentProcess())
            {
                lastEvent = p.StartTime;
                Output("Start time", p.StartTime.ToLongTimeString() + " , " + p.StartTime.ToLongDateString());
            }

            if (Program.Problem != null)
            {
                Output("Startup issues", null);
                foreach (var p in Program.Problem) Output(null, p);
            }
            {
                var l = Program.Settings.LoadError;
                if (l.Count > 0)
                {
                    Output("Settings file issues", null);
                    foreach (var p in l) Output(null, p);
                }
            }

            comboFilesByOrigin.SelectedIndex = 0;
            comboFilesByOrigin.Items.Add(ConfigFunction.DirectoryContents);
            comboFilesByOrigin.Items.Add(ConfigFunction.ExplicitReference);
            comboFilesByOrigin.Items.Add(ConfigFunction.Inherited);
            comboFilesByOrigin.Items.Add(ConfigFunction.Not | ConfigFunction.Inherited);

            Output("Event trace", null);
        }

        public Composure()
        {
            InitializeComponent();
            switch (Program.TraceHighOrLow)
            {
                case true:
                    radioTraceHigh.Checked = true;
                    break;
                case false:
                    radioTraceLow.Checked = true;
                    break;
                default:
                    radioTraceOff.Checked = true;
                    break;
            }
            ResetSessionInfo();

            {
                var rx = new System.Text.RegularExpressions.Regex(@"\.[^\.]{1,4}$", System.Text.RegularExpressions.RegexOptions.Compiled);
                foreach (var e in Program.GetEmbedsIn("Help", true).OrderBy(e => e.Item1))
                {
                    var page = new TabPage(rx.Replace(e.Item2.Name, string.Empty));
                    page.Tag = e.Item2;
                    tabHelpSelector.TabPages.Add(page);
                }
                tabHelpSelector_SelectedIndexChanged(tabHelpSelector, EventArgs.Empty);
            }

            PopulateFileSystemList();
        }

        ListViewItem[] configFileSysUnfiltered;
        ConfigFunction GetCfgFunc(ListViewItem item) => (ConfigFunction)item.SubItems[chCfgFunction.Index].Tag;
        string GetCfgPath(ListViewItem item) => item.SubItems[chCfgPath.Index].Text;

        void FilterFileList(ICollection<ConfigFunction> criteria)
        {
            configFileSystem.SuspendLayout();
            try
            {
                if (criteria.Count == 0)
                {
                    if (configFileSysUnfiltered != null)
                    {
                        configFileSystem.Items.Clear();
                        configFileSystem.Items.AddRange(configFileSysUnfiltered);
                        configFileSysUnfiltered = null;
                    }
                    return;
                }

                if (configFileSysUnfiltered == null)
                {
                    configFileSysUnfiltered = new ListViewItem[configFileSystem.Items.Count];
                    configFileSystem.Items.CopyTo(configFileSysUnfiltered, 0);
                }

                configFileSystem.Items.Clear();
                foreach (var item in configFileSysUnfiltered) {
                    foreach (var f in criteria)
                    {
                        if ((f & GetCfgFunc(item)) == 0)
                        {
                            if ((f & ConfigFunction.Not) == 0) goto filterOut;
                        }
                        else
                        {
                            if ((f & ConfigFunction.Not) == ConfigFunction.Not) goto filterOut;
                        }
                    }
                    configFileSystem.Items.Add(item);
                    filterOut:;
                }
            }
            finally {  configFileSystem.ResumeLayout(); }
        }

        ListViewItem addConfigItem(string name, ConfigFunction function, string path)
        {
            var s = new string[4];
            string xm = null;
            try
            {
                switch (function & (ConfigFunction.File | ConfigFunction.Directory |  ConfigFunction.Missing))
                {
                    case ConfigFunction.File:
                        if (!File.Exists(path)) function |= ConfigFunction.Missing;
                        break;
                    case ConfigFunction.Directory:
                        if (!Directory.Exists(path)) function |= ConfigFunction.Missing;
                        break;
                }
            }
            catch (IOException ex)
            {
                xm = ex.Message;
            }
            s[chCfgFunction.Index] = xm ?? function.ToString();
            s[chCfgName.Index] = name ?? string.Empty;
            s[chCfgPath.Index] = path ?? string.Empty;

            ListViewItem i;
            configFileSystem.Items.Add(i = new ListViewItem(s));
            i.SubItems[chCfgFunction.Index].Tag = function;
            return i;
        }

        public void PopulateFileSystemList()
        {
            configFileSystem.SuspendLayout();
            try
            {
                configFileSystem.Items.Clear();
                configFileSysUnfiltered = null;
                var added = new Dictionary<string, ListViewItem>(StringComparer.OrdinalIgnoreCase);

                void addfileorsubdir(string fullpath, string rawpath = null, ConfigFunction fnc = 0, FileAttributes attr = 0)
                {
                    if (attr == 0 && !Program.TryGetFileAttributes(fullpath, out attr))
                        fnc |= ConfigFunction.Missing;
                    else
                    {
                        if ((attr & FileAttributes.Directory) == 0)
                            fnc |= ConfigFunction.File;
                        else
                        {
                            added[fullpath] = addConfigItem(rawpath == null || rawpath == fullpath ? Path.GetFileName(fullpath) : rawpath,
                                fnc | ConfigFunction.Directory, fullpath);
                            foreach (var c in Program.GetAvailableFilesFromTree(fullpath))
                                if (!added.ContainsKey(c.Key)) addfileorsubdir(c.Key, null, ConfigFunction.DirectoryContents, c.Value);
                            return;
                        }
                    }

                    switch (Path.GetExtension(fullpath).ToLowerInvariant())
                    {
                        case ".cfg":
                            fnc |= ConfigFunction.Config;
                            break;
                        case ".slang":
                            fnc |= ConfigFunction.Shader;
                            break;
                        case ".sgp":
                            fnc |= ConfigFunction.ShaderProfile;
                            break;
                    }

                    added[fullpath] = addConfigItem(
                        rawpath == null || rawpath == fullpath ? Path.GetFileName(fullpath) : rawpath, fnc, fullpath
                        );
                }

                var l = Program.AppDataLocation;
                {
                    var d = Program.DefaultAppDataLocation;

                    if (!string.Equals(d, l, StringComparison.OrdinalIgnoreCase))
                        added.Add(d, addConfigItem("%AppData%\\..", ConfigFunction.DataDirectory | ConfigFunction.Default, d));

                    addConfigItem(Path.GetFileName(l), added.Count == 0 ? ConfigFunction.DataDirectory : ConfigFunction.DataDirectory | ConfigFunction.Active, l);

                    d = Program.Settings.LocationOnDisk;
                    if (!string.IsNullOrEmpty(d))
                    {
                        added[d] = addConfigItem(Path.GetFileName(d), ConfigFunction.Config | ConfigFunction.Active, d);

                        foreach (var s in Program.Settings.IncludedSettingsRequiredIfTrueWithFullPath)
                        {
                            if (added.ContainsKey(s.Item3 ?? s.Item1)) continue;
                            added.Add(s.Item3 ?? s.Item1, addConfigItem(Path.GetFileName(s.Item1), s.Item2 ?
                                ConfigFunction.Config | ConfigFunction.Required : ConfigFunction.Config | ConfigFunction.Optional, s.Item3 ?? s.Item1));
                        }

                        foreach (var f in Program.Settings.ResourcePaths_valueShowsDirectSpecification)
                            addfileorsubdir(f.Key, f.Value, f.Value == null ? ConfigFunction.Inherited | ConfigFunction.ExplicitReference : ConfigFunction.ExplicitReference);

                        foreach (var s in Program.Settings.AccumulatedRequiredFiles)
                            if (added.TryGetValue(s, out var i))
                            {
                                var f = GetCfgFunc(i);
                                if ((f & (ConfigFunction.Required | ConfigFunction.File)) == (ConfigFunction.Required | ConfigFunction.File)) continue;
                                f = (f | ConfigFunction.Required | ConfigFunction.Inherited | ConfigFunction.File) & ~ConfigFunction.Optional;
                                i.SubItems[chCfgFunction.Index].Text = f.ToString();
                            }
                            else addfileorsubdir(s, fnc: ConfigFunction.Required | ConfigFunction.Inherited | ConfigFunction.File);
                        foreach (var s in Program.Settings.AccumulatedOptionalFiles)
                            if (added.TryGetValue(s, out var i))
                            {
                                var f = GetCfgFunc(i);
                                switch (f & (ConfigFunction.Required | ConfigFunction.Optional | ConfigFunction.File))
                                {
                                    case ConfigFunction.Required | ConfigFunction.File:
                                        continue;
                                    case ConfigFunction.Optional | ConfigFunction.File:
                                        continue;
                                }
                                f |= ConfigFunction.Optional | ConfigFunction.Inherited | ConfigFunction.File;
                                i.SubItems[chCfgFunction.Index].Text = f.ToString();
                            }
                            else addfileorsubdir(s, fnc: ConfigFunction.Optional | ConfigFunction.Inherited | ConfigFunction.File);

                    }

                    d = Program.ShaderGlassExecutable;
                    if (d != null)
                        added[d] = addConfigItem("ShaderGlass.exe", ConfigFunction.ShaderApp, d);

                    d = Program.Settings.ShaderGlassProfile;
                    if (!string.IsNullOrEmpty(d))
                    {
                        var f = Program.FindPathTo(d);
                        if (f == null)
                            addConfigItem(Path.GetFileName(d), ConfigFunction.ShaderProfile | ConfigFunction.Active | ConfigFunction.Missing, d);
                        else
                            added[f] = addConfigItem(d, ConfigFunction.ShaderProfile | ConfigFunction.Active, f);
                    }

                    d = Program.Settings.ShaderGlassCustomShader;
                    if (!string.IsNullOrEmpty(d))
                    {
                        var f = Program.FindPathTo(d);
                        if (f == null)
                            addConfigItem(Path.GetFileName(d), ConfigFunction.Shader | ConfigFunction.Active | ConfigFunction.Missing, d);
                        else
                            added[f] = addConfigItem(d, ConfigFunction.Shader | ConfigFunction.Active, f);
                    }
                }
                if (Directory.Exists(l))
                    foreach (var e in Directory.GetFileSystemEntries(l))
                    {
                        if (added.ContainsKey(e)) continue;
                        if (Program.TryGetFileAttributes(e, out var attr) == false || 0 != (attr & (FileAttributes.Hidden | FileAttributes.System)))
                            continue;
                        addfileorsubdir(e, null, ConfigFunction.DirectoryContents, attr);
                    }

                filelistfilter_Changed(this, EventArgs.Empty);
                chCfgName.Width = -2;
                chCfgPath.Width = -2;
            }
            finally { configFileSystem.ResumeLayout(); }
       }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Program.Events.Count == 0 || Visible == false) return;
            GrabEvents();
        }

        DateTime lastEvent;

        public void GrabEvents()
        {
            if (!Visible)
                return;
            foreach (var log in Program.Events)
            {
                if (log.Item2.Date != lastEvent.Date)
                {
                    Output(log.Item2.ToLongDateString(), "(session spans multiple calendar days)");
                    lastEvent = log.Item2;
                }
                Output((log.Item2.ToLongTimeString()).ToString(), log.Item1);
            }
            Program.Events.Clear();
        }

        void Output(string header, string detail)
        {
            outputSessionDetails.Select(outputSessionDetails.Text.Length, 0);

            if (header == null)
                goto detail;

            if (detail == null)
            {
                // big header
                outputSessionDetails.SelectionColor = Color.FromArgb(
                    Math.Min((outputSessionDetails.ForeColor.R * 11 - outputSessionDetails.BackColor.R) / 10, 255),
                    Math.Min((outputSessionDetails.ForeColor.G * 11 - outputSessionDetails.BackColor.G) / 10, 255),
                    Math.Min((outputSessionDetails.ForeColor.B * 11 - outputSessionDetails.BackColor.B) / 10, 255)
                    );
                outputSessionDetails.SelectionFont = new Font(outputSessionDetails.Font.FontFamily,
                    outputSessionDetails.Font.Size * 1.15f, FontStyle.Bold | FontStyle.Underline);
                outputSessionDetails.AppendText((outputSessionDetails.Text.Length == 0 ? header : "\n" + header) + "\n\n");
                outputSessionDetails.SelectionColor = outputSessionDetails.ForeColor;
                return;
            }
            else
            {
                // subheader
                outputSessionDetails.SelectionFont = new Font(outputSessionDetails.Font.FontFamily,
                    outputSessionDetails.Font.Size * 1.1f, FontStyle.Bold);
                outputSessionDetails.AppendText(header + ": ");
                outputSessionDetails.Select(outputSessionDetails.Text.Length, 0);
            }
        detail:;
            outputSessionDetails.SelectionFont = outputSessionDetails.Font;
            outputSessionDetails.AppendText((header == null && !outputSessionDetails.Text.EndsWith("\n\n") ? "\n" + detail : detail) + "\n");
        }

        private void radioTrace_CheckedChanged(object sender, EventArgs e)
        {
            Program.TraceHighOrLow =
                radioTraceOff.Checked ? (bool?)null :
                radioTraceLow.Checked ? false : true;
        }

        private void buttonClearTrace_Click(object sender, EventArgs e)
        {
            ResetSessionInfo();
        }

        private void tabHelpSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            helpTextBox.Rtf = ((ZipArchiveEntry)tabHelpSelector.SelectedTab.Tag).Read();
            helpTextBox.Select(0, 0);
            helpTextBox.ScrollToCaret();
        }

        private void filelistfilter_Changed(object sender, EventArgs e)
        {
            ConfigFunction filter_no = 0;
            var filters = new List<ConfigFunction>();

            void apply(ConfigFunction filter)
            {
                if ((filter & ConfigFunction.Not) == 0) 
                {
                    if (checkShowDirs.Checked)
                        filter |= ConfigFunction.Directory;
                    filters.Add(filter);
                }
                else filter_no |= filter;
            }

            {
                if (comboFilesByOrigin.SelectedItem is ConfigFunction filter)
                    apply(filter);
            }

            if (!checkShowDirs.Checked) apply(ConfigFunction.Directory | ConfigFunction.Not);

            if (radioOtherfiles.Checked) apply(
                (ConfigFunction.Not | ConfigFunction.Config | ConfigFunction.Shader | ConfigFunction.ShaderProfile | ConfigFunction.ShaderApp)
                & ~ConfigFunction.File);

            else if (radioShadio.Checked) apply((ConfigFunction.Shader | ConfigFunction.ShaderApp) & ~ConfigFunction.File);
            else if (radioShadioProfilo.Checked) apply((ConfigFunction.ShaderProfile | ConfigFunction.ShaderApp) & ~ConfigFunction.File);

            else if (radioShowConfigs.Checked) apply((ConfigFunction.Config) & ~ConfigFunction.File);
            else if (radioShowNoFiles.Checked) apply(ConfigFunction.Not | ConfigFunction.File);

            switch (checkMissing.CheckState)
            {
                case CheckState.Checked:
                    filters.Add(ConfigFunction.Missing);
                    break;
                case CheckState.Unchecked:
                    filter_no |= ConfigFunction.Not | ConfigFunction.Missing;
                    break;
            }

            if (filter_no != 0) filters.Add(filter_no);
            FilterFileList(filters);
        }

        private void labelFileOrigin_Click(object sender, EventArgs e)
        {
            comboFilesByOrigin.Focus();
        }

        private void buttonActivate_Click(object sender, EventArgs e)
        {
            var i = configFileSystem.SelectedItems[0];
            
            switch (GetCfgFunc(i) & (ConfigFunction.Shader | ConfigFunction.ShaderProfile))
            {
                case ConfigFunction.Shader:
                    Program.Settings.ShaderGlassCustomShader = GetCfgPath(i);
                    ShaderChanged?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    Program.Settings.ShaderGlassProfile = GetCfgPath(i);
                    ShaderProfileChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
            PopulateFileSystemList();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            var i = configFileSystem.SelectedItems[0];
            var p = GetCfgPath(i);

            switch (GetCfgFunc(i) & (ConfigFunction.Directory))
            {
                case ConfigFunction.Directory:
                    try
                    {
                        Process.Start(p)?.Dispose();
                    }
                    catch (Win32Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Open " + p, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                default:
                    using (var process = new Process())
                    {
                        process.StartInfo = new ProcessStartInfo("notepad.exe", "\"" + p + "\"");
                        process.Start();
                    }
                    break;
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {

        }

        private void configFileSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (configFileSystem.SelectedItems.Count == 0)
            {
                panelFileActions.Enabled = false;
                return;
            }
            panelFileActions.Enabled = true;

            var i = configFileSystem.SelectedItems[0];
            var f = GetCfgFunc(i);

            bool allow(ConfigFunction require_any) => 0 != (require_any & f);
            bool require(ConfigFunction require_all) => require_all == (f & require_all);
            bool forbid(ConfigFunction forbid_all) => 0 == (forbid_all & f);
            bool filetypes(ConfigFunction filetypes_any)
            {
                switch (filetypes_any & f)
                {
                    case ConfigFunction.File:
                    case 0:
                        return false;
                }
                return true;
            }

            void update(Control target, bool visibility)
            {
                target.Visible = visibility;
                if (visibility) target.BringToFront();
            }

            update(buttonCreate, false && require(ConfigFunction.Missing));
            update(buttonActivate, forbid(ConfigFunction.Active | ConfigFunction.Missing)
                && filetypes(ConfigFunction.ShaderProfile | ConfigFunction.Shader));
            update(buttonOpen, forbid(ConfigFunction.Missing) && allow(ConfigFunction.Directory | ConfigFunction.File) &&
                !GetCfgPath(i).EndsWith(".exe", StringComparison.OrdinalIgnoreCase));
        }
    }
}
