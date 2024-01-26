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
        private void CloneGo_button_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> swatchList =
                ToneList_dataGridView.Rows.Cast<DataGridViewRow>().Where(k => Convert.ToBoolean(k.Cells[CloneTone.Name].Value) == true).ToList();
            if (swatchList.Count == 0)
            {
                MessageBox.Show("You must select at least one skintone!");
                return;
            }
            TGI dummy = new TGI(0U, 0U, 0UL);
            Package newPack = (Package)Package.NewPackage(0);

            if (Default_radioButton.Checked)
            {
                foreach (DataGridViewRow row in swatchList)
                {
                    int ind = Int32.Parse(row.Tag.ToString());
                    myPack = TONEitems[ind].TONEpackage;
                    Predicate<IResourceIndexEntry> getTONE = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.TONE &
                                                                  r.Instance == TONEitems[ind].TONEinstance;
                    IResourceIndexEntry rtone = myPack.Find(getTONE);
                    Stream s = myPack.GetResource(rtone);
                    s.Position = 0;
                    BinaryReader br = new BinaryReader(s);
                    TONE tone = new TONE(br);
                    MemoryStream m = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(m);
                    tone.Write(bw);
                    IResourceIndexEntry irieTone = newPack.AddResource(rtone, m, true);
                    irieTone.Compressed = (ushort)0x5A42;
                }
            }

            else if (New_radioButton.Checked)
            {
                List<TONE> TONElist = new List<TONE>();
                List<IResourceIndexEntry> irTONElist = new List<IResourceIndexEntry>();

                foreach (DataGridViewRow row in swatchList)
                {
                    // process TONEs in the skintone list
                    int ind = Int32.Parse(row.Tag.ToString());
                    myPack = TONEitems[ind].TONEpackage;
                    Predicate<IResourceIndexEntry> getTONE = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.TONE &
                                                      r.Instance == TONEitems[ind].TONEinstance;
                    IResourceIndexEntry rtone = myPack.Find(getTONE);
                    Stream s = myPack.GetResource(rtone);
                    s.Position = 0;
                    BinaryReader br = new BinaryReader(s);
                    TONE tone = new TONE(br);
                    TONElist.Add(tone);

                    TGIBlock rtoneNew;
                    rtoneNew = new TGIBlock(0, null, rtone.ResourceType,
                                   rtone.ResourceGroup, (uint)ran.Next());
                    MemoryStream m = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(m);
                    tone.Write(bw);
                    IResourceIndexEntry irieTone = newPack.AddResource(rtoneNew, m, true);
                    irieTone.Compressed = (ushort)0x5A42;
                    irTONElist.Add(irieTone);
                    s.Dispose();
                }

                if (SelectPack_radioButton.Checked && TONElist.Count > 0)       // If cloning from a CC package, clone the textures as well
                {
                    List<ulong> maskInstances = new List<ulong>();
                    List<ulong> overInstances = new List<ulong>();
                    List<ulong> maskInstanceNew = new List<ulong>();
                    List<ulong> overInstanceNew = new List<ulong>();
                    for (int i = 0; i < TONElist.Count; i++)
                    {
                        for (int j = 0; j < TONElist[i].SkinSets.Length; j++)
                        {
                            if (TONElist[i].SkinSets[j].TextureInstance > 0)
                            {
                                Predicate<IResourceIndexEntry> getRes = r => r.Instance == TONElist[i].SkinSets[j].TextureInstance;
                                List<IResourceIndexEntry> rRes = myPack.FindAll(getRes);
                                if (rRes != null || rRes.Count > 0)
                                {
                                    TGI tgiNew;
                                    ulong newInstance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                                    foreach (IResourceIndexEntry ir in rRes)
                                    {
                                        TGI tgiOld = new TGI(ir.ResourceType, ir.ResourceGroup, ir.Instance);
                                        tgiNew = new TGI(ir.ResourceType, ir.ResourceGroup, newInstance);
                                        CopyToneTexture(tgiOld, tgiNew, newPack);
                                    }
                                    TONElist[i].SetSkinSetTextureInstance(j, newInstance);
                                }
                            }
                            if (TONElist[i].SkinSets[j].OverlayInstance > 0)
                            {
                                int ind = maskInstances.IndexOf(TONElist[i].SkinSets[j].OverlayInstance);
                                if (ind == -1)       // if not copied already
                                {
                                    Predicate<IResourceIndexEntry> getRes = r => r.Instance == TONElist[i].SkinSets[j].OverlayInstance;
                                    List<IResourceIndexEntry> rRes = myPack.FindAll(getRes);
                                    if (rRes != null || rRes.Count > 0)
                                    {
                                        TGI tgiNew;
                                        ulong newInstance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                                        foreach (IResourceIndexEntry ir in rRes)
                                        {
                                            TGI tgiOld = new TGI(ir.ResourceType, ir.ResourceGroup, ir.Instance);
                                            tgiNew = new TGI(ir.ResourceType, ir.ResourceGroup, newInstance);
                                            CopyToneTexture(tgiOld, tgiNew, newPack);
                                        }
                                        maskInstances.Add(TONElist[i].SkinSets[j].OverlayInstance);
                                        TONElist[i].SetSkinSetOverlayInstance(j, newInstance);
                                        maskInstanceNew.Add(newInstance);
                                    }
                                }
                                else
                                {
                                    TONElist[i].SetSkinSetOverlayInstance(j, maskInstanceNew[ind]);
                                }
                            }
                            //else
                            //{
                            //    TONElist[i].SetOverLayInstance(j - 1, newInstance);
                            //}
                        }
                        for (int j = 0; j < TONElist[i].NumberOverlays; j++)
                        {
                            ulong instance = TONElist[i].GetOverLayInstance(j);
                            int ind = overInstances.IndexOf(instance);
                            if (ind == -1)       // if not copied already
                            {
                                if (instance > 0)
                                {
                                    Predicate<IResourceIndexEntry> getRes = r => r.Instance == TONElist[i].GetOverLayInstance(j);
                                    List<IResourceIndexEntry> rRes = myPack.FindAll(getRes);
                                    if (rRes != null || rRes.Count > 0)
                                    {
                                        TGI tgiNew;
                                        ulong newInstance = ((ulong)ran.Next() + ((ulong)ran.Next() << 32)) | 0x8000000000000000;
                                        foreach (IResourceIndexEntry ir in rRes)
                                        {
                                            TGI tgiOld = new TGI(ir.ResourceType, ir.ResourceGroup, ir.Instance);
                                            tgiNew = new TGI(ir.ResourceType, ir.ResourceGroup, newInstance);
                                            CopyToneTexture(tgiOld, tgiNew, newPack);
                                        }
                                        overInstances.Add(instance);
                                        TONElist[i].SetOverLayInstance(j, newInstance);
                                        overInstanceNew.Add(newInstance);
                                    }
                                }
                            }
                            else
                            {
                                TONElist[i].SetOverLayInstance(j, overInstanceNew[ind]);
                            }
                        }

                        DeleteResource(newPack, irTONElist[i]);
                        MemoryStream m = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(m);
                        TONElist[i].Write(bw);
                        IResourceIndexEntry irieTone = newPack.AddResource(irTONElist[i], m, true);
                        irieTone.Compressed = (ushort)0x5A42;
                    }
                }
            }

            if (WritePackage("Save new package", newPack, ""))
            {
                newPack.Dispose();
            }
            else
            {
                MessageBox.Show("Could not save cloned package!");
                newPack.Dispose();
            }
        }

        internal bool CopyToneTexture(TGI oldTGI, TGI newTGI, Package newPack)
        {
            Predicate<IResourceIndexEntry> getTexture = r => r.ResourceType == oldTGI.Type & r.ResourceGroup == oldTGI.Group &
                                r.Instance == oldTGI.Instance;
            IResourceIndexEntry irTexture = myPack.Find(getTexture);
            if (irTexture == null) return false;
            Stream s = myPack.GetResource(irTexture);
            s.Position = 0;
            TGIBlock rResNew = new TGIBlock(1, null, newTGI.Type, newTGI.Group, newTGI.Instance);
            IResourceIndexEntry irieRes = newPack.AddResource(rResNew, s, true);
            if (irieRes != null) irieRes.Compressed = (ushort)0x5A42;
            return true;
        }

        internal IResourceIndexEntry FindResource(Predicate<IResourceIndexEntry> getRes, out Package foundInPackage)
        {
            IResourceIndexEntry rRes = null;
            if (myPack != null)
            {
                rRes = myPack.Find(getRes);
            }
            if (rRes == null)
            {
                if (GamePack_radioButton.Checked)
                {
                    foreach (Package p in resourcePacks)
                    {
                        rRes = p.Find(getRes);
                        if (rRes != null)
                        {
                            foundInPackage = p;
                            return rRes;
                        }
                    }
                    if (rRes == null)
                    {
                        foundInPackage = null;
                        return null;
                    }
                }
                else
                {
                    foundInPackage = null;
                    return null;
                }
            }
            foundInPackage = myPack;
            return rRes;
        }
    }
}
