using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sol.ShieldOfFaith
{
    public partial class Composure : Form
    {
        [Flags]
        enum ConfigFunction
        {
            Undefined,

            File = 0x1,
            Directory = 0x2,
            Default = 0x4,
            Active = 0x8,

            Missing = unchecked((int)0x80000000),

            Referenced = 0x10,
            Optional = 0x20,
            Required = 0x40,
            Inherited = 0x80,

            DataDirectory = 0x100| Directory,
            Config = 0x200 | File,
            Shader = 0x300 | File,
            ShaderProfile = 0x400 | File,
            ShaderApp = 0x500 | File,
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
            return i;
        }

        void PopulateFileSystemList()
        {
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
                            if (!added.ContainsKey(c.Key)) addfileorsubdir(c.Key, attr: c.Value);
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
                        if (added.ContainsKey(s.Item3)) continue;
                        added.Add(s.Item3, addConfigItem(Path.GetFileName(s.Item1), s.Item2 ?
                            ConfigFunction.Config | ConfigFunction.Required : ConfigFunction.Config | ConfigFunction.Optional, s.Item3));
                    }

                    foreach (var f in Program.Settings.ResourcePaths_valueShowsDirectSpecification)
                        addfileorsubdir(f.Key, f.Value, f.Value == null ? ConfigFunction.Inherited | ConfigFunction.Referenced : ConfigFunction.Referenced);

                    foreach (var s in Program.Settings.AccumulatedRequiredFiles)
                        if (added.TryGetValue(s, out var i) && Enum.TryParse<ConfigFunction>(i.SubItems[chCfgFunction.Index].Text, out var f))
                        {
                            if ((f & (ConfigFunction.Required | ConfigFunction.File)) == (ConfigFunction.Required | ConfigFunction.File)) continue;
                            f = (f | ConfigFunction.Required | ConfigFunction.Inherited | ConfigFunction.File) & ~ConfigFunction.Optional;
                            i.SubItems[chCfgFunction.Index].Text = f.ToString();
                        }
                        else addfileorsubdir(s, fnc: ConfigFunction.Required | ConfigFunction.Inherited | ConfigFunction.File);
                    foreach (var s in Program.Settings.AccumulatedOptionalFiles)
                        if (added.TryGetValue(s, out var i) && Enum.TryParse<ConfigFunction>(i.SubItems[chCfgFunction.Index].Text, out var f))
                        {
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
                addfileorsubdir(e, attr: attr);
            }

            chCfgName.Width = -2;
            chCfgPath.Width = -2;
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
    }
}
