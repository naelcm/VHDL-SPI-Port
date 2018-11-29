namespace ExpChargeTechTool
{
    partial class TopForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbDevices = new System.Windows.Forms.ComboBox();
            this.tmr100 = new System.Windows.Forms.Timer(this.components);
            this.grpRegisters = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose port:";
            // 
            // cmbDevices
            // 
            this.cmbDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDevices.FormattingEnabled = true;
            this.cmbDevices.Location = new System.Drawing.Point(85, 6);
            this.cmbDevices.Name = "cmbDevices";
            this.cmbDevices.Size = new System.Drawing.Size(191, 21);
            this.cmbDevices.TabIndex = 2;
            this.cmbDevices.DropDown += new System.EventHandler(this.cmbDevices_DropDown);
            this.cmbDevices.DropDownClosed += new System.EventHandler(this.cmbDevices_DropDownClosed);
            // 
            // tmr100
            // 
            this.tmr100.Tick += new System.EventHandler(this.tmr100_Tick);
            // 
            // grpRegisters
            // 
            this.grpRegisters.Location = new System.Drawing.Point(12, 122);
            this.grpRegisters.Name = "grpRegisters";
            this.grpRegisters.Size = new System.Drawing.Size(468, 267);
            this.grpRegisters.TabIndex = 3;
            this.grpRegisters.TabStop = false;
            this.grpRegisters.Text = "SPI registers";
            // 
            // TopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 423);
            this.Controls.Add(this.grpRegisters);
            this.Controls.Add(this.cmbDevices);
            this.Controls.Add(this.label1);
            this.Name = "TopForm";
            this.Text = "LCMX02-7000HE Tech Tool";
            this.Load += new System.EventHandler(this.TopForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbDevices;
        private System.Windows.Forms.Timer tmr100;
        private System.Windows.Forms.GroupBox grpRegisters;
    }
}

