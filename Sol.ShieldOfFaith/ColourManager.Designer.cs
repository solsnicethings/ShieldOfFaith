namespace Sol.ShieldOfFaith
{
    partial class ColourManager
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColourManager));
            this.panelColour = new System.Windows.Forms.Panel();
            this.panelPrevious = new System.Windows.Forms.Panel();
            this.buttonPrevC = new System.Windows.Forms.Button();
            this.buttonPickColour = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.trackBoxAlpha = new Sol.ShieldOfFaith.TrackBox.Composite();
            this.trackB = new Sol.ShieldOfFaith.TrackBox.Composite();
            this.trackG = new Sol.ShieldOfFaith.TrackBox.Composite();
            this.trackR = new Sol.ShieldOfFaith.TrackBox.Composite();
            this.panelColour.SuspendLayout();
            this.panelPrevious.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelColour
            // 
            this.panelColour.Controls.Add(this.panelPrevious);
            this.panelColour.Controls.Add(this.buttonPickColour);
            this.panelColour.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelColour.Location = new System.Drawing.Point(208, 0);
            this.panelColour.Name = "panelColour";
            this.panelColour.Size = new System.Drawing.Size(71, 136);
            this.panelColour.TabIndex = 0;
            // 
            // panelPrevious
            // 
            this.panelPrevious.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPrevious.Controls.Add(this.buttonPrevC);
            this.panelPrevious.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPrevious.Location = new System.Drawing.Point(0, 84);
            this.panelPrevious.Name = "panelPrevious";
            this.panelPrevious.Size = new System.Drawing.Size(71, 52);
            this.panelPrevious.TabIndex = 2;
            // 
            // buttonPrevC
            // 
            this.buttonPrevC.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonPrevC.Location = new System.Drawing.Point(0, 0);
            this.buttonPrevC.Name = "buttonPrevC";
            this.buttonPrevC.Size = new System.Drawing.Size(69, 36);
            this.buttonPrevC.TabIndex = 1;
            this.buttonPrevC.Text = "Previous pick";
            this.buttonPrevC.UseVisualStyleBackColor = false;
            this.buttonPrevC.Click += new System.EventHandler(this.buttonPrevC_Click);
            // 
            // buttonPickColour
            // 
            this.buttonPickColour.BackColor = System.Drawing.Color.Transparent;
            this.buttonPickColour.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonPickColour.BackgroundImage")));
            this.buttonPickColour.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonPickColour.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonPickColour.Location = new System.Drawing.Point(0, 0);
            this.buttonPickColour.Name = "buttonPickColour";
            this.buttonPickColour.Size = new System.Drawing.Size(71, 63);
            this.buttonPickColour.TabIndex = 0;
            this.buttonPickColour.UseVisualStyleBackColor = false;
            this.buttonPickColour.Click += new System.EventHandler(this.buttonPickColour_Click);
            // 
            // trackBoxAlpha
            // 
            this.trackBoxAlpha.AlternateTextColor = System.Drawing.Color.Transparent;
            this.trackBoxAlpha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(210)))), ((int)(((byte)(200)))));
            this.trackBoxAlpha.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackBoxAlpha.HighValueColor = System.Drawing.Color.LightBlue;
            this.trackBoxAlpha.HoveredValueColor = System.Drawing.Color.Transparent;
            this.trackBoxAlpha.Location = new System.Drawing.Point(0, 99);
            this.trackBoxAlpha.LowValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.trackBoxAlpha.Name = "trackBoxAlpha";
            this.trackBoxAlpha.Size = new System.Drawing.Size(208, 33);
            this.trackBoxAlpha.TabIndex = 4;
            this.trackBoxAlpha.Text = "alpha (opacity / intensity) : ";
            this.trackBoxAlpha.ValueToStringOverride = null;
            this.trackBoxAlpha.ValueChanged += new System.EventHandler(this.track_ValueChanged);
            // 
            // trackB
            // 
            this.trackB.AlternateTextColor = System.Drawing.Color.Silver;
            this.trackB.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackB.HighValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(255)))));
            this.trackB.HoveredValueColor = System.Drawing.Color.Transparent;
            this.trackB.Location = new System.Drawing.Point(0, 66);
            this.trackB.LowValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.trackB.Name = "trackB";
            this.trackB.Size = new System.Drawing.Size(208, 33);
            this.trackB.TabIndex = 3;
            this.trackB.Text = "blue : ";
            this.trackB.ValueToStringOverride = null;
            this.trackB.ValueChanged += new System.EventHandler(this.track_ValueChanged);
            // 
            // trackG
            // 
            this.trackG.AlternateTextColor = System.Drawing.Color.Transparent;
            this.trackG.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackG.HighValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(255)))), ((int)(((byte)(60)))));
            this.trackG.HoveredValueColor = System.Drawing.Color.Transparent;
            this.trackG.Location = new System.Drawing.Point(0, 33);
            this.trackG.LowValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.trackG.Name = "trackG";
            this.trackG.Size = new System.Drawing.Size(208, 33);
            this.trackG.TabIndex = 2;
            this.trackG.Text = "green : ";
            this.trackG.ValueToStringOverride = null;
            this.trackG.ValueChanged += new System.EventHandler(this.track_ValueChanged);
            // 
            // trackR
            // 
            this.trackR.AlternateTextColor = System.Drawing.Color.White;
            this.trackR.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackR.HighValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.trackR.HoveredValueColor = System.Drawing.Color.Transparent;
            this.trackR.Location = new System.Drawing.Point(0, 0);
            this.trackR.LowValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.trackR.Name = "trackR";
            this.trackR.Size = new System.Drawing.Size(208, 33);
            this.trackR.TabIndex = 1;
            this.trackR.Text = "red  :  ";
            this.trackR.ValueToStringOverride = null;
            this.trackR.ValueChanged += new System.EventHandler(this.track_ValueChanged);
            // 
            // ColourManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBoxAlpha);
            this.Controls.Add(this.trackB);
            this.Controls.Add(this.trackG);
            this.Controls.Add(this.trackR);
            this.Controls.Add(this.panelColour);
            this.Name = "ColourManager";
            this.Size = new System.Drawing.Size(279, 136);
            this.panelColour.ResumeLayout(false);
            this.panelPrevious.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelColour;
        private System.Windows.Forms.Button buttonPickColour;
        private TrackBox.Composite trackR;
        private TrackBox.Composite trackG;
        private TrackBox.Composite trackB;
        private System.Windows.Forms.Button buttonPrevC;
        private TrackBox.Composite trackBoxAlpha;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel panelPrevious;
    }
}
