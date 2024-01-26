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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using s4pi.ImageResource;
using Xmods.DataLib;

namespace XMODS
{
    public partial class Form1 : Form
    {
        //public void StartDetailsPreview(LRLE lrle, XmodsEnums.Age age, XmodsEnums.Gender gender)
        //{
        //    Image displayImage = lrle.image;
        //    skinDetailsPreviewer1.Start_Mesh(age, gender, displayImage, detailsPreviewUndies_checkBox.Checked);
        //}

        public void StartDetailsPreview(int index, bool setBackground)
        {
            detailsIndex = index;
            Image displayImage = null;
            XmodsEnums.Species species = defSkinData[index].species;
            XmodsEnums.Age age = defSkinData[index].age;
            XmodsEnums.Gender gender = defSkinData[index].gender;
            Physique physique = defSkinData[index].physique;
            if (CustomDetailsView1_radioButton.Checked)
            {
                LRLE lrle = FindDetailsPackTextureLRLE(defSkinData[index].instance);
                displayImage = lrle.image;
                if (setBackground) detailsPreviewBackground_checkBox.Checked = physique == Physique.neutral && defSkinData[index].layer == 0;
            }
            else
            {
                List<DetailSkins> temp = new List<DetailSkins>();
                foreach (DetailSkins d in defSkinData)
                {
                    if (d.age == age && d.gender == gender && (d.physique == physique || d.physique == Physique.neutral)) temp.Add(d);
                }
                temp.Sort((x, y) => x.layer.CompareTo(y.layer));
                for (int i = 0; i < temp.Count; i++)
                {
                    LRLE lrle = FindDetailsPackTextureLRLE(temp[i].instance);
                    if (lrle == null) lrle = FindDetailsGameTextureLRLE(temp[i].instance);
                    if (lrle == null) continue;
                    if (displayImage == null)
                    {
                        displayImage = lrle.image;
                    }
                    else
                    {
                        using (Graphics g = Graphics.FromImage(displayImage))
                        {
                            g.DrawImage(lrle.image, new Rectangle(0, 0, displayImage.Width, displayImage.Height));
                        }
                    }
                }
                if (setBackground) detailsPreviewBackground_checkBox.Checked = true;
            }

            List<DMap> morphs = new List<DMap>();
            if (age == XmodsEnums.Age.Elder)
            {
                ulong shapeID = FNVhash.FNV64("e" + (gender == XmodsEnums.Gender.Male ? "m" : "f") + "Body_Average_Shape");
                DMap shape = FetchGameDMap(new TGI((uint)XmodsEnums.ResourceTypes.DMAP, 0, shapeID));
                ulong normalID = FNVhash.FNV64("e" + (gender == XmodsEnums.Gender.Male ? "m" : "f") + "Body_Average_Normals");
                DMap normals = FetchGameDMap(new TGI((uint)XmodsEnums.ResourceTypes.DMAP, 0, normalID));
                morphs.Add(shape);
                morphs.Add(normals);
            }

            if (physique != Form1.Physique.neutral && age > XmodsEnums.Age.Baby)
            {
                string prefix;
                if (age == XmodsEnums.Age.Baby) prefix = "bu";
                else if (age == XmodsEnums.Age.Infant) prefix = "iu";
                else if (age == XmodsEnums.Age.Toddler) prefix = "pu";
                else if (age == XmodsEnums.Age.Child) prefix = "cu";
                else if (age >= XmodsEnums.Age.Teen && age <= XmodsEnums.Age.Adult) prefix = "y";
                else prefix = "e";
                if (age > XmodsEnums.Age.Child && age <= XmodsEnums.Age.Elder) prefix += gender.ToString().Substring(0, 1).ToLower();
                if (physique == Physique.fat)
                {
                    prefix += "Body_Heavy";
                }
                if (physique == Physique.thin)
                {
                    prefix += "Body_Lean";
                }
                if (physique == Physique.muscle)
                {
                    prefix += "Body_Fit";
                }
                if (physique == Physique.bony)
                {
                    prefix += "Body_Bony";
                }
                ulong shapeID = FNVhash.FNV64(prefix + "_Shape");
                DMap shape = FetchGameDMap(new TGI((uint)XmodsEnums.ResourceTypes.DMAP, 0, shapeID));
                ulong normalID = FNVhash.FNV64(prefix + "_Normals");
                DMap normals = FetchGameDMap(new TGI((uint)XmodsEnums.ResourceTypes.DMAP, 0, normalID));
                morphs.Add(shape);
                morphs.Add(normals);
            }

            skinDetailsPreviewer1.Start_Mesh(species, age, gender, morphs.ToArray(), displayImage, detailsPreviewUndies_checkBox.Checked, detailsPreviewBackground_checkBox.Checked);
        }

        public static GEOM LoadDMapMorph(GEOM baseMesh, DMap dmapShape, DMap dmapNormals)
        {
            MorphMap mapShape = dmapShape != null ? dmapShape.ToMorphMap(false) : null;
            MorphMap mapNormals = dmapNormals != null ? dmapNormals.ToMorphMap(false) : null;
            return LoadDMapMorph(baseMesh, mapShape, mapNormals);
        }

        public static GEOM LoadDMapMorph(GEOM baseMesh, MorphMap morphShape, MorphMap morphNormals)
        {
            if (baseMesh == null) return null;
            if (morphShape == null & morphNormals == null) return new GEOM(baseMesh);
            if (!baseMesh.hasTags || !baseMesh.hasUVset(1)) return new GEOM(baseMesh);
            GEOM morphMesh = new GEOM(baseMesh);
            Vector3 empty = new Vector3(0, 0, 0);

            if (morphShape != null && morphMesh.hasUVset(1))
            {
                for (int i = 0; i < morphMesh.numberVertices; i++)
                {
                    float[] pos = morphMesh.getPosition(i);
                    float[] norm = morphMesh.getNormal(i);
                    List<float[]> stitchList = morphMesh.GetStitchUVs(i);
                    int x, y;
                    Vector3 shapeVector = new Vector3();
                    Vector3 normVector = new Vector3();
                    if (stitchList.Count > 0)
                    {
                        float[] uv1 = stitchList[0];
                        x = (int)(Math.Abs(morphShape.MapWidth * uv1[0]) - morphShape.MinCol - 0.5f);
                        y = (int)((morphShape.MapHeight * uv1[1]) - morphShape.MinRow - 0.5f);
                    }
                    else
                    {
                        float[] uv1 = morphMesh.getUV(i, 1);
                        x = (int)(Math.Abs(morphShape.MapWidth * uv1[0]) - morphShape.MinCol - 0.5f);
                        y = (int)((morphShape.MapHeight * uv1[1]) - morphShape.MinRow - 0.5f);
                    }

                    if (y > morphShape.MaxRow - morphShape.MinRow) y = (int)(morphShape.MaxRow - morphShape.MinRow - 0.5f); //not sure about this

                    if (x >= 0 && x <= (morphShape.MaxCol - morphShape.MinCol) &&
                        y >= 0 && y <= (morphShape.MaxRow - morphShape.MinRow))
                    {
                        shapeVector = morphShape.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(morphMesh.getTagval(i) & 0x3F));
                        if (morphNormals != null)
                        {
                            normVector = morphNormals.GetAdjustedDelta(x, y, pos[0] < 0, (byte)(morphMesh.getTagval(i) & 0x3F));
                        }
                    }

                    if (shapeVector != empty)
                    {
                        // float vertWeight = ((morphMesh.getTagval(i) & 0xFF00) >> 8) / 255f;
                        float vertWeight = Math.Min(((morphMesh.getTagval(i) & 0xFF00) >> 8) / 127f, 1f);
                        pos[0] -= shapeVector.X * vertWeight;
                        pos[1] -= shapeVector.Y * vertWeight;
                        pos[2] -= shapeVector.Z * vertWeight;
                        norm[0] -= normVector.X * vertWeight;
                        norm[1] -= normVector.Y * vertWeight;
                        norm[2] -= normVector.Z * vertWeight;
                        morphMesh.setPosition(i, pos);
                        morphMesh.setNormal(i, norm);
                    }
                }
            }

            return morphMesh;
        }

        private void detailsPreviewUndies_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            skinDetailsPreviewer1.SetUndies(detailsPreviewUndies_checkBox.Checked);
        }

        private void CustomDetailsView1_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            StartDetailsPreview(detailsIndex, true);
        }

        private void detailsPreviewBackground_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            skinDetailsPreviewer1.SetBackground(detailsPreviewBackground_checkBox.Checked);
        }
    }
}
