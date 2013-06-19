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
            ((System.ComponentModel.ISupportInitialize)(this.axMappointControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // axMappointControl1
            // 
            this.axMappointControl1.Enabled = true;
            this.axMappointControl1.Location = new System.Drawing.Point(0, 0);
            this.axMappointControl1.Name = "axMappointControl1";
            this.axMappointControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMappointControl1.OcxState")));
            this.axMappointControl1.Size = new System.Drawing.Size(992, 394);
            this.axMappointControl1.TabIndex = 0;
            // 
            // FlightComboBox
            // 
            this.FlightComboBox.FormattingEnabled = true;
            this.FlightComboBox.Location = new System.Drawing.Point(861, 12);
            this.FlightComboBox.Name = "FlightComboBox";
            this.FlightComboBox.Size = new System.Drawing.Size(121, 21);
            this.FlightComboBox.TabIndex = 1;
            this.FlightComboBox.SelectedIndexChanged += new System.EventHandler(this.FlightComboBox_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 394);
            this.Controls.Add(this.FlightComboBox);
            this.Controls.Add(this.axMappointControl1);
            this.Name = "Form1";
            this.Text = "BalloonMap";
            ((System.ComponentModel.ISupportInitialize)(this.axMappointControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private AxMapPoint.AxMappointControl axMappointControl1;
        private System.Windows.Forms.ComboBox FlightComboBox;
    }
}

