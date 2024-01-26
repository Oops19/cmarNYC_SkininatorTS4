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
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Xmods.DataLib;
using s4pi.ImageResource;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        List<TONEinfo> clonePackTONEs;
        TONEinfo myTONE;
        IResourceIndexEntry myKey;
        int myRow, myTONEindex, mySkinState;
        bool changesTone = false;
        string[] skinStateNames = new string[] { "Normal", "Tanned", "Burned" };

        private void GetSkinStates(TONEinfo tone)
        {
            tone.skinStates = new TONEinfo.SkinStates[tone.SkinSets.Length];

            if (tone.Version >= 10)
            {
                for (int i = 0; i < tone.SkinSets.Length; i++)
                {
                    TONE.SkinSetDesc set = tone.SkinSets[i];
                    IResourceIndexEntry iresSkin, iresMask;
                    bool clonedSkin, clonedMask;
                    LRLE skin = FindCloneTexture(set.TextureInstance, out iresSkin, out clonedSkin);
                    RLEResource mask = FindMaskTexture(set.OverlayInstance, out iresMask, out clonedMask);
                    tone.skinStates[i] = new TONEinfo.SkinStates(set, skin, mask, clonedSkin, false, clonedMask, false);
                }
            }

            else
            {
                TONE.SkinSetDesc set = tone.SkinSets[0];
                IResourceIndexEntry iresSkin;
                bool clonedSkin;
                LRLE skin = FindCloneTexture(set.TextureInstance, out iresSkin, out clonedSkin);
                tone.skinStates[0] = new TONEinfo.SkinStates(set, skin, null, clonedSkin, false, false, false);
                tone.skinStates[1] = new TONEinfo.SkinStates(new TONE.SkinSetDesc(), null, null, false, false, false, false);
                tone.skinStates[2] = new TONEinfo.SkinStates(new TONE.SkinSetDesc(), null, null, false, false, false, false);
            }
        }

        private void ListTONEflags(DataGridView flagsGrid, List<uint[]> flagsList, string[] categoryNames, uint[] categoryNumeric)
        {
            foreach (uint[] f in flagsList)
            {
                if (Array.IndexOf(categoryNumeric, f[0]) < 0) continue;
                flagsGrid.Rows.Add();
                int index = flagsGrid.Rows.Count - 1;
                DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)flagsGrid.Rows[index].Cells[0];
                flag.Items.AddRange(categoryNames);
                string selectedFlagType = "";
                bool found = false;
                for (int i = 0; i < categoryNumeric.Length; i++)
                {
                    if (f[0] == categoryNumeric[i])
                    {
                        flag.Value = categoryNames[i];
                        selectedFlagType = categoryNames[i];
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    MessageBox.Show("Unknown category found: " + f[0].ToString());
                    flag.Items.Add(f[0].ToString());
                    flag.Value = f[0].ToString();
                }
                DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)flagsGrid.Rows[index].Cells[1];
                List<string> tmp = new List<string>();
                for (int i = 0; i < tagNames.Count; i++)
                {
                    int ind = tagNames[i].IndexOf("_");
                    if (ind > 0 && String.Compare(selectedFlagType, tagNames[i].Substring(0, ind)) == 0)
                    {
                        tmp.Add(tagNames[i].Substring(tagNames[i].IndexOf("_") + 1));
                    }
                }
                //if (errors.Length > 1) MessageBox.Show(errors);
                val.Items.AddRange(tmp.ToArray());
                found = false;
                for (int i = 0; i < tagNumbers.Count; i++)
                {
                    if (f[1] == tagNumbers[i])
                    {
                        if (tagNames[i].IndexOf("_") > 0) val.Value = tagNames[i].Substring(tagNames[i].IndexOf("_") + 1);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    MessageBox.Show("Unknown category value found: " + f[1].ToString());
                    val.Items.Add(f[1].ToString());
                    val.Value = f[1].ToString();
                }
                if (!val.Items.Contains(val.Value))
                {
                    MessageBox.Show("Value " + val.Value.ToString() + " does not match category " + flag.Value.ToString() + ", please correct!");
                    string s = "* " + val.Value.ToString() + " *";
                    val.Items.Add(s);
                    val.Value = s;
                }
            }
        }

        private void CloneFlags_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
           // DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)grid.Rows[e.RowIndex].Cells[0];
            DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)grid.Rows[e.RowIndex].Cells[1];
           // MessageBox.Show("Value " + val.Value.ToString() + " does not match category " + flag.Value.ToString());
           // val.Items.Add(val.Value.ToString());
            if (val.Items.Contains(val.Value))
            {
                MessageBox.Show("An unknown data error has occurred: " + e.Context.ToString());
            }
        }

        private void CloneAddFlag_button_Click(DataGridView grid, EventArgs e, bool forAll)
        {
            grid.Rows.Add();
            int index = grid.Rows.Count - 1;
            DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)grid.Rows[index].Cells[0];
            flag.Items.AddRange(tagCategoryNamesSkin);
        }

        private void CloneFlags_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (e.ColumnIndex == 2 & e.RowIndex >= 0)
            {
                DialogResult res = MessageBox.Show("Okay to delete this TONE property?", "Delete Property", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    grid.Rows.RemoveAt(e.RowIndex);
                    changesTone = true;
                }
            }
        }

        private void CloneFlags_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (e.ColumnIndex == 0 & e.RowIndex >= 0)
            {
                string catName = grid.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)grid.Rows[e.RowIndex].Cells[1];
                List<string> tmp = new List<string>();
                for (int i = 0; i < tagNames.Count; i++)
                {
                    int ind = tagNames[i].IndexOf("_");
                    if (ind > 0 && String.Compare(catName, tagNames[i].Substring(0, ind)) == 0)
                    {
                        tmp.Add(tagNames[i].Substring(tagNames[i].IndexOf("_") + 1));
                    }
                }
                val.Value = null;
                val.Items.Clear();
                val.Items.AddRange(tmp.ToArray());
            }
            changesTone = true;
        }

        private List<uint[]> ReadPropertyFlags(DataGridView flagsGrid)
        {
            List<uint[]> newFlags = new List<uint[]>();
            for (int i = 0; i < flagsGrid.Rows.Count; i++)
            {
                DataGridViewComboBoxCell flag = (DataGridViewComboBoxCell)flagsGrid.Rows[i].Cells[0];
                string flagName = flag.Value.ToString();
                int ind = tagCategoryNames.IndexOf(flagName);
                uint flagNumeric = tagCategoryNumbers[ind];
                DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)flagsGrid.Rows[i].Cells[1];
                string flagVal = flagName + "_" + val.Value.ToString();
                ind = tagNames.IndexOf(flagVal);
                uint valNumeric = tagNumbers[ind];
                newFlags.Add(new uint[] { flagNumeric, valNumeric });
            }
            return newFlags;
        }

        private List<uint[]> UpdatePropertyFlags(List<uint[]> caspFlags, List<uint[]> modifiedFlags, bool forAll)
        {
            List<uint[]> newFlags = new List<uint[]>();
            newFlags.AddRange(modifiedFlags);
            return newFlags;
        }

        private void CloneTONEsList(bool selectFirst)
        {
            CloneTONElist_dataGridView.Rows.Clear();
            int index = 0;
            foreach (TONEinfo t in clonePackTONEs)
            {
                int ind = CloneTONElist_dataGridView.Rows.Add();
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                if (t.ColorList.Length > 0) style.BackColor = t.ColorList[0];
                CloneTONElist_dataGridView.Rows[ind].Cells["TONEorder"].Value = t.SortOrder.ToString().PadLeft(3);
                CloneTONElist_dataGridView.Rows[ind].Cells["TONEcolor"].Style = style;
                CloneTONElist_dataGridView.Rows[ind].Cells[3].Value = t.selected;
                CloneTONElist_dataGridView.Rows[ind].Tag = index;
                index++;
            }
            CloneTONElist_dataGridView.Sort(CloneTONElist_dataGridView.Columns["TONEorder"], ListSortDirection.Ascending);
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                CloneTONElist_dataGridView.Rows[i].Cells[0].Selected = false;
            }
            if (selectFirst)
            {
                myRow = 0;
                myTONEindex = (int)CloneTONElist_dataGridView.Rows[0].Tag;
                myTONE = clonePackTONEs[myTONEindex];
                myKey = clonePackTONEs[myTONEindex].resourceEntry;
            }
            else
            {
                myTONEindex = (int)CloneTONElist_dataGridView.Rows[myRow].Tag;
                myTONE = clonePackTONEs[myTONEindex];
                myKey = clonePackTONEs[myTONEindex].resourceEntry;
            }
            CloneTONElist_dataGridView.Rows[myRow].Cells[0].Selected = true;
            CloneTONEdisplay();
        }

        private void CloneTONElist_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == 3)
            {
                return;
            }
            else
            {
                if (myTONEindex != (int)CloneTONElist_dataGridView.Rows[e.RowIndex].Tag)
                {
                    if (changesTone)
                    {
                        DialogResult res = MessageBox.Show("Any uncommitted changes will be lost if you move to another Skintone!",
                                                            "Unsaved Changes", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.Cancel)
                        {
                            CloneTONElist_dataGridView.Rows[e.RowIndex].Cells[0].Selected = false;
                            CloneTONElist_dataGridView.Rows[myRow].Cells[0].Selected = true;
                            return;
                        }
                    }
                    myTONEindex = (int)CloneTONElist_dataGridView.Rows[e.RowIndex].Tag;
                    myTONE = clonePackTONEs[myTONEindex];
                    myKey = clonePackTONEs[myTONEindex].resourceEntry;
                    myRow = e.RowIndex;
                    CloneTONElist_dataGridView.Rows[myRow].Cells[0].Selected = true;
                    CloneTONEdisplay();
                    changesTone = false;
                }

                if (e.ColumnIndex == CloneTONElist_dataGridView.Columns["TONEdelete"].Index)
                {
                    if (clonePackTONEs.Count <= 1)
                    {
                        MessageBox.Show("You can't delete all the skintones in the package!");
                        return;
                    }
                    DialogResult res = MessageBox.Show("Okay to permanently delete this skintone? This cannot be undone!", "Delete Recolor", MessageBoxButtons.OKCancel);
                    if (res == DialogResult.OK)
                    {
                        foreach (TONEinfo.SkinStates set in myTONE.skinStates)
                        {
                            if (set.skinCloned)
                            {
                                TGI tex = new TGI((uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0U, set.TextureInstance);
                                DeleteResource(clonePack, tex);
                                tex = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0U, set.TextureInstance);
                                DeleteResource(clonePack, tex);
                                tex = new TGI((uint)XmodsEnums.ResourceTypes.LRLE, 0U, set.TextureInstance);
                                DeleteResource(clonePack, tex);
                            }
                            if (set.maskCloned)
                            {
                                TGI tex = new TGI((uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0U, set.OverlayInstance);
                                DeleteResource(clonePack, tex);
                                tex = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0U, set.OverlayInstance);
                                DeleteResource(clonePack, tex);
                            }
                        }
                        DeleteResource(clonePack, myKey);
                        clonePackTONEs.RemoveAt(myTONEindex);
                        myRow = Math.Min(myRow, CloneTONElist_dataGridView.Rows.Count - 2);
                        CloneTONEsList(false);
                    }
                    changesTone = false;
                }
            }
        }

        private void CloneTONElist_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 3)
            {
                DataGridViewCheckBoxCell check = (DataGridViewCheckBoxCell)CloneTONElist_dataGridView.Rows[e.RowIndex].Cells[3];
                if ((bool)check.EditedFormattedValue)
                {
                    CloneTONElist_dataGridView.Rows[e.RowIndex].Cells[3].Value = "T";
                    clonePackTONEs[(int)CloneTONElist_dataGridView.Rows[e.RowIndex].Tag].selected = "T";
                }
                else
                {
                    CloneTONElist_dataGridView.Rows[e.RowIndex].Cells[3].Value = "F";
                    clonePackTONEs[(int)CloneTONElist_dataGridView.Rows[e.RowIndex].Tag].selected = "F";
                }
                CloneTONElist_dataGridView.Rows[e.RowIndex].Cells[0].Selected = false;
                CloneTONElist_dataGridView.Rows[myRow].Cells[0].Selected = true;
            }
        }

        private void CloneTONEdisplay()
        {
            cloneSkinState_comboBox.SelectedIndexChanged -= cloneSkinState_comboBox_SelectedIndexChanged;
            GetSkinStates(myTONE);
            cloneWait_label.Visible = true;
            cloneWait_label.Refresh();
            CloneProp_dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(CloneFlags_dataGridView_CellValueChanged);
            CloneSortOrder.TextChanged -= CloneColorSortOrder_TextChanged;
            cloneHue_value.TextChanged -= cloneHue_value_TextChanged;
            cloneHue_saturation.TextChanged -= cloneHue_saturation_TextChanged;
            ClonePropVerson.Text = myTONE.Version.ToString();
            List<uint[]> catFlags = myTONE.CategoryTags;
            CloneProp_dataGridView.Rows.Clear();
            ListTONEflags(CloneProp_dataGridView, catFlags, tagCategoryNamesSkin, tagCategoryNumbersSkin);
            cloneSkinState_comboBox.SelectedIndex = 0;

            SetupImageDisplay(CloneTexture_pictureBox, myTONE.skinStates[0].skinImage);
            CloneTexture_pictureBox.Refresh();
            SetupImageDisplay(CloneBurnMask_pictureBox, myTONE.skinStates[0].maskImage);
            CloneBurnMask_pictureBox.Refresh();
            cloneBurnMultiplier.Text = myTONE.skinStates[0].OverlayMultiplier.ToString();
            cloneMakeupOpacity1.Text = myTONE.skinStates[0].MakeupOpacity.ToString();
            cloneMakeupOpacity2.Text = myTONE.skinStates[0].MakeupOpacity2.ToString();
            cloneBurnMultiplier.Tag = new float[] { myTONE.skinStates[0].OverlayMultiplier, myTONE.skinStates[1].OverlayMultiplier, myTONE.skinStates[2].OverlayMultiplier };
            cloneMakeupOpacity1.Tag = new float[] { myTONE.skinStates[0].MakeupOpacity, myTONE.skinStates[1].MakeupOpacity, myTONE.skinStates[2].MakeupOpacity };
            cloneMakeupOpacity2.Tag = new float[] { myTONE.skinStates[0].MakeupOpacity2, myTONE.skinStates[1].MakeupOpacity2, myTONE.skinStates[2].MakeupOpacity2 };
            mySkinState = 0;

            SetupOverlays();
            CloneSortOrder.Text = myTONE.SortOrder.ToString();
            byte[] rgb = GetRGB(myTONE.Hue, myTONE.Saturation, 128);
            cloneHue_pictureBox.BackColor = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            cloneHue_value.Text = myTONE.Hue.ToString();
            cloneHue_saturation.Text = myTONE.Saturation.ToString();
            clonePass2Opacity.Text = myTONE.Opacity.ToString();
            if (myTONE.Version >= 8)
            {
                cloneTuningRef_comboBox.Enabled = true;
                int ind = skintoneTuning.IndexOf(myTONE.TuningInstance);
                if (ind >= 0)
                {
                    cloneTuningRef_comboBox.SelectedIndex = ind;
                }
                else
                {
                    cloneTuningRef_comboBox.SelectedIndex = tuningHuman;
                }
            }
            else
            {
                cloneTuningRef_comboBox.Enabled = false;
            }
            if (myTONE.Version >= 11)
            {
                ClonePanel_comboBox.Enabled = true;
                CloneSlider_groupBox.Enabled = true;
                ClonePanel_comboBox.SelectedIndex = (ushort)myTONE.SkinType - 1;
                CloneSliderLow.Text = myTONE.SliderMinimum.ToString();
                CloneSliderHigh.Text = myTONE.SliderMaximum.ToString();
                CloneSliderIncrement.Text = myTONE.SliderIncrement.ToString();
            }
            else
            {
                ClonePanel_comboBox.Enabled = false;
                CloneSlider_groupBox.Enabled = false;
            }
            if (myTONE.Version >= 12)
            {
                CloneUnknown0.Enabled = true;
                CloneUnknown1.Enabled = true;
                CloneUnknown2.Enabled = true;
                CloneUnknown0.Text = myTONE.Unknown[0].ToString();
                CloneUnknown1.Text = myTONE.Unknown[1].ToString();
                CloneUnknown2.Text = myTONE.Unknown[2].ToString();
            }
            else
            {
                CloneUnknown0.Enabled = false;
                CloneUnknown1.Enabled = false;
                CloneUnknown2.Enabled = false;
            }
            if (myTONE.ColorList.Length > 0)
            {
                CloneColor_pictureBox.BackColor = myTONE.ColorList[0];
                previewSkinColor_pictureBox.BackColor = myTONE.ColorList[0];
            }
            else
            {
                CloneColor_pictureBox.BackColor = Color.Transparent;
                previewSkinColor_pictureBox.BackColor = Color.Transparent;
            }
            CloneProp_dataGridView.CellValueChanged += new DataGridViewCellEventHandler(CloneFlags_dataGridView_CellValueChanged);
            CloneSortOrder.TextChanged += CloneColorSortOrder_TextChanged;
            cloneHue_value.TextChanged += cloneHue_value_TextChanged;
            cloneHue_saturation.TextChanged += cloneHue_saturation_TextChanged;
            StartPreview();
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
            changesTone = false;
            cloneSkinState_comboBox.SelectedIndexChanged += cloneSkinState_comboBox_SelectedIndexChanged;
            bool gotSkinStates = myTONE.Version >= 10;
            cloneSkinState_comboBox.Enabled = gotSkinStates;
            cloneBurnMultiplier.Enabled = gotSkinStates;
            Texture2_label.Visible = gotSkinStates;
            CloneBurnMask_pictureBox.Visible = gotSkinStates;
        }

        private void CloneColorAddFlag_button_Click(object sender, EventArgs e)                                                                                                             
        {
            CloneAddFlag_button_Click(CloneProp_dataGridView, e, false);
            changesTone = true;
        }

        private void CloneColorProp_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CloneFlags_dataGridView_CellContentClick(sender, e);
        }

        private void CloneTONEFlags_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CloneFlags_dataGridView_CellValueChanged(sender, e);
        }

        private void CloneAddTONE_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Make a copy of the currently selected Skintone?", "Copy Properties Prompt", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Cancel) return;
            TONEinfo newTONE = null;
            if (res == DialogResult.Yes)        //create cloned TONE and clone the textures
            {
                Stream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms);
                myTONE.Write(bw);
                ms.Position = 0;
                BinaryReader br = new BinaryReader(ms);
                newTONE = new TONEinfo(br, null, "F");
                ms.Dispose();

                for (int i = 0; i < newTONE.SkinSets.Length; i++)
                {
                    if (newTONE.SkinSets[i] != null)
                    {
                        if (newTONE.SkinSets[i].TextureInstance > 0ul)
                        {
                            ulong instance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                            newTONE.SetSkinSetTextureInstance(i, instance);
                            //IResourceKey ikDDS = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0x00000000, newTONE.skinStates[i].TextureInstance);
                            //IResourceKey ikRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000, newTONE.skinStates[i].TextureInstance);
                            IResourceKey ikLRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0x00000000, instance);
                            LRLE skin = FindCloneTexture(myTONE.SkinSets[i].TextureInstance);
                            if (skin != null)
                            {
                                IResourceIndexEntry iresLRLE = clonePack.AddResource(ikLRLE, skin.Stream, true);
                                iresLRLE.Compressed = (ushort)0x5A42;
                               // newTONE.skinStates[i].skinTexture = skin;
                            }
                            //if (skin != null)
                            //{
                            //    Stream s = new MemoryStream();
                            //    skin.Save(s);
                            //    s.Position = 0;
                            //    IResourceIndexEntry iresDDS = clonePack.AddResource(ikDDS, s, true);
                            //    iresDDS.Compressed = (ushort)0x5A42;
                            //    Stream s2 = new MemoryStream();
                            //    s.CopyTo(s2);
                            //    DdsFile dds2 = new DdsFile();
                            //    dds2.Load(s2, false);
                            //    newTONE.skinStates[i].skinTexture = dds2;
                            //}
                            //RLEResource rle = FindCloneTextureRLE(myTONE.skinStates[i].TextureInstance);
                            //if (rle != null)
                            //{
                            //    IResourceIndexEntry iresRLE = clonePack.AddResource(ikRLE, rle.Stream, true);
                            //    iresRLE.Compressed = (ushort)0x5A42;
                            //}
                            //if (skin != null) newTONE.skinStates[i].skinCloned = true;
                            //newTONE.skinStates[i].skinImported = false;
                        }
                        if (newTONE.SkinSets[i].OverlayInstance > 0ul)
                        {
                            ulong instance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                            newTONE.SetSkinSetOverlayInstance(i, instance);
                           // IResourceKey ikDDS = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0x00000000, newTONE.skinStates[i].OverlayInstance);
                            IResourceKey ikRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000, instance);
                            //DdsFile dds = FindCloneTextureDDS(myTONE.skinStates[i].OverlayInstance);
                            //if (dds != null)
                            //{
                            //    Stream s = new MemoryStream();
                            //    dds.Save(s);
                            //    s.Position = 0;
                            //    IResourceIndexEntry iresDDS = clonePack.AddResource(ikDDS, s, true);
                            //    iresDDS.Compressed = (ushort)0x5A42;
                            //    Stream s2 = new MemoryStream();
                            //    s.CopyTo(s2);
                            //    DdsFile dds2 = new DdsFile();
                            //    dds2.Load(s2, false);
                            //    newTONE.skinStates[i].maskTexture = dds2;
                            //}
                            RLEResource rle = FindMaskTexture(myTONE.SkinSets[i].OverlayInstance);
                            if (rle != null)
                            {
                                IResourceIndexEntry iresRLE = clonePack.AddResource(ikRLE, rle.Stream, true);
                                iresRLE.Compressed = (ushort)0x5A42;
                            }
                            //newTONE.skinStates[i].maskCloned = true;
                            //newTONE.skinStates[i].maskImported = false;
                        }
                    }
                }
            }
            else        //create a blank TONE with only a base skin texture template
            {
                newTONE = new TONEinfo(myTONE != null ? myTONE.Version : latestVersion);
                newTONE.selected = "F";
                List<uint[]> tmp = new List<uint[]>();
                int ind = tagNames.IndexOf("Occult_Human");
                tmp.Add(new uint[] { (uint)XmodsEnums.CatFlagsSkintone.Occult, tagNumbers[ind] });
                newTONE.CategoryTags = tmp;
                newTONE.TuningInstance = skintoneTuning[tuningHuman];
                newTONE.ColorList = new Color[] { Color.LightGray };

                Stream s = new MemoryStream(Properties.Resources.SkinColorTemplateDDS);
                s.Position = 0;
                newTONE.SetSkinSetTextureInstance(0, ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000);
               // IResourceKey ikDDS = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0x00000000, newTONE.GetSkinSetTextureInstance(0));
                IResourceKey ikLRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0x00000000, newTONE.GetSkinSetTextureInstance(0));
               // IResourceIndexEntry iresTextureDDS = clonePack.AddResource(ikDDS, s, true);
               // iresTextureDDS.Compressed = (ushort)0x5A42;
                DdsFile dds = new DdsFile();
                dds.Load(s, false);
                LRLE myTextureLRLE = new LRLE(dds.Image);
                IResourceIndexEntry iresTextureLRLE = clonePack.AddResource(ikLRLE, myTextureLRLE.Stream, true);
                iresTextureLRLE.Compressed = (ushort)0x5A42;
                //RLEResource myTextureRLE = new RLEResource(1, null);
                //myTextureRLE.ImportToRLE(new MemoryStream(Properties.Resources.SkinColorTemplateDDS));
                //IResourceIndexEntry iresTextureRLE = clonePack.AddResource(ikRLE, myTextureRLE.Stream, true);
                //iresTextureRLE.Compressed = (ushort)0x5A42;
            }

            MemoryStream ms2 = new MemoryStream();
            BinaryWriter bw2 = new BinaryWriter(ms2);
            newTONE.Write(bw2);
            ms2.Position = 0;
            IResourceKey rk = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.TONE, 0U, (uint)ran.Next());
            myKey = clonePack.AddResource(rk, ms2, true);
            newTONE.resourceEntry = myKey;
            clonePackTONEs.Add(newTONE);
            myTONE = newTONE;
            int j = CloneTONElist_dataGridView.Rows.Add(new object[] { newTONE.SortOrder.ToString().PadLeft(3) });
            CloneTONElist_dataGridView.Rows[j].Tag = j;
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            if (myTONE.ColorList.Length > 0) style.BackColor = myTONE.ColorList[0];
            CloneTONElist_dataGridView.Rows[j].Cells["TONEcolor"].Style = style;
            CloneTONElist_dataGridView.Rows[myRow].Selected = false;
            CloneTONElist_dataGridView.Rows[j].Cells[0].Selected = true;
            myRow = j;
            myTONEindex = clonePackTONEs.Count - 1;
            CloneTONEdisplay();
        }

        private void CloneColor_pictureBox_Click(object sender, EventArgs e)
        {
            Color_pictureBox_Click(sender, e);
        }

        private void CloneHue_pictureBox_Click(object sender, EventArgs e)
        {
            Color_pictureBox_Click(sender, e);
            ushort[] hsl = GetHSL(cloneHue_pictureBox.BackColor.R, cloneHue_pictureBox.BackColor.G, cloneHue_pictureBox.BackColor.B);
            cloneHue_value.TextChanged -= cloneHue_value_TextChanged;
            cloneHue_saturation.TextChanged -= cloneHue_saturation_TextChanged;
            cloneHue_value.Text = hsl[0].ToString();
            cloneHue_saturation.Text = hsl[1].ToString();
            cloneHue_value.TextChanged += cloneHue_value_TextChanged;
            cloneHue_saturation.TextChanged += cloneHue_saturation_TextChanged;
           // MessageBox.Show(cloneHue_pictureBox.BackColor.GetHue().ToString() + ", " + cloneHue_pictureBox.BackColor.GetSaturation().ToString());
           // cloneHue_value.Text = cloneHue_pictureBox.BackColor.GetHue().ToString();
           // cloneHue_saturation.Text = (cloneHue_pictureBox.BackColor.GetSaturation() * 240).ToString();
        }

        private void Color_pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clonePbox = sender as PictureBox;
            ColorDialog cd = new ColorDialog();
            cd.FullOpen = true;
            cd.Color = clonePbox.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                clonePbox.BackColor = cd.Color;
                changesTone = true;
            }
        }

        private void cloneHue_value_TextChanged(object sender, EventArgs e)
        {
            UpdateCloneOverlaidColor();
        }

        private void cloneHue_saturation_TextChanged(object sender, EventArgs e)
        {
            UpdateCloneOverlaidColor();
        }

        private void UpdateCloneOverlaidColor()
        {
            ushort hue, sat;
            try { hue = ushort.Parse(cloneHue_value.Text); }
            catch { MessageBox.Show("Please enter a valid number in the Overlaid Color Hue box!"); return; }
            try { sat = ushort.Parse(cloneHue_saturation.Text); }
            catch { MessageBox.Show("Please enter a valid number in the Overlaid Color Saturation box!"); return; }
            byte[] rgb = GetRGB(hue, sat, 128);
            cloneHue_pictureBox.BackColor = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            changesTone = true;
        }
        
        private void CloneColorSortOrder_TextChanged(object sender, EventArgs e)
        {
            changesTone = true;
        }

        private void cloneHueOpacity_TextChanged(object sender, EventArgs e)
        {
            uint hue;
            bool hueParse = uint.TryParse(clonePass2Opacity.Text, out hue);
            if (hueParse && hue > 100) clonePass2Opacity.Text = "100";
            changesTone = true;
        }

        private void cloneMakeupOpacity1_TextChanged(object sender, EventArgs e)
        {
            float opacity;
            bool opacityParse = float.TryParse(cloneMakeupOpacity1.Text, out opacity);
            if (opacityParse)
            {
                if (opacity > 1f) cloneMakeupOpacity1.Text = "1";
                if (opacity < 0f) cloneMakeupOpacity1.Text = "0";
            }
            changesTone = true;
        }

        private void cloneMakeupOpacity2_TextChanged(object sender, EventArgs e)
        {
            float opacity;
            bool opacityParse = float.TryParse(cloneMakeupOpacity2.Text, out opacity);
            if (opacityParse)
            {
                if (opacity > 1f) cloneMakeupOpacity2.Text = "1";
                if (opacity < 0f) cloneMakeupOpacity2.Text = "0";
            }
            changesTone = true;
        }

        private void cloneTuningRef_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            changesTone = true;
        }

        private void CloneCommit_button_Click(object sender, EventArgs e)
        {
            if (myTONE == null) return;
            if (!CheckSizes()) return;

            try { myTONE.SortOrder = float.Parse(CloneSortOrder.Text); }
            catch { MessageBox.Show("Please enter a valid number in the Sort Order box!"); return; }
            try { myTONE.Opacity = uint.Parse(clonePass2Opacity.Text); }
            catch { MessageBox.Show("Please enter a valid number in the Second Pass Opacity box!"); return; }
            try { myTONE.Hue = ushort.Parse(cloneHue_value.Text); }
            catch { MessageBox.Show("Please enter a valid number in the Overlaid Color Hue box!"); return; }
            try { myTONE.Saturation = ushort.Parse(cloneHue_saturation.Text); }
            catch { MessageBox.Show("Please enter a valid number in the Overlaid Color Saturation box!"); return; }
            float[] burnMultipliers = cloneBurnMultiplier.Tag as float[];
            float[] makeupOpacitys = cloneMakeupOpacity1.Tag as float[];
            float[] makeupOpacity2s = cloneMakeupOpacity2.Tag as float[];
            int index = cloneSkinState_comboBox.SelectedIndex;
            try { burnMultipliers[index] = float.Parse(cloneBurnMultiplier.Text); }
            catch { MessageBox.Show("Please enter a valid number in the " + skinStateNames[index] + " Burn Multiplier box!"); return; }
            try { makeupOpacitys[index] = float.Parse(cloneMakeupOpacity1.Text); }
            catch { MessageBox.Show("Please enter a valid number in the " + skinStateNames[index] + " Makeup Opacity box!"); return; }
            try { makeupOpacity2s[index] = float.Parse(cloneMakeupOpacity2.Text); }
            catch { MessageBox.Show("Please enter a valid number in the " + skinStateNames[index] + " Special Makeup Opacity box!"); return; }
            try { myTONE.SliderMinimum = float.Parse(CloneSliderLow.Text); }
            catch { MessageBox.Show("Please enter a valid number in the slider low value box!"); return; }
            try { myTONE.SliderMaximum = float.Parse(CloneSliderHigh.Text); }
            catch { MessageBox.Show("Please enter a valid number in the slider high value box!"); return; }
            try { myTONE.SliderIncrement = float.Parse(CloneSliderIncrement.Text); }
            catch { MessageBox.Show("Please enter a valid number in the slider increment value box!"); return; }

            myTONE.SkinType = (TONE.SkinPanel)(ClonePanel_comboBox.SelectedIndex + 1);
            List<uint[]> flags = UpdatePropertyFlags(myTONE.CategoryTags, ReadPropertyFlags(CloneProp_dataGridView), false);
            myTONE.CategoryTags = flags;
            CloneTONElist_dataGridView.Rows[myRow].Cells["TONEorder"].Value = myTONE.SortOrder.ToString().PadLeft(3);
            myTONE.ColorList = new Color[] { CloneColor_pictureBox.BackColor };
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = CloneColor_pictureBox.BackColor;
            CloneTONElist_dataGridView.Rows[myRow].Cells["TONEcolor"].Style = style;
            previewSkinColor_pictureBox.BackColor = CloneColor_pictureBox.BackColor;
            if (myTONE.Version >= 8) myTONE.TuningInstance = skintoneTuning[cloneTuningRef_comboBox.SelectedIndex];
            if (myTONE.Version >= 12)
            {
                try
                {
                    myTONE.Unknown[0] = float.Parse(CloneUnknown0.Text);
                    myTONE.Unknown[1] = float.Parse(CloneUnknown1.Text);
                    myTONE.Unknown[2] = float.Parse(CloneUnknown2.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid number in the slider increment value box!"); return;
                }
            }

            for (int i = 0; i < myTONE.skinStates.Length; i++)
            {
                if (myTONE.skinStates[i] == null) continue;
                if (myTONE.skinStates[i].skinImported)
                {
                    ulong textureInstance = myTONE.skinStates[i].TextureInstance;
                    //IResourceKey ikDDS = null;
                    IResourceKey ikLRLE = null;
                    if (myTONE.skinStates[i].skinTexture != null)
                    {
                        if (myTONE.skinStates[i].skinCloned && !TextureUsedElsewhere(myTONE.skinStates[i].TextureInstance))
                        {
                            //TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0u, myTONE.skinStates[i].TextureInstance);
                            //DeleteResource(clonePack, tgi);
                            //ikDDS = new TGIBlock(0, null, tgi.Type, tgi.Group, tgi.Instance);
                            //tgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0u, myTONE.skinStates[i].TextureInstance);
                            //DeleteResource(clonePack, tgi);
                            TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.LRLE, 0u, myTONE.skinStates[i].TextureInstance);
                            DeleteResource(clonePack, tgi);
                            ikLRLE = new TGIBlock(0, null, tgi.Type, tgi.Group, tgi.Instance);
                        }
                        else
                        {
                            textureInstance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                            myTONE.skinStates[i].TextureInstance = textureInstance;
                            //ikDDS = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0U, textureInstance);
                            ikLRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0U, textureInstance);
                        }
                        //Stream s = new MemoryStream();
                        //myTONE.skinStates[i].skinTexture.UseDXT = false;
                        //myTONE.skinStates[i].skinTexture.Save(s);
                        //s.Position = 0;
                        //IResourceIndexEntry iresTextureDDS = clonePack.AddResource(ikDDS, s, true);
                        //iresTextureDDS.Compressed = (ushort)0x5A42;
                        //Stream s2 = new MemoryStream();
                        //myTONE.skinStates[i].skinDDS.UseDXT = true;
                        //myTONE.skinStates[i].skinDDS.Save(s2);
                        //RLEResource skinRLE = new RLEResource(1, null);
                        //skinRLE.ImportToRLE(s2);
                        IResourceIndexEntry iresTextureLRLE = clonePack.AddResource(ikLRLE, myTONE.skinStates[i].skinTexture.Stream, true);
                        iresTextureLRLE.Compressed = (ushort)0x5A42;
                        myTONE.skinStates[i].skinCloned = true;
                    }
                    else
                    {
                        if (myTONE.skinStates[i].skinCloned && !TextureUsedElsewhere(myTONE.skinStates[i].TextureInstance))
                        {
                            //TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0u, myTONE.skinStates[i].TextureInstance);
                            //DeleteResource(clonePack, tgi);
                            //tgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0u, myTONE.skinStates[i].TextureInstance);
                            //DeleteResource(clonePack, tgi);
                            TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.LRLE, 0u, myTONE.skinStates[i].TextureInstance);
                            DeleteResource(clonePack, tgi);
                        }
                        myTONE.skinStates[i].TextureInstance = 0ul;
                        myTONE.skinStates[i].skinCloned = false;
                    }
                    myTONE.skinStates[i].skinImported = false;
               }
                if (myTONE.skinStates[i].maskImported)
                {
                    ulong maskInstance = myTONE.skinStates[i].OverlayInstance;
                    //IResourceKey ikDDS = null;
                    IResourceKey ikRLE = null;
                    if (myTONE.skinStates[i].maskTexture != null)
                    {
                        if (myTONE.skinStates[i].maskCloned && !TextureUsedElsewhere(myTONE.skinStates[i].OverlayInstance))
                        {
                            //TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0u, myTONE.skinStates[i].OverlayInstance);
                            //DeleteResource(clonePack, tgi);
                            //ikDDS = new TGIBlock(0, null, tgi.Type, tgi.Group, tgi.Instance);
                            TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0u, myTONE.skinStates[i].OverlayInstance);
                            DeleteResource(clonePack, tgi);
                            ikRLE = new TGIBlock(0, null, tgi.Type, tgi.Group, tgi.Instance);
                        }
                        else
                        {
                            maskInstance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                            myTONE.skinStates[i].OverlayInstance = maskInstance;
                            //ikDDS = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0U, maskInstance);
                            ikRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0U, maskInstance);
                        }
                        //Stream m = new MemoryStream();
                        //myTONE.skinStates[i].maskTexture.UseDXT = false;
                        //myTONE.skinStates[i].maskTexture.Save(m);
                        //m.Position = 0;
                        //IResourceIndexEntry iresMaskDDS = clonePack.AddResource(ikDDS, m, true);
                        //iresMaskDDS.Compressed = (ushort)0x5A42;
                        //Stream m2 = new MemoryStream();
                        //myTONE.skinStates[i].maskTexture.UseDXT = true;
                        //myTONE.skinStates[i].maskTexture.Save(m2);
                        //RLEResource maskRLE = new RLEResource(1, null);
                        //maskRLE.ImportToRLE(m2);
                        IResourceIndexEntry iresMaskRLE = clonePack.AddResource(ikRLE, myTONE.skinStates[i].maskTexture.Stream, true);
                        iresMaskRLE.Compressed = (ushort)0x5A42;
                        myTONE.skinStates[i].maskCloned = true;
                    }
                    else
                    {
                        if (myTONE.skinStates[i].maskCloned && !TextureUsedElsewhere(myTONE.skinStates[i].OverlayInstance))
                        {
                            //TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0u, myTONE.skinStates[i].OverlayInstance);
                            //DeleteResource(clonePack, tgi);
                            TGI tgi = new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0u, myTONE.skinStates[i].OverlayInstance);
                            DeleteResource(clonePack, tgi);
                        }
                        myTONE.skinStates[i].OverlayInstance = 0ul;
                        myTONE.skinStates[i].maskCloned = false;
                    }
                    myTONE.skinStates[i].maskImported = false;
                }
                myTONE.skinStates[i].OverlayMultiplier = burnMultipliers[i];
                myTONE.skinStates[i].MakeupOpacity = makeupOpacitys[i];
                myTONE.skinStates[i].MakeupOpacity2 = makeupOpacity2s[i];
            }
            myTONE.SkinSets = myTONE.skinStates;
            DeleteResource(clonePack, myKey);
            MemoryStream mem = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(mem);
            myTONE.Write(bw);
            mem.Position = 0;
            clonePack.AddResource(myKey, mem, true);
            changesTone = false;
            clonePackTONEs[myTONEindex] = myTONE;
            StartPreview();
        }

        private bool CheckSizes()
        {
            List<Size> sizes = new List<Size>();

            if ((myTONE.skinStates[0].skinCloned || myTONE.skinStates[0].skinImported) && myTONE.skinStates[0].skinTexture != null) sizes.Add(new Size(myTONE.skinStates[0].skinTexture.Width, myTONE.skinStates[0].skinTexture.Height));
            if ((myTONE.skinStates[1].skinCloned || myTONE.skinStates[1].skinImported) && myTONE.skinStates[1].skinTexture != null) sizes.Add(new Size(myTONE.skinStates[1].skinTexture.Width, myTONE.skinStates[1].skinTexture.Height));
            if ((myTONE.skinStates[2].skinCloned || myTONE.skinStates[2].skinImported) && myTONE.skinStates[2].skinTexture != null) sizes.Add(new Size(myTONE.skinStates[2].skinTexture.Width, myTONE.skinStates[2].skinTexture.Height));
            foreach (OverlayItem over in overlayList) if ((over.cloned || over.imported) && over.overlay != null) sizes.Add(new Size(over.overlay.Width, over.overlay.Height));
            for (int i = 0; i < sizes.Count - 1; i++)
            {
                if (!sizes[i].Equals(sizes[i + 1]))
                {
                    DialogResult res = MessageBox.Show("Not all custom textures, masks, and overlays for this TONE are the same size. Continue anyway?", "Texture size mismatch", MessageBoxButtons.OKCancel);
                    if (res == DialogResult.Cancel) return false;
                }
            }
            return true;
        }

        private bool TextureUsedElsewhere(ulong textureInstance)
        {
            for (int i = 0; i < clonePackTONEs.Count; i++)
            {
                for (int j = 0; j < clonePackTONEs[i].SkinSets.Length; j++)
                {
                    if (!(i == myTONEindex && j == mySkinState) && 
                        (textureInstance == clonePackTONEs[i].SkinSets[j].TextureInstance | textureInstance == clonePackTONEs[i].SkinSets[j].OverlayInstance))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CloneTONEDiscard_button_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to discard changes?", "Discard Changes", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel) return;
            myTONEindex = (int)CloneTONElist_dataGridView.Rows[myRow].Tag;
            myTONE = clonePackTONEs[myTONEindex];
            myKey = clonePackTONEs[myTONEindex].resourceEntry;
            GetSkinStates(myTONE);
            CloneTONEdisplay();
            changesTone = false;
        }

        private void DeleteCloneDups(ulong instance, XmodsEnums.ResourceTypes typeToKeep)
        {
            Predicate<IResourceIndexEntry> itex = r => r.Instance == instance;
            List<IResourceIndexEntry> ctex = clonePack.FindAll(itex);
            foreach (IResourceIndexEntry irie in ctex)
            {
                if (irie.ResourceType != (uint)typeToKeep) DeleteResource(clonePack, irie);
            }
        }

        private LRLE FindCloneTexture(ulong instance)
        {
            IResourceIndexEntry foundKey;
            bool inClonePack;
            return FindCloneTexture(instance, out foundKey, out inClonePack);
        }

        private LRLE FindCloneTexture(ulong instance, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (instance == 0)
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE &
                                    r.Instance == instance;
            IResourceIndexEntry ctex = clonePack.Find(itex);            //searches for LRLE, then LRLE disguised as RLE2, then DDS in cloned package
            if (ctex != null)
            {
                inClonePack = true;
                foundKey = ctex;
                LRLE tmp = new LRLE(new BinaryReader(clonePack.GetResource(ctex)));
                return tmp;
            }
            Predicate<IResourceIndexEntry> rtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 & r.Instance == instance;
            ctex = clonePack.Find(rtex);
            if (ctex != null)
            {
                try
                {
                    LRLE tmp = new LRLE(new BinaryReader(clonePack.GetResource(ctex)));
                    inClonePack = true;
                    foundKey = ctex;
                    return tmp;
                }
                catch { }
            }
            Predicate<IResourceIndexEntry> dtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed & r.Instance == instance;
            ctex = clonePack.Find(dtex);
            if (ctex != null)
            {
                Stream s = clonePack.GetResource(ctex);
                if (s.Length > 0)
                {
                    inClonePack = true;
                    foundKey = ctex;
                    DdsFile tmp = new DdsFile();
                    tmp.Load(s, false);
                    return new LRLE(tmp.Image);
                }
                else
                {
                    inClonePack = false;
                    foundKey = null;
                    return null;
                }
            }

            foreach (Package p in gamePacksOther)                   //assumes game textures are LRLE
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    LRLE tmp = new LRLE(new BinaryReader(p.GetResource(ptex)));
                    return tmp;
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    LRLE tmp = new LRLE(new BinaryReader(p.GetResource(ptex)));
                    return tmp;
                }
            }
            inClonePack = false;
            foundKey = null;
            return null;
        }

        private RLEResource FindMaskTexture(ulong instance)
        {
            IResourceIndexEntry foundKey;
            bool inClonePack;
            return FindMaskTexture(instance, out foundKey, out inClonePack);
        }

        private RLEResource FindMaskTexture(ulong instance, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (instance == 0)
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &
                                    r.Instance == instance;
            IResourceIndexEntry ctex = clonePack.Find(itex);            //searches for RLE2, then DDS in cloned package
            if (ctex != null)
            {
                Stream s = clonePack.GetResource(ctex);
                if (s.Length > 0)
                {
                    inClonePack = true;
                    foundKey = ctex;
                    RLEResource tmp = new RLEResource(1, s);
                    return tmp;
                }
                else
                {
                    inClonePack = false;
                    foundKey = null;
                    return null;
                }
            }
            Predicate<IResourceIndexEntry> dtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed & r.Instance == instance;
            ctex = clonePack.Find(dtex);
            if (ctex != null)
            {
                Stream s = clonePack.GetResource(ctex);
                if (s.Length > 0)
                {
                    inClonePack = true;
                    foundKey = ctex;
                    DdsFile dds = new DdsFile();
                    dds.Load(s, false);
                    Stream ms = new MemoryStream();
                    dds.Save(ms);
                    ms.Position = 0;
                    RLEResource tmp = new RLEResource(1, null);
                    tmp.ImportToRLE(ms);
                    return tmp;
                }
                else
                {
                    inClonePack = false;
                    foundKey = null;
                    return null;
                }
            }

            foreach (Package p in gamePacksOther)                   //assumes game textures are RLE2
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0)
                    {
                        inClonePack = false;
                        foundKey = ptex;
                        RLEResource tmp = new RLEResource(1, s);
                        return tmp;
                    }
                    else
                    {
                        inClonePack = false;
                        foundKey = null;
                        return null;
                    }
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0)
                    {
                        inClonePack = false;
                        foundKey = ptex;
                        RLEResource tmp = new RLEResource(1, s);
                        return tmp;
                    }
                    else
                    {
                        inClonePack = false;
                        foundKey = null;
                        return null;
                    }
                }
            }
            inClonePack = false;
            foundKey = null;
            return null;
        }

        private RLEResource FindOverlayTexture(ulong instance)
        {
            IResourceIndexEntry foundKey;
            bool inClonePack;
            return FindOverlayTexture(instance, out foundKey, out inClonePack);
        }

        private RLEResource FindOverlayTexture(ulong instance, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (instance == 0)
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &
                                    r.Instance == instance;
            IResourceIndexEntry ctex = clonePack.Find(itex);            //searches for RLE2, then DDS in cloned package
            if (ctex != null)
            {
                Stream s = clonePack.GetResource(ctex);
                if (s.Length > 0)
                {
                    inClonePack = true;
                    foundKey = ctex;
                    RLEResource tmp = new RLEResource(1, s);
                    return tmp;
                }
                else
                {
                    inClonePack = false;
                    foundKey = null;
                    return null;
                }
            }
            Predicate<IResourceIndexEntry> dtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed & r.Instance == instance;
            ctex = clonePack.Find(dtex);
            if (ctex != null)
            {
                Stream s = clonePack.GetResource(ctex);
                if (s.Length > 0)
                {
                    inClonePack = true;
                    foundKey = ctex;
                    DdsFile tmp = new DdsFile();
                    tmp.Load(s, false);
                    tmp.UseDXT = true;
                    Stream s2 = new MemoryStream();
                    tmp.Save(s2);
                    RLEResource rle = new RLEResource(1, null);
                    rle.ImportToRLE(s2);
                    return rle;
                }
                else
                {
                    inClonePack = false;
                    foundKey = null;
                    return null;
                }
            }

            foreach (Package p in gamePacksOther)                   //assumes game textures are RLE2
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0)
                    {
                        inClonePack = false;
                        foundKey = ptex;
                        RLEResource tmp = new RLEResource(1, s);
                        return tmp;
                    }
                    else
                    {
                        inClonePack = false;
                        foundKey = null;
                        return null;
                    }
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    Stream s = p.GetResource(ptex);
                    if (s.Length > 0)
                    {
                        inClonePack = false;
                        foundKey = ptex;
                        RLEResource tmp = new RLEResource(1, s);
                        return tmp;
                    }
                    else
                    {
                        inClonePack = false;
                        foundKey = null;
                        return null;
                    }
                }
            }
            inClonePack = false;
            foundKey = null;
            return null;
        }

        private DdsFile FindCloneTextureDDS(ulong instance)
        {
            IResourceIndexEntry foundKey;
            bool inClonePack;
            return FindCloneTextureDDS(instance, out foundKey, out inClonePack);
        }

        private DdsFile FindCloneTextureDDS(ulong instance, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (instance == 0)
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &
                                    r.Instance == instance; 
            IResourceIndexEntry ctex = clonePack.Find(itex);
            if (ctex != null)
            {
                inClonePack = true;
                foundKey = ctex;
                DdsFile tmp = new DdsFile();
                tmp.Load(clonePack.GetResource(ctex), false);
                return tmp;
               // return (MemoryStream)clonePack.GetResource(ctex);
            }
            foreach (Package p in gamePacksOther)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    DdsFile tmp = new DdsFile();
                    tmp.Load(p.GetResource(ptex), false);
                    return tmp;
                   // return (MemoryStream)p.GetResource(ptex);
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    DdsFile tmp = new DdsFile();
                    tmp.Load(p.GetResource(ptex), false);
                    return tmp;
                   // return (MemoryStream)p.GetResource(ptex);
                }
            }
            inClonePack = false;
            foundKey = null;
            return null;
        }

        private RLEResource FindCloneTextureRLE(ulong instance)
        {
            IResourceIndexEntry foundKey;
            bool inClonePack;
            return FindCloneTextureRLE(instance, out foundKey, out inClonePack);
        }

        private RLEResource FindCloneTextureRLE(ulong instance, out IResourceIndexEntry foundKey, out bool inClonePack)
        {
            if (instance == 0)
            {
                inClonePack = false;
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &
                                    r.Instance == instance;
            IResourceIndexEntry ctex = clonePack.Find(itex);
            if (ctex != null)
            {
                inClonePack = true;
                foundKey = ctex;
                return new RLEResource(1, clonePack.GetResource(ctex));
            }
            foreach (Package p in gamePacksOther)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    return new RLEResource(1, p.GetResource(ptex));
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    inClonePack = false;
                    foundKey = ptex;
                    return new RLEResource(1, p.GetResource(ptex));
                }
            }
            inClonePack = false;
            foundKey = null;
            return null;
        }

        private void CloneTexture_pictureBox_Click(object sender, EventArgs e)
        {
            int index = cloneSkinState_comboBox.SelectedIndex;
            LRLE lrle;
            if (myTONE.skinStates[index].skinTexture != null) lrle = myTONE.skinStates[index].skinTexture;
            else lrle = FindCloneTexture(myTONE.skinStates[index].TextureInstance);
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(lrle, ImageType.TextureLRLE, "Import/Export Skintone Texture");
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                LRLE tmp = null;
                if (imgDisplay.ReturnIMG != null)
                {
                    tmp = new LRLE(imgDisplay.ReturnIMG);
                }
                else if (imgDisplay.ReturnDDS != null)
                {
                    tmp = new LRLE(imgDisplay.ReturnDDS.Image);
                }
                myTONE.skinStates[index].skinTexture = tmp;
                SetupImageDisplay(CloneTexture_pictureBox, myTONE.skinStates[index].skinImage);
                CloneTexture_pictureBox.Refresh();
                myTONE.skinStates[index].skinImported = true;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesTone = true;
            }
        }
        private void CloneMask_pictureBox_Click(object sender, EventArgs e)
        {
            int index = cloneSkinState_comboBox.SelectedIndex;
            if (index == 0)
            {
                DialogResult res1 = MessageBox.Show("The burn mask is not used for the Normal skin state. Continue anyway?", "Unused texture", MessageBoxButtons.OKCancel);
                if (res1 == DialogResult.Cancel) return;
            }
            IResourceIndexEntry ires;
            bool cloned;
            RLEResource rle = null;
            if (myTONE.skinStates[index].maskTexture != null) rle = myTONE.skinStates[index].maskTexture;
            else rle = FindMaskTexture(myTONE.skinStates[index].OverlayInstance, out ires, out cloned);
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(rle, ImageType.TextureRLE, "Import/Export BurnMask Texture");
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                RLEResource tmp = null;
                if (imgDisplay.ReturnIMG != null)
                {
                    DdsFile tmp2 = new DdsFile();
                    tmp2.CreateImage(imgDisplay.ReturnIMG, false);
                    tmp2.UseDXT = true;
                    tmp2.GenerateMipMaps();
                    Stream s = new MemoryStream();
                    tmp2.Save(s);
                    s.Position = 0;
                    tmp = new RLEResource(1, null);
                    tmp.ImportToRLE(s);
                }
                else if (imgDisplay.ReturnDDS != null)
                {
                    DdsFile tmp2 = imgDisplay.ReturnDDS;
                    tmp2.UseDXT = true;
                    if (tmp2.MipMaps <= 1) tmp2.GenerateMipMaps();
                    Stream s = new MemoryStream();
                    tmp2.Save(s);
                    s.Position = 0;
                    tmp = new RLEResource(1, null);
                    tmp.ImportToRLE(s);
                }
                myTONE.skinStates[index].maskTexture = tmp;
                SetupImageDisplay(CloneBurnMask_pictureBox, myTONE.skinStates[index].maskImage);
                CloneBurnMask_pictureBox.Refresh();
                myTONE.skinStates[index].maskImported = true;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesTone = true;
            }
        }

        private void SetupImageDisplay(PictureBox pictureBox, Image image)
        {
            pictureBox.Image = image != null ? image : Properties.Resources.NullImage;
            pictureBox.BackgroundImage = image != null ? Properties.Resources.Transparency : null;
        }

        private static void SortCategories(string[] catNameList, uint[] catValueList)
        {
            for (int i = catNameList.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (string.Compare(catNameList[j], catNameList[j + 1]) > 0)
                    {
                        string tmp = catNameList[j];
                        catNameList[j] = catNameList[j + 1];
                        catNameList[j + 1] = tmp;
                        uint stmp = catValueList[j];
                        catValueList[j] = catValueList[j + 1];
                        catValueList[j + 1] = stmp;
                    }
                }
            }
        }

        private void ClonePackageWipe()
        {
            myTONE = null;
            myKey = null;
            myRow = 0;
            myTONEindex = 0;
            clonePackTONEs.Clear();
            List<ushort[]> caspFlags = new List<ushort[]>();
            CloneTONElist_dataGridView.Rows.Clear();
            List<ushort[]> catFlags = new List<ushort[]>();
            CloneProp_dataGridView.Rows.Clear();
            CloneTexture_pictureBox.Image = null;
            CloneBurnMask_pictureBox.Image = null;
            CloneSortOrder.Text = "";
            CloneColor_pictureBox.BackColor = Color.White;
            cloneHue_pictureBox.BackColor = Color.White;
            clonePass2Opacity.Text = "";
            cloneMakeupOpacity1.Text = "";
            cloneMakeupOpacity2.Text = "";
        }

        private void CloneTONEselectAll_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in CloneTONElist_dataGridView.Rows)
            {
                if (row.Cells[3].Value.ToString() == "F")
                {
                    row.Cells[3].Value = "T";
                    clonePackTONEs[(int)row.Tag].selected = "T";
                }
                else
                {
                    row.Cells[3].Value = "F";
                    clonePackTONEs[(int)row.Tag].selected = "F";
                }
            }
        }

        private void cloneSkinState_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool ng = false;
            float[] mult = cloneBurnMultiplier.Tag as float[];
            float[] makeup1 = cloneMakeupOpacity1.Tag as float[];
            float[] makeup2 = cloneMakeupOpacity2.Tag as float[];
            try { mult[mySkinState] = float.Parse(cloneBurnMultiplier.Text); }
            catch { MessageBox.Show("Please enter a valid number in the " + skinStateNames[mySkinState] + " Burn Multiplier box!"); ng = true; }
            try { makeup1[mySkinState] = float.Parse(cloneMakeupOpacity1.Text); }
            catch { MessageBox.Show("Please enter a valid number in the " + skinStateNames[mySkinState] + " Makeup Opacity box!"); ng = true; }
            try { makeup2[mySkinState] = float.Parse(cloneMakeupOpacity2.Text); }
            catch { MessageBox.Show("Please enter a valid number in the " + skinStateNames[mySkinState] + " Special Makeup Opacity box!"); ng = true; }
            if (ng)
            {
                cloneSkinState_comboBox.SelectedIndexChanged -= cloneSkinState_comboBox_SelectedIndexChanged;
                cloneSkinState_comboBox.SelectedIndex = mySkinState;
                cloneSkinState_comboBox.SelectedIndexChanged += cloneSkinState_comboBox_SelectedIndexChanged;
            }
            cloneBurnMultiplier.Tag = mult;
            cloneMakeupOpacity1.Tag = makeup1;
            cloneMakeupOpacity2.Tag = makeup2;
            int index = cloneSkinState_comboBox.SelectedIndex;
            SetupImageDisplay(CloneTexture_pictureBox, myTONE.skinStates[index].skinImage);
            SetupImageDisplay(CloneBurnMask_pictureBox, myTONE.skinStates[index].maskImage);
            cloneBurnMultiplier.Text = mult[index].ToString();
            cloneMakeupOpacity1.Text = makeup1[index].ToString();
            cloneMakeupOpacity2.Text = makeup2[index].ToString();
            mySkinState = index;
        }

        private void CloneBatchTags_button_Click(object sender, EventArgs e)
        {
            if (changesTone)
            {
                MessageBox.Show("You have unsaved TONE changes! Please save or discard them before continuing.");
                return;
            }

            CloneBatchTag batchTags = new CloneBatchTag();
            batchTags.ShowDialog();
            if (batchTags.DialogResult == DialogResult.Cancel) return;

            if (batchTags.AddPropertyTag)
                TonePropAddAll(batchTags.FlagNumeric, batchTags.ValueNumeric);
            else
                TonePropRemoveAll(batchTags.FlagNumeric, batchTags.ValueNumeric);
        }

        private void CloneBatchTuning_button_Click(object sender, EventArgs e)
        {
            if (changesTone)
            {
                MessageBox.Show("You have unsaved TONE changes! Please save or discard them before continuing.");
                return;
            }

            CloneBatchTuning batchTuning = new CloneBatchTuning();
            batchTuning.ShowDialog();
            if (batchTuning.DialogResult == DialogResult.Cancel) return;

            TonePropTuning(batchTuning.TuningInstance);
        }

        private void CloneBatchPanel_button_Click(object sender, EventArgs e)
        {
            if (changesTone)
            {
                MessageBox.Show("You have unsaved TONE changes! Please save or discard them before continuing.");
                return;
            }

            CloneBatchPanel batchPanel = new CloneBatchPanel();
            batchPanel.ShowDialog();
            if (batchPanel.DialogResult == DialogResult.Cancel) return;

            TonePropPanel(batchPanel.skinPanel);
        }

        private void TonePropAddAll(uint flagNumeric, uint valNumeric)
        {
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                if ((string)CloneTONElist_dataGridView.Rows[i].Cells[3].Value == "F") continue;
                int index = (int)CloneTONElist_dataGridView.Rows[i].Tag;
                List<uint[]> tmpTags = new List<uint[]>(clonePackTONEs[index].CategoryTags);
                bool alreadyThere = false;
                for (int j = 0; j < tmpTags.Count; j++)
                {
                    if (tmpTags[j][0] == flagNumeric && tmpTags[j][1] == valNumeric)
                    {
                        alreadyThere = true;
                        break;
                    }
                }
                if (!alreadyThere) tmpTags.Add(new uint[] { flagNumeric, valNumeric });
                clonePackTONEs[index].CategoryTags = tmpTags;
                MemoryStream m = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(m);
                clonePackTONEs[index].Write(bw);
                m.Position = 0;
                ReplaceResource(clonePack, clonePackTONEs[index].resourceEntry, m);
            }
            CloneTONEsList(false);
        }

        private void TonePropRemoveAll(uint flagNumeric, uint valNumeric)
        {
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

        private void TonePropTuning(ulong tuningInstance)
        {
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                if ((string)CloneTONElist_dataGridView.Rows[i].Cells[3].Value == "F") continue;
                int index = (int)CloneTONElist_dataGridView.Rows[i].Tag;
                clonePackTONEs[index].TuningInstance = tuningInstance;
                MemoryStream m = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(m);
                clonePackTONEs[index].Write(bw);
                m.Position = 0;
                ReplaceResource(clonePack, clonePackTONEs[index].resourceEntry, m);
            }
            CloneTONEsList(false);
        }

        private void TonePropPanel(ushort panel)
        {
            for (int i = 0; i < CloneTONElist_dataGridView.Rows.Count; i++)
            {
                if ((string)CloneTONElist_dataGridView.Rows[i].Cells[3].Value == "F") continue;
                int index = (int)CloneTONElist_dataGridView.Rows[i].Tag;
                clonePackTONEs[index].SkinType = (TONE.SkinPanel)panel;
                MemoryStream m = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(m);
                clonePackTONEs[index].Write(bw);
                m.Position = 0;
                ReplaceResource(clonePack, clonePackTONEs[index].resourceEntry, m);
            }
            CloneTONEsList(false);
        }
    }
}
