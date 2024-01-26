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
using s4pi.DataResource;

namespace XMODS
{
    public partial class Form1 : Form
    {
        string TS4SkininatorVersion = "2.6.1.0";

        string Packagefilter = "Package files (*.package)|*.package|All files (*.*)|*.*";
        string ImageFilter = "PNG files (*.png)|*.png|DDS files (*.dds)|*.dds|All files (*.*)|*.*";
        Package myPack, clonePack;
        List<Package> resourcePacks;
        Package[] gamePacks0, gamePacksOther;
        string[] gamePacks0PackIDs;
        ItemCollection[] TONEitems;
        List<IResourceIndexEntry> foundResList = new List<IResourceIndexEntry>();
        Random ran;

        uint latestVersion = new TONE().LatestVersion;

        public static List<string> tagCategoryNames;
        public static List<uint> tagCategoryNumbers;
        public static List<string> tagNames;
        public static List<uint> tagNumbers;
        public static string[] tagCategoryNamesSkin;
        public static uint[] tagCategoryNumbersSkin;

        public static List<ulong> skintoneTuning;
        public static List<string> skintoneTuningNames;
        public static int tuningHuman;

        public enum ImageType
        {
            TextureDDS,
            TextureRLE,
            TextureLRLE,
            Overlay
        }

        public class TONEinfo : TONE
        {
            public IResourceIndexEntry resourceEntry;
            public string selected;
            public SkinStates[] skinStates;

            public TONEinfo(uint version) : base(version)
            {
            }

            public TONEinfo(BinaryReader br, IResourceIndexEntry ires, string isSelected)
                : base(br)
            {
                resourceEntry = ires;
                selected = isSelected;
            }

            public class SkinStates : TONE.SkinSetDesc
            {
                internal LRLE skinTexture;
                internal RLEResource maskTexture;
                internal bool skinCloned, skinImported;
                internal bool maskCloned, maskImported;
                public Image skinImage { get { return (this.skinTexture != null ? this.skinTexture.image : null); } }
                public Image maskImage
                {
                    get
                    {
                        if (this.maskTexture == null) return null;
                        else 
                        {
                            DdsFile dds = new DdsFile();
                            dds.Load((this.maskTexture as RLEResource).ToDDS(), false);
                            return dds.Image;
                        }
                }
            }

            internal SkinStates(TONE.SkinSetDesc other, LRLE skin, RLEResource mask, bool skinCloned, bool skinImported, bool maskCloned, bool maskImported)
                    : base(other)
                {
                    this.skinTexture = skin;
                    this.maskTexture = mask;
                    this.skinCloned = skinCloned;
                    this.skinImported = skinImported;
                    this.maskCloned = maskCloned;
                    this.maskImported = maskImported;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();

            this.Text = "TS4 Skininator v" + TS4SkininatorVersion;
            CASTools_tabControl.TabPages.Remove(FaceOverlays_tabPage);
            cloneWait_label.Location = new Point(420, 260);

            try
            {
                if (String.Compare(Properties.Settings.Default.TS4Path, " ") <= 0)
                {
                    string tmp = (string)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Maxis\\The Sims 4", "Install Dir", null);
                    if (tmp != null) Properties.Settings.Default.TS4Path = tmp;
                    //MessageBox.Show(tmp);
                    Properties.Settings.Default.Save();
                }
                if (String.Compare(Properties.Settings.Default.TS4UserPath, " ") <= 0)
                {
                    string tmp = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\The Sims 4";
                    if (tmp != null)
                    {
                        if (!Directory.Exists(tmp))
                        {
                            string[] tmp2 = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts", "localthumbcache.package", SearchOption.AllDirectories);
                            if (tmp2.Length > 0)
                            {
                                tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            }
                        }
                        Properties.Settings.Default.TS4UserPath = tmp;
                    }
                    //MessageBox.Show(tmp);
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in detecting game and/or user file paths: " + e.Message + Environment.NewLine + e.StackTrace);
            }
            if (String.Compare(Properties.Settings.Default.Creator, "anon") == 0 | Properties.Settings.Default.TS4Path == null)
            {
                Form f = new CreatorPrompt((String.Compare(Properties.Settings.Default.Creator, "anon") == 0) ? "" : Properties.Settings.Default.Creator, Properties.Settings.Default.TS4Path, Properties.Settings.Default.TS4UserPath);
                f.ShowDialog();
            }

            try
            {
                tagCategoryNames = Xmods.DataLib.PropertyTags.tagCategoryString;
                tagCategoryNumbers = Xmods.DataLib.PropertyTags.tagCategory;
                tagNames = Xmods.DataLib.PropertyTags.tagString;
                tagNumbers = Xmods.DataLib.PropertyTags.tag;
                Xmods.DataLib.PropertyTags.ParseSpecialCategories("S4_SkintoneProperties.xml", out tagCategoryNamesSkin, out tagCategoryNumbersSkin);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                //  Application.Exit();
            }

            if (SetupGamePacks())
            {
                PackFilter_comboBox.SelectedIndex = 0;
                SortCategories(tagCategoryNamesSkin, tagCategoryNumbersSkin);
            }
            else
            {
                MessageBox.Show("Skininator was unable to open the game packages; cloning cannot be done.");
            }

           // string[] deltas = Directory.GetFiles(Properties.Settings.Default.TS4Path, "ClientDeltaBuild0.package", SearchOption.AllDirectories);
           // foreach (string s in deltas)
           // {
           // }

            ran = new Random();
            defSkinData = LoadDetailSkins();
            SetupEASkinDetailsList();

            Predicate<IResourceIndexEntry> idata = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DATA &
                                    r.ResourceGroup == 0x00C36C35;
            skintoneTuning = new List<ulong>();
            skintoneTuningNames = new List<string>();
            if (gamePacks0 == null) return;
            foreach (Package p in gamePacks0)
            {
                List<IResourceIndexEntry> dfl = p.FindAll(idata);
                foreach (IResourceIndexEntry ir in dfl)
                {
                    if (!skintoneTuning.Contains(ir.Instance))
                    {
                        DataResource dres = new DataResource(0, p.GetResource(ir));
                        bool named = false;
                        foreach (DataResource.Data d in dres.DataTable)
                        {
                            if (d.Name.Length > 0)
                            {
                                skintoneTuning.Add(ir.Instance);
                                skintoneTuningNames.Add(d.Name.Replace("client_CAS", ""));
                                named = true;
                            }
                        }
                        if (!named)
                        {
                            skintoneTuning.Add(ir.Instance);
                            skintoneTuningNames.Add(ir.Instance.ToString("x16"));
                        }
                    }
                }
            }
            for (int i = 0; i < skintoneTuning.Count; i++)
            {
                cloneTuningRef_comboBox.Items.Add(skintoneTuningNames[i]);
            }
            tuningHuman = skintoneTuningNames.FindIndex(x => x.Contains("Human"));

            previewSkinState_comboBox.SelectedIndexChanged -= previewSkinState_comboBox_SelectedIndexChanged;
            previewSkinState_comboBox.SelectedIndex = 0;
            previewSkinState_comboBox.SelectedIndexChanged += previewSkinState_comboBox_SelectedIndexChanged;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TS4 Skininator " + TS4SkininatorVersion + Environment.NewLine +
                "by cmar" + Environment.NewLine +
                "This is free software available from modthesims.info!");
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TS4 Skininator, a tool for the creation of default and non-default" + Environment.NewLine +
                "custom skintones for The Sims 4." + Environment.NewLine +
                "Copyright (C) 2015  C. Marinetti" + Environment.NewLine +
                "This program comes with ABSOLUTELY NO WARRANTY. This is" + Environment.NewLine +
                "free software, and you are welcome to redistribute it under" + Environment.NewLine +
                "certain conditions. See the GNU-3.0 license included with this" + Environment.NewLine +
                "distribution for details.");
        }

        private void systemInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var os = (from x in new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<System.Management.ManagementObject>()
                      select x.GetPropertyValue("Caption")).FirstOrDefault();
            string info = "OS: " + (os != null ? os.ToString().Trim() : "Unknown ") + (Environment.Is64BitOperatingSystem ? " x64" : " x32") + Environment.NewLine;
            var processor = (from x in new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get().OfType<System.Management.ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            info += "Processor: " + (processor != null ? processor.ToString() : "Unknown ") + Environment.NewLine;
            var video = (from x in new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_VideoController").Get().OfType<System.Management.ManagementObject>()
                             select x.GetPropertyValue("Caption")).FirstOrDefault();
            info += "Video: " + (video != null ? video.ToString() : "Unknown ") + Environment.NewLine;
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.Bounds;
            info += "Screen resolution: " + area.Width.ToString() + "x" + area.Height.ToString() + Environment.NewLine;
            Graphics graphics = this.CreateGraphics();
            info += "DPI: " + graphics.DpiX.ToString() + Environment.NewLine;
            string[] gamepath = Directory.GetFiles(Properties.Settings.Default.TS4UserPath, "GameVersion.txt", SearchOption.AllDirectories);
            byte[] gameversion = new byte[16];
            using (FileStream myStream = new FileStream(gamepath[0], FileMode.Open, FileAccess.Read))
            {
                myStream.Read(gameversion, 0, 16);
            }
            info += "Game version: ";
            for (int i = 4; i < gameversion.Length; i++)
            {
                info += (char)gameversion[i];
            }
            info += Environment.NewLine;
            info += "TS4 Skininator version: " + TS4SkininatorVersion;
            MessageBox.Show(info);
        }

        private void changeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new CreatorPrompt(Properties.Settings.Default.Creator, Properties.Settings.Default.TS4Path, Properties.Settings.Default.TS4UserPath);
            f.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        internal string GetFilename(string title, string filter)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = filter;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Title = title;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal Package OpenPackage(string packagePath, bool readwrite)
        {
            try
            {
                Package package = (Package)Package.OpenPackage(0, packagePath, readwrite);
                return package;
            }
            catch
            {
                MessageBox.Show("Unable to read valid package data from " + packagePath);
                return null;
            }
        }

        internal bool WritePackage(string title, Package pack, string defaultFilename)
        {
            string tmp;
            return WritePackage(title, pack, defaultFilename, out tmp);
        }
        
        internal bool WritePackage(string title, Package pack, string defaultFilename, out string newFilename)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = Packagefilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "package";
            saveFileDialog1.OverwritePrompt = true;
            if (String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pack.SaveAs(saveFileDialog1.FileName);
                    newFilename = saveFileDialog1.FileName;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    newFilename = saveFileDialog1.FileName;
                    return false;
                }
            }
            newFilename = "";
            return false;
        }

        internal class ItemCollection
        {
            internal UInt64 TONEinstance;
            internal string pack;
            internal Package TONEpackage;
            internal Color[] TONEcolor;
            internal float sortOrder;

            internal ItemCollection(ulong toneID, string gamePack, Package package, Color[] color, float sortOrder)
            {
                this.TONEinstance = toneID;
                this.pack = gamePack;
                this.TONEpackage = package;
                this.TONEcolor = color;
                this.sortOrder = sortOrder;
            }
        }

        internal bool TONEitemExists(List<ItemCollection> toneList, ulong toneID)
        {
            for (int i = 0; i < toneList.Count; i++)
            {
                if (toneID == toneList[i].TONEinstance) return true;
            }
            return false;
        }

        private void SelectPack_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            PackSet_radioButton_Changed();
        }

        private void GamePack_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            PackSet_radioButton_Changed();
        }

        internal void PackSet_radioButton_Changed()
        {
            if (SelectPack_radioButton.Checked)
            {
                SelectPackFile.Enabled = true;
                SelectPack_button.Enabled = true;
                ToneList_dataGridView.Rows.Clear();
                New_radioButton.Checked = true;
                Default_radioButton.Enabled = false;
            }
            else if (GamePack_radioButton.Checked)
            {
                SelectPackFile.Enabled = false;
                SelectPackFile.Text = "";
                SelectPack_button.Enabled = false;
                Default_radioButton.Enabled = true;
                SelectPackFile.Text = "";
                resourcePacks = new List<Package>(gamePacks0);
                resourcePacks.AddRange(gamePacksOther);
                ReadSourcePackageTONEs();
                PopulateItemsList();
            }
        }

        private bool SetupGamePacks()
        {
            string TS4FilesPath = Properties.Settings.Default.TS4Path;
            string[] paths0;
            try
            {
                paths0 = Directory.GetFiles(TS4FilesPath, "Client*Build0.package", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Your game packages path is invalid! Please go to File / Change Settings and correct it or make it blank to reset, then restart.");
                return false;
            }
            catch (IOException)
            {
                MessageBox.Show("Your game packages path is invalid or a network error has occurred! Please go to File / Change Settings and correct it or make it blank to reset, then restart.");
                return false;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Your game packages path is not specified correctly! Please go to File / Change Settings and correct it or make it blank to reset, then restart.");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You do not have the required permissions to access the game packages folder! Please restart with admin privileges.");
                return false;
            }
            if (paths0.Length == 0)
            {
                MessageBox.Show("Can't find game packages! Please go to File / Change Settings and correct the game packages path or make it blank to reset, then restart.");
                return false;
            }
            try
            {
                Array.Sort(paths0);
                gamePacks0 = new Package[paths0.Length];
                gamePacks0PackIDs = new string[paths0.Length];
                PackFilter_comboBox.Items.Add("All");
                PackFilter_comboBox.Items.Add("BaseGame");
                for (int i = 0; i < paths0.Length; i++)
                {
                    gamePacks0[i] = OpenPackage(paths0[i], false);
                    if (gamePacks0[i] == null)
                    {
                        MessageBox.Show("Can't read game packages!");
                        return false;
                    }
                    if (paths0[i].IndexOf("\\Data") < 0)
                    {
                        string tmp = Path.GetDirectoryName(paths0[i]);
                        tmp = tmp.Substring(tmp.LastIndexOf("\\") + 1);
                        if (!PackFilter_comboBox.Items.Contains(tmp)) PackFilter_comboBox.Items.Add(tmp);
                        gamePacks0PackIDs[i] = tmp;
                    }
                    else
                    {
                        gamePacks0PackIDs[i] = "BaseGame";
                    }
                }

                string[] paths6 = Directory.GetFiles(TS4FilesPath, "Client*Build6.package", SearchOption.AllDirectories);
                string[] paths7 = Directory.GetFiles(TS4FilesPath, "Client*Build7.package", SearchOption.AllDirectories);
                string[] paths8 = Directory.GetFiles(TS4FilesPath, "Client*Build8.package", SearchOption.AllDirectories);
                if (paths6.Length == 0 || paths7.Length == 0 || paths8.Length == 0)
                {
                    MessageBox.Show("Can't find game packages! Please go to File / Change Settings and correct the game packages path or make it blank to reset, then restart.");
                    return false;
                }
                List<string> paths = new List<string>(paths6);
                paths.AddRange(paths7);
                paths.AddRange(paths8);
                paths.Sort();
                gamePacksOther = new Package[paths.Count];
                for (int i = 0; i < paths.Count; i++)
                {
                    gamePacksOther[i] = OpenPackage(paths[i], false);
                    if (gamePacksOther[i] == null)
                    {
                        MessageBox.Show("Can't read game packages!");
                        return false;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You do not have the required permissions to open the game packages! Please restart with admin privileges.");
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            return true;
        }

        private void SelectPack_button_Click(object sender, EventArgs e)
        {
            SelectPackFile.Text = GetFilename("Select Package File", Packagefilter);
            if (!File.Exists(SelectPackFile.Text))
            {
                MessageBox.Show("You have not selected a valid package file!");
                return;
            }
            myPack = OpenPackage(SelectPackFile.Text, false);
            if (myPack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            resourcePacks = new List<Package>(gamePacks0);
            resourcePacks.AddRange(gamePacksOther);
            ReadSourcePackageTONEs();
            PopulateItemsList();
        }

        private void PackFilter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamePack_radioButton.Checked) PopulateItemsList();
        }

        private void ReadSourcePackageTONEs()
        {
            Predicate<IResourceIndexEntry> isTONE = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.TONE;
            List<ItemCollection> tmpItems = new List<ItemCollection>();

            Package[] packs;
            if (SelectPack_radioButton.Checked)
            {
                packs = new Package[] { myPack };
            }
            else
            {
                packs = gamePacks0; 
            }
            for (int i = 0; i < packs.Length; i++)
            {
                List<IResourceIndexEntry> TONElist = packs[i].FindAll(isTONE);
                foreach (IResourceIndexEntry rtone in TONElist)
                {
                    Stream s = packs[i].GetResource(rtone);
                    s.Position = 0;
                    BinaryReader br = new BinaryReader(s);
                    TONE tone = null;
                    try
                    {
                        tone = new TONE(br);
                    }
                    catch
                    {
                        MessageBox.Show("Can't read this TONE: " + rtone.ToString());
                        continue;
                    }

                    if (!TONEitemExists(tmpItems, rtone.Instance))
                    {
                        tmpItems.Add(new ItemCollection(rtone.Instance, gamePacks0PackIDs[i], packs[i], tone.ColorList, tone.SortOrder));
                    }
                }
            }

            if (tmpItems.Count == 0)
            {
                MessageBox.Show("No TONE files found!");
                return;
            }
            TONEitems = tmpItems.ToArray();
        }
        
        private void PopulateItemsList()
        {
            if (TONEitems == null) return;
            ToneList_dataGridView.Rows.Clear();

            for (int i = 0; i < TONEitems.Length; i++)
            {
                if (GamePack_radioButton.Checked)
                {
                    string packFilterValue = ((string)PackFilter_comboBox.SelectedItem);
                    if (!(String.Compare(packFilterValue, "All") == 0))
                    {
                        if (String.Compare(TONEitems[i].pack, packFilterValue) != 0)
                        {
                            continue;
                        }
                    }
                } 
                int currentRow = ToneList_dataGridView.Rows.Add();
                ToneList_dataGridView.Rows[currentRow].Cells["TONEsort"].Value = TONEitems[i].sortOrder.ToString().PadLeft(3);
                ToneList_dataGridView.Rows[currentRow].Cells["GamePack"].Value = TONEitems[i].pack;
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                if (TONEitems[i].TONEcolor.Length > 0) style.BackColor = Color.FromArgb(TONEitems[i].TONEcolor[0].ToArgb());
                ToneList_dataGridView.Rows[currentRow].Cells["SkinColor"].Style = style;
                ToneList_dataGridView.Rows[currentRow].Tag = i;
            }
            ToneList_dataGridView.Sort(new TonesSorter());
        }

        private class TonesSorter : System.Collections.IComparer
        {
            public int Compare(object obj1, object obj2)
            {
                DataGridViewRow row1 = (DataGridViewRow)obj1;
                DataGridViewRow row2 = (DataGridViewRow)obj2;
                int tmp = String.Compare(row1.Cells["GamePack"].Value.ToString(), row2.Cells["GamePack"].Value.ToString());
                if (tmp == 0)
                {
                    return String.Compare(row1.Cells["TONEsort"].Value.ToString(), row2.Cells["TONEsort"].Value.ToString());
                }
                else
                {
                    return tmp;
                }
            }
        }

        private void CloneAll_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in ToneList_dataGridView.Rows)
            {
                r.Cells["CloneTone"].Value = true;
            }
        }

        private void CloneNone_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in ToneList_dataGridView.Rows)
            {
                r.Cells["CloneTone"].Value = false;
            }
        }

        private void PackageEditFile_button_Click(object sender, EventArgs e)
        {
            if (clonePack != null)
            {
                DialogResult res = MessageBox.Show("Do you want to close the currently open package and open a new one?", "Open a new package", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return;
                if (!CheckForUnsaved()) return;
                clonePack.Dispose();
            }
            string s = GetFilename("Select Package File", Packagefilter);
            if (!File.Exists(s))
            {
                MessageBox.Show("You have not selected a valid package file!");
                return;
            }
            PackageEditFile.Text = s;
            OpenPackageForEdit(true);
        }

        private void OpenPackageForEdit(bool verbose)
        {
            clonePack = OpenPackage(PackageEditFile.Text, true);
            if (clonePack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            Predicate<IResourceIndexEntry> isTONE = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.TONE;
            cloneWait_label.Visible = true;
            cloneWait_label.Refresh();
            clonePackTONEs = new List<TONEinfo>();
            List<IResourceIndexEntry> iresTONEs = clonePack.FindAll(isTONE);
            if (iresTONEs.Count == 0)
            {
                MessageBox.Show("This package has no TONEs in it!");
                ClonePackageWipe();
                OverlaysWipe();
                skintonePreviewer1.Stop_Mesh();
                previewSkinColor_pictureBox.BackColor = Color.White;
                cloneWait_label.Visible = false;
                return;
            }
            bool hasLowerVersion = false;
            bool hasObsoleteTextures = false;
            List<ulong> converted = new List<ulong>();

            int convertTextureCount = 0;
            foreach (IResourceIndexEntry ir in iresTONEs)
            {
                Stream sc = clonePack.GetResource(ir);
                sc.Position = 0;
                BinaryReader brc = new BinaryReader(sc);
                try
                {
                    ResourceIndexEntry irCopy = (ResourceIndexEntry)ir;
                    TONEinfo tone = new TONEinfo(brc, irCopy, "F");
                    clonePackTONEs.Add(tone);
                    if (!hasLowerVersion) hasLowerVersion = tone.Version < latestVersion;
                    for (int i = 0; i < 3; i++)
                    {
                        Predicate<IResourceIndexEntry> isLrle = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE && r.Instance == tone.GetSkinSetTextureInstance(i);
                        IResourceIndexEntry iresLrle = clonePack.Find(isLrle);
                        Predicate<IResourceIndexEntry> isTexture = r => r.Instance == tone.GetSkinSetTextureInstance(i);
                        List<IResourceIndexEntry> iresTex = clonePack.FindAll(isTexture);
                        if (iresLrle == null && iresTex.Count > 0)
                        {
                            hasObsoleteTextures = true;
                            convertTextureCount++;
                        }
                        Predicate<IResourceIndexEntry> isRle = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 && r.Instance == tone.GetSkinSetOverlayInstance(i);
                        IResourceIndexEntry iresRle = clonePack.Find(isRle);
                        isTexture = r => r.Instance == tone.GetSkinSetOverlayInstance(i);
                        iresTex = clonePack.FindAll(isTexture);
                        if (iresRle == null && iresTex.Count > 0)
                        {
                            hasObsoleteTextures = true;
                            convertTextureCount++;
                        }
                    }
                    for (int i = 0; i < tone.OverlayList.Count; i++)
                    {
                        ulong instance = tone.GetOverLayInstance(i);
                        if (converted.Contains(instance)) continue;
                        else converted.Add(instance);
                        Predicate<IResourceIndexEntry> isRLE = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 && r.Instance == instance;
                        IResourceIndexEntry iresRLE = clonePack.Find(isRLE);
                        Predicate<IResourceIndexEntry> isTexture = r => r.Instance == instance;
                        List<IResourceIndexEntry> iresTex = clonePack.FindAll(isTexture);
                        if (iresRLE == null && iresTex.Count > 0)
                        {
                            hasObsoleteTextures = true;
                            convertTextureCount++;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Can't read the TONE: " + ir.ToString());
                }
            }
            if (hasLowerVersion)
            {
                if (verbose) MessageBox.Show("Outdated versions of skintones will be updated to the latest version.");
                for (int i = 0; i < clonePackTONEs.Count; i++)
                {
                    clonePackTONEs[i].UpdateToLatestVerson();
                    MemoryStream m = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(m);
                    clonePackTONEs[i].Write(bw);
                    m.Position = 0;
                    ReplaceResource(clonePack, clonePackTONEs[i].resourceEntry, m);
                }
            }
            if (hasObsoleteTextures)
            {
                if (verbose) MessageBox.Show("About " + convertTextureCount.ToString() + 
                    " outdated textures will be updated to the latest standard. This may take a while.");
                Convert_progressBar.Minimum = 0;
                Convert_progressBar.Maximum = convertTextureCount;
                Convert_progressBar.Step = 1;
                Convert_progressBar.Value = 0;
                Convert_progressBar.Visible = true;
                Convert_progressBar.Refresh();
                converted = new List<ulong>();
                for (int i = 0; i < clonePackTONEs.Count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        ulong instance = clonePackTONEs[i].GetSkinSetTextureInstance(j);
                        Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE &
                            r.Instance == instance;
                        IResourceIndexEntry ctex = clonePack.Find(itex);            //searches for LRLE, then RLE disguised as LRLE, then DDS in cloned package
                        if (ctex == null)
                        {
                            bool found = false;
                            Predicate<IResourceIndexEntry> rtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 & r.Instance == instance;
                            ctex = clonePack.Find(rtex);
                            if (ctex != null)
                            {
                                try
                                {
                                    LRLE tmp = new LRLE(new BinaryReader(clonePack.GetResource(ctex)));
                                    IResourceKey ikLRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0x00000000, instance);
                                    IResourceIndexEntry iresLRLE = clonePack.AddResource(ikLRLE, tmp.Stream, true);
                                    iresLRLE.Compressed = (ushort)0x5A42;
                                    Convert_progressBar.PerformStep();
                                    found = true;
                                }
                                catch { }
                            }
                            if (!found)
                            {
                                Predicate<IResourceIndexEntry> dtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &
                                    r.Instance == instance;
                                ctex = clonePack.Find(dtex);
                                if (ctex != null)
                                {
                                    DdsFile tmp = new DdsFile();
                                    tmp.Load(clonePack.GetResource(ctex), false);
                                    LRLE lrle = new LRLE(tmp.Image);
                                    IResourceKey ikLRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.LRLE, 0x00000000, instance);
                                    IResourceIndexEntry iresLRLE = clonePack.AddResource(ikLRLE, lrle.Stream, true);
                                    iresLRLE.Compressed = (ushort)0x5A42;
                                    Convert_progressBar.PerformStep();
                                }
                            }
                        }
                        DeleteCloneDups(instance, XmodsEnums.ResourceTypes.LRLE);

                        instance = clonePackTONEs[i].GetSkinSetOverlayInstance(j);
                        itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 & r.Instance == instance;
                        ctex = clonePack.Find(itex); 
                        if (ctex == null)
                        {
                            Predicate<IResourceIndexEntry> dtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &
                                r.Instance == instance;
                            ctex = clonePack.Find(dtex);
                            if (ctex != null)
                            {
                                DdsFile tmp = new DdsFile();
                                tmp.Load(clonePack.GetResource(ctex), false);
                                Stream s = new MemoryStream();
                                tmp.Save(s);
                                s.Position = 0;
                                RLEResource rle = new RLEResource(1, null);
                                rle.ImportToRLE(s);
                                IResourceKey ikRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000, instance);
                                IResourceIndexEntry iresRLE = clonePack.AddResource(ikRLE, rle.Stream, true);
                                iresRLE.Compressed = (ushort)0x5A42;
                                Convert_progressBar.PerformStep();
                            }
                        }
                        DeleteCloneDups(instance, XmodsEnums.ResourceTypes.DXT5RLE2);
                    }

                    for (int j = 0; j < clonePackTONEs[i].OverlayList.Count; j++)
                    {
                        ulong instance = clonePackTONEs[i].GetOverLayInstance(j);
                        if (converted.Contains(instance)) continue;
                        Predicate<IResourceIndexEntry> itex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 & 
                            r.Instance == instance;
                        IResourceIndexEntry ctex = clonePack.Find(itex);            //searches for RLE2, then DDS in cloned package
                        if (ctex == null)
                        {
                            Predicate<IResourceIndexEntry> dtex = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed &
                                r.Instance == instance;
                            ctex = clonePack.Find(dtex);
                            if (ctex != null)
                            {
                                DdsFile tmp = new DdsFile();
                                tmp.Load(clonePack.GetResource(ctex), false);
                                Stream s = new MemoryStream();
                                tmp.Save(s);
                                s.Position = 0;
                                RLEResource rle = new RLEResource(1, null);
                                rle.ImportToRLE(s);
                                IResourceKey ikRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000, instance);
                                IResourceIndexEntry iresRLE = clonePack.AddResource(ikRLE, s, true);
                                iresRLE.Compressed = (ushort)0x5A42;
                                Convert_progressBar.PerformStep();
                            }
                        }
                        DeleteCloneDups(instance, XmodsEnums.ResourceTypes.DXT5RLE2);
                    }

                }
                Convert_progressBar.Visible = false;
            }

            if (hasObsoleteTextures)
            {
                clonePackTONEs = new List<TONEinfo>();
                foreach (IResourceIndexEntry ir in iresTONEs)
                {
                    Stream sc = clonePack.GetResource(ir);
                    sc.Position = 0;
                    BinaryReader brc = new BinaryReader(sc);
                    try
                    {
                        ResourceIndexEntry irCopy = (ResourceIndexEntry)ir;
                        TONEinfo tone = new TONEinfo(brc, irCopy, "F");
                        clonePackTONEs.Add(tone);
                    }
                    catch
                    {
                        //MessageBox.Show("Can't read the TONE: " + ir.ToString());
                    }
                }
            }

            myRow = 0;
            CloneTONEsList(true);
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
        }

        private void SaveLegacy_button_Click(object sender, EventArgs e)
        {
            
            if (clonePack != null)
            {
                if (!CheckForUnsaved()) return;
                Package lePack = (Package)Package.NewPackage(1);
                string newName;
                if (!WritePackage("Save Legacy Edition compatible package", lePack, "", out newName))
                {
                    MessageBox.Show("Could not save package!");
                    lePack.Dispose();
                    return;
                }
                lePack = (Package)Package.OpenPackage(1, newName, true);
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();

                List<IResourceIndexEntry> resList = clonePack.GetResourceList;
                int numTextures = 0;
                foreach (IResourceIndexEntry ires in resList)
                {
                    if (ires.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE || ires.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2 ||
                        ires.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed) numTextures++;
                }

                MessageBox.Show(numTextures.ToString() + " textures will be converted for LE compatibility. This may take a while.");
                Convert_progressBar.Minimum = 0;
                Convert_progressBar.Maximum = numTextures;
                Convert_progressBar.Step = 1;
                Convert_progressBar.Value = 0;
                Convert_progressBar.Visible = true;
                Convert_progressBar.Refresh();

                foreach (IResourceIndexEntry res in resList)
                {
                    if (res.ResourceType == (uint)XmodsEnums.ResourceTypes.TONE)
                    {
                        Stream r = clonePack.GetResource(res);
                        r.Position = 0;
                        TONE tone = new TONE(new BinaryReader(r));
                        tone.Version = 10;
                        Stream w = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(w);
                        tone.Write(bw);
                        IResourceIndexEntry ires = lePack.AddResource(res, w, true);
                        ires.Compressed = (ushort)0x5A42;
                    }
                    else if (res.ResourceType == (uint)XmodsEnums.ResourceTypes.LRLE)
                    {
                        Stream r = clonePack.GetResource(res);
                        r.Position = 0;
                        LRLE lrle = new LRLE(new BinaryReader(r));
                        DdsFile dds = new DdsFile();
                        dds.CreateImage(lrle.image, false);
                        dds.GenerateMipMaps();
                        dds.UseDXT = false;
                        Stream s = new MemoryStream();
                        dds.Save(s);
                        s.Position = 0;
                        IResourceKey ikDDS = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0x00000000, res.Instance);
                        IResourceIndexEntry iresDDS = lePack.AddResource(ikDDS, s, true);
                        iresDDS.Compressed = (ushort)0x5A42;
                        dds.UseDXT = true;
                        Stream s2 = new MemoryStream();
                        dds.Save(s2);
                        s2.Position = 0;
                        RLEResource rle = new RLEResource(1, null);
                        rle.ImportToRLE(s2);
                        IResourceKey ikRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000, res.Instance);
                        IResourceIndexEntry iresRLE = lePack.AddResource(ikRLE, rle.Stream, true);
                        iresRLE.Compressed = (ushort)0x5A42;
                        Convert_progressBar.PerformStep();
                    }
                    else if (res.ResourceType == (uint)XmodsEnums.ResourceTypes.DXT5RLE2)
                    {
                        Stream r = clonePack.GetResource(res);
                        r.Position = 0;
                        RLEResource rle = new RLEResource(1, r);
                        r.Position = 0;
                        IResourceIndexEntry ires = lePack.AddResource(res, r, true);
                        ires.Compressed = (ushort)0x5A42;
                        DdsFile dds = new DdsFile();
                        dds.Load(rle.ToDDS(), false);
                        Stream s = new MemoryStream();
                        dds.Save(s);
                        s.Position = 0;
                        IResourceKey ikDDS = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DDSuncompressed, 0x00000000, res.Instance);
                        IResourceIndexEntry iresDDS = lePack.AddResource(ikDDS, s, true);
                        iresDDS.Compressed = (ushort)0x5A42;
                        Convert_progressBar.PerformStep();
                    }
                    else if (res.ResourceType == (uint)XmodsEnums.ResourceTypes.DDSuncompressed)
                    {
                        Stream r = clonePack.GetResource(res);
                        r.Position = 0;
                        DdsFile dds = new DdsFile();
                        dds.Load(r, false);
                        r.Position = 0;
                        IResourceIndexEntry ires = lePack.AddResource(res, r, true);
                        ires.Compressed = (ushort)0x5A42;
                        dds.UseDXT = true;
                        Stream s = new MemoryStream();
                        dds.Save(s);
                        s.Position = 0;
                        RLEResource rle = new RLEResource(1, null);
                        rle.ImportToRLE(s);
                        IResourceKey ikRLE = new TGIBlock(0, null, (uint)XmodsEnums.ResourceTypes.DXT5RLE2, 0x00000000, res.Instance);
                        IResourceIndexEntry iresRLE = lePack.AddResource(ikRLE, rle.Stream, true);
                        iresRLE.Compressed = (ushort)0x5A42;
                        Convert_progressBar.PerformStep();
                    }
                    else
                    {
                        Stream r = clonePack.GetResource(res);
                        r.Position = 0;
                        IResourceIndexEntry ires = lePack.AddResource(res, r, true);
                        ires.Compressed = (ushort)0x5A42;
                    }
                }
                Convert_progressBar.Visible = false;
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                
                lePack.SavePackage();
                lePack.Dispose();
            }
            else
            {
                MessageBox.Show("You don't have an open package to save!");
            }
        }

        private void SavePackage_button_Click(object sender, EventArgs e)
        {
            if (clonePack != null)
            {
                cloneWait_label.Text = "Saving...";
                cloneWait_label.Refresh();
                if (CheckForUnsaved()) clonePack.SavePackage();
                clonePack.Dispose();
                cloneWait_label.Visible = false;
                cloneWait_label.Text = "Please Wait...";
                cloneWait_label.Refresh();
                OpenPackageForEdit(false);
            }
            else
            {
                MessageBox.Show("You don't have an open package to save!");
            }
        }

        private void SaveAsPackage_button_Click(object sender, EventArgs e)
        {
            if (clonePack != null)
            {
                if (!CheckForUnsaved()) return;
                string newName;
                cloneWait_label.Text = "Saving...";
                cloneWait_label.Refresh();
                if (!WritePackage("Save new package", clonePack, "", out newName))
                {
                    MessageBox.Show("Could not save package!");
                    return;
                }
                PackageEditFile.Text = newName;
                clonePack.Dispose();
                cloneWait_label.Visible = false;
                cloneWait_label.Text = "Please Wait...";
                cloneWait_label.Refresh();
                OpenPackageForEdit(false);
            }
            else
            {
                MessageBox.Show("You don't have an open package to save!");
            }
        }

        private void ClosePackage_button_Click(object sender, EventArgs e)
        {
            if (!CheckForUnsaved()) return;
            DialogResult res = MessageBox.Show("Are you sure you want to close the package?", "Package Close", MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                clonePack.Dispose();
                clonePack = null;
                ClonePackageWipe();
                OverlaysWipe();
                skintonePreviewer1.Stop_Mesh();
                previewSkinColor_pictureBox.BackColor = Color.White;
                PackageEditFile.Text = "";
            }
        }

        private bool CheckForUnsaved()
        {
            if (changesTone)
            {
                string unsaved = "";
                if (changesTone) unsaved += "You have unsaved changes to TONEs" + Environment.NewLine;
                if (changesOverlay) unsaved += "You have unsaved changes to overlays" + Environment.NewLine;
                unsaved += "Continue anyway?";
                DialogResult res = MessageBox.Show(unsaved, "Unsaved Changes", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return false;
            }
            return true;
        }

        private void DeleteResource(Package package, TGI tgiToDelete)
        {
            IResourceKey key = new TGIBlock(1, null, tgiToDelete.Type, tgiToDelete.Group, tgiToDelete.Instance);
            DeleteResource(package, key);
        }
        private void DeleteResource(Package package, IResourceIndexEntry keyToDelete)
        {
            DeleteResource(package, (IResourceKey)keyToDelete);
        }
        private void DeleteResource(Package package, IResourceKey keyToDelete)
        {
            Predicate<IResourceIndexEntry> idel = r => r.ResourceType == keyToDelete.ResourceType &
                    r.ResourceGroup == keyToDelete.ResourceGroup & r.Instance == keyToDelete.Instance;
            List<IResourceIndexEntry> iries = package.FindAll(idel);
            foreach (IResourceIndexEntry irie in iries)
            {
                package.DeleteResource(irie);
            }
            iries = package.FindAll(idel);
            if (iries.Count > 0) MessageBox.Show("DeleteResource didn't work correctly! " + iries.Count.ToString() + " are left.");
            foreach (IResourceIndexEntry irie in iries)
            {
                package.DeleteResource(irie);
            }
            iries = package.FindAll(idel);
            if (iries.Count > 0) MessageBox.Show("DeleteResource didn't work correctly! " + iries.Count.ToString() + " are still left.");
        }

        private void ReplaceResource(Package package, IResourceIndexEntry keyToReplace, Stream s)
        {
            IResource res = new Resource(0, s);
            package.ReplaceResource(keyToReplace, res);
            keyToReplace.Compressed = (ushort)0x5A42;
        }

        internal class Resource : AResource
        {
            internal Resource(int APIversion, Stream s) : base(APIversion, s) { }

            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            protected override Stream UnParse()
            {
                return this.stream;
            }
        }
    }
}
