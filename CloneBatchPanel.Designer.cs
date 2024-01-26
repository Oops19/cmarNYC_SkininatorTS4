namespace XMODS
{
    partial class CloneBatchPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.Panel_comboBox = new System.Windows.Forms.ComboBox();
            this.ClonePropPanel_button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Panel: ";
            // 
            // Panel_comboBox
            // 
            this.Panel_comboBox.FormattingEnabled = true;
            this.Panel_comboBox.Items.AddRange(new object[] {
            "Warm",
            "Neutral",
            "Cool",
            "Miscellaneous"});
            this.Panel_comboBox.Location = new System.Drawing.Point(117, 32);
            this.Panel_comboBox.Name = "Panel_comboBox";
            this.Panel_comboBox.Size = new System.Drawing.Size(121, 21);
            this.Panel_comboBox.TabIndex = 1;
            // 
            // ClonePropPanel_button
            // 
            this.ClonePropPanel_button.Location = new System.Drawing.Point(41, 73);
            this.ClonePropPanel_button.Name = "ClonePropPanel_button";
            this.ClonePropPanel_button.Size = new System.Drawing.Size(197, 63);
            this.ClonePropPanel_button.TabIndex = 2;
            this.ClonePropPanel_button.Text = "Apply to Selected Skintones in Package";
            this.ClonePropPanel_button.UseVisualStyleBackColor = true;
            this.ClonePropPanel_button.Click += new System.EventHandler(this.ClonePropPanel_button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ClonePropPanel_button);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Panel_comboBox);
            this.groupBox1.Location = new System.Drawing.Point(72, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 171);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Skincolor Panel";
            // 
            // CloneBatchPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 256);
            this.Controls.Add(this.groupBox1);
            this.Name = "CloneBatchPanel";
            this.Text = "Select Skincolor Panel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Panel_comboBox;
        private System.Windows.Forms.Button ClonePropPanel_button;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}