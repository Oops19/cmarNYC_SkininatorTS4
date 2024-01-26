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
using System.Drawing.Drawing2D;

using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Xmods.DataLib;

namespace XMODS
{
    public partial class Form1 : Form
    {
        public void StartPreview()
        {
            XmodsEnums.Age age;
            XmodsEnums.Gender gender;
            Bitmap skinDetails;
            Image displayOverlay;
            int index = previewSkinState_comboBox.SelectedIndex;
            Bitmap textureImage = (Bitmap)myTONE.skinStates[index].skinImage;
            Bitmap burnMaskImage = PreviewBurnMask_checkBox.Checked ? (Bitmap)myTONE.skinStates[index].maskImage : null;
            float burnMultiplier = myTONE.skinStates[index].OverlayMultiplier;
            if (textureImage == null) textureImage = (Bitmap)myTONE.skinStates[0].skinImage;
            SetupAgeGender(out age, out gender);
            skinDetails = SetupSkinDetails(age, gender);
            displayOverlay = SetupSkinOverlay(age, gender);
            Image displayImage = DisplayableSkintone((Bitmap)textureImage, skinDetails, burnMaskImage, burnMultiplier, myTONE.Hue, myTONE.Saturation, myTONE.Opacity);
            skintonePreviewer1.Start_Mesh(age, gender, displayImage, displayOverlay,
                previewLight1_checkBox.Checked, previewLight2_checkBox.Checked, previewUndies_checkBox.Checked);
        }

        private Image DisplayableSkintone(Bitmap skintoneImage, Bitmap skinDetailsImage, Bitmap burnMaskImage, float burnMultiplier, ushort overlayHue, ushort overlaySaturation, uint Pass2Opacity)
        {
            if (skintoneImage == null | skinDetailsImage == null) return null;
            Bitmap skin = new Bitmap(skintoneImage, 1024, 2048);
            Bitmap details = new Bitmap(skinDetailsImage, 1024, 2048);
            Bitmap mask = burnMaskImage != null ? new Bitmap(burnMaskImage, 1024, 2048) : null;

            Rectangle rect1 = new Rectangle(0, 0, skin.Width, skin.Height);
            System.Drawing.Imaging.BitmapData bmpData1 = skin.LockBits(rect1, ImageLockMode.ReadWrite,
                skin.PixelFormat);
            IntPtr ptr1;
            if (bmpData1.Stride > 0) ptr1 = bmpData1.Scan0;
            else ptr1 = bmpData1.Scan0 + bmpData1.Stride * (skin.Height - 1);
            int bytes1 = Math.Abs(bmpData1.Stride) * skin.Height;
            byte[] argbValues1 = new byte[bytes1];
            System.Runtime.InteropServices.Marshal.Copy(ptr1, argbValues1, 0, bytes1);

            Rectangle rect2 = new Rectangle(0, 0, details.Width, details.Height);
            System.Drawing.Imaging.BitmapData bmpData2 = details.LockBits(rect2, ImageLockMode.ReadWrite,
                details.PixelFormat);
            IntPtr ptr2;
            if (bmpData2.Stride > 0) ptr2 = bmpData2.Scan0;
            else ptr2 = bmpData2.Scan0 + bmpData2.Stride * (details.Height - 1);
            int bytes2 = Math.Abs(bmpData2.Stride) * details.Height;
            byte[] argbValues2 = new byte[bytes2];
            System.Runtime.InteropServices.Marshal.Copy(ptr2, argbValues2, 0, bytes2);

            //System.Drawing.Imaging.BitmapData bmpData3 = null;
            //IntPtr ptr3;
            //int bytes3;
            //byte[] argbValues3 = new byte[0];
            //if (mask != null)
            //{
            //    Rectangle rect3 = new Rectangle(0, 0, mask.Width, mask.Height);
            //    bmpData3 = mask.LockBits(rect2, ImageLockMode.ReadWrite,
            //        mask.PixelFormat);
            //    if (bmpData3.Stride > 0) ptr3 = bmpData3.Scan0;
            //    else ptr3 = bmpData3.Scan0 + bmpData3.Stride * (mask.Height - 1);
            //    bytes3 = Math.Abs(bmpData3.Stride) * mask.Height;
            //    argbValues3 = new byte[bytes3];
            //    System.Runtime.InteropServices.Marshal.Copy(ptr3, argbValues3, 0, bytes3);
            //}

            float pass2opacity = Pass2Opacity / 100f;
            byte[] rgbOver = GetRGB(overlayHue, overlaySaturation, 100);
            //float overAlpha = overlaySaturation / 100f;

            for (int i = 0; i < argbValues1.Length; i += 4)
            {
                for (int j = 0; j < 3; j++)
                {
                    float tmp = 0;
                    if (argbValues1[i + j] > 128)           //first pass hard light blend, color over details
                        tmp = 255 - ((255f - 2f * (argbValues1[i + j] - 128f)) * (255f - argbValues2[i + j]) / 256f);
                    else
                        tmp = (2f * argbValues1[i + j] * argbValues2[i + j]) / 256f;

                    float tmp2;
                    if (tmp > 128)           //second pass overlay blend, details over color
                        tmp2 = 255 - ((255f - 2f * (argbValues2[i + j] - 128f)) * (255f - tmp) / 256f);
                    else
                        tmp2 = (2f * argbValues2[i + j] * tmp) / 256f;

                    // float tmp2 = 0;
                    //if (argbValues2[i + j] > 128)           //second pass pin light blend, details over color
                    //    tmp2 = ((Math.Max(argbValues1[i + j], argbValues2[i + j]) * .40f) + (argbValues1[i + j] * .60f));
                    //else
                    //    tmp2 = ((Math.Min(argbValues1[i + j], argbValues2[i + j]) * .40f) + (argbValues1[i + j] * .60f));

                    // float tmp2 = ((tmp / 255f) * (tmp + ((2f * argbValues2[i + j]) / 255f) * (255f - tmp)));  //2nd pass is soft light blend, details over color

                    // float tmp1 = 255f - (((255f - argbValues2[i + j]) * (255f - argbValues1[i + j])) / 255f);       // 2nd pass is screen/soft light?, details over color
                    //  float tmp2 = (((255f - argbValues1[i + j]) * (float)argbValues2[i + j] + tmp1) / 255f) * argbValues1[i + j];

                    tmp = ((tmp2 * pass2opacity) + (tmp * (1f - pass2opacity)));          // blend using 2nd pass opacity

                    if (overlaySaturation <= 100)
                        tmp = ((tmp / 255f) * (tmp + ((2f * rgbOver[2 - j]) / 255f) * (255f - tmp)));  //3rd pass is soft light blend, color over all

                    //if (mask != null)
                    //{
                    //    float tmp3 = ((tmp / 255f) * (tmp + ((2f * argbValues3[i + j]) / 255f) * (255f - tmp)));  //soft light blend, burn over all
                    //    //float tmp3;
                    //    //if (tmp > 128)
                    //    //    tmp3 = 255 - ((255f - 2f * (argbValues3[i + j] - 128f)) * (255f - tmp) / 256f);
                    //    //else
                    //    //    tmp3 = (2f * argbValues3[i + j] * tmp) / 256f;
                    //    float alpha = argbValues3[i + 3] / 255f * burnMultiplier;
                    //    tmp = ((tmp3 * alpha) + (tmp * (1f - alpha))); 
                    //}

                  //  if (tmp > 255) tmp = 255;
                  //  if (tmp < 0) tmp = 0;

                    argbValues1[i + j] = (byte)tmp;
                }
                if (overlaySaturation > 100)
                {
                    ushort[] hslTmp1 = GetHSL(argbValues1[i + 2], argbValues1[i + 1], argbValues1[i]);       // apply overlaid hue
                    byte[] rgbNew = GetRGB(overlayHue, hslTmp1[1], hslTmp1[2]);
                    for (int j = 0; j < 3; j++) argbValues1[i + j] = rgbNew[2 - j];
                }

                // ushort[] hslTmp2 = GetHSL(argbValues2[i + 2], argbValues2[i + 1], argbValues2[i]);
                // byte[] rgbOver = GetRGB((ushort)((overlayHue * hueAlpha) + (hslTmp1[0] * (1f - hueAlpha))), hslTmp1[1], hslTmp1[2]);
                // byte[] rgbOver = GetRGB(overlayHue, hslTmp1[1], hslTmp1[2]);

                //argbValues1[i] = (byte)((rgbOver[2] * hueAlpha) + (argbValues1[i] * (1f - hueAlpha)));
                //argbValues1[i + 1] = (byte)((rgbOver[1] * hueAlpha) + (argbValues1[i + 1] * (1f - hueAlpha)));
                //argbValues1[i + 2] = (byte)((rgbOver[0] * hueAlpha) + (argbValues1[i + 2] * (1f - hueAlpha)));

                //  argbValues1[i] = (byte)((Math.Max(rgbOver[2], argbValues1[i]) * hueAlpha) + (argbValues1[i] * (1f - hueAlpha)));
                //  argbValues1[i + 1] = (byte)((Math.Max(rgbOver[1], argbValues1[i + 1]) * hueAlpha) + (argbValues1[i + 1] * (1f - hueAlpha)));
                //  argbValues1[i + 2] = (byte)((Math.Max(rgbOver[0], argbValues1[i + 2]) * hueAlpha) + (argbValues1[i + 2] * (1f - hueAlpha)));
            }
            System.Runtime.InteropServices.Marshal.Copy(argbValues1, 0, ptr1, bytes1);
            skin.UnlockBits(bmpData1);
            details.UnlockBits(bmpData2);
            details.Dispose();
           // if (mask != null) mask.UnlockBits(bmpData3);
            //using (Graphics gr = Graphics.FromImage(skin))
            //{
            //    gr.DrawImage(skin, new Rectangle(0, 0, skin.Width, skin.Height), 0, 0, skin.Width, skin.Height, GraphicsUnit.Pixel, overAttributes);
            //}

            if (mask != null)
            {
                float[][] maskMatrix = { 
                   new float[] {1, 0, 0, 0, 0},       // m00 = red scaling factor
                   new float[] {0, 1, 0, 0, 0},       // m11 = green scaling factor
                   new float[] {0, 0, 1, 0, 0},       // m22 = blue scaling factor
                   new float[] {0, 0, 0, .20f, 0},    // m33 = alpha scaling factor
                   new float[] {0, 0, 0, 0, 1}        // increments for R, G, B, A
                };
                ColorMatrix maskConvert = new ColorMatrix(maskMatrix);
                ImageAttributes maskAttributes = new ImageAttributes();
                maskAttributes.SetColorMatrix(maskConvert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                using (Graphics gr = Graphics.FromImage(skin))
                {
                    gr.DrawImage(mask, new Rectangle(0, 0, skin.Width, skin.Height), 0, 0, skin.Width, skin.Height, GraphicsUnit.Pixel, maskAttributes);
                }
            }
            return skin;
        }

        private void SetupAgeGender(out XmodsEnums.Age age, out XmodsEnums.Gender gender)
        {
            if (previewInfant_radioButton.Checked) age = XmodsEnums.Age.Infant;
            else if (previewToddler_radioButton.Checked) age = XmodsEnums.Age.Toddler;
            else if (previewChild_radioButton.Checked) age = XmodsEnums.Age.Child;
            else if (previewTeen_radioButton.Checked) age = XmodsEnums.Age.Teen;
            else if (previewYA_radioButton.Checked) age = XmodsEnums.Age.YoungAdult;
            else if (previewAdult_radioButton.Checked) age = XmodsEnums.Age.Adult;
            else age = XmodsEnums.Age.Elder;
            if (previewMale_radioButton.Checked) gender = XmodsEnums.Gender.Male;
            else gender = XmodsEnums.Gender.Female;
        }

        private Bitmap SetupSkinDetails(XmodsEnums.Age age, XmodsEnums.Gender gender)
        {
            if ((age & XmodsEnums.Age.TeenToElder) > 0)
            {
                if (gender == XmodsEnums.Gender.Male)
                {
                    return Properties.Resources.MaleSkin;
                }
                else
                {
                    return Properties.Resources.FemaleSkin;
                }
            }
            else if (age == XmodsEnums.Age.Infant)
            {
                return Properties.Resources.InfantSkin;
            }
            else if (age == XmodsEnums.Age.Toddler)
            {
                return Properties.Resources.ToddlerSkin;
            }
            else
            {
                return Properties.Resources.ChildSkin;
            }
        }

        private Bitmap SetupSkinOverlay(XmodsEnums.Age age, XmodsEnums.Gender gender)
        {
            for (int i = 0; i < overlayList.Count; i++)
            {
                if (((uint)age & overlayList[i].age) > 0 && ((uint)gender & overlayList[i].gender) > 0 &&
                    overlayList[i].overlayInstance > 0ul && overlayList[i].overlay != null)
                {
                    return new Bitmap(overlayList[i].image);
                }
            }
            return null;
        }

        private void previewUndies_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            skintonePreviewer1.SetUndies(previewUndies_checkBox.Checked);
        }

        private void previewLight1_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            skintonePreviewer1.SetLights(previewLight1_checkBox.Checked, previewLight2_checkBox.Checked);
        }

        private void previewLight2_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            skintonePreviewer1.SetLights(previewLight1_checkBox.Checked, previewLight2_checkBox.Checked);
        }

        private void previewMale_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void previewFemale_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void previewTeen_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void previewAdult_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void previewYA_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void previewElder_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void previewChild_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void previewToddler_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setDisplayAgeGender();
        }

        private void setDisplayAgeGender()
        {
            if (myTONE == null) return;
            XmodsEnums.Age age;
            XmodsEnums.Gender gender;
            Bitmap skinDetails;
            Image displayOverlay;
            int index = previewSkinState_comboBox.SelectedIndex;
            Bitmap textureImage = (Bitmap)myTONE.skinStates[index].skinImage;
            Bitmap burnMaskImage = PreviewBurnMask_checkBox.Checked ? (Bitmap)myTONE.skinStates[index].maskImage : null;
            float burnMultiplier = myTONE.skinStates[index].OverlayMultiplier;
            if (textureImage == null) textureImage = (Bitmap)myTONE.skinStates[0].skinImage;
            SetupAgeGender(out age, out gender);
            skinDetails = SetupSkinDetails(age, gender);
            displayOverlay = SetupSkinOverlay(age, gender);
            Image displayImage = DisplayableSkintone(textureImage, skinDetails, burnMaskImage, burnMultiplier, myTONE.Hue, myTONE.Saturation, myTONE.Opacity);
            skintonePreviewer1.SetAgeGender(age, gender, displayImage, displayOverlay);
        }

        private void previewCAS_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setPreviewSkinTexture(true);
        }

        private void previewGame_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            setPreviewSkinTexture(false);
        }

        private void setPreviewSkinTexture(bool CAStexture)
        {
            if (myTONE == null) return;
            XmodsEnums.Age age;
            XmodsEnums.Gender gender;
            Bitmap skinDetails;
            int index = previewSkinState_comboBox.SelectedIndex;
            Bitmap textureImage = (Bitmap)myTONE.skinStates[index].skinImage;
            Bitmap burnMaskImage = PreviewBurnMask_checkBox.Checked ? (Bitmap)myTONE.skinStates[index].maskImage : null;
            float burnMultiplier = myTONE.skinStates[index].OverlayMultiplier;
            if (textureImage == null) textureImage = (Bitmap)myTONE.skinStates[0].skinImage;
            SetupAgeGender(out age, out gender);
            skinDetails = SetupSkinDetails(age, gender);
            Image displayImage = DisplayableSkintone(textureImage, skinDetails, burnMaskImage, burnMultiplier, myTONE.Hue, myTONE.Saturation, myTONE.Opacity);
            skintonePreviewer1.SetSkinTexture(displayImage);
        }

        private void previewOverlay_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            skintonePreviewer1.SetOverlay(previewOverlay_checkBox.Checked);
        }

        private void previewSkinState_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myTONE == null) return;
            XmodsEnums.Age age;
            XmodsEnums.Gender gender;
            Bitmap skinDetails;
            int index = previewSkinState_comboBox.SelectedIndex;
            Bitmap textureImage = (Bitmap)myTONE.skinStates[index].skinImage;
            Bitmap burnMaskImage = PreviewBurnMask_checkBox.Checked ? (Bitmap)myTONE.skinStates[index].maskImage : null;
            float burnMultiplier = myTONE.skinStates[index].OverlayMultiplier;
            if (textureImage == null) textureImage = (Bitmap)myTONE.skinStates[0].skinImage;
            SetupAgeGender(out age, out gender);
            skinDetails = SetupSkinDetails(age, gender);
            Image displayImage = DisplayableSkintone(textureImage, skinDetails, burnMaskImage, burnMultiplier, myTONE.Hue, myTONE.Saturation, myTONE.Opacity);
            skintonePreviewer1.SetSkinTexture(displayImage);
        }


        private void PreviewBurnMask_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (myTONE == null) return;
            XmodsEnums.Age age;
            XmodsEnums.Gender gender;
            Bitmap skinDetails;
            int index = previewSkinState_comboBox.SelectedIndex;
            Bitmap textureImage = (Bitmap)myTONE.skinStates[index].skinImage;
            Bitmap burnMaskImage = PreviewBurnMask_checkBox.Checked ? (Bitmap)myTONE.skinStates[index].maskImage : null;
            float burnMultiplier = myTONE.skinStates[index].OverlayMultiplier;
            if (textureImage == null) textureImage = (Bitmap)myTONE.skinStates[0].skinImage;
            SetupAgeGender(out age, out gender);
            skinDetails = SetupSkinDetails(age, gender);
            Image displayImage = DisplayableSkintone(textureImage, skinDetails, burnMaskImage, burnMultiplier, myTONE.Hue, myTONE.Saturation, myTONE.Opacity);
            skintonePreviewer1.SetSkinTexture(displayImage);
        }

        /// <summary>
        /// Converts hue, saturation, and luminance to RGB. 
        /// </summary>
        /// <param name="hue">Hue (0 - 239)</param>
        /// <param name="saturation">Saturation (0 - 240)</param>
        /// <param name="luminance">Luminance (0 - 240)</param>
        /// <returns>return byte array: RGB</returns>
        private byte[] GetRGB(ushort hue, ushort saturation, ushort luminance)
        {
            float l = luminance / 240f;
            if (l > 1f) l = 1f;
            if (saturation == 0) { byte r = (byte)(255f * l); return new byte[] { r, r, r }; }
            float s = saturation / 240f;
            float tmp1 = 0, tmp2 = 0;
            if (l < .5) tmp1 = l * (1f + s);
            else tmp1 = (l + s) - (l * s);
            tmp2 = 2f * l - tmp1;
            float H = hue / 239f;
            byte R = CalcChannel(H + .333f, tmp1, tmp2);
            byte G = CalcChannel(H, tmp1, tmp2);
            byte B = CalcChannel(H - .333f, tmp1, tmp2);
            return new byte[] { R, G, B };
        }

        private byte CalcChannel(float value, float adjust1, float adjust2)
        {
            if (value < 0f) value += 1f;
            else if (value > 1f) value -= 1f;
            float C = 0;
            if ((6f * value) < 1f) C = adjust2 + ((adjust1 - adjust2) * 6f * value);
            else if ((2f * value) < 1f) C = adjust1;
            else if ((3f * value) < 2f) C = adjust2 + ((adjust1 - adjust2) * (.666f - value) * 6f);
            else C = adjust2;
            C = C * 255f;
            if (C < 0f) C = 0f;
            if (C > 255f) C = 255f;
            return (byte)(C + 0.5f);
        }

        /// <summary>
        /// Returns hue (0 - 239) in element 0, saturation (0 - 240) in element 1, luminance (0 - 240) in element 2
        /// </summary>
        /// <param name="Red">Red component</param>
        /// <param name="Green">Green component</param>
        /// <param name="Blue">Blue component</param>
        /// <returns></returns>
        private ushort[] GetHSL(byte Red, byte Green, byte Blue)
        {
            float R = Red / 255f;
            float G = Green / 255f;
            float B = Blue / 255f;

            bool rMax = false, gMax = false, bMax = false;
            float maxVal = 0, minVal = 0;

            if (R >= G && R >= B) { rMax = true; maxVal = R; }
            else if (G >= R && G >= B) { gMax = true; maxVal = G; }
            else if (B >= G && B >= R) { bMax = true; maxVal = B; }
            if (R <= G && R <= B) { minVal = R; }
            else if (G <= R && G <= B) { minVal = G; }
            else if (B <= G && B <= R) { minVal = B; }

           // float L = (R + G + B) / 3f;
            float L = (float)Math.Sqrt((R * R * .241) + (G * G * .691) + (B * B * .068));

            if (maxVal == minVal) return new ushort[] { 0, 0, (ushort)((L * 240f) + 0.5f) };

            float S = 0;
            if (L < 0.5)
            {
                S = (maxVal - minVal) / (maxVal + minVal);
            }
            else
            {
                S = (maxVal - minVal) / (2.0f - maxVal - minVal);
            }
            if (S < 0) S = 0f;
            if (S > 1f) S = 1f;
            S = S * 240f;
            L = L * 240f;

            float H = 0;
            if (rMax) H = ((G - B) / (maxVal - minVal)) * 60f;
            else if (gMax) H = (2f + (B - R) / (maxVal - minVal)) * 60f;
            else if (bMax) H = (4f + (R - G) / (maxVal - minVal)) * 60f;
            if (H < 0) H += 360f;
            if (H > 360f) H -= 360f;
            H = (H / 360f) * 239f;

            return new ushort[] { (ushort)(H + 0.5f), (ushort)(S + 0.5f), (ushort)(L + 0.5f) };
        }
    }
}
