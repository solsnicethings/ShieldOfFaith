using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using IPC = ShaderIPCforCLRControl.ShaderMessagingWindow;

namespace Sol.ShieldOfFaith
{
    public partial class Faith : Form
    {
        Composure configWin;
        Shield shieldWin;

        // public Faith(Shield shield) : this() => shieldWin = shield;

        IPC ipc;
        Process shaderglass;
        string executable
        {
            get
            {
                var p = Program.Settings.ShaderGlassPath;
                if (string.IsNullOrWhiteSpace(p))
                    return null;

                // can only be relative to program folder (presumed higher security than user folders),
                // otherwise low-security folders can be used to unexpectedly swap in executable
                return Path.GetFullPath(Path.Combine(Program.GetExecutableContainingFolder(), p));
            }
            set
            {
                Program.Settings.ShaderGlassPath = value;
                var path = executable;

                { if (toggleButtonGlass.Tag is string s) toggleButtonGlass.Text = s; }
                {
                    var img = toggleButtonGlass.BackgroundImage;
                    toggleButtonGlass.BackgroundImage = null;
                    protectorGlass.HeaderImage = null;
                    img?.Dispose();
                }
                if (path == null) return;
                try
                {
                    var ico = Icon.ExtractAssociatedIcon(path);
                    if (ico != null)
                    {
                        /*
                        Bitmap bmp = new Bitmap(ico.Width, ico.Height);
                        try
                        {
                            using (var g = Graphics.FromImage(bmp))
                                g.DrawIcon(ico, ico.Width, ico.Height);
                        }
                        catch { bmp.Dispose(); throw; }
                        finally { ico.Dispose(); }*/
                        var bmp = ico.ToBitmap();
                        toggleButtonGlass.BackgroundImage = bmp;
                        protectorGlass.HeaderImage = bmp;
                        if (toggleButtonGlass.Tag == null) toggleButtonGlass.Tag = toggleButtonGlass.Text;
                        toggleButtonGlass.Text = String.Empty;
                    }
                }
                catch (InvalidOperationException)
                {

                }
                catch (ArgumentException)
                {
                }
                catch (IOException)
                {
                }
            }
        }

        void ClearIPC()
        {
            buttonRefresh.Enabled = false;
            buttonRefresh.ToggledOn = false;

            protectorGlass.ResumeLayout();

            var ipc = this.ipc;
            var shaderglass = this.shaderglass;
            if (ipc == null && shaderglass == null)
                return;
            this.ipc = null;
            this.shaderglass = null;

            configWin.RecordEvent("Clearing out IPC process", true);
            if (ipc != null)
            {
                ipc.Dispose();
            }
            if (shaderglass != null)
            {
                try
                {
                    if (!shaderglass.WaitForExit(500))
                    {
                        shaderglass.CloseMainWindow();
                        if (!shaderglass.WaitForExit(2000))
                            shaderglass.Kill();
                    }
                }
                catch (InvalidOperationException) { }
                shaderglass.Dispose();
            }
        }

        bool preparing_to_load_shader;
        string shader_profile;

        void CreateIPCWin()
        {
            toggleButtonGlass.Enabled = true;
            var executable = this.executable;
            retry:
            if (string.IsNullOrWhiteSpace(executable))
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                this.executable = executable = openFileDialog1.FileName;
            }
            try
            {
                ClearIPC();

                configWin.RecordEvent("Launching " + executable, true);

                ipc = new IPC();

                shaderglass = new Process();

                var profile = Program.Settings.ShaderGlassProfile;

                if (!string.IsNullOrEmpty(profile))
                {
                    profile = Program.FindPathTo(profile);
                    if (profile == null)
                    {
                        if (MessageBox.Show(profile + " not found. Launch without specifying a profile? (its default settings may be inconvenient; putting the right profile path in the configuration file may be better.)", "ShaderGlass profile missing", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                            != DialogResult.Yes)
                        {
                            ClearIPC();
                            return;
                        }
                    }
                    else profile = " \"" + profile + "\"";
                }
                shader_profile = string.IsNullOrEmpty(profile) ? null : profile;

                shaderglass.StartInfo = new ProcessStartInfo(executable,
                    (toggleGlassWindow.ToggledOn ? "-ipc": "-f -ipc") + profile);

                if (shader_profile != null)
                    shader_profile = shader_profile.Replace("\"", string.Empty).Trim();

                if (!string.IsNullOrEmpty(shaderglass.StartInfo.Arguments))
                    configWin.RecordEvent("with arguments = " + shaderglass.StartInfo.Arguments);
                shaderglass.EnableRaisingEvents = true;
                shaderglass.Exited += Shaderglass_Exited;
                shaderglass.Start();
            }
            catch (Win32Exception)
            {
                ClearIPC();
                if (MessageBox.Show("Unable to start " + executable + ".\r\n\r\nLocate program manually and retry?",
                    "Launch failed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    executable = null;
                    goto retry;
                }
                return;
            }
            catch { this.executable = null;ClearIPC(); configWin.RecordEvent("Launch failed", true); throw; }

            if (this.executable != executable) this.executable = executable;

            toggleButtonGlass.Enabled = false;

            ipc.AfterHandshake += Ipc_AfterHandshake;
            ipc.Created += Ipc_Created;
            ipc.BeginRedefine += Ipc_BeginRedefine;
            ipc.EndRedefine += Ipc_EndRedefine;
            ipc.ParameterReceived += Ipc_ParameterReceived;
            ipc.FullscreenChanged += Ipc_FullscreenChanged;
            ipc.MinimizedChanged += Ipc_MinimizedChanged;
            ipc.SendFailed += Ipc_SendFailed;
            ipc.DiagnosticTextReceived += Ipc_DiagnosticTextReceived;

            ipc.ProfileIsLoaded += Ipc_ProfileIsLoaded;
            ipc.ShaderIsLoaded += Ipc_ShaderIsLoaded;
            preparing_to_load_shader = true;

            buttonRefresh.Enabled = true;
            buttonRefresh.ToggledOn = false;

            ipc.SetPartnerDetails(shaderglass.Id, null, null);
            ipc.StandardInit(); 

            return;
        }

        private void Ipc_ShaderIsLoaded(object sender, EventArgs e)
        {
            if (sender != ipc) return;
            configWin.RecordEvent("Shader loaded: " + ipc.GetLastLoadedShader(), true);
            init_load_shader();
        }

        void init_load_shader(/*bool trigger_A_load_Anyway = false*/)
        {
            if (preparing_to_load_shader)
            {
                if (shader_profile == null || string.Equals(shader_profile, ipc.GetLastLoadedProfilePath(), StringComparison.OrdinalIgnoreCase))
                {
                    preparing_to_load_shader = false;
                    var p = Program.Settings.ShaderGlassCustomShader;
                    if (string.IsNullOrEmpty(p)) return;
                    p = Program.FindPathTo(p);
                    if (p == null)
                    {
                        configWin.RecordEvent("Unable to locate shader " + Program.Settings.ShaderGlassCustomShader + " by absolute or relative path.");
                        return;
                    }
                    if (string.Equals(p, ipc.GetLastLoadedShader(), StringComparison.OrdinalIgnoreCase)) return;
                    configWin.RecordEvent("Requesting shader switch: " + p, true);
                    Application.Idle += Application_Idle;
                    void Application_Idle(object sender, EventArgs e)
                    {
                        Application.Idle -= Application_Idle;
                        ipc.LoadShader(p);
                    }
                    return;
                }
            }/*
            if (trigger_A_load_Anyway)
            {
                var p = Program.Settings.ShaderGlassCustomShader;
                if (string.IsNullOrEmpty(p)) return;
                if (string.Equals(p, ipc.GetLastLoadedShader(), StringComparison.OrdinalIgnoreCase)) return;
                configWin.RecordEvent("Requesting shader switch although there may be an immediately upcoming profile switch: " + p, true);
                ipc.LoadShader(p);
            }*/
        }
        private void Ipc_ProfileIsLoaded(object sender, EventArgs e)
        {
            if (sender != ipc) return;

            configWin.RecordEvent("Profile loaded: " + ipc.GetLastLoadedProfilePath());
            init_load_shader();
        }

        private void Ipc_DiagnosticTextReceived(ShaderIPCforCLRControl.ShaderMessagingWindow win, string data)
        {
            if (win == ipc)
                configWin.RecordEvent("Diagnostic message: " + data);
        }

        IAsyncResult StartInvocably(Action action)
        {
            if (InvokeRequired) return BeginInvoke(action);
            action();
            return null;
        }

        private void Shaderglass_Exited(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                try
                {
                    StartInvocably((Action)(() =>
                    {
                        configWin.RecordEvent("ShaderGlass executable terminated", true);
                        if (shaderglass == sender)
                        {
                            ClearIPC();
                            toggleButtonGlass.Enabled = true;
                            toggleButtonGlass.ToggledOn = false;
                        }
                    }));
                }
                catch (InvalidOperationException) { }
            }
        }

        private void Ipc_SendFailed(ShaderIPCforCLRControl.ShaderMessagingWindow win, ulong data)
        {
            if (win != ipc) return;
            configWin.RecordEvent("IPC message for ShaderGlass executable failed: " + data, true);
            ClearIPC();
            toggleButtonGlass.ToggledOn = false;
        }

        private void Ipc_MinimizedChanged(object sender, EventArgs e)
        {
            if (sender != ipc) return;
            configWin.RecordEvent("IPC: " + (ipc.GetMinimized() ? "minimized" : "restored"));
            UpdateFromGlass(() => toggleButtonGlass.ToggledOn = !ipc.GetMinimized());
        }

        bool glass_action;
        void UpdateFromGlass(Action apply)
        {
            glass_action = true;
            try { apply(); }
            finally { glass_action = false; }
        }

        private void Ipc_FullscreenChanged(object sender, EventArgs e)
        {
            if (sender != ipc) return;
            configWin.RecordEvent("IPC: " + (ipc.GetFullscreen() ? "fullscreen" : "window"));
            UpdateFromGlass(() => toggleGlassWindow.ToggledOn = !ipc.GetFullscreen());
        }

        private void Ipc_ParameterReceived(ShaderIPCforCLRControl.ShaderMessagingWindow win, int range, int relativeValue, string name, string tooltip)
        {
            if (win != ipc || string.IsNullOrWhiteSpace(name)) return;
            configWin.RecordEvent("IPC: PARAM " + name + "=" + relativeValue);
            if (range > 0)
            {
                configWin.RecordEvent("IPC: new PARAM range size = " + range);
                if (!string.IsNullOrEmpty(tooltip))
                    configWin.RecordEvent("IPC: PARAM tooltip = " + tooltip);
                confirm_redefine.Add(new ParamData { Name = name, Desc = tooltip, range_size = range, range_pos = relativeValue });
            }
            if (param_inputs.TryGetValue(name, out var c) && c.Enabled == false)
                c.Value = relativeValue;
        }

        List<ParamData>
            previous_redefine = new List<ParamData> { new ParamData() },
            confirm_redefine = new List<ParamData>();
        readonly Dictionary<string, TrackBox.Composite>
            param_inputs = new Dictionary<string, TrackBox.Composite>();
        

        private void Ipc_EndRedefine(object sender, EventArgs e)
        {
            try
            {
                if (sender != ipc) return;

                init_load_shader();

                configWin.RecordEvent("IPC: new definitions received", true);

                var previous_redefine = this.previous_redefine;
                var confirm_redefine = this.confirm_redefine;
                this.previous_redefine = confirm_redefine;
                this.confirm_redefine = new List<ParamData>();

                {
                    int i = confirm_redefine.Count;
                    if (i != previous_redefine.Count)
                        goto reconfirm;
                    while (--i > -1)
                    {
                        var a = confirm_redefine[i];
                        var b = previous_redefine[i];
                        if (a.Name != b.Name || a.Desc != b.Desc || a.range_size != b.range_size)
                            goto reconfirm;
                    }
                }

                foreach (var c in param_inputs.Values) c.Dispose();
                param_inputs.Clear();
                foreach (var p in confirm_redefine)
                {
                    var c = new TrackBox.Composite();
                    try { param_inputs.Add(p.Name, c); }
                    catch (ArgumentException ex)
                    {
                        configWin.RecordEvent("Parameter error: " + ex.Message, true);
                        c.Dispose();
                        continue;
                    }
                    c.Max = p.range_size;
                    c.Value = p.range_pos;
                    c.Text = p.Name;
                    c.ValueToStringOverride = v => string.Empty;
                    if (!string.IsNullOrWhiteSpace(p.Desc)) AttachHelperText(c, p.Desc, true);
                    c.Dock = DockStyle.Top;
                    c.Height = 40;
                    c.ValueChanged += GlassParam_ValueChanged;
                    protectorGlass.Controls.Add(c);
                }

                buttonRefresh.Enabled = true;
                buttonRefresh.ToggledOn = false;
                return;
            reconfirm:
                ipc?.Refresh(333);
            }
            finally
            {
                protectorGlass.ResumeLayout();
            }
        }

        private void GlassParam_ValueChanged(object sender, EventArgs e)
        {
            var c = (TrackBox.Composite)sender;
            if (c.Enabled) ipc?.SendParameterValue(c.Text, c.Value);
        }

        private void Ipc_BeginRedefine(object sender, EventArgs e)
        {
            if (sender != ipc) return;
            confirm_redefine.Clear();
            configWin.RecordEvent("IPC: new definitions");
            protectorGlass.SuspendLayout();
        }

        private void Ipc_Created(object sender, EventArgs e)
        {
            if (sender != ipc) return;
            configWin.RecordEvent("IPC: init");
        }

        private void Ipc_AfterHandshake(object sender, EventArgs e)
        {
            if (sender != ipc) return;
            toggleButtonGlass.Enabled = true;
            configWin.RecordEvent("IPC: HANDSHAKE");
            ipc.Refresh(600);
            init_load_shader(/*true*/);
        }

        public Faith()
        {
            InitializeComponent();
            labelTitle.MouseDown += FormsUtil.FormOrControl_MouseDown_LikeMovableTitleBar;
            protectorShield.HeaderImage = toggleButtonShield.BackgroundImage;
            inputIntensityLimit_ValueChanged(null, EventArgs.Empty);

            AttachHelperText(containerIntensityLimit, "An intensity limit around 200 ensures that you don't accidentally make the screen fully dark.");
            AttachHelperText(colourManagerShield.BarAlpha, "Increase value to make the shield colour apply more strongly. Values above 200 are hard to see through.", true);
            AttachHelperText(labelTitle, labelTooltip.Text = "Complete programs and source code made available under GNU GPL 3.0 license");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            shieldWin?.Dispose();
        }

        void ShowOptions()
        {
            if (configWin == null)
                configWin = new Composure();
            configWin.FormClosing += ConfigWin_FormClosing;
            configWin.Visible = true;
            configWin.Activate();
        }

        private void ConfigWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (configWin == sender)
            {
                e.Cancel = true;
                configWin.Visible = false;
            }
        }

        private void Faith_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
            if (Program.Settings.SaveStateOnClose != Settings.When.Never)
            {
                Program.Settings.MainWindowBorderStyle = FormBorderStyle;
                Program.Settings.MainWindowLocation = lastplace;
                Program.Settings.MainWindowSize = lastsize;
            }
            {
                var i = toggleButtonGlass.BackgroundImage;
                if (i != null)
                {
                    toggleButtonGlass.BackgroundImage = null;
                    i.Dispose();
                }
            }
            ClearIPC();
        }

        void ShowShield()
        {
            if (shieldWin == null)
            {
                shieldWin = new Shield();
                shieldWin.Intensity = Program.Settings.ShieldIntensity;
            }
            shieldWin.Visible = true;
        }


        bool ShowGlass()
        {
            if (shaderglass == null || shaderglass.HasExited)
            {
                CreateIPCWin();
                return shaderglass == null || shaderglass.HasExited;
            }
            else
                ipc.SetMinimized(false);
            return true;
        }

        private void Faith_Load(object sender, EventArgs e)
        {
            if (!Program.Settings.StartupWindow.Contains(Settings.StartupWindows.Main))
                WindowState = FormWindowState.Minimized;

            FormBorderStyle = Program.Settings.MainWindowBorderStyle;
            { var l = Program.Settings.MainWindowLocation; if (l.HasValue) Location = l.Value; }
            { var s = Program.Settings.MainWindowSize; if (s.HasValue) Size = s.Value; }

            if (executable != null)
                executable = executable;

            BorderlessResizer.WindowResizer.ApplyWindowResizer(this);

            colourManagerShield.Value = Program.Settings.ShieldColorAndIntensity;
        }

        Size lastsize; Point lastplace;

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (WindowState != FormWindowState.Minimized)
                lastplace = Location;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (WindowState != FormWindowState.Minimized)
                lastsize = Size;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }


        TabPage tabOption;
        private void buttonOptions_Click(object sender, EventArgs e)
        {
            ShowOptions();
            if (sender == buttonOptions)
            {
                if (tabOption != null && configWin.Tabs.SelectedTab == configWin.tabHelp)
                    configWin.Tabs.SelectedTab = tabOption;
                tabOption = configWin.Tabs.SelectedTab;
            }
            else
            {
                if (configWin.Tabs.SelectedTab != configWin.tabHelp)
                {
                    tabOption = configWin.Tabs.SelectedTab;
                    configWin.Tabs.SelectedTab = configWin.tabHelp;
                }
            }
        }

        private void toggleButtonGlass_Toggled(object sender, EventArgs e)
        {
            if (glass_action)
                return;
            if (toggleButtonGlass.ToggledOn)
            {
                if (!ShowGlass())
                    toggleButtonGlass.ToggledOn = false;
            }
            else
            {
                ipc?.SetMinimized(true);
            }
        }

        private void toggleButtonShield_Toggled(object sender, EventArgs e)
        {
            if (toggleButtonShield.ToggledOn)
                ShowShield();
            else if (shieldWin != null) shieldWin.Visible = false;
        }

        private void Faith_Shown(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Program.GetExecutedFilePath());

            foreach (var win in Program.Settings.StartupWindow)
                switch (win)
                {
                    case Settings.StartupWindows.Options:
                        ShowOptions();
                        break;
                    case Settings.StartupWindows.Shield:
                        toggleButtonShield.ToggledOn = true;
                        break;
                    case Settings.StartupWindows.Glass:
                        toggleButtonGlass.ToggledOn = true;
                        break;
                }
        }

        private void toggle_reevaluate(object sender, EventArgsColourSelect select)
        {
            if (((ButtonToggle)sender).ToggledOn)
            {
                var c = select.SelectBackground;
                select.SelectBackground = Color.FromArgb(c.R / 2, c.G / 2, c.B / 2);
            }
        }

        public void AttachHelperText(Control c, string text, bool apply_to_childs = false)
        {
            EventHandler enter(string t)
            {
                return (o, e) =>
                {
                    labelTooltip.Text = t;
                };
            }
            EventHandler leave(string t)
            {
                return (o, e) =>
                {
                    if (labelTooltip.Text != t) return;
                    Control ctl = (Control)o;
                    var p = ctl.PointToClient(Cursor.Position);
                    if (p.X < 0 || p.X >= ctl.Width || p.Y < 0 || p.Y >= ctl.Height)
                        labelTooltip.Text = string.Empty;
                };
            }

            var ce = enter(text);
            var cl = leave(text);

            c.MouseEnter += ce;
            c.MouseLeave += cl;
            
            if (apply_to_childs)
            {
                EventHandler forchild(EventHandler parent, Control parentc)
                {
                    return (o, e) => parent(parentc, e);
                }
                var se = forchild(ce, c);
                var sl = forchild(cl, c);
                
                void apply(Control tochildren)
                {
                    foreach (Control ch in tochildren.Controls)
                    {
                        ch.MouseEnter += se;
                        ch.MouseLeave += sl;
                        apply(ch);
                    }
                }
                apply(c);
            }
        }

        private void colourManagerShield_ValueChanged(object sender, EventArgs e)
        {
            if (shieldWin == null) return;
            var c = colourManagerShield.Value;
            shieldWin.BackColor = Color.FromArgb(255, c);
            shieldWin.Intensity = c.A;
        }

        private void inputIntensityLimit_ValueChanged(object sender, EventArgs e)
        {
            colourManagerShield.BarAlpha.Max = (int)inputIntensityLimit.IntegerValue;
        }

        private void buttonPickMonitor_Click(object sender, EventArgs e)
        {
            toggleButtonShield.ToggledOn = true;
            shieldWin.WindowState = FormWindowState.Normal;
            shieldWin.DesktopBounds = DesktopBounds;
            shieldWin.WindowState = FormWindowState.Maximized;
        }

        private void buttonBreakGlass_Click(object sender, EventArgs e)
        {
            ClearIPC();
            toggleButtonGlass.ToggledOn = false;
            toggleButtonGlass.Enabled = true;
        }

        private void toggleGlassWindow_Toggled(object sender, EventArgs e)
        {
            if (!glass_action)
                ipc?.SetFullscreen(!toggleGlassWindow.ToggledOn);
        }

        private void buttonRefresh_Toggled(object sender, EventArgs e)
        {
            if (ipc == null)
                return;
            if (buttonRefresh.ToggledOn)
            {
                ipc.Refresh();

                foreach (var c in param_inputs.Values) c.Enabled = false;
                buttonRefresh.Enabled = false;
            }
            else foreach (var c in param_inputs.Values) c.Enabled = true;
        }
    }
}