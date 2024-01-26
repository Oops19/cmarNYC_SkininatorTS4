namespace XMODS
{
    partial class CloneBatchTuning
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
            this.TonePropTuning_groupBox = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.TonePropTuning_comboBox = new System.Windows.Forms.ComboBox();
            this.TonePropTuning_button = new System.Windows.Forms.Button();
            this.TonePropTuning_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // TonePropTuning_groupBox
            // 
            this.TonePropTuning_groupBox.Controls.Add(this.label17);
            this.TonePropTuning_groupBox.Controls.Add(this.TonePropTuning_comboBox);
            this.TonePropTuning_groupBox.Controls.Add(this.TonePropTuning_button);
            this.TonePropTuning_groupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TonePropTuning_groupBox.Location = new System.Drawing.Point(16, 38);
            this.TonePropTuning_groupBox.Name = "TonePropTuning_groupBox";
            this.TonePropTuning_groupBox.Size = new System.Drawing.Size(432, 150);
            this.TonePropTuning_groupBox.TabIndex = 12;
            this.TonePropTuning_groupBox.TabStop = false;
            this.TonePropTuning_groupBox.Text = "Tuning";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(17, 43);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(76, 13);
            this.label17.TabIndex = 7;
            this.label17.Text = "Select Tuning:";
            // 
            // TonePropTuning_comboBox
            // 
            this.TonePropTuning_comboBox.FormattingEnabled = true;
            this.TonePropTuning_comboBox.Location = new System.Drawing.Point(106, 40);
            this.TonePropTuning_comboBox.Name = "TonePropTuning_comboBox";
            this.TonePropTuning_comboBox.Size = new System.Drawing.Size(263, 21);
            this.TonePropTuning_comboBox.TabIndex = 6;
            // 
            // TonePropTuning_button
            // 
            this.TonePropTuning_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TonePropTuning_button.Location = new System.Drawing.Point(106, 76);
            this.TonePropTuning_button.Name = "TonePropTuning_button";
            this.TonePropTuning_button.Size = new System.Drawing.Size(263, 50);
            this.TonePropTuning_button.TabIndex = 8;
            this.TonePropTuning_button.Text = "Apply to Selected Skintones in Package";
            this.TonePropTuning_button.UseVisualStyleBackColor = true;
            this.TonePropTuning_button.Click += new System.EventHandler(this.TonePropTuning_button_Click);
            // 
            // CloneBatchTuning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 241);
            this.Controls.Add(this.TonePropTuning_groupBox);
            this.Name = "CloneBatchTuning";
            this.Text = "Batch Adjust Tuning";
            this.TonePropTuning_groupBox.ResumeLayout(false);
            this.TonePropTuning_groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox TonePropTuning_groupBox;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox TonePropTuning_comboBox;
        private System.Windows.Forms.Button TonePropTuning_button;
    }
}