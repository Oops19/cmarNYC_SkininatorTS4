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
using Xmods.DataLib;
using s4pi.ImageResource;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        static string[] detailNames = new string[] { "Baby Unisex", 
            "Infant Unisex Neutral", "Infant Unisex Fat", "Infant Unisex Thin",
            "Toddler Unisex Neutral", "Toddler Unisex Fat", "Toddler Unisex Thin", 
            "Child Unisex Neutral", "Child Unisex Fat", "Child Unisex Thin", 
            "Male Teen Neutral", "Male Teen Fat", "Male Teen Thin", "Male Teen Muscle", "Male Teen Bony",
            "Male YA Neutral", "Male YA Fat", "Male YA Thin", "Male YA Muscle", "Male YA Bony", 
            "Male Adult Neutral", "Male Adult Fat", "Male Adult Thin", "Male Adult Muscle", "Male Adult Bony",
            "Male Elder Neutral", "Male Elder Fat", "Male Elder Thin", "Male Elder Muscle", "Male Elder Bony",
            "Male Teen Chest Overlay Neutral", "Male Teen Chest Overlay Fat", "Male Teen Chest Overlay Thin", "Male Teen Chest Overlay Muscle", "Male Teen Chest Overlay Bony",
            "Male YA Chest Overlay Neutral", "Male YA Chest Overlay Fat", "Male YA Chest Overlay Thin", "Male YA Chest Overlay Muscle", "Male YA Chest Overlay Bony",
            "Male Adult Underlayer Neutral", "Male Adult Underlayer Fat", "Male Adult Underlayer Thin", "Male Adult Underlayer Muscle", "Male Adult Underlayer Bony",
            "Male Elder Chest Overlay Neutral", "Male Elder Chest Overlay Fat", "Male Elder Chest Overlay Thin", "Male Elder Chest Overlay Muscle", "Male Elder Chest Overlay Bony",
            "Female Teen Neutral", "Female Teen Fat", "Female Teen Thin", "Female Teen Muscle", "Female Teen Bony",
            "Female YA Neutral", "Female YA Fat", "Female YA Thin", "Female YA Muscle", "Female YA Bony", 
            "Female Adult Neutral", "Female Adult Fat", "Female Adult Thin", "Female Adult Muscle", "Female Adult Bony",
            "Female Elder Neutral", "Female Elder Fat", "Female Elder Thin", "Female Elder Muscle", "Female Elder Bony",
            "Female Teen Breast Overlay Neutral", "Female Teen Breast Overlay Fat", "Female Teen Breast Overlay Thin", "Female Teen Breast Overlay Muscle", "Female Teen Breast Overlay Bony",
            "Female YA Breast Overlay Neutral", "Female YA Breast Overlay Fat", "Female YA Breast Overlay Thin", "Female YA Breast Overlay Muscle", "Female YA Breast Overlay Bony",
            "Female Adult Underlayer Neutral", "Female Adult Underlayer Fat", "Female Adult Underlayer Thin", "Female Adult Underlayer Muscle", "Female Adult Underlayer Bony",
            "Female Elder Breast Overlay Neutral", "Female Elder Breast Overlay Fat", "Female Elder Breast Overlay Thin", "Female Elder Breast Overlay Muscle", "Female Elder Breast Overlay Bony",
            "Werewolf M/F Teen-Elder Neutral", "Werewolf M/F Teen-Elder Fat", "Werewolf M/F Teen-Elder Thin","Werewolf M/F Teen-Elder Muscle","Werewolf M/F Teen-Elder Bony"
        };

        static ulong[] detailInstance = new ulong[] { 0x0A11C0657FBDB54FU,
            0xB86DB26314525BC8U, 0x58DDF53AE00AD850U, 0xD1F895CE9F29A00FU,
            0xD19E353A4001EC4DU, 0xCB74D8715AACAEE5U, 0xFFECB88D957AB9C8U,
            0x9CB2C5C93E357C62U, 0xD35E44A00EC82DD2U, 0xDCE1DE32790EEE2DU,
            0x48F11375333EDB51U, 0xC5A686CB8DEAD669U, 0x5B5168D56FB549CCU, 0x050DA429AF8F8579U, 0x493919D5653D6D22U,
            0x58F8275474E1AE00U, 0x8225CF86E9ADE5A8U, 0xA6DF5710210EC357U, 0xF5A23C00099ADF1CU, 0x1E40BE1064236B31U,
            0x265B16FA4E7DA19BU, 0x25549E2A0EA2EA03U, 0xBE003BD8D1E4F6CCU, 0x049310925E1F7507U, 0x0FF4F3A205CF3792U,
            0x24DFF8E30DC7E5DCU, 0xF949199CADA33974U, 0x120B1D9B35364743U, 0xA64804A650DC3CB8U, 0x47F4E49B544D0695U,

            0xA062AF087257C3AAU, 0x614DF350B2288202U, 0xA30CC4B09CB5AA9FU, 0x9B76618343015672U, 0x15CCD44E2A798561U,
            0xA3EC609A2DAB31D3U, 0xE6997ACD10E7ADFBU, 0x983C2A528E708C94U, 0x8A4EC419617EFB8FU, 0x4C97A925081CFE8AU,
            0x308855B3BFF0E848U, 0x0D2BCAD6902710D0U, 0x37EF8E5A749B458FU, 0x14F31CC55B6B8D94U, 0x2930155A6CFBC099U,
            0x25EBBD9BED791D4FU, 0x15B4CE7F555B78E7U, 0x0DDA0E70371F1850U, 0x02749727C1E4936BU, 0xE4DC67D79FE3259EU, 

            0x737A5FF0EB729888U, 0xCB60F0F987055510U, 0x3A8BC2ABBFEA0DCFU, 0xD9BEAB29970846D4U, 0x286649ABB5675FD9U,
            0x36C865290B1F4E79U, 0x5A0156E11FEB7ED1U, 0x1A2B3CB6DF532C84U, 0x980C64FFD5139131U, 0x95006DB72556DFEAU,
            0x53F13B3669333A6AU, 0x6B5D4119F5F3E3C2U, 0x38E5BC3422C0E85FU, 0x6E20B376A7440832U, 0x4C23AB3892B08421U, 
            0x2356ABE32AC4C255U, 0xF6F5AF6EB76D95CDU, 0x33AAC3C4E5BB2680U, 0xD197EDA66965137DU, 0x65CEB4C5019CBDFEU,

            0xF85FB112905485DBU, 0x983E38E671724943U, 0x24C0445548F4DD0CU, 0x87C948F0D0911447U, 0x769AFB69C35341D2U,
            0x0A136CA1147B1772U, 0xDA72BC0116BD2B2AU, 0x084BCF43E871B757U, 0xE2C41710B0F9748AU, 0xB356C29F445C7C69U,
            0x59093C1074E2C911U, 0xB45360AD81F01829U, 0x58B534842466818CU, 0x404315C573F47F39U, 0x469DE58419F058E2U,
            0x1E1930AE6138725EU, 0xDFD33956DE308196U, 0x718F473471CA0653U, 0xDA922B3518C2D1E6U, 0x2433EE3BE4F044ADU,
            0x1F52549DEC59E9DAU, 0x59A47D8E3E13DEF2U, 0x7DBF069EF4CC602FU, 0x342ADDA798142502U, 0xD26D60979C93ABD1U
        };

        DetailSkins[] defSkinData;

        //static XmodsEnums.Age[] detailAges = new XmodsEnums.Age[] { XmodsEnums.Age.Baby, 
        //    XmodsEnums.Age.Toddler, XmodsEnums.Age.Toddler, XmodsEnums.Age.Toddler, 
        //    XmodsEnums.Age.Child, XmodsEnums.Age.Child, XmodsEnums.Age.Child, 
        //    XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen,
        //    XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, 
        //    XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult,
        //    XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder,
        //    XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen,
        //    XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult,
        //    XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult,
        //    XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder,
        //    XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen,
        //    XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, 
        //    XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult,
        //    XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder,
        //    XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen, XmodsEnums.Age.Teen,
        //    XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult, XmodsEnums.Age.YoungAdult,
        //    XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult, XmodsEnums.Age.Adult,
        //    XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder, XmodsEnums.Age.Elder };
        //static XmodsEnums.Gender[] detailGenders = new XmodsEnums.Gender[] { XmodsEnums.Gender.Unisex, 
        //    XmodsEnums.Gender.Unisex, XmodsEnums.Gender.Unisex, XmodsEnums.Gender.Unisex, 
        //    XmodsEnums.Gender.Unisex, XmodsEnums.Gender.Unisex, XmodsEnums.Gender.Unisex, 
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male, XmodsEnums.Gender.Male,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female,
        //    XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female, XmodsEnums.Gender.Female };

        public class DetailSkins
        {
            internal string name;
            internal ulong instance;
            internal XmodsEnums.Species species;
            internal XmodsEnums.Age age;
            internal XmodsEnums.Gender gender;
            internal Physique physique;
            internal int layer;
            internal DetailSkins(string name, ulong instance)
            {
                this.name = name;
                this.instance = instance;
                this.species = GetSpecies(name);
                this.age = GetAge(name);
                this.gender = GetGender(name);
                this.physique = GetPhysique(name);
                this.layer = GetLayer(name, this.physique);
            }
            private XmodsEnums.Species GetSpecies(string name)
            {
                if (name.Contains("Werewolf")) return XmodsEnums.Species.Werewolf;
                else return XmodsEnums.Species.Human;
            }
            private XmodsEnums.Age GetAge(string name)
            {
                if (name.Contains("Baby")) return XmodsEnums.Age.Baby;
                else if (name.Contains("Infant")) return XmodsEnums.Age.Infant;
                else if (name.Contains("Toddler")) return XmodsEnums.Age.Toddler;
                else if (name.Contains("Child")) return XmodsEnums.Age.Child;
                else if (name.Contains("Teen")) return XmodsEnums.Age.Teen;
                else if (name.Contains("YA")) return XmodsEnums.Age.YoungAdult;
                else if (name.Contains("Adult")) return XmodsEnums.Age.Adult;
                else return XmodsEnums.Age.Elder;
            }
            private XmodsEnums.Gender GetGender(string name)
            {
                if (name.Contains("Male")) return XmodsEnums.Gender.Male;
                else if (name.Contains("Female")) return XmodsEnums.Gender.Female;
                else return XmodsEnums.Gender.Unisex;
            }
            private Physique GetPhysique(string name)
            {
                if (name.Contains("Fat")) return Physique.fat;
                else if (name.Contains("Thin")) return Physique.thin;
                else if (name.Contains("Muscle")) return Physique.muscle;
                else if (name.Contains("Bony")) return Physique.bony;
                else return Physique.neutral;
            }
            // -1 = underlayer neutral, 0 = main neutral, 1 = overlay neutral, 2 = underlayer morph,  3 = main morph, 4 = overlay morph
            private int GetLayer(string name, Physique physique)
            {
                if (name.Contains("Underlayer"))
                {
                    if (physique == Physique.neutral) return -1;
                    else return 2;
                }
                else if (name.Contains("Overlay"))
                {
                    if (physique == Physique.neutral) return 1;
                    else return 4;
                }
                else
                {
                    if (physique == Physique.neutral) return 0;
                    else return 3;
                }
            }
        }

        public enum Physique
        {
            neutral,
            fat,
            thin,
            muscle,
            bony
        }

        internal DetailSkins[] LoadDetailSkins()
        {
            DetailSkins[] temp = new DetailSkins[detailNames.Length];
            for (int i = 0; i < detailNames.Length; i++)
            {
                temp[i] = new DetailSkins(detailNames[i], detailInstance[i]);
            }
            return temp;
        }

        Package detailsPack = null;
        int detailsIndex;

        static ulong[] oldAdultInstance = new ulong[] { 0x308855B3BFF0E848U, 0x0D2BCAD6902710D0U, 0x37EF8E5A749B458FU,
            0x14F31CC55B6B8D94U, 0x2930155A6CFBC099U, 0x59093C1074E2C911U, 0xB45360AD81F01829U, 0x58B534842466818CU,
            0x404315C573F47F39U, 0x469DE58419F058E2U };
        static ulong[] newAdultInstance = new ulong[] { 0x265B16FA4E7DA19BU, 0x25549E2A0EA2EA03U, 0xBE003BD8D1E4F6CCU, 
            0x049310925E1F7507U, 0x0FF4F3A205CF3792U, 0x53F13B3669333A6AU, 0x6B5D4119F5F3E3C2U, 0x38E5BC3422C0E85FU, 
            0x6E20B376A7440832U, 0x4C23AB3892B08421U };
        static ulong[] TYAEInstance = new ulong[] {  0x48F11375333EDB51U, 0xC5A686CB8DEAD669U, 0x5B5168D56FB549CCU, 0x050DA429AF8F8579U, 0x493919D5653D6D22U,
            0x58F8275474E1AE00U, 0x8225CF86E9ADE5A8U, 0xA6DF5710210EC357U, 0xF5A23C00099ADF1CU, 0x1E40BE1064236B31U,
            0x24DFF8E30DC7E5DCU, 0xF949199CADA33974U, 0x120B1D9B35364743U, 0xA64804A650DC3CB8U, 0x47F4E49B544D0695U,
            0x737A5FF0EB729888U, 0xCB60F0F987055510U, 0x3A8BC2ABBFEA0DCFU, 0xD9BEAB29970846D4U, 0x286649ABB5675FD9U,
            0x36C865290B1F4E79U, 0x5A0156E11FEB7ED1U, 0x1A2B3CB6DF532C84U, 0x980C64FFD5139131U, 0x95006DB72556DFEAU,
            0x2356ABE32AC4C255U, 0xF6F5AF6EB76D95CDU, 0x33AAC3C4E5BB2680U, 0xD197EDA66965137DU, 0x65CEB4C5019CBDFEU };
        static ulong[] TYAEOverlay = new ulong[] { 0xA062AF087257C3AAU, 0x614DF350B2288202U, 0xA30CC4B09CB5AA9FU, 0x9B76618343015672U, 0x15CCD44E2A798561U,
            0xA3EC609A2DAB31D3U, 0xE6997ACD10E7ADFBU, 0x983C2A528E708C94U, 0x8A4EC419617EFB8FU, 0x4C97A925081CFE8AU,
            0x25EBBD9BED791D4FU, 0x15B4CE7F555B78E7U, 0x0DDA0E70371F1850U, 0x02749727C1E4936BU, 0xE4DC67D79FE3259EU, 
            0xF85FB112905485DBU, 0x983E38E671724943U, 0x24C0445548F4DD0CU, 0x87C948F0D0911447U, 0x769AFB69C35341D2U,
            0x0A136CA1147B1772U, 0xDA72BC0116BD2B2AU, 0x084BCF43E871B757U, 0xE2C41710B0F9748AU, 0xB356C29F445C7C69U,
            0x1E1930AE6138725EU, 0xDFD33956DE308196U, 0x718F473471CA0653U, 0xDA922B3518C2D1E6U, 0x2433EE3BE4F044ADU };

        public void SetupEASkinDetailsList()
        {
            for (int i = 0; i < defSkinData.Length; i++)
            {
                string[] tmp = new string[] { defSkinData[i].name, defSkinData[i].instance.ToString("X16") };
                int ind = GameSkinDetails_dataGridView.Rows.Add(tmp);
                GameSkinDetails_dataGridView.Rows[ind].Tag = defSkinData[i];
                if (defSkinData[i].name.Contains("Male"))
                    GameSkinDetails_dataGridView.Rows[ind].DefaultCellStyle.BackColor = Color.AliceBlue;
                if (defSkinData[i].name.Contains("Female"))
                    GameSkinDetails_dataGridView.Rows[ind].DefaultCellStyle.BackColor = Color.OldLace;
                if (defSkinData[i].name.Contains("Werewolf"))
                    GameSkinDetails_dataGridView.Rows[ind].DefaultCellStyle.BackColor = Color.MintCream;
            }
        }

        private void MakeDefRepPackage_button_Click(object sender, EventArgs e)
        {
            Package defRepPack = (Package)Package.NewPackage(0);
            bool gotStuff = false;
            if (MakeDefRepSimplified_checkBox.Checked)
            {
                Wait_label.Visible = true;
                Wait_label.Refresh();
            }

            foreach (DataGridViewRow r in GameSkinDetails_dataGridView.Rows)
            {
                if (Convert.ToBoolean(r.Cells["EASkinDetailClone"].Value))
                {
                    DetailSkins skin = (DetailSkins)r.Tag;
                    ulong instance = skin.instance;
                    LRLE lrle = null;

                    if (MakeDefRepSimplified_checkBox.Checked && skin.age > XmodsEnums.Age.Child)
                    {
                        if (skin.layer == 0 || skin.layer == 3)
                        {
                            List<DetailSkins> list = new List<DetailSkins>();
                            foreach (DetailSkins d in defSkinData)
                            {
                                if (d.age == skin.age && d.gender == skin.gender && d.physique == skin.physique) list.Add(d);
                            }
                            list.Sort((x, y) => x.layer.CompareTo(y.layer));
                            Bitmap img = null;
                            for (int i = 0; i < list.Count; i++)
                            {
                                LRLE tmp = FindDetailsGameTextureLRLE(list[i].instance);
                                if (tmp == null) continue;
                                if (img == null)
                                {
                                    img = tmp.image;
                                }
                                else
                                {
                                    using (Graphics g = Graphics.FromImage(img))
                                    {
                                        g.DrawImage(tmp.image, new Rectangle(0, 0, img.Width, img.Height));
                                    }
                                }
                            }
                            lrle = new LRLE(img);
                        }
                        else
                        {
                            lrle = new LRLE(new Bitmap(1024, 2048));
                        }
                    }
                    else
                    {
                        lrle = FindDetailsGameTextureLRLE(instance);
                    }

                    if (lrle != null)
                    {
                        IResourceKey ik = new TGIBlock(1, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0, instance);
                        IResourceIndexEntry ires = defRepPack.AddResource(ik, lrle.Stream, true);
                        ires.Compressed = (ushort)0x5A42;
                        gotStuff = true;
                    }
                }
            }

            if (MakeDefRepSimplified_checkBox.Checked) Wait_label.Visible = false;
            if (!gotStuff)
            {
                MessageBox.Show("You haven't selected any textures to include in your package!");
                return;
            }
            if (WritePackage("Save new package", defRepPack, ""))
            {
                defRepPack.Dispose();
            }
            else
            {
                MessageBox.Show("Could not save package!");
                defRepPack.Dispose();
            }
        }

        private LRLE FindDetailsGameTextureLRLE(ulong instance)
        {
            TGI dummy;
            return FindDetailsGameTextureLRLE(instance, out dummy);
        }
        private LRLE FindDetailsGameTextureLRLE(ulong instance, out TGI foundKey)
        {
            if (instance == 0)
            {
                foundKey = null;
                return null;
            }
            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE &
                                    r.Instance == instance;
            Predicate<IResourceIndexEntry> irle = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &
                                    r.Instance == instance;

            foreach (Package p in gamePacksOther)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    foundKey = new TGI(ptex.ResourceType, ptex.ResourceGroup, ptex.Instance);
                    BinaryReader br = new BinaryReader(p.GetResource(ptex));
                    LRLE lrle = new LRLE(br);
                    return lrle;
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = p.Find(itex);
                if (ptex != null)
                {
                    foundKey = new TGI(ptex.ResourceType, ptex.ResourceGroup, ptex.Instance);
                    BinaryReader br = new BinaryReader(p.GetResource(ptex));
                    LRLE lrle = new LRLE(br);
                    return lrle;
                }
            }
            foreach (Package p in gamePacksOther)
            {
                IResourceIndexEntry ptex = p.Find(irle);
                if (ptex != null)
                {
                    RLEResource rle = new RLEResource(1, p.GetResource(ptex));
                    DdsFile dds = new DdsFile();
                    dds.Load(rle.ToDDS(), false);
                    LRLE lrle = new LRLE(dds.Image);
                    foundKey = new TGI((uint)XmodsEnums.ResourceTypes.LRLE, ptex.ResourceGroup, ptex.Instance);
                    return lrle;
                }
            }
            foreach (Package p in gamePacks0)
            {
                IResourceIndexEntry ptex = clonePack.Find(irle);
                if (ptex != null)
                {
                    RLEResource rle = new RLEResource(1, p.GetResource(ptex));
                    DdsFile dds = new DdsFile();
                    dds.Load(rle.ToDDS(), false);
                    LRLE lrle = new LRLE(dds.Image);
                    foundKey = new TGI((uint)XmodsEnums.ResourceTypes.LRLE, ptex.ResourceGroup, ptex.Instance);
                    return lrle;
                }
            }
            MessageBox.Show("Can't find texture instance " + instance.ToString("X16") + " in game packages!");
            foundKey = null;
            return null;
        }

        private LRLE FindDetailsPackTextureLRLE(ulong instance)
        {
            if (instance == 0) return null;

            Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE &
                                    r.Instance == instance;
            Predicate<IResourceIndexEntry> irle = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &
                                    r.Instance == instance;

            IResourceIndexEntry ctex = detailsPack.Find(itex);
            if (ctex != null)
            {
                try
                {
                    BinaryReader br = new BinaryReader(detailsPack.GetResource(ctex));
                    LRLE lrle = new LRLE(br);
                    if (lrle != null) return lrle;
                }
                catch
                {
                    try
                    {
                        RLEResource rle = new RLEResource(1, detailsPack.GetResource(ctex));
                        DdsFile dds = new DdsFile();
                        dds.Load(rle.ToDDS(), false);
                        LRLE lrle = new LRLE(dds.Image);
                        return lrle;
                    }
                    catch
                    {
                        MessageBox.Show("Can't read texture: " + ctex.ToString());
                        return null;
                    }
                }
            }
            ctex = detailsPack.Find(irle);
            if (ctex != null)
            {
                RLEResource rle = new RLEResource(1, detailsPack.GetResource(ctex));
                DdsFile dds = new DdsFile();
                dds.Load(rle.ToDDS(), false);
                LRLE lrle = new LRLE(dds.Image);
                return lrle;
            }
            else
            {
                return null;
            }
        }

        private DMap FetchGameDMap(TGI tgi)
        {
            if (tgi.Instance == 0ul) return null;
            Predicate<IResourceIndexEntry> pred = r => r.ResourceType == tgi.Type & r.Instance == tgi.Instance;
            for (int i = 0; i < gamePacks0.Length; i++)
            {
                Package p = gamePacks0[i];
                IResourceIndexEntry irie = p.Find(pred);
                if (irie != null)
                {
                    using (BinaryReader br = new BinaryReader(p.GetResource(irie)))
                    {
                        try
                        {
                            DMap dmap = new DMap(br);
                            return dmap;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Can't read DMap " + tgi.ToString() + " : " + e.Message);
                            return null;
                        }
                    }
                }
            }
            return null;
        }


        private void GameSkinDetailsMaleToggle_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < defSkinData.Length; i++)
            {
                if (defSkinData[i].gender == XmodsEnums.Gender.Male)
                    GameSkinDetails_dataGridView.Rows[i].Cells["EASkinDetailClone"].Value =
                        Convert.ToBoolean(GameSkinDetails_dataGridView.Rows[i].Cells["EASkinDetailClone"].Value) ^ true;
            }
        }

        private void GameSkinDetailsFemaleToggle_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < defSkinData.Length; i++)
            {
                if (defSkinData[i].gender == XmodsEnums.Gender.Female)
                    GameSkinDetails_dataGridView.Rows[i].Cells["EASkinDetailClone"].Value =
                        Convert.ToBoolean(GameSkinDetails_dataGridView.Rows[i].Cells["EASkinDetailClone"].Value) ^ true;
            }
        }

        private void SimplifyHelp_button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In a simplified package all the underlayers and overlays are" + Environment.NewLine +
                            "combined with the main texture for each age/gender/physique," + Environment.NewLine +
                            "and the underlayers and overlays are made blank and fully" + Environment.NewLine +
                            "transparent. You then only have to modify the main textures." + Environment.NewLine + Environment.NewLine +
                            "Note that you must clone and include the entire set of blank" + Environment.NewLine +
                            "underlayers and overlays in the package or the game textures" + Environment.NewLine +
                            "will conflict with your skin.");
        }

        private void DetailsPackageEditFile_button_Click(object sender, EventArgs e)
        {
            if (detailsPack != null)
            {
                DialogResult res = MessageBox.Show("Do you want to close the currently open package and open a new one?", "Open a new package", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return;
                detailsPack.Dispose();
                CustomSkinDetails_dataGridView.Rows.Clear();
            }
            DetailsPackageEditFile.Text = GetFilename("Select Default Replacement Package File", Packagefilter);
            if (!File.Exists(DetailsPackageEditFile.Text))
            {
                MessageBox.Show("You have not selected a valid package file!");
                return;
            }
            detailsPack = OpenPackage(DetailsPackageEditFile.Text, true);
            if (detailsPack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            Predicate<IResourceIndexEntry> isSkin = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE;
            List<IResourceIndexEntry> iresSkins = detailsPack.FindAll(isSkin);
            Predicate<IResourceIndexEntry> isRle = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2;
            iresSkins.AddRange(detailsPack.FindAll(isRle));
            bool wasWarned = false;
            foreach (IResourceIndexEntry ir in iresSkins)
            {
                if (!isLRLE(ir, detailsPack))
                {
                    if (!wasWarned)
                    {
                        MessageBox.Show("Outdated textures will be converted to the current format. This may take some time.");
                        wasWarned = true;
                    }
                    LRLE lrle = FindDetailsPackTextureLRLE(ir.Instance);
                    DeleteResource(detailsPack, ir);
                    TGIBlock tgi = new TGIBlock(1, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0, ir.Instance);
                    IResourceIndexEntry inew = detailsPack.AddResource(tgi, lrle.Stream, true);
                    inew.Compressed = (ushort)0x5A42;
                }
                for (int i = 0; i < detailInstance.Length; i++)
                {
                    if (ir.Instance == detailInstance[i])
                    {
                        string[] tmp = new string[] { detailNames[i] };
                        int ind = CustomSkinDetails_dataGridView.Rows.Add(tmp);
                        CustomSkinDetails_dataGridView.Rows[ind].Tag = i;
                        CustomSkinDetails_dataGridView.Rows[ind].Cells[0].Tag = ((uint)defSkinData[i].age).ToString("D2") + ((uint)defSkinData[i].gender).ToString() + i.ToString("D2");
                    }
                }
                if (CustomDetailsSort2_radioButton.Checked) CustomSkinDetails_dataGridView.Sort(new RowComparer(2));
            }
            if (CustomSkinDetails_dataGridView.RowCount == 0)
            {
                MessageBox.Show("No body skin definition images found!");
                return;
            }
            CustomSkinDetails_dataGridView.Rows[0].Cells[0].Selected = true;
            int index = (int)CustomSkinDetails_dataGridView.Rows[0].Tag;
         //   LRLE lrle = FindDetailsPackTextureLRLE(detailInstance[index]);
            StartDetailsPreview(index, true);
         //   detailsIndex = index;
        }

        private void CustomDetailsSort2_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CustomDetailsSort1_radioButton.Checked)
                CustomSkinDetails_dataGridView.Sort(new RowComparer(1));
            else
                CustomSkinDetails_dataGridView.Sort(new RowComparer(2));
        }

        private class RowComparer : System.Collections.IComparer
        {
            private static int sortType = 1;

            public RowComparer(int sortOrder) { sortType = sortOrder; }

            public int Compare(object x, object y)
            {
                DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
                DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;
                if (sortType == 1)
                    return ((int)DataGridViewRow1.Tag).CompareTo((int)DataGridViewRow2.Tag);
                else
                    return ((string)DataGridViewRow1.Cells[0].Tag).CompareTo((string)DataGridViewRow2.Cells[0].Tag);
            }
        }

        private bool isLRLE(IResourceIndexEntry ires, Package pack)
        {
            byte[] magic = new byte[] { 0x4C, 0x52, 0x4C, 0x45 };
            Stream s = pack.GetResource(ires);
            byte[] test = new byte[4];
            s.Read(test, 0, 4);
            return magic.SequenceEqual(test);
        }

        private void CustomSkinDetails_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int index = (int)CustomSkinDetails_dataGridView.Rows[e.RowIndex].Tag;
            if (index != detailsIndex)
            {
               // LRLE lrle = FindDetailsPackTextureLRLE(detailInstance[index]);
                StartDetailsPreview(index, true);
            }
            if (e.ColumnIndex == CustomSkinDetails_dataGridView.Columns["CCSkinDetailsExport"].Index)
            {
                LRLE lrle = FindDetailsPackTextureLRLE(detailInstance[index]);
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = ImageFilter;
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.Title = "Save skin details texture image file";
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.DefaultExt = "png";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.FileName = (new TGI((uint)XmodsEnums.ResourceTypes.LRLE, 0, detailInstance[index])).ToString() + "_" + detailNames[index];
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                        {
                            if (String.Compare(Path.GetExtension(saveFileDialog1.FileName).ToLower(), ".png") == 0)
                            {
                                lrle.image.Save(myStream, ImageFormat.Png);
                            }
                            else
                            {
                                DdsFile dds = new DdsFile();
                                dds.UseDXT = false;
                                dds.CreateImage(lrle.image, false);
                                dds.Save(myStream);
                            }
                            myStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    }
                }
            }
            else if (e.ColumnIndex == CustomSkinDetails_dataGridView.Columns["CCSkinDetailsImport"].Index)
            {
                string importFile = GetFilename("Select skin details texture image", ImageFilter);
                if (!File.Exists(importFile))
                {
                    MessageBox.Show("You have not selected a valid file!");
                    return;
                }
                LRLE tmp = null;
                using (FileStream myStream = new FileStream(importFile, FileMode.Open, FileAccess.Read))
                {
                    if (String.Compare(Path.GetExtension(importFile).ToLower(), ".png") == 0)
                    {
                        Bitmap img = new Bitmap(myStream);
                        tmp = new LRLE(img);
                    }
                    else if (String.Compare(Path.GetExtension(importFile).ToLower(), ".dds") == 0)
                    {
                        DdsFile dds = new DdsFile();
                        dds.Load(myStream, false);
                        Bitmap img = new Bitmap(dds.Image);
                        tmp = new LRLE(img);
                    }
                    else
                    {
                        MessageBox.Show("Unrecognized image type!");
                        return;
                    }
                }
                Predicate<IResourceIndexEntry> texture = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 &
                    r.Instance == detailInstance[index];
                IResourceIndexEntry iresTexture = detailsPack.Find(texture);
                if (iresTexture != null) DeleteResource(detailsPack, iresTexture);
                texture = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE &
                    r.Instance == detailInstance[index];
                IResourceIndexEntry iresLRLE = detailsPack.Find(texture);
                if (iresLRLE != null)
                {
                    ReplaceResource(detailsPack, iresLRLE, tmp.Stream);
                }
                else
                {
                    TGIBlock tgi = new TGIBlock(1, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0, detailInstance[index]);
                    detailsPack.AddResource(tgi, tmp.Stream, true);
                }
                StartDetailsPreview(detailsIndex, false);
            }
        }

        private void CustomSkinDetailsSave_button_Click(object sender, EventArgs e)
        {
            if (detailsPack != null)
            {
                detailsPack.SavePackage();
            }
            else
            {
                MessageBox.Show("You don't have an open package to save!");
            }
        }

        private void CustomSkinDetailsSaveAs_button_Click(object sender, EventArgs e)
        {
            if (detailsPack != null)
            {
                string newName;
                if (!WritePackage("Save new package", detailsPack, "", out newName))
                {
                    MessageBox.Show("Could not save package!");
                    return;
                }
                DetailsPackageEditFile.Text = newName;
                detailsPack.Dispose();
                detailsPack = OpenPackage(newName, true);
            }
            else
            {
                MessageBox.Show("You don't have an open package to save!");
            }
        }

        private void ConvertPackageFile_button_Click(object sender, EventArgs e)
        {
            ConvertPackageFile.Text = GetFilename("Select Skin Package to Convert", Packagefilter);
        }

        private void ConvertGo_button_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ConvertPackageFile.Text))
            {
                MessageBox.Show("You have not selected a valid package file!");
                return;
            }
            Package convertPack = OpenPackage(ConvertPackageFile.Text, true);
            if (convertPack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            Predicate<IResourceIndexEntry> isSkin = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2;
            List<IResourceIndexEntry> iresSkins = convertPack.FindAll(isSkin);
            bool found = false;
            for (int ir = 0; ir < iresSkins.Count; ir++ )
            {
                bool foundIn = false;
                for (int i = 0; i < oldAdultInstance.Length; i++)
                {
                    if (iresSkins[ir].Instance == oldAdultInstance[i])
                    {
                        Stream s = convertPack.GetResource(iresSkins[ir]);
                        DeleteResource(convertPack, iresSkins[ir]);
                        TGIBlock newTgi = new TGIBlock(1, null, iresSkins[ir].ResourceType, iresSkins[ir].ResourceGroup, newAdultInstance[i]);
                        IResourceIndexEntry ires = convertPack.AddResource(newTgi, s, true);
                        found = true;
                        foundIn = true;
                        break;
                    }
                }
                if (foundIn) continue;
                for (int i = 0; i < TYAEInstance.Length; i++)
                {
                    if (iresSkins[ir].Instance == TYAEInstance[i])
                    {
                        Stream s = convertPack.GetResource(iresSkins[ir]);
                        DeleteResource(convertPack, iresSkins[ir]);
                        TGIBlock newTgi = new TGIBlock(1, null, iresSkins[ir].ResourceType, iresSkins[ir].ResourceGroup, TYAEOverlay[i]);
                        IResourceIndexEntry ires = convertPack.AddResource(newTgi, s, true);
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
            {
                MessageBox.Show("No default skin detail files found to convert!");
                return;
            }
            if (!WritePackage("Save as a new package", convertPack, ""))
            {
                MessageBox.Show("Could not save package!");
                return;
            }
            convertPack.Dispose();
        }
    }
}
