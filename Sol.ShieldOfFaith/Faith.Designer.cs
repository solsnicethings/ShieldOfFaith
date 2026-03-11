namespace Sol.ShieldOfFaith
{
    partial class Faith
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Faith));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.buttonMinimize = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.toggleButtonGlass = new Sol.ShieldOfFaith.ButtonToggle();
            this.toggleButtonShield = new Sol.ShieldOfFaith.ButtonToggle();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.protectorShield = new Sol.ShieldOfFaith.ProtectorPanel();
            this.flowLayoutShield = new System.Windows.Forms.FlowLayoutPanel();
            this.containerIntensityLimit = new System.Windows.Forms.GroupBox();
            this.inputIntensityLimit = new Sol.ShieldOfFaith.NumericTextBox();
            this.buttonPickMonitor = new System.Windows.Forms.Button();
            this.colourManagerShield = new Sol.ShieldOfFaith.ColourManager();
            this.protectorGlass = new Sol.ShieldOfFaith.ProtectorPanel();
            this.flowLayoutGlass = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonBreakGlass = new System.Windows.Forms.Button();
            this.toggleGlassWindow = new Sol.ShieldOfFaith.ButtonToggle();
            this.buttonRefresh = new Sol.ShieldOfFaith.ButtonToggle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelTooltip = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.protectorShield.SuspendLayout();
            this.flowLayoutShield.SuspendLayout();
            this.containerIntensityLimit.SuspendLayout();
            this.protectorGlass.SuspendLayout();
            this.flowLayoutGlass.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "ShaderGlassForShieldOfFaith.exe";
            this.openFileDialog1.Filter = "ShaderGlassForShieldOfFaith.exe|ShaderGlassForShieldOfFaith.exe";
            this.openFileDialog1.Title = "Shader glass executable needed";
            // 
            // buttonOptions
            // 
            this.buttonOptions.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonOptions.Location = new System.Drawing.Point(63, 0);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(77, 48);
            this.buttonOptions.TabIndex = 6;
            this.buttonOptions.Text = "&Options";
            this.buttonOptions.UseVisualStyleBackColor = true;
            this.buttonOptions.Click += new System.EventHandler(this.buttonOptions_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Controls.Add(this.buttonMinimize);
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Controls.Add(this.toggleButtonGlass);
            this.panel1.Controls.Add(this.toggleButtonShield);
            this.panel1.Controls.Add(this.buttonOptions);
            this.panel1.Controls.Add(this.buttonHelp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(683, 48);
            this.panel1.TabIndex = 7;
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.labelTitle.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(70)))));
            this.labelTitle.Location = new System.Drawing.Point(309, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(300, 48);
            this.labelTitle.TabIndex = 7;
            this.labelTitle.Text = "Shield of Faith                  ";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonMinimize
            // 
            this.buttonMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.buttonMinimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMinimize.ForeColor = System.Drawing.Color.Navy;
            this.buttonMinimize.Location = new System.Drawing.Point(609, 0);
            this.buttonMinimize.Name = "buttonMinimize";
            this.buttonMinimize.Size = new System.Drawing.Size(36, 48);
            this.buttonMinimize.TabIndex = 9;
            this.buttonMinimize.Text = "-";
            this.buttonMinimize.UseVisualStyleBackColor = false;
            this.buttonMinimize.Click += new System.EventHandler(this.buttonMinimize_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.buttonClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.ForeColor = System.Drawing.Color.Maroon;
            this.buttonClose.Location = new System.Drawing.Point(645, 0);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(38, 48);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "X";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // toggleButtonGlass
            // 
            this.toggleButtonGlass.AccessibleName = "Toggle screen shader app";
            this.toggleButtonGlass.AutoEllipsis = false;
            this.toggleButtonGlass.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toggleButtonGlass.DialogResult = System.Windows.Forms.DialogResult.None;
            this.toggleButtonGlass.DisabledColorBlend = System.Drawing.Color.Empty;
            this.toggleButtonGlass.Dock = System.Windows.Forms.DockStyle.Left;
            this.toggleButtonGlass.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.toggleButtonGlass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.toggleButtonGlass.Image = null;
            this.toggleButtonGlass.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toggleButtonGlass.ImageIndex = -1;
            this.toggleButtonGlass.ImageKey = "";
            this.toggleButtonGlass.ImageList = null;
            this.toggleButtonGlass.Location = new System.Drawing.Point(221, 0);
            this.toggleButtonGlass.Name = "toggleButtonGlass";
            this.toggleButtonGlass.Size = new System.Drawing.Size(88, 48);
            this.toggleButtonGlass.TabIndex = 11;
            this.toggleButtonGlass.Text = "Shader Glass app";
            this.toggleButtonGlass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toggleButtonGlass.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toggleButtonGlass.ToggledBackColour = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.toggleButtonGlass.ToggledForeColour = System.Drawing.Color.Transparent;
            this.toggleButtonGlass.ToggledOn = false;
            this.toggleButtonGlass.UseVisualStyleBackColor = false;
            this.toggleButtonGlass.Toggled += new System.EventHandler(this.toggleButtonGlass_Toggled);
            // 
            // toggleButtonShield
            // 
            this.toggleButtonShield.AccessibleName = "Toggle translucent screen covering";
            this.toggleButtonShield.AutoEllipsis = false;
            this.toggleButtonShield.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toggleButtonShield.BackgroundImage")));
            this.toggleButtonShield.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toggleButtonShield.DialogResult = System.Windows.Forms.DialogResult.None;
            this.toggleButtonShield.DisabledColorBlend = System.Drawing.Color.Empty;
            this.toggleButtonShield.Dock = System.Windows.Forms.DockStyle.Left;
            this.toggleButtonShield.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.toggleButtonShield.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.toggleButtonShield.Image = null;
            this.toggleButtonShield.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toggleButtonShield.ImageIndex = -1;
            this.toggleButtonShield.ImageKey = "";
            this.toggleButtonShield.ImageList = null;
            this.toggleButtonShield.Location = new System.Drawing.Point(140, 0);
            this.toggleButtonShield.Name = "toggleButtonShield";
            this.toggleButtonShield.Size = new System.Drawing.Size(81, 48);
            this.toggleButtonShield.TabIndex = 10;
            this.toggleButtonShield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toggleButtonShield.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.toggleButtonShield.ToggledBackColour = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.toggleButtonShield.ToggledForeColour = System.Drawing.Color.Transparent;
            this.toggleButtonShield.ToggledOn = false;
            this.toggleButtonShield.UseVisualStyleBackColor = false;
            this.toggleButtonShield.Toggled += new System.EventHandler(this.toggleButtonShield_Toggled);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonHelp.Location = new System.Drawing.Point(0, 0);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(63, 48);
            this.buttonHelp.TabIndex = 12;
            this.buttonHelp.Text = "&Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonOptions_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 48);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.protectorShield);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.protectorGlass);
            this.splitContainer1.Size = new System.Drawing.Size(683, 382);
            this.splitContainer1.SplitterDistance = 341;
            this.splitContainer1.TabIndex = 10;
            // 
            // protectorShield
            // 
            this.protectorShield.AdjustHeaderHeightToText = true;
            this.protectorShield.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.protectorShield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectorShield.Controls.Add(this.flowLayoutShield);
            this.protectorShield.Controls.Add(this.colourManagerShield);
            this.protectorShield.Dock = System.Windows.Forms.DockStyle.Fill;
            this.protectorShield.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            // 
            // 
            // 
            this.protectorShield.Header.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.protectorShield.Header.Location = new System.Drawing.Point(0, 0);
            this.protectorShield.Header.Name = "headerLabel";
            this.protectorShield.Header.Padding = new System.Windows.Forms.Padding(6);
            this.protectorShield.Header.TabIndex = 0;
            this.protectorShield.Header.Text = "Shield controls (translucent screen cover)";
            this.protectorShield.HeaderImage = null;
            this.protectorShield.Location = new System.Drawing.Point(0, 0);
            this.protectorShield.Name = "protectorShield";
            this.protectorShield.Size = new System.Drawing.Size(341, 382);
            this.protectorShield.TabIndex = 8;
            // 
            // flowLayoutShield
            // 
            this.flowLayoutShield.Controls.Add(this.containerIntensityLimit);
            this.flowLayoutShield.Controls.Add(this.buttonPickMonitor);
            this.flowLayoutShield.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutShield.Location = new System.Drawing.Point(0, 148);
            this.flowLayoutShield.Name = "flowLayoutShield";
            this.flowLayoutShield.Size = new System.Drawing.Size(339, 232);
            this.flowLayoutShield.TabIndex = 11;
            // 
            // containerIntensityLimit
            // 
            this.containerIntensityLimit.Controls.Add(this.inputIntensityLimit);
            this.containerIntensityLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.containerIntensityLimit.Location = new System.Drawing.Point(3, 3);
            this.containerIntensityLimit.Name = "containerIntensityLimit";
            this.containerIntensityLimit.Size = new System.Drawing.Size(126, 59);
            this.containerIntensityLimit.TabIndex = 10;
            this.containerIntensityLimit.TabStop = false;
            this.containerIntensityLimit.Text = "Intensity limit";
            // 
            // inputIntensityLimit
            // 
            this.inputIntensityLimit.AllowFractions = false;
            this.inputIntensityLimit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.inputIntensityLimit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inputIntensityLimit.FloatingPointValue = 220D;
            this.inputIntensityLimit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.inputIntensityLimit.IntegerValue = ((long)(220));
            this.inputIntensityLimit.Location = new System.Drawing.Point(6, 19);
            this.inputIntensityLimit.Max = 255D;
            this.inputIntensityLimit.Min = 100D;
            this.inputIntensityLimit.Name = "inputIntensityLimit";
            this.inputIntensityLimit.Size = new System.Drawing.Size(77, 19);
            this.inputIntensityLimit.TabIndex = 0;
            this.inputIntensityLimit.Text = "220";
            this.inputIntensityLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.inputIntensityLimit.ValueChanged += new System.EventHandler(this.inputIntensityLimit_ValueChanged);
            // 
            // buttonPickMonitor
            // 
            this.buttonPickMonitor.Location = new System.Drawing.Point(135, 3);
            this.buttonPickMonitor.Name = "buttonPickMonitor";
            this.buttonPickMonitor.Size = new System.Drawing.Size(120, 60);
            this.buttonPickMonitor.TabIndex = 11;
            this.buttonPickMonitor.Text = "&Move shield to this monitor";
            this.buttonPickMonitor.UseVisualStyleBackColor = true;
            this.buttonPickMonitor.Click += new System.EventHandler(this.buttonPickMonitor_Click);
            // 
            // colourManagerShield
            // 
            this.colourManagerShield.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            // 
            // 
            // 
            this.colourManagerShield.BarAlpha.AlternateTextColor = System.Drawing.Color.Transparent;
            this.colourManagerShield.BarAlpha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.colourManagerShield.BarAlpha.Dock = System.Windows.Forms.DockStyle.Top;
            this.colourManagerShield.BarAlpha.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colourManagerShield.BarAlpha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.colourManagerShield.BarAlpha.HighValueColor = System.Drawing.Color.Black;
            this.colourManagerShield.BarAlpha.HoveredValueColor = System.Drawing.Color.Transparent;
            this.colourManagerShield.BarAlpha.Location = new System.Drawing.Point(0, 0);
            this.colourManagerShield.BarAlpha.LowValueColor = System.Drawing.Color.Transparent;
            this.colourManagerShield.BarAlpha.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.colourManagerShield.BarAlpha.Name = "trackBoxAlpha";
            this.colourManagerShield.BarAlpha.Size = new System.Drawing.Size(339, 0);
            this.colourManagerShield.BarAlpha.TabIndex = 4;
            this.colourManagerShield.BarAlpha.Text = "intensity: ";
            this.colourManagerShield.BarAlpha.Value = 255;
            this.colourManagerShield.BarAlpha.ValueToStringOverride = null;
            // 
            // 
            // 
            this.colourManagerShield.BarBlue.AlternateTextColor = System.Drawing.Color.Silver;
            this.colourManagerShield.BarBlue.Dock = System.Windows.Forms.DockStyle.Top;
            this.colourManagerShield.BarBlue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colourManagerShield.BarBlue.HighValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(255)))));
            this.colourManagerShield.BarBlue.HoveredValueColor = System.Drawing.Color.Transparent;
            this.colourManagerShield.BarBlue.Location = new System.Drawing.Point(0, 0);
            this.colourManagerShield.BarBlue.LowValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.colourManagerShield.BarBlue.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.colourManagerShield.BarBlue.Name = "trackB";
            this.colourManagerShield.BarBlue.Size = new System.Drawing.Size(0, 0);
            this.colourManagerShield.BarBlue.TabIndex = 3;
            this.colourManagerShield.BarBlue.Text = "blue : ";
            this.colourManagerShield.BarBlue.ValueToStringOverride = null;
            this.colourManagerShield.BarBlue.Visible = false;
            this.colourManagerShield.BarFont = null;
            // 
            // 
            // 
            this.colourManagerShield.BarGreen.AlternateTextColor = System.Drawing.Color.Transparent;
            this.colourManagerShield.BarGreen.Dock = System.Windows.Forms.DockStyle.Top;
            this.colourManagerShield.BarGreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colourManagerShield.BarGreen.HighValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(255)))), ((int)(((byte)(60)))));
            this.colourManagerShield.BarGreen.HoveredValueColor = System.Drawing.Color.Transparent;
            this.colourManagerShield.BarGreen.Location = new System.Drawing.Point(0, 0);
            this.colourManagerShield.BarGreen.LowValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.colourManagerShield.BarGreen.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.colourManagerShield.BarGreen.Name = "trackG";
            this.colourManagerShield.BarGreen.Size = new System.Drawing.Size(0, 0);
            this.colourManagerShield.BarGreen.TabIndex = 2;
            this.colourManagerShield.BarGreen.Text = "green : ";
            this.colourManagerShield.BarGreen.ValueToStringOverride = null;
            this.colourManagerShield.BarGreen.Visible = false;
            // 
            // 
            // 
            this.colourManagerShield.BarRed.AlternateTextColor = System.Drawing.Color.White;
            this.colourManagerShield.BarRed.Dock = System.Windows.Forms.DockStyle.Top;
            this.colourManagerShield.BarRed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colourManagerShield.BarRed.HighValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.colourManagerShield.BarRed.HoveredValueColor = System.Drawing.Color.Transparent;
            this.colourManagerShield.BarRed.Location = new System.Drawing.Point(0, 0);
            this.colourManagerShield.BarRed.LowValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colourManagerShield.BarRed.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.colourManagerShield.BarRed.Name = "trackR";
            this.colourManagerShield.BarRed.Size = new System.Drawing.Size(0, 0);
            this.colourManagerShield.BarRed.TabIndex = 1;
            this.colourManagerShield.BarRed.Text = "red  :  ";
            this.colourManagerShield.BarRed.ValueToStringOverride = null;
            this.colourManagerShield.BarRed.Visible = false;
            this.colourManagerShield.DisplayAlpha = true;
            this.colourManagerShield.DisplayColourPick = false;
            this.colourManagerShield.DisplayPreviousPick = false;
            this.colourManagerShield.DisplayRGB = false;
            this.colourManagerShield.Dock = System.Windows.Forms.DockStyle.Top;
            this.colourManagerShield.Location = new System.Drawing.Point(0, 0);
            this.colourManagerShield.Name = "colourManagerShield";
            this.colourManagerShield.Size = new System.Drawing.Size(339, 148);
            this.colourManagerShield.TabIndex = 9;
            this.colourManagerShield.Value = System.Drawing.Color.Gray;
            this.colourManagerShield.ValueChanged += new System.EventHandler(this.colourManagerShield_ValueChanged);
            // 
            // protectorGlass
            // 
            this.protectorGlass.AdjustHeaderHeightToText = true;
            this.protectorGlass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.protectorGlass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectorGlass.Controls.Add(this.flowLayoutGlass);
            this.protectorGlass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.protectorGlass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            // 
            // 
            // 
            this.protectorGlass.Header.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Underline);
            this.protectorGlass.Header.Location = new System.Drawing.Point(0, 0);
            this.protectorGlass.Header.Name = "headerLabel";
            this.protectorGlass.Header.Padding = new System.Windows.Forms.Padding(6);
            this.protectorGlass.Header.Size = new System.Drawing.Size(100, 32);
            this.protectorGlass.Header.TabIndex = 0;
            this.protectorGlass.Header.Text = "Glass controls (external shader program)";
            this.protectorGlass.HeaderImage = null;
            this.protectorGlass.Location = new System.Drawing.Point(0, 0);
            this.protectorGlass.Name = "protectorGlass";
            this.protectorGlass.Size = new System.Drawing.Size(338, 382);
            this.protectorGlass.TabIndex = 9;
            // 
            // flowLayoutGlass
            // 
            this.flowLayoutGlass.Controls.Add(this.buttonBreakGlass);
            this.flowLayoutGlass.Controls.Add(this.toggleGlassWindow);
            this.flowLayoutGlass.Controls.Add(this.buttonRefresh);
            this.flowLayoutGlass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutGlass.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutGlass.Name = "flowLayoutGlass";
            this.flowLayoutGlass.Size = new System.Drawing.Size(336, 380);
            this.flowLayoutGlass.TabIndex = 1;
            // 
            // buttonBreakGlass
            // 
            this.buttonBreakGlass.Location = new System.Drawing.Point(3, 3);
            this.buttonBreakGlass.Name = "buttonBreakGlass";
            this.buttonBreakGlass.Size = new System.Drawing.Size(113, 50);
            this.buttonBreakGlass.TabIndex = 0;
            this.buttonBreakGlass.Text = "&Close shader application";
            this.buttonBreakGlass.UseVisualStyleBackColor = true;
            this.buttonBreakGlass.Click += new System.EventHandler(this.buttonBreakGlass_Click);
            // 
            // toggleGlassWindow
            // 
            this.toggleGlassWindow.AutoEllipsis = false;
            this.toggleGlassWindow.DialogResult = System.Windows.Forms.DialogResult.None;
            this.toggleGlassWindow.DisabledColorBlend = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.toggleGlassWindow.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.toggleGlassWindow.Image = null;
            this.toggleGlassWindow.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toggleGlassWindow.ImageIndex = -1;
            this.toggleGlassWindow.ImageKey = "";
            this.toggleGlassWindow.ImageList = null;
            this.toggleGlassWindow.Location = new System.Drawing.Point(122, 3);
            this.toggleGlassWindow.Name = "toggleGlassWindow";
            this.toggleGlassWindow.Size = new System.Drawing.Size(84, 50);
            this.toggleGlassWindow.TabIndex = 1;
            this.toggleGlassWindow.Text = "&Window mode";
            this.toggleGlassWindow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toggleGlassWindow.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toggleGlassWindow.ToggledBackColour = System.Drawing.Color.Transparent;
            this.toggleGlassWindow.ToggledForeColour = System.Drawing.Color.Transparent;
            this.toggleGlassWindow.ToggledOn = false;
            this.toggleGlassWindow.UseVisualStyleBackColor = false;
            this.toggleGlassWindow.Toggled += new System.EventHandler(this.toggleGlassWindow_Toggled);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.AutoEllipsis = false;
            this.buttonRefresh.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonRefresh.DisabledColorBlend = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.buttonRefresh.Enabled = false;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.buttonRefresh.Image = null;
            this.buttonRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonRefresh.ImageIndex = -1;
            this.buttonRefresh.ImageKey = "";
            this.buttonRefresh.ImageList = null;
            this.buttonRefresh.Location = new System.Drawing.Point(212, 3);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(87, 173);
            this.buttonRefresh.TabIndex = 2;
            this.buttonRefresh.Text = "&Refresh button not working well";
            this.buttonRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.buttonRefresh.ToggledBackColour = System.Drawing.Color.Transparent;
            this.buttonRefresh.ToggledForeColour = System.Drawing.Color.Transparent;
            this.buttonRefresh.ToggledOn = false;
            this.buttonRefresh.UseVisualStyleBackColor = false;
            this.buttonRefresh.Visible = false;
            this.buttonRefresh.Toggled += new System.EventHandler(this.buttonRefresh_Toggled);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelTooltip);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 430);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(683, 27);
            this.panel2.TabIndex = 11;
            // 
            // labelTooltip
            // 
            this.labelTooltip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.labelTooltip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTooltip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTooltip.Location = new System.Drawing.Point(0, 0);
            this.labelTooltip.Name = "labelTooltip";
            this.labelTooltip.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.labelTooltip.Size = new System.Drawing.Size(683, 27);
            this.labelTooltip.TabIndex = 0;
            // 
            // Faith
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(683, 457);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Faith";
            this.Text = "Shield of Faith";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Faith_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Faith_Load);
            this.Shown += new System.EventHandler(this.Faith_Shown);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.protectorShield.ResumeLayout(false);
            this.flowLayoutShield.ResumeLayout(false);
            this.containerIntensityLimit.ResumeLayout(false);
            this.containerIntensityLimit.PerformLayout();
            this.protectorGlass.ResumeLayout(false);
            this.flowLayoutGlass.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonOptions;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button buttonMinimize;
        private System.Windows.Forms.Button buttonClose;
        private ButtonToggle toggleButtonGlass;
        private ButtonToggle toggleButtonShield;
        private System.Windows.Forms.Button buttonHelp;
        private ProtectorPanel protectorShield;
        private ProtectorPanel protectorGlass;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelTooltip;
        private ColourManager colourManagerShield;
        private System.Windows.Forms.GroupBox containerIntensityLimit;
        private NumericTextBox inputIntensityLimit;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutShield;
        private System.Windows.Forms.Button buttonPickMonitor;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutGlass;
        private System.Windows.Forms.Button buttonBreakGlass;
        private ButtonToggle toggleGlassWindow;
        private ButtonToggle buttonRefresh;
    }
}

