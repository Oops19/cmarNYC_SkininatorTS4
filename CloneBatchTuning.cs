using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XMODS
{
    public partial class CloneBatchTuning : Form
    {
        ulong tuning;
        public ulong TuningInstance { get { return this.tuning; } }

        public CloneBatchTuning()
        {
            InitializeComponent();
            TonePropTuning_comboBox.Items.AddRange(Form1.skintoneTuningNames.ToArray());
        }

        private void TonePropTuning_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to apply this tuning to all selected tones in the package? This cannot be undone!",
               "Confirm tuning change", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) this.DialogResult = DialogResult.Cancel;
            this.tuning = Form1.skintoneTuning[TonePropTuning_comboBox.SelectedIndex];
            this.DialogResult = DialogResult.OK;
        }
    }
}
