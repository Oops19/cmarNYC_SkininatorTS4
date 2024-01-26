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
    public partial class CloneBatchTag : Form
    {
        bool addPropertyTag;
        uint flagNumeric, valueNumeric;
        public bool AddPropertyTag { get { return this.addPropertyTag; } }
        public uint FlagNumeric { get { return this.flagNumeric; } }
        public uint ValueNumeric { get { return this.valueNumeric; } }

        public CloneBatchTag()
        {
            InitializeComponent();
            TonePropAll_comboBox.Items.AddRange(Form1.tagCategoryNamesSkin);
        }

        private void TonePropAll_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string catName = TonePropAll_comboBox.SelectedItem.ToString();
            List<string> tmp = new List<string>();
            tmp.Add("*ALL*");
            for (int i = 0; i < Form1.tagNames.Count; i++)
            {
                int ind = Form1.tagNames[i].IndexOf("_");
                if (ind > 0 && String.Compare(catName, Form1.tagNames[i].Substring(0, ind)) == 0)
                {
                    tmp.Add(Form1.tagNames[i].Substring(Form1.tagNames[i].IndexOf("_") + 1));
                }
            }
            TonePropValueAll_comboBox.Items.Clear();
            TonePropValueAll_comboBox.Items.AddRange(tmp.ToArray());
        }

        private void TonePropAddAll_button_Click(object sender, EventArgs e)
        {
            if (String.Compare(TonePropValueAll_comboBox.SelectedItem.ToString(), "*ALL*") == 0)
            {
                MessageBox.Show("You must select a specific value for the property when adding!");
                return;
            }
            DialogResult res = MessageBox.Show("Are you sure you want to add this property to all selected tones in the package? This cannot be undone!",
                "Confirm property addition", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) this.DialogResult = DialogResult.Cancel;
            string flagName = TonePropAll_comboBox.SelectedItem.ToString();
            int ind = Form1.tagCategoryNames.IndexOf(flagName);
            uint flagNumeric = Form1.tagCategoryNumbers[ind];
            string flagVal = flagName + "_" + TonePropValueAll_comboBox.SelectedItem.ToString();
            ind = Form1.tagNames.IndexOf(flagVal);
            uint valNumeric = Form1.tagNumbers[ind];
            this.flagNumeric = flagNumeric;
            this.valueNumeric = valNumeric;
            this.addPropertyTag = true;
            this.DialogResult = DialogResult.OK;
        }

        private void TonePropRemoveAll_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to remove this property from all selected tones in the package? This cannot be undone!",
                "Confirm property removal", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) this.DialogResult = DialogResult.Cancel;
            string flagName = TonePropAll_comboBox.SelectedItem.ToString();
            int ind = Form1.tagCategoryNames.IndexOf(flagName);
            uint flagNumeric = Form1.tagCategoryNumbers[ind];
            uint valNumeric = 0;
            if (String.Compare(TonePropValueAll_comboBox.SelectedItem.ToString(), "*ALL*") != 0)
            {
                string flagVal = flagName + "_" + TonePropValueAll_comboBox.SelectedItem.ToString();
                ind = Form1.tagNames.IndexOf(flagVal);
                valNumeric = Form1.tagNumbers[ind];
            }
            this.flagNumeric = flagNumeric;
            this.valueNumeric = valNumeric;
            this.addPropertyTag = false;
            this.DialogResult = DialogResult.OK;
        }
    }
}
