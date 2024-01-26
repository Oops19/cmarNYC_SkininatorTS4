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
    public partial class CloneBatchPanel : Form
    {
        public ushort skinPanel;

        public CloneBatchPanel()
        {
            InitializeComponent();
            Panel_comboBox.SelectedIndex = 0;
        }

        private void ClonePropPanel_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to apply this skincolor panel choice to all selected tones in the package? This cannot be undone!",
               "Confirm panel change", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) this.DialogResult = DialogResult.Cancel;
            this.skinPanel = (ushort)(Panel_comboBox.SelectedIndex + 1);
            this.DialogResult = DialogResult.OK;
        }
    }
}
