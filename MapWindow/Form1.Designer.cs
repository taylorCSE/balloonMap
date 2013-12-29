namespace MapWindow
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.axMappointControl1 = new AxMapPoint.AxMappointControl();
            this.FlightComboBox = new System.Windows.Forms.ComboBox();
            this.FlightLabel = new System.Windows.Forms.Label();
            this.EnablePathsCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.axMappointControl1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // axMappointControl1
            // 
            this.axMappointControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.axMappointControl1.Enabled = true;
            this.axMappointControl1.Location = new System.Drawing.Point(-9, -19);
            this.axMappointControl1.Margin = new System.Windows.Forms.Padding(0);
            this.axMappointControl1.Name = "axMappointControl1";
            this.axMappointControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMappointControl1.OcxState")));
            this.axMappointControl1.Size = new System.Drawing.Size(1022, 424);
            this.axMappointControl1.TabIndex = 0;
            // 
            // FlightComboBox
            // 
            this.FlightComboBox.FormattingEnabled = true;
            this.FlightComboBox.Location = new System.Drawing.Point(47, 12);
            this.FlightComboBox.Margin = new System.Windows.Forms.Padding(12);
            this.FlightComboBox.Name = "FlightComboBox";
            this.FlightComboBox.Size = new System.Drawing.Size(127, 21);
            this.FlightComboBox.TabIndex = 1;
            this.FlightComboBox.SelectedIndexChanged += new System.EventHandler(this.FlightComboBox_SelectedIndexChanged);
            // 
            // FlightLabel
            // 
            this.FlightLabel.AutoSize = true;
            this.FlightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FlightLabel.Location = new System.Drawing.Point(8, 15);
            this.FlightLabel.Name = "FlightLabel";
            this.FlightLabel.Size = new System.Drawing.Size(32, 13);
            this.FlightLabel.TabIndex = 2;
            this.FlightLabel.Text = "Flight";
            // 
            // EnablePathsCheckbox
            // 
            this.EnablePathsCheckbox.AutoSize = true;
            this.EnablePathsCheckbox.Location = new System.Drawing.Point(11, 48);
            this.EnablePathsCheckbox.Name = "EnablePathsCheckbox";
            this.EnablePathsCheckbox.Size = new System.Drawing.Size(89, 17);
            this.EnablePathsCheckbox.TabIndex = 3;
            this.EnablePathsCheckbox.Text = "Enable Paths";
            this.EnablePathsCheckbox.UseVisualStyleBackColor = true;
            this.EnablePathsCheckbox.CheckedChanged += new System.EventHandler(this.EnablePathsCheckbox_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.EnablePathsCheckbox);
            this.panel1.Controls.Add(this.FlightComboBox);
            this.panel1.Controls.Add(this.FlightLabel);
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(188, 76);
            this.panel1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 394);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.axMappointControl1);
            this.Name = "Form1";
            this.Text = "BalloonMap";
            ((System.ComponentModel.ISupportInitialize)(this.axMappointControl1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private AxMapPoint.AxMappointControl axMappointControl1;
        private System.Windows.Forms.ComboBox FlightComboBox;
        private System.Windows.Forms.Label FlightLabel;
        private System.Windows.Forms.CheckBox EnablePathsCheckbox;
        private System.Windows.Forms.Panel panel1;
    }
}

