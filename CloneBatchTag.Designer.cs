namespace XMODS
{
    partial class CloneBatchTag
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
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.TonePropAll_comboBox = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.TonePropValueAll_comboBox = new System.Windows.Forms.ComboBox();
            this.TonePropAddAll_button = new System.Windows.Forms.Button();
            this.TonePropRemoveAll_button = new System.Windows.Forms.Button();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.TonePropAll_comboBox);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Controls.Add(this.label15);
            this.groupBox6.Controls.Add(this.TonePropValueAll_comboBox);
            this.groupBox6.Controls.Add(this.TonePropAddAll_button);
            this.groupBox6.Controls.Add(this.TonePropRemoveAll_button);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(12, 32);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(430, 226);
            this.groupBox6.TabIndex = 11;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Property Tags";
            // 
            // TonePropAll_comboBox
            // 
            this.TonePropAll_comboBox.FormattingEnabled = true;
            this.TonePropAll_comboBox.Location = new System.Drawing.Point(110, 48);
            this.TonePropAll_comboBox.Name = "TonePropAll_comboBox";
            this.TonePropAll_comboBox.Size = new System.Drawing.Size(259, 21);
            this.TonePropAll_comboBox.TabIndex = 0;
            this.TonePropAll_comboBox.SelectedIndexChanged += new System.EventHandler(this.TonePropAll_comboBox_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(21, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Property Tag:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(56, 87);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Value:";
            // 
            // TonePropValueAll_comboBox
            // 
            this.TonePropValueAll_comboBox.FormattingEnabled = true;
            this.TonePropValueAll_comboBox.Location = new System.Drawing.Point(110, 84);
            this.TonePropValueAll_comboBox.Name = "TonePropValueAll_comboBox";
            this.TonePropValueAll_comboBox.Size = new System.Drawing.Size(259, 21);
            this.TonePropValueAll_comboBox.TabIndex = 3;
            // 
            // TonePropAddAll_button
            // 
            this.TonePropAddAll_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TonePropAddAll_button.Location = new System.Drawing.Point(110, 121);
            this.TonePropAddAll_button.Name = "TonePropAddAll_button";
            this.TonePropAddAll_button.Size = new System.Drawing.Size(253, 34);
            this.TonePropAddAll_button.TabIndex = 4;
            this.TonePropAddAll_button.Text = "Add to Selected Skintones in Package";
            this.TonePropAddAll_button.UseVisualStyleBackColor = true;
            this.TonePropAddAll_button.Click += new System.EventHandler(this.TonePropAddAll_button_Click);
            // 
            // TonePropRemoveAll_button
            // 
            this.TonePropRemoveAll_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TonePropRemoveAll_button.Location = new System.Drawing.Point(110, 161);
            this.TonePropRemoveAll_button.Name = "TonePropRemoveAll_button";
            this.TonePropRemoveAll_button.Size = new System.Drawing.Size(253, 34);
            this.TonePropRemoveAll_button.TabIndex = 5;
            this.TonePropRemoveAll_button.Text = "Remove From Selected Skintones in Package";
            this.TonePropRemoveAll_button.UseVisualStyleBackColor = true;
            this.TonePropRemoveAll_button.Click += new System.EventHandler(this.TonePropRemoveAll_button_Click);
            // 
            // CloneBatchTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 300);
            this.Controls.Add(this.groupBox6);
            this.Name = "CloneBatchTag";
            this.Text = "Batch Adjust Property Tags";
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ComboBox TonePropAll_comboBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox TonePropValueAll_comboBox;
        private System.Windows.Forms.Button TonePropAddAll_button;
        private System.Windows.Forms.Button TonePropRemoveAll_button;
    }
}