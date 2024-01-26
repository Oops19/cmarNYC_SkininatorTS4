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
using s4pi.ImageResource;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        static string[] ageNames = new string[] { "Infant", "Toddler", "Child", "Teen", "Young Adult", "Adult", "Elder" };
        static uint[] ageValues = new uint[] { 128, 2, 4, 8, 16, 32, 64 };
        static string[] genderNames = new string[] { "Male", "Female" };
        static uint[] genderValues = new uint[] { 1, 2 };
        List<OverlayItem> overlayList;
        int overlayIndex = 0;
        bool changesOverlay = false;
        List<ulong> imagesToDelete = new List<ulong>();

        internal class OverlayItem
        {
            internal RLEResource overlay;
            internal ulong overlayInstance;
            internal bool cloned, imported;
            internal IResourceIndexEntry irIndexEntry;
            internal uint age, gender;
            public Bitmap image
            {
                get
                {
                    if (overlay != null)
                    {
                        DdsFile dds = new DdsFile();
                        dds.Load(overlay.ToDDS(), false);
                        return dds.Image;
                    }
                    else return null;
                }
            }
            internal OverlayItem(RLEResource overlayImage, ulong instance, bool isCloned, 
                IResourceIndexEntry iresIndexEntry, XmodsEnums.Age age, XmodsEnums.Gender gender)
            {
                this.overlay = overlayImage;
                this.overlayInstance = instance;
                this.cloned = isCloned;
                this.imported = false;
                this.irIndexEntry = iresIndexEntry;
                this.age = (uint)age;
                this.gender = (uint)gender;
            }
            internal void AddAgeGender(XmodsEnums.Age age, XmodsEnums.Gender gender)
            {
                if ((this.age & (uint)age) == 0) this.age += (uint)age;
                if ((this.gender & (uint)gender) == 0) this.gender += (uint)gender;
            }
            internal void SetAgeGender(uint age, uint gender)
            {
                this.age = age;
                this.gender = gender;
            }
        }

        public void SetupOverlays()
        {
            SetupOverlays(false);
        }
        public void SetupOverlays(bool endOfList)
        {
            Overlays_dataGridView.Rows.Clear();
            overlayList = new List<OverlayItem>();
            RLEResource tmpRLE;
            bool tmpCloned;
            IResourceIndexEntry tmpIRIE;
            List<ulong> instanceList = new List<ulong>();
            for (int i = 0; i < myTONE.NumberOverlays; i++)
            {
                int ind = instanceList.IndexOf(myTONE.GetOverLayInstance(i));
                if (ind < 0)
                {
                    instanceList.Add(myTONE.GetOverLayInstance(i));
                    ind = instanceList.IndexOf(myTONE.GetOverLayInstance(i));
                    tmpRLE = FindOverlayTexture(myTONE.GetOverLayInstance(i), out tmpIRIE, out tmpCloned);
                    overlayList.Add(new OverlayItem(tmpRLE, myTONE.GetOverLayInstance(i), tmpCloned, tmpIRIE,
                        myTONE.GetOverLayAge(i), myTONE.GetOverLayGender(i)));
                }
                else
                {
                    overlayList[ind].AddAgeGender(myTONE.GetOverLayAge(i), myTONE.GetOverLayGender(i));
                }
            }
            for (int i = 0; i < overlayList.Count; i++)
            {
                string tmp = "";
                for (int j = 0; j < ageValues.Length; j++)
                {
                    if ((ageValues[j] & overlayList[i].age) > 0)
                    {
                        if (tmp.Length > 0) tmp += ", ";
                        tmp += ageNames[j];
                    }
                }
                for (int j = 0; j < genderValues.Length; j++)
                {
                    if ((genderValues[j] & overlayList[i].gender) > 0)
                    {
                        if (tmp.Length > 0) tmp += ", ";
                        tmp += genderNames[j];
                    }
                }
                int ind = Overlays_dataGridView.Rows.Add(tmp);
                if (overlayList[i].overlay != null)
                {
                    Overlays_dataGridView.Rows[ind].Cells["OverlayImage"].Value = new Bitmap(overlayList[i].image);
                }
                else
                {
                    Overlays_dataGridView.Rows[ind].Cells["OverlayImage"].Value = null;
                }
            }
            if (endOfList)
            {
                overlayIndex = Overlays_dataGridView.RowCount - 1;
            }
            else
            {
                overlayIndex = 0;
            }
            if (overlayList.Count > 0)
            {
                OverlayImage_pictureBox.Enabled = true;
                OverlayCommit_button.Enabled = true;
                OverlayDiscard_button.Enabled = true;
                OverlayApplyAll_button.Enabled = true;
                SetupSelectedOverlay(overlayIndex);
                for (int i = 0; i < Overlays_dataGridView.Rows.Count; i++)
                {
                    Overlays_dataGridView.Rows[i].Selected = false;
                }
                Overlays_dataGridView.Rows[overlayIndex].Cells[0].Selected = true;
            }
            else
            {
                overlayAge_checkedListBox.Items.Clear();
                overlayGender_checkedListBox.Items.Clear();
                OverlayImage_pictureBox.Enabled = false;
                OverlayImage_pictureBox.Image = null;
                OverlayCommit_button.Enabled = false;
                OverlayDiscard_button.Enabled = false;
                OverlayApplyAll_button.Enabled = false;
            }
        }

        internal void SetupSelectedOverlay(int index)
        {
            overlayAge_checkedListBox.ItemCheck -= overlayAge_checkedListBox_ItemCheck;
            overlayGender_checkedListBox.ItemCheck -= overlayGender_checkedListBox_ItemCheck;
            SetupImageDisplay(OverlayImage_pictureBox, (Image)Overlays_dataGridView.Rows[index].Cells["OverlayImage"].Value);
            overlayAge_checkedListBox.Items.Clear();
            overlayGender_checkedListBox.Items.Clear();
            overlayAge_checkedListBox.Items.AddRange(ageNames);
            overlayGender_checkedListBox.Items.AddRange(genderNames);
            for (int i = 0; i < ageValues.Length; i++)
            {
                if ((overlayList[index].age & ageValues[i]) > 0)
                {
                    overlayAge_checkedListBox.SetItemChecked(i, true);
                }
            }

            for (int i = 0; i < genderValues.Length; i++)
            {
                if ((overlayList[index].gender & genderValues[i]) > 0)
                {
                    overlayGender_checkedListBox.SetItemChecked(i, true);
                }
            }
            overlayAge_checkedListBox.ItemCheck += overlayAge_checkedListBox_ItemCheck;
            overlayGender_checkedListBox.ItemCheck += overlayGender_checkedListBox_ItemCheck;
        }

        private void OverlayImage_pictureBox_Click(object sender, EventArgs e)
        {
            ImageDisplayImportExport imgDisplay = new ImageDisplayImportExport(overlayList[overlayIndex].overlay, ImageType.Overlay, "Import/Export Overlay Texture");
            DialogResult res = imgDisplay.ShowDialog();
            if (res == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                if (imgDisplay.ReturnIMG != null)
                {
                    Bitmap tmp = imgDisplay.ReturnIMG;
                    DdsFile dds = new DdsFile();
                    dds.CreateImage(tmp, false);
                    dds.GenerateMipMaps();
                    dds.UseDXT = true;
                    Stream s = new MemoryStream();
                    dds.Save(s);
                    s.Position = 0;
                    RLEResource rle = new RLEResource(1, null);
                    rle.ImportToRLE(s);
                    overlayList[overlayIndex].overlay = rle;
                    if (overlayList[overlayIndex].cloned & !OverlayUsedElsewhere(overlayList[overlayIndex].overlayInstance)) imagesToDelete.Add(overlayList[overlayIndex].overlayInstance);
                    overlayList[overlayIndex].overlayInstance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                    overlayList[overlayIndex].irIndexEntry = null;
                    overlayList[overlayIndex].cloned = true;
                    overlayList[overlayIndex].imported = true;
                    Overlays_dataGridView.Rows[overlayIndex].Cells["OverlayImage"].Value = new Bitmap(tmp);

                }
                else if (imgDisplay.ReturnDDS != null)
                { 
                    DdsFile tmp = imgDisplay.ReturnDDS;
                    tmp.UseDXT = true;
                    if (tmp.MipMaps <= 1) tmp.GenerateMipMaps();
                    Stream s = new MemoryStream();
                    tmp.Save(s);
                    s.Position = 0;
                    RLEResource rle = new RLEResource(1, null);
                    rle.ImportToRLE(s);
                    overlayList[overlayIndex].overlay = rle;
                    if (overlayList[overlayIndex].cloned & !OverlayUsedElsewhere(overlayList[overlayIndex].overlayInstance)) imagesToDelete.Add(overlayList[overlayIndex].overlayInstance);
                    overlayList[overlayIndex].overlayInstance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                    overlayList[overlayIndex].irIndexEntry = null;
                    overlayList[overlayIndex].cloned = true;
                    overlayList[overlayIndex].imported = true;
                    Overlays_dataGridView.Rows[overlayIndex].Cells["OverlayImage"].Value = new Bitmap(tmp.Image);
                }
                else
                {
                    overlayList[overlayIndex].overlay = null;
                    if (overlayList[overlayIndex].cloned & !OverlayUsedElsewhere(overlayList[overlayIndex].overlayInstance)) imagesToDelete.Add(overlayList[overlayIndex].overlayInstance);
                    overlayList[overlayIndex].overlayInstance = 0ul;
                    overlayList[overlayIndex].irIndexEntry = null;
                    overlayList[overlayIndex].cloned = false;
                    overlayList[overlayIndex].imported = false;
                    Overlays_dataGridView.Rows[overlayIndex].Cells["OverlayImage"].Value = null;
                }
                SetupImageDisplay(OverlayImage_pictureBox, (Image)Overlays_dataGridView.Rows[overlayIndex].Cells["OverlayImage"].Value);
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                changesOverlay = true;
            }
        }

        private void Overlays_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.RowIndex != overlayIndex)
            {
                if (changesOverlay)
                {
                    DialogResult res = MessageBox.Show("You have unsaved changes which will be lost if you go to another overlay!", "Uncommitted Changes", MessageBoxButtons.OKCancel);
                    if (res == DialogResult.Cancel)
                    {
                        Overlays_dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;
                        Overlays_dataGridView.Rows[overlayIndex].Cells[0].Selected = true;
                        return;
                    }
                    else
                    {
                        changesOverlay = false;
                    }
                }
                overlayIndex = e.RowIndex;
                SetupSelectedOverlay(overlayIndex);
            }
            if (e.ColumnIndex == Overlays_dataGridView.Columns["OverlayDelete"].Index)
            {
                DialogResult res = MessageBox.Show("Are you sure you want to delete this overlay? This cannot be undone!", "Delete Overlay", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    if (!OverlayUsedElsewhere(overlayList[overlayIndex].overlayInstance) && overlayList[overlayIndex].irIndexEntry != null)
                    {
                        clonePack.DeleteResource(overlayList[overlayIndex].irIndexEntry);
                    }
                    overlayList.RemoveAt(overlayIndex);
                    myTONE.OverlayList = SetTONEOverlays(overlayList);
                    DeleteResource(clonePack, myKey);
                    MemoryStream m = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(m);
                    myTONE.Write(bw);
                    m.Position = 0;
                    clonePack.AddResource(myKey, m, true);
                    changesOverlay = false;
                    SetupOverlays();
                    StartPreview();
                }
            }
        }

        private bool OverlayUsedElsewhere(ulong overlayInstance)
        {
            for (int i = 0; i < clonePackTONEs.Count; i++)
            {
                for (int j = 0; j < clonePackTONEs[i].NumberOverlays; j++)
                {
                    if (!(i == myTONEindex && j == overlayIndex) && overlayInstance == clonePackTONEs[i].GetOverLayInstance(j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<TONE.OverlayDesc> SetTONEOverlays(List<OverlayItem> overlays)
        {
            List<TONE.OverlayDesc> tmp = new List<TONE.OverlayDesc>();
            List<uint> overlayAG = new List<uint>();
            foreach (OverlayItem overlay in overlays)
            {
                bool added = false;
                for (int i = 0; i < ageValues.Length; i++)
                {
                    if ((ageValues[i] & overlay.age) > 0)
                    {
                        for (int j = 0; j < genderValues.Length; j++)
                        {
                            if ((genderValues[j] & overlay.gender) > 0)
                            {
                                tmp.Add(new TONE.OverlayDesc((XmodsEnums.Age)ageValues[i], (XmodsEnums.Gender)genderValues[j], overlay.overlayInstance));
                                overlayAG.Add(ageValues[i] + genderValues[j]);
                                added = true;
                            }
                        }
                    }
                }
                if (!added) tmp.Add(new TONE.OverlayDesc(XmodsEnums.Age.None, XmodsEnums.Gender.None, overlay.overlayInstance));
                if (overlay.irIndexEntry == null && overlay.overlay != null)
                {
                    TGIBlock tgi = new TGIBlock(1, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000, overlay.overlayInstance);
                    Stream s = new MemoryStream();
                    overlay.overlay.Stream.CopyTo(s);
                    s.Position = 0;
                    overlay.irIndexEntry = clonePack.AddResource(tgi, s, true);
                    overlay.irIndexEntry.Compressed = (ushort)0x5A42;
                }
                if (overlayAG.Distinct().ToList().Count < overlayAG.Count) MessageBox.Show("You have made more than one overlay for at least one age/gender combination." +
                    Environment.NewLine + "The game will use only ONE overlay for each age/gender." + Environment.NewLine + "This should be corrected.");
            }
            return tmp;
        }


        private void OverlayAdd_button_Click(object sender, EventArgs e)
        {
            if (changesOverlay)
            {
                MessageBox.Show("You have unsaved changes - please click Commit or Discard before adding an overlay!");
                return;
            }
            RLEResource newImage = new RLEResource(1, new MemoryStream(Properties.Resources.SkinOverlayTemplateRLE));
            ulong instance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
            overlayList.Add(new OverlayItem(newImage, instance, true, null, XmodsEnums.Age.None, XmodsEnums.Gender.None));
            myTONE.OverlayList = SetTONEOverlays(overlayList);
            DeleteResource(clonePack, myKey);
            MemoryStream m = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(m);
            myTONE.Write(bw);
            m.Position = 0;
            clonePack.AddResource(myKey, m, true);
            changesOverlay = false;
            SetupOverlays(true);
            StartPreview();
        }

        private void overlayAge_checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            changesOverlay = true;
        }

        private void overlayGender_checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            changesOverlay = true;
        }

        private void OverlayCommit_button_Click(object sender, EventArgs e)
        {
            if (overlayList[overlayIndex].overlayInstance == 0ul)
            {
                overlayList.RemoveAt(overlayIndex);
            }
            else
            {
                if (!CheckSizes()) return;
                uint age = 0, gender = 0;
                for (int i = 0; i < overlayAge_checkedListBox.Items.Count; i++)
                {
                    if (overlayAge_checkedListBox.GetItemChecked(i)) age += ageValues[i];
                }
                for (int i = 0; i < overlayGender_checkedListBox.Items.Count; i++)
                {
                    if (overlayGender_checkedListBox.GetItemChecked(i)) gender += genderValues[i];
                }
                overlayList[overlayIndex].SetAgeGender(age, gender);
            }
            myTONE.OverlayList = SetTONEOverlays(overlayList);
            foreach (ulong d in imagesToDelete)
            {
                DeleteResource(clonePack, new TGI((uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000U, d));
            }
            imagesToDelete.Clear();
            DeleteResource(clonePack, myKey);
            MemoryStream m = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(m);
            myTONE.Write(bw);
            m.Position = 0;
            clonePack.AddResource(myKey, m, true);
            changesOverlay = false;
            SetupOverlays();
            StartPreview();
        }

        private void OverlayDiscard_button_Click(object sender, EventArgs e)
        {
            SetupOverlays();
            changesOverlay = false;
            imagesToDelete.Clear();
        }

        private void OverlayApplyAll_button_Click(object sender, EventArgs e)
        {
            if (changesOverlay)
            {
                MessageBox.Show("You have unsaved changes - please click Commit or Discard before applying to all TONEs!");
                return;
            }
            for (int i = 0; i < clonePackTONEs.Count; i++)
            {
                clonePackTONEs[i].OverlayList = SetTONEOverlays(overlayList);
                DeleteResource(clonePack, clonePackTONEs[i].resourceEntry);
                MemoryStream m = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(m);
                clonePackTONEs[i].Write(bw);
                m.Position = 0;
                clonePack.AddResource(clonePackTONEs[i].resourceEntry, m, true);
            }
        }

        private void OverlaysWipe()
        {
            overlayIndex = 0;
            if (overlayList != null) overlayList.Clear();
            Overlays_dataGridView.Rows.Clear();
            OverlayImage_pictureBox.Image = null;
            overlayAge_checkedListBox.Items.Clear();
            overlayGender_checkedListBox.Items.Clear();
        }
    }
}
