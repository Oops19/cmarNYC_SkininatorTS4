/* TS4 Skininator, a tool for creating custom content for The Sims 4,
   Copyright (C) 2015  C. Marinetti

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
   The author may be contacted at modthesims.info, username cmarNYC. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Xmods.DataLib;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        private void TonePropAllSetup(string[] categoryNames)
        {
            TonePropAll_comboBox.Items.AddRange(categoryNames);
        }

        private void TonePropAll_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string catName = TonePropAll_comboBox.SelectedItem.ToString();
            List<string> tmp = new List<string>();
            tmp.Add("*ALL*");
            for (int i = 0; i < tagNames.Count; i++)
            {
                int ind = tagNames[i].IndexOf("_");
                if (ind > 0 && String.Compare(catName, tagNames[i].Substring(0, ind)) == 0)
                {
                    tmp.Add(tagNames[i].Substring(tagNames[i].IndexOf("_") + 1));
                }
            }
            TonePropValueAll_comboBox.Items.Clear();
            TonePropValueAll_comboBox.Items.AddRange(tmp.ToArray());
        }

        private void TonePropAddAll_button_Click(object sender, EventArgs e)
        {
            if (changesTone)
            {
                MessageBox.Show("You have unsaved TONE changes! Please save or discard them before continuing.");
                return;
            }
            if (String.Compare(TonePropValueAll_comboBox.SelectedItem.ToString(), "*ALL*") == 0)
            {
                MessageBox.Show("You must select a specific value for the property when adding!");
                return;
            }
            DialogResult res = MessageBox.Show("Are you sure you want to add this property to all selected tones in the package? This cannot be undone!",
                "Confirm property addition", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            string flagName = TonePropAll_comboBox.SelectedItem.ToString();
            int ind = tagCategoryNames.IndexOf(flagName);
            uint flagNumeric = tagCategoryNumbers[ind];
            string flagVal = flagName + "_" + TonePropValueAll_comboBox.SelectedItem.ToString();
            ind = tagNames.IndexOf(flagVal);
            uint valNumeric = tagNumbers[ind];
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                if ((string)CloneTONElist_dataGridView.Rows[i].Cells[3].Value == "F") continue;
                int index = (int)CloneTONElist_dataGridView.Rows[i].Tag;
                List<uint[]> tmpTags = new List<uint[]>(clonePackTONEs[index].CategoryTags);
                tmpTags.Add(new uint[] { flagNumeric, valNumeric });
                clonePackTONEs[index].CategoryTags = tmpTags;
                MemoryStream m = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(m);
                clonePackTONEs[index].Write(bw);
                m.Position = 0;
                ReplaceResource(clonePack, clonePackTONEs[index].resourceEntry, m);
            }
            CloneTONEsList(false);
        }

        private void TonePropRemoveAll_button_Click(object sender, EventArgs e)
        {
            if (changesTone)
            {
                MessageBox.Show("You have unsaved TONE changes! Please save or discard them before continuing.");
                return;
            }
            DialogResult res = MessageBox.Show("Are you sure you want to remove this property from all selected tones in the package? This cannot be undone!",
                "Confirm property removal", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            string flagName = TonePropAll_comboBox.SelectedItem.ToString();
            int ind = tagCategoryNames.IndexOf(flagName);
            uint flagNumeric = tagCategoryNumbers[ind];
            uint valNumeric = 0;
            if (String.Compare(TonePropValueAll_comboBox.SelectedItem.ToString(), "*ALL*") != 0)
            {
                string flagVal = flagName + "_" + TonePropValueAll_comboBox.SelectedItem.ToString();
                ind = tagNames.IndexOf(flagVal);
                valNumeric = tagNumbers[ind];
            }
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                if ((string)CloneTONElist_dataGridView.Rows[i].Cells[3].Value == "F") continue;
                int index = (int)CloneTONElist_dataGridView.Rows[i].Tag;
                List<uint[]> tmpTags = new List<uint[]>();
                foreach (uint[] tag in clonePackTONEs[index].CategoryTags)
                {
                    if (tag[0] == flagNumeric)
                    {
                        if (valNumeric != 0 && tag[1] != valNumeric)
                        {
                            tmpTags.Add(tag);
                        }
                    }
                    else
                    {
                        tmpTags.Add(tag);
                    }
                }
                clonePackTONEs[index].CategoryTags = tmpTags;
                MemoryStream m = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(m);
                clonePackTONEs[index].Write(bw);
                m.Position = 0;
                ReplaceResource(clonePack, clonePackTONEs[index].resourceEntry, m);
            }
            CloneTONEsList(false);
        }

        private void TonePropTuning_button_Click(object sender, EventArgs e)
        {
            if (changesTone)
            {
                MessageBox.Show("You have unsaved TONE changes! Please save or discard them before continuing.");
                return;
            }
            DialogResult res = MessageBox.Show("Are you sure you want to apply this tuning to all selected tones in the package? This cannot be undone!",
                "Confirm tuning change", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                if ((string)CloneTONElist_dataGridView.Rows[i].Cells[3].Value == "F") continue;
                int index = (int)CloneTONElist_dataGridView.Rows[i].Tag;
                clonePackTONEs[index].TuningInstance = skintoneTuning[TonePropTuning_comboBox.SelectedIndex];
                MemoryStream m = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(m);
                clonePackTONEs[index].Write(bw);
                m.Position = 0;
                ReplaceResource(clonePack, clonePackTONEs[index].resourceEntry, m);
            }
            CloneTONEsList(false);
        }

        private void TonePropVersion_button_Click(object sender, EventArgs e)
        {
            if (changesTone)
            {
                MessageBox.Show("You have unsaved TONE changes! Please save or discard them before continuing.");
                return;
            }
            DialogResult res = MessageBox.Show("Are you sure you want to update the version for ALL tones in the package? This cannot be undone!",
                "Confirm version change", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                int index = (int)CloneTONElist_dataGridView.Rows[i].Tag;
                if (clonePackTONEs[index].Version < latestVersion)
                {
                    clonePackTONEs[index].Version = latestVersion;
                    if (clonePackTONEs[index].TuningInstance == 0)
                    {
                        bool isHuman = false; bool isVampire = false; bool isAlien = false; bool isNatural = false; bool isFantasy = false;
                        foreach (uint[] f in clonePackTONEs[index].CategoryTags)
                        {
                            int ind = tagNumbers.IndexOf(f[1]);
                            if (tagNames[ind].Contains("Human")) isHuman = true;
                            if (tagNames[ind].Contains("Alien")) isAlien = true;
                            if (tagNames[ind].Contains("Vampire")) isVampire = true;
                            if (tagNames[ind].Contains("Natural")) isNatural = true;
                            if (tagNames[ind].Contains("Fantasy")) isFantasy = true;
                        }
                        if (isHuman && isVampire)
                        {
                            clonePackTONEs[index].TuningInstance = skintoneTuning[skintoneTuningNames.FindIndex(x => x.Contains("Human_Vampire"))];
                        }
                        else if (isHuman && isFantasy)
                        {
                            clonePackTONEs[index].TuningInstance = skintoneTuning[skintoneTuningNames.FindIndex(x => x.Contains("Human_Fantasy"))];
                        }
                        else if (isHuman)
                        {
                            clonePackTONEs[index].TuningInstance = skintoneTuning[tuningHuman];
                        }
                        else if (isVampire && isFantasy)
                        {
                            clonePackTONEs[index].TuningInstance = skintoneTuning[skintoneTuningNames.FindIndex(x => x.Contains("Vampire_Fantasy"))];
                        }
                        else if (isVampire)
                        {
                            clonePackTONEs[index].TuningInstance = skintoneTuning[skintoneTuningNames.FindIndex(x => x.Contains("Vampire"))];
                        }
                        else if (isAlien)
                        {
                            clonePackTONEs[index].TuningInstance = skintoneTuning[skintoneTuningNames.FindIndex(x => x.Contains("Alien"))];
                        }
                        else
                        {
                            clonePackTONEs[index].TuningInstance = skintoneTuning[skintoneTuningNames.FindIndex(x => x.Contains("Mannequin"))];
                        }
                    }
                    MemoryStream m = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(m);
                    clonePackTONEs[index].Write(bw);
                    m.Position = 0;
                    ReplaceResource(clonePack, clonePackTONEs[index].resourceEntry, m);
                }
            }
            TonePropVersion_groupBox.Visible = false;
            TonePropTuning_groupBox.Visible = true;
            CloneTONEsList(false);
        }
    }
}
