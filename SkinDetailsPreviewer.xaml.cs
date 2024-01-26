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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xmods.DataLib;

namespace XMODS
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SkinDetailsPreviewer : UserControl
    {
        AxisAngleRotation3D rot_x;
        AxisAngleRotation3D rot_y;
        ScaleTransform3D zoom = new ScaleTransform3D(1, 1, 1);
        Transform3DGroup modelTransform, cameraTransform;
        DirectionalLight DirLight1 = new DirectionalLight();
        DirectionalLight DirLight2 = new DirectionalLight();
        PerspectiveCamera Camera1 = new PerspectiveCamera();
        Model3DGroup modelGroup = new Model3DGroup();
        Viewport3D myViewport = new Viewport3D();
        GeometryModel3D myHead = new GeometryModel3D();
        GeometryModel3D myTop = new GeometryModel3D();
        GeometryModel3D myBottom = new GeometryModel3D();
        GeometryModel3D myFeet = new GeometryModel3D();
        DiffuseMaterial mySkin = new DiffuseMaterial();
        DiffuseMaterial myUnderwear = new DiffuseMaterial();
        DiffuseMaterial myFaceBackground = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(224, 224, 224)));
        MaterialGroup myFace = new MaterialGroup();
        MaterialGroup myMaterials = new MaterialGroup();

        public SkinDetailsPreviewer()
        {
            InitializeComponent();
            rot_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            rot_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);

            cameraTransform = new Transform3DGroup();
            cameraTransform.Children.Add(zoom);
            // cameraTransform.Children.Add(center);
            modelTransform = new Transform3DGroup();
            modelTransform.Children.Add(new RotateTransform3D(rot_y));
            modelTransform.Children.Add(new RotateTransform3D(rot_x));

            DirLight1.Color = Colors.White;
            DirLight1.Direction = new Vector3D(.5, -.5, -1);

            Camera1.FarPlaneDistance = 20;
            Camera1.NearPlaneDistance = 0.05;
            Camera1.FieldOfView = 45;
            Camera1.Position = new Point3D(0, 0, 2.8);
            Camera1.LookDirection = new Vector3D(0, 0, -3);
            Camera1.UpDirection = new Vector3D(0, 1, 0);
            ModelVisual3D modelsVisual = new ModelVisual3D();
            modelsVisual.Content = modelGroup;

            myHead.Transform = modelTransform;
            myTop.Transform = modelTransform;
            myBottom.Transform = modelTransform;
            myFeet.Transform = modelTransform;

            myViewport.Camera = Camera1;
            myViewport.Children.Add(modelsVisual);
            myViewport.Height = 480;
            myViewport.Width = 450;
            myViewport.Camera.Transform = cameraTransform;
            this.canvas1.Children.Insert(0, myViewport);

            Canvas.SetTop(myViewport, 0);
            Canvas.SetLeft(myViewport, 0);
            this.Width = myViewport.Width;
            this.Height = myViewport.Height;
        }

        public void Start_Mesh(XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, DMap[] morphs, System.Drawing.Image skinTexture, bool showUndies, bool fillBackground)
        {
            GetPartGeometries(species, age, gender, morphs);

            myMaterials.Children.Clear();
            if (skinTexture != null)
            {
                mySkin = new DiffuseMaterial(SkintonePreviewer.GetImageBrush(skinTexture));
                myMaterials.Children.Add(mySkin);
            }
            else
            {
                Brush nullBackground = new SolidColorBrush(Color.FromArgb(128, 224, 224, 224));
                myMaterials.Children.Add(new DiffuseMaterial(nullBackground));
            }
            myUnderwear = SkintonePreviewer.GetUnderwearTexture(age, gender);
            if (showUndies) myMaterials.Children.Add(myUnderwear);

            myFace.Children.Clear();
            if (fillBackground)
            {
                myFace.Children.Add(myFaceBackground);
            }
            myFace.Children.Add(mySkin);

            myHead.Material = myFace;
            myTop.Material = myMaterials;
            myBottom.Material = myMaterials;
            myFeet.Material = myMaterials;

            modelGroup.Children.Clear();
            modelGroup.Children.Add(DirLight1);
            modelGroup.Children.Add(myHead);
            modelGroup.Children.Add(myTop);
            modelGroup.Children.Add(myBottom);
            modelGroup.Children.Add(myFeet);
        }

        private void GetPartGeometries(XmodsEnums.Species species, XmodsEnums.Age age, XmodsEnums.Gender gender, DMap[] morphs)
        {
            GEOM head = null, top = null, bottom = null, feet = null;

            if (species == XmodsEnums.Species.Werewolf)
            {
                if (gender == XmodsEnums.Gender.Female)
                {
                    head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfwHead_lod0)));
                    top = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfwTop_lod0)));
                    bottom = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfwBottom_lod0)));
                    feet = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfwFeet_lod0)));
                }
                else
                {
                    head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymwHead_lod0)));
                    top = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymwTop_lod0)));
                    bottom = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymwBottom_lod0)));
                    feet = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymwFeet_lod0)));
                }
            }
            else
            {
                if ((age & XmodsEnums.Age.TeenToElder) > 0)
                {
                    if (gender == XmodsEnums.Gender.Female)
                    {
                        head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfHead_lod0)));
                        top = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfTop_lod0)));
                        bottom = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfBottom_lod0)));
                        feet = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfFeet_lod0)));
                    }
                    else
                    {
                        head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymHead_lod0)));
                        top = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymTop_lod0)));
                        bottom = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymBottom_lod0)));
                        feet = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymFeet_lod0)));
                    }
                }
                else if (age == XmodsEnums.Age.Child)
                {
                    head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuHead_lod0)));
                    top = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuTop_lod0)));
                    bottom = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuBottom_lod0)));
                    feet = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuFeet_lod0)));
                }
                else if (age == XmodsEnums.Age.Toddler)
                {
                    head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puHead_lod0)));
                    top = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puTop_lod0)));
                    bottom = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puBottom_lod0)));
                    feet = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puFeet_lod0)));
                }
                else if (age == XmodsEnums.Age.Infant)
                {
                    head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuHead_lod0)));
                    top = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuTop_lod0)));
                    bottom = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuBottom_lod0)));
                    feet = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuFeet_lod0)));
                }
                else if (age == XmodsEnums.Age.Baby)
                {
                    head = new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.buBody_lod0)));
                }

                if (morphs != null && morphs.Length > 1)
                {
                    for (int i = 0; i < morphs.Length; i += 2)
                    {
                        head = Form1.LoadDMapMorph(head, morphs[i], morphs[i + 1]);
                        top = Form1.LoadDMapMorph(top, morphs[i], morphs[i + 1]);
                        bottom = Form1.LoadDMapMorph(bottom, morphs[i], morphs[i + 1]);
                        feet = Form1.LoadDMapMorph(feet, morphs[i], morphs[i + 1]);
                    }
                }
            }

            myHead.Geometry = SkintonePreviewer.SimMesh(head);
            myTop.Geometry = SkintonePreviewer.SimMesh(top);
            myBottom.Geometry = SkintonePreviewer.SimMesh(bottom);
            myFeet.Geometry = SkintonePreviewer.SimMesh(feet);
        }

        public void Stop_Mesh()
        {
            modelGroup.Children.Clear();
        }
        private void sliderXRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_x.Angle = sliderXRot.Value;
        }
        private void sliderYRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rot_y.Angle = sliderYRot.Value;
        }
        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(Camera1.Position.X, Camera1.Position.Y, -sliderZoom.Value);
        }
        private void sliderYMove_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(Camera1.Position.X, -sliderYMove.Value, Camera1.Position.Z);
        }
        private void sliderXMove_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Camera1.Position = new Point3D(-sliderXMove.Value, Camera1.Position.Y, Camera1.Position.Z);
        }
        public void SetUndies(bool showUndies)
        {
            if (showUndies && !myMaterials.Children.Contains(myUnderwear))
            {
                myMaterials.Children.Add(myUnderwear);
            }
            else if (!showUndies && myMaterials.Children.Contains(myUnderwear))
            {
                myMaterials.Children.Remove(myUnderwear);
            }
        }
        public void SetBackground(bool fillBackground)
        {
            if (fillBackground && !myFace.Children.Contains(myFaceBackground))
            {
                myFace.Children.Insert(0, myFaceBackground);
            }
            else if (!fillBackground && myFace.Children.Contains(myFaceBackground))
            {
                myFace.Children.Remove(myFaceBackground);
            }
        }
    }
}
