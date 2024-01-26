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
using XMODS;
using s4pi.Interfaces;
using s4pi.Package;
using s4pi.ImageResource;

namespace XMODS
{
    public partial class ImageDisplayImportExport : Form
    {
        DdsFile myDDS = null;
        //RLEResource myRLE = null;
        Bitmap myImage = null;
        Bitmap returnImg = null;
        DdsFile returnDDS = null;
        string ImageFilter = "DDS or PNG files (*.dds; *.png)|*.dds; *.png|DDS files (*.dds)|*.dds|PNG files (*.png)|*.png|All files (*.*)|*.*";
        Form1.ImageType type;

        //public RLEResource ReturnRLE
        //{
        //    get { return returnRLE; }
        //}
        public Bitmap ReturnIMG
        {
            get { return returnImg; }
        }
        public DdsFile ReturnDDS
        {
            get { return returnDDS; }
        }

        public ImageDisplayImportExport(RLEResource rleImage, Form1.ImageType imageType, string windowTitle)
        {
            InitializeComponent();
            this.Text = windowTitle;
            Import_label.Text = "Import PNG or DDS/DXT5 files";
            if (rleImage != null)
            {
               // myRLE = new RLEResource(1, rleImage.Stream);
                myDDS = new DdsFile();
                myDDS.Load(rleImage.ToDDS(), false);
                myImage = myDDS.Image;
                DoSetup(myImage, imageType);
            }
            else
            {
                myDDS = null;
                myImage = null;
               // myRLE = null;
                DoSetup(null, imageType);
            }
        }

        public ImageDisplayImportExport(LRLE lrle, Form1.ImageType imageType, string windowTitle)
        {
            InitializeComponent();
            this.Text = windowTitle;
            Import_label.Text = "Import PNG or DDS/uncompressed files";
            myImage = lrle != null ? lrle.image : null;
            myDDS = null;
            DoSetup(myImage, imageType);
        }

        public ImageDisplayImportExport(DdsFile dds, Form1.ImageType imageType, string windowTitle)
        {
            InitializeComponent();
            this.Text = windowTitle;
            Import_label.Text = "Import PNG or DDS/uncompressed files";
            myDDS = dds;
            myImage = myDDS != null ? myDDS.Image : null;
            DoSetup(myImage, imageType);
        }

        internal void DoSetup(Image image, Form1.ImageType imageType)
        {
            if (image != null)
            {
                DisplayImage(image);
                cloneTextureBlank_button.Visible = true;
                cloneTextureBlank_button.Text = "Remove";
            }
            else
            {
                DisplayImage(null);
                cloneTextureExport_button.Enabled = false;
            }
            cloneTextureSave_button.Enabled = false;
            type = imageType;
        }

        private void DisplayImage(Image image)
        {
            if (image == null)
            {
                cloneTextureDimensions.Text = "No image";
                cloneTexture_pictureBox.Width = 175;
                cloneTexture_pictureBox.Height = 215;
                this.Width = 431;
                this.Height = 393;
                cloneTexture_pictureBox.Image = Properties.Resources.NullImage;
                cloneTexture_pictureBox.BackgroundImage = null;
                cloneTextureExport_button.Enabled = false;
                return;
            }
            cloneTextureDimensions.Text = image.Width.ToString() + "x" + image.Height.ToString();
            int h = Math.Min(image.Height + 150, Screen.FromControl(this).WorkingArea.Height / 2);
            float zoom = (float)h / (float)image.Height;
            if (zoom > 1) zoom = 1;
            int pboxWidth = (int)(image.Width * zoom);
            int pboxHeight = (int)(image.Height * zoom);
            this.Width = pboxWidth + 400 - 225;
            this.Height = Math.Max(pboxHeight + 350 - 225, 350);
            cloneTexture_pictureBox.Width = pboxWidth;
            cloneTexture_pictureBox.Height = pboxHeight;
            cloneTexture_pictureBox.Image = image;
            cloneTextureExport_button.Enabled = true;
        }

        private void cloneTextureExport_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = ImageFilter;
            saveFileDialog1.Title = "Save Texture Image File";
            saveFileDialog1.DefaultExt = "png";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.AddExtension = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (String.Compare(Path.GetExtension(saveFileDialog1.FileName).ToLower(), ".png") == 0)
                {
                    using (FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                    {
                        if (myImage != null) myImage.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                else
                {
                   // if (myDDS == null && myImage != null)
                    {
                        myDDS = new DdsFile();
                        myDDS.CreateImage(myImage, false);
                        if (type == Form1.ImageType.TextureRLE) myDDS.UseDXT = true;
                        else myDDS.UseDXT = false;
                        myDDS.GenerateMipMaps();
                    }
                    using (FileStream myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                    {
                        if (myDDS != null) myDDS.Save(myStream);
                        myStream.Close();
                    }
                }
            }
        }

        private void cloneTextureImport_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = ImageFilter;
            if (type == Form1.ImageType.TextureLRLE)
            {
                openFileDialog1.Title = "Select DDS or PNG Skintone Image File";
            }
            else if (type == Form1.ImageType.TextureRLE)
            {
                openFileDialog1.Title = "Select DXT5 DDS or PNG BurnMask Image File";
            }
            else if (type == Form1.ImageType.Overlay)
            {
                openFileDialog1.Title = "Select DDS or PNG Overlay Image File";
            }
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                {
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        Bitmap img = new Bitmap(myStream);
                        myImage = img;
                        returnImg = img;
                        myDDS = null;
                        returnDDS = null;
                        //Stream ddsMS = GetConvertedPNG(img, true);
                        //returnDDS = new DdsFile();
                        //returnDDS.Load(ddsMS, false);
                    }
                }
                else
                {
                    Stream ddsMS = new MemoryStream();
                    using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        //tmp.Load(myStream, false);
                        myStream.CopyTo(ddsMS);
                    }
                    //   DSTResource tmp2 = new DSTResource(1, tmp.Stream);
                    ddsMS.Position = 0;
                    returnDDS = new DdsFile();
                    returnDDS.Load(ddsMS, false);
                    if (returnDDS.MipMaps <= 1) returnDDS.GenerateMipMaps();
                    myDDS = returnDDS;
                    myImage = myDDS.Image;
                    returnImg = null;
                }
                    

                //else if (type == Form1.ImageType.TextureRLE)
                //{
                //    if (String.Compare(Path.GetExtension(openFileDialog1.FileName).ToLower(), ".png") == 0)
                //    {
                //        using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                //        {
                //            myImage = new Bitmap(myStream);
                //        }
                //    }
                //    else
                //    {
                //        using (FileStream myStream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                //        {
                //            DdsFile dds = new DdsFile();
                //            dds.Load(myStream, false);
                //            dds.UseDXT = true;
                //            dds.AlphaDepth = 5;
                //            if (dds.MipMaps <= 1) dds.GenerateMipMaps();
                //            myDDS = dds;
                //        }
                //    }
                //}

                DisplayImage(myImage != null ? myImage : null);
                cloneTextureSave_button.Enabled = true;
                cloneTextureBlank_button.Visible = true;
                cloneTextureBlank_button.Text = "Remove";
            }
        }

        private MemoryStream GetConvertedPNG(Image image, bool generateMipMaps)
        {
            DdsFile dds = new DdsFile();
            dds.UseDXT = false;
            dds.CreateImage(image, false);
            if (generateMipMaps) dds.GenerateMipMaps();
            MemoryStream m = new MemoryStream();
            dds.Save(m);
            return m;
        }

        private void cloneTextureBlank_button_Click(object sender, EventArgs e)
        {            
            returnDDS = null;
            returnImg = null;
            DisplayImage(null);
            cloneTextureSave_button.Enabled = true;
        }

        private void cloneTextureSave_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cloneTextureCancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
