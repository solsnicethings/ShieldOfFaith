namespace Sol.ShieldOfFaith
{
    partial class Composure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Composure));
            this.outputSessionDetails = new System.Windows.Forms.RichTextBox();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.configFileSystem = new System.Windows.Forms.ListView();
            this.chCfgName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCfgFunction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCfgPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboFilesByOrigin = new System.Windows.Forms.ComboBox();
            this.radioShowNoFiles = new System.Windows.Forms.RadioButton();
            this.radioOtherfiles = new System.Windows.Forms.RadioButton();
            this.radioShadioProfilo = new System.Windows.Forms.RadioButton();
            this.radioShadio = new System.Windows.Forms.RadioButton();
            this.radioShowConfigs = new System.Windows.Forms.RadioButton();
            this.radioShowFiles = new System.Windows.Forms.RadioButton();
            this.checkShowDirs = new System.Windows.Forms.CheckBox();
            this.tabSession = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClearTrace = new System.Windows.Forms.Button();
            this.radioTraceHigh = new System.Windows.Forms.RadioButton();
            this.radioTraceLow = new System.Windows.Forms.RadioButton();
            this.radioTraceOff = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tabHelp = new System.Windows.Forms.TabPage();
            this.helpTextBox = new System.Windows.Forms.RichTextBox();
            this.tabHelpSelector = new System.Windows.Forms.TabControl();
            this.checkMissing = new System.Windows.Forms.CheckBox();
            this.labelFileOrigin = new System.Windows.Forms.Label();
            this.Tabs.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabSession.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputSessionDetails
            // 
            this.outputSessionDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.outputSessionDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputSessionDetails.ForeColor = System.Drawing.Color.Khaki;
            this.outputSessionDetails.Location = new System.Drawing.Point(4, 3);
            this.outputSessionDetails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.outputSessionDetails.Name = "outputSessionDetails";
            this.outputSessionDetails.ReadOnly = true;
            this.outputSessionDetails.Size = new System.Drawing.Size(638, 396);
            this.outputSessionDetails.TabIndex = 1;
            this.outputSessionDetails.Text = "";
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.tabPage2);
            this.Tabs.Controls.Add(this.tabSession);
            this.Tabs.Controls.Add(this.tabHelp);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tabs.Location = new System.Drawing.Point(0, 0);
            this.Tabs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(654, 458);
            this.Tabs.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(100)))), ((int)(((byte)(95)))));
            this.tabPage2.Controls.Add(this.configFileSystem);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Size = new System.Drawing.Size(646, 431);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Configuration files";
            // 
            // configFileSystem
            // 
            this.configFileSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(122)))));
            this.configFileSystem.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chCfgName,
            this.chCfgFunction,
            this.chCfgPath});
            this.configFileSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configFileSystem.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configFileSystem.ForeColor = System.Drawing.Color.Black;
            this.configFileSystem.FullRowSelect = true;
            this.configFileSystem.HideSelection = false;
            this.configFileSystem.Location = new System.Drawing.Point(4, 3);
            this.configFileSystem.Name = "configFileSystem";
            this.configFileSystem.Size = new System.Drawing.Size(534, 359);
            this.configFileSystem.TabIndex = 0;
            this.configFileSystem.UseCompatibleStateImageBehavior = false;
            this.configFileSystem.View = System.Windows.Forms.View.Details;
            // 
            // chCfgName
            // 
            this.chCfgName.Text = "Name";
            this.chCfgName.Width = 95;
            // 
            // chCfgFunction
            // 
            this.chCfgFunction.Text = "Function";
            this.chCfgFunction.Width = 173;
            // 
            // chCfgPath
            // 
            this.chCfgPath.Text = "Path";
            this.chCfgPath.Width = 262;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelFileOrigin);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel2.Location = new System.Drawing.Point(538, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(104, 359);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comboFilesByOrigin);
            this.panel3.Controls.Add(this.checkMissing);
            this.panel3.Controls.Add(this.radioShowNoFiles);
            this.panel3.Controls.Add(this.radioOtherfiles);
            this.panel3.Controls.Add(this.radioShadioProfilo);
            this.panel3.Controls.Add(this.radioShadio);
            this.panel3.Controls.Add(this.radioShowConfigs);
            this.panel3.Controls.Add(this.radioShowFiles);
            this.panel3.Controls.Add(this.checkShowDirs);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.Location = new System.Drawing.Point(4, 362);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(638, 66);
            this.panel3.TabIndex = 2;
            // 
            // comboFilesByOrigin
            // 
            this.comboFilesByOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilesByOrigin.FormattingEnabled = true;
            this.comboFilesByOrigin.Items.AddRange(new object[] {
            "Any"});
            this.comboFilesByOrigin.Location = new System.Drawing.Point(413, 5);
            this.comboFilesByOrigin.Name = "comboFilesByOrigin";
            this.comboFilesByOrigin.Size = new System.Drawing.Size(221, 30);
            this.comboFilesByOrigin.TabIndex = 8;
            this.comboFilesByOrigin.SelectedIndexChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // radioShowNoFiles
            // 
            this.radioShowNoFiles.AutoSize = true;
            this.radioShowNoFiles.Location = new System.Drawing.Point(339, 6);
            this.radioShowNoFiles.Name = "radioShowNoFiles";
            this.radioShowNoFiles.Size = new System.Drawing.Size(68, 26);
            this.radioShowNoFiles.TabIndex = 6;
            this.radioShowNoFiles.Text = "&None";
            this.radioShowNoFiles.UseVisualStyleBackColor = true;
            this.radioShowNoFiles.CheckedChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // radioOtherfiles
            // 
            this.radioOtherfiles.AutoSize = true;
            this.radioOtherfiles.Location = new System.Drawing.Point(554, 34);
            this.radioOtherfiles.Name = "radioOtherfiles";
            this.radioOtherfiles.Size = new System.Drawing.Size(88, 26);
            this.radioOtherfiles.TabIndex = 5;
            this.radioOtherfiles.Text = " &Other";
            this.radioOtherfiles.UseVisualStyleBackColor = true;
            this.radioOtherfiles.CheckedChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // radioShadioProfilo
            // 
            this.radioShadioProfilo.AutoSize = true;
            this.radioShadioProfilo.Location = new System.Drawing.Point(391, 34);
            this.radioShadioProfilo.Name = "radioShadioProfilo";
            this.radioShadioProfilo.Size = new System.Drawing.Size(158, 26);
            this.radioShadioProfilo.TabIndex = 4;
            this.radioShadioProfilo.Text = "&Glass profile";
            this.radioShadioProfilo.UseVisualStyleBackColor = true;
            this.radioShadioProfilo.CheckedChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // radioShadio
            // 
            this.radioShadio.AutoSize = true;
            this.radioShadio.Location = new System.Drawing.Point(297, 34);
            this.radioShadio.Name = "radioShadio";
            this.radioShadio.Size = new System.Drawing.Size(88, 26);
            this.radioShadio.TabIndex = 3;
            this.radioShadio.Text = "&Shader";
            this.radioShadio.UseVisualStyleBackColor = true;
            this.radioShadio.CheckedChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // radioShowConfigs
            // 
            this.radioShowConfigs.AutoSize = true;
            this.radioShowConfigs.Location = new System.Drawing.Point(203, 34);
            this.radioShowConfigs.Name = "radioShowConfigs";
            this.radioShowConfigs.Size = new System.Drawing.Size(88, 26);
            this.radioShowConfigs.TabIndex = 2;
            this.radioShowConfigs.Text = "&Config";
            this.radioShowConfigs.UseVisualStyleBackColor = true;
            this.radioShowConfigs.CheckedChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // radioShowFiles
            // 
            this.radioShowFiles.AutoSize = true;
            this.radioShowFiles.Checked = true;
            this.radioShowFiles.Location = new System.Drawing.Point(203, 6);
            this.radioShowFiles.Name = "radioShowFiles";
            this.radioShowFiles.Size = new System.Drawing.Size(138, 26);
            this.radioShowFiles.TabIndex = 1;
            this.radioShowFiles.TabStop = true;
            this.radioShowFiles.Text = "Show &files:";
            this.radioShowFiles.UseVisualStyleBackColor = true;
            this.radioShowFiles.CheckedChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // checkShowDirs
            // 
            this.checkShowDirs.AutoSize = true;
            this.checkShowDirs.Checked = true;
            this.checkShowDirs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkShowDirs.Location = new System.Drawing.Point(4, 6);
            this.checkShowDirs.Name = "checkShowDirs";
            this.checkShowDirs.Size = new System.Drawing.Size(189, 26);
            this.checkShowDirs.TabIndex = 0;
            this.checkShowDirs.Text = "Show &Directories";
            this.checkShowDirs.UseVisualStyleBackColor = true;
            this.checkShowDirs.CheckedChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // tabSession
            // 
            this.tabSession.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(100)))), ((int)(((byte)(95)))));
            this.tabSession.Controls.Add(this.outputSessionDetails);
            this.tabSession.Controls.Add(this.panel1);
            this.tabSession.Location = new System.Drawing.Point(4, 23);
            this.tabSession.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabSession.Name = "tabSession";
            this.tabSession.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabSession.Size = new System.Drawing.Size(646, 431);
            this.tabSession.TabIndex = 0;
            this.tabSession.Text = "Session info";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonClearTrace);
            this.panel1.Controls.Add(this.radioTraceHigh);
            this.panel1.Controls.Add(this.radioTraceLow);
            this.panel1.Controls.Add(this.radioTraceOff);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel1.Location = new System.Drawing.Point(4, 399);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 29);
            this.panel1.TabIndex = 2;
            // 
            // buttonClearTrace
            // 
            this.buttonClearTrace.AutoSize = true;
            this.buttonClearTrace.BackColor = System.Drawing.Color.Black;
            this.buttonClearTrace.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonClearTrace.Location = new System.Drawing.Point(502, 0);
            this.buttonClearTrace.Name = "buttonClearTrace";
            this.buttonClearTrace.Size = new System.Drawing.Size(136, 29);
            this.buttonClearTrace.TabIndex = 4;
            this.buttonClearTrace.Text = "clear event trace";
            this.buttonClearTrace.UseVisualStyleBackColor = false;
            this.buttonClearTrace.Click += new System.EventHandler(this.buttonClearTrace_Click);
            // 
            // radioTraceHigh
            // 
            this.radioTraceHigh.AutoSize = true;
            this.radioTraceHigh.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioTraceHigh.Location = new System.Drawing.Point(251, 0);
            this.radioTraceHigh.Name = "radioTraceHigh";
            this.radioTraceHigh.Size = new System.Drawing.Size(102, 29);
            this.radioTraceHigh.TabIndex = 3;
            this.radioTraceHigh.TabStop = true;
            this.radioTraceHigh.Text = "high detail";
            this.radioTraceHigh.UseVisualStyleBackColor = true;
            this.radioTraceHigh.CheckedChanged += new System.EventHandler(this.radioTrace_CheckedChanged);
            // 
            // radioTraceLow
            // 
            this.radioTraceLow.AutoSize = true;
            this.radioTraceLow.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioTraceLow.Location = new System.Drawing.Point(156, 0);
            this.radioTraceLow.Name = "radioTraceLow";
            this.radioTraceLow.Size = new System.Drawing.Size(95, 29);
            this.radioTraceLow.TabIndex = 2;
            this.radioTraceLow.TabStop = true;
            this.radioTraceLow.Text = "low detail";
            this.radioTraceLow.UseVisualStyleBackColor = true;
            this.radioTraceLow.CheckedChanged += new System.EventHandler(this.radioTrace_CheckedChanged);
            // 
            // radioTraceOff
            // 
            this.radioTraceOff.AutoSize = true;
            this.radioTraceOff.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioTraceOff.Location = new System.Drawing.Point(110, 0);
            this.radioTraceOff.Name = "radioTraceOff";
            this.radioTraceOff.Size = new System.Drawing.Size(46, 29);
            this.radioTraceOff.TabIndex = 0;
            this.radioTraceOff.TabStop = true;
            this.radioTraceOff.Text = "OFF";
            this.radioTraceOff.UseVisualStyleBackColor = true;
            this.radioTraceOff.CheckedChanged += new System.EventHandler(this.radioTrace_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(6);
            this.label1.Size = new System.Drawing.Size(110, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Event trace: ";
            // 
            // tabHelp
            // 
            this.tabHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(100)))), ((int)(((byte)(95)))));
            this.tabHelp.Controls.Add(this.helpTextBox);
            this.tabHelp.Controls.Add(this.tabHelpSelector);
            this.tabHelp.Location = new System.Drawing.Point(4, 23);
            this.tabHelp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabHelp.Name = "tabHelp";
            this.tabHelp.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabHelp.Size = new System.Drawing.Size(646, 431);
            this.tabHelp.TabIndex = 2;
            this.tabHelp.Text = "Help";
            // 
            // helpTextBox
            // 
            this.helpTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.helpTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpTextBox.Location = new System.Drawing.Point(4, 29);
            this.helpTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.ReadOnly = true;
            this.helpTextBox.Size = new System.Drawing.Size(638, 399);
            this.helpTextBox.TabIndex = 2;
            this.helpTextBox.Text = "";
            // 
            // tabHelpSelector
            // 
            this.tabHelpSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabHelpSelector.Location = new System.Drawing.Point(4, 3);
            this.tabHelpSelector.Name = "tabHelpSelector";
            this.tabHelpSelector.SelectedIndex = 0;
            this.tabHelpSelector.Size = new System.Drawing.Size(638, 26);
            this.tabHelpSelector.TabIndex = 3;
            this.tabHelpSelector.SelectedIndexChanged += new System.EventHandler(this.tabHelpSelector_SelectedIndexChanged);
            // 
            // checkMissing
            // 
            this.checkMissing.AutoSize = true;
            this.checkMissing.Checked = true;
            this.checkMissing.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkMissing.Location = new System.Drawing.Point(4, 35);
            this.checkMissing.Name = "checkMissing";
            this.checkMissing.Size = new System.Drawing.Size(99, 26);
            this.checkMissing.TabIndex = 9;
            this.checkMissing.Text = "&Missing";
            this.checkMissing.ThreeState = true;
            this.checkMissing.UseVisualStyleBackColor = true;
            this.checkMissing.CheckStateChanged += new System.EventHandler(this.filelistfilter_Changed);
            // 
            // labelFileOrigin
            // 
            this.labelFileOrigin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelFileOrigin.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFileOrigin.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelFileOrigin.Location = new System.Drawing.Point(0, 320);
            this.labelFileOrigin.Name = "labelFileOrigin";
            this.labelFileOrigin.Size = new System.Drawing.Size(104, 39);
            this.labelFileOrigin.TabIndex = 8;
            this.labelFileOrigin.Text = "File o&rigin:";
            this.labelFileOrigin.Click += new System.EventHandler(this.labelFileOrigin_Click);
            // 
            // Composure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(100)))), ((int)(((byte)(95)))));
            this.ClientSize = new System.Drawing.Size(654, 458);
            this.Controls.Add(this.Tabs);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Composure";
            this.Text = "Composure (Shield of Faith configuration window)";
            this.Tabs.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabSession.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabHelp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox outputSessionDetails;
        private System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.TabPage tabHelp;
        private System.Windows.Forms.RichTextBox helpTextBox;
        public System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage tabSession;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioTraceOff;
        private System.Windows.Forms.RadioButton radioTraceLow;
        private System.Windows.Forms.Button buttonClearTrace;
        private System.Windows.Forms.RadioButton radioTraceHigh;
        private System.Windows.Forms.TabControl tabHelpSelector;
        private System.Windows.Forms.ListView configFileSystem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ColumnHeader chCfgName;
        private System.Windows.Forms.ColumnHeader chCfgFunction;
        private System.Windows.Forms.ColumnHeader chCfgPath;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton radioShowFiles;
        private System.Windows.Forms.CheckBox checkShowDirs;
        private System.Windows.Forms.RadioButton radioShowNoFiles;
        private System.Windows.Forms.RadioButton radioOtherfiles;
        private System.Windows.Forms.RadioButton radioShadioProfilo;
        private System.Windows.Forms.RadioButton radioShadio;
        private System.Windows.Forms.RadioButton radioShowConfigs;
        private System.Windows.Forms.ComboBox comboFilesByOrigin;
        private System.Windows.Forms.CheckBox checkMissing;
        private System.Windows.Forms.Label labelFileOrigin;
    }
}