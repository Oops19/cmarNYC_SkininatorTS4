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
    /// Interaction logic for SkintonePreviewer.xaml
    /// </summary>
    public partial class SkintonePreviewer : UserControl
    {
        AxisAngleRotation3D rot_x;
        AxisAngleRotation3D rot_y;
        ScaleTransform3D zoom = new ScaleTransform3D(1, 1, 1);
        Transform3DGroup modelTransform, cameraTransform;
        DirectionalLight DirLight1 = new DirectionalLight();
        DirectionalLight DirLight2 = new DirectionalLight();
        AmbientLight Ambient1 = new AmbientLight();
        PerspectiveCamera Camera1 = new PerspectiveCamera();
        Model3DGroup modelGroup = new Model3DGroup();
        Viewport3D myViewport = new Viewport3D();
        GeometryModel3D myHead = new GeometryModel3D();
        GeometryModel3D myTop = new GeometryModel3D();
        GeometryModel3D myBottom = new GeometryModel3D();
        GeometryModel3D myFeet = new GeometryModel3D();
        DiffuseMaterial mySkin = new DiffuseMaterial();
        DiffuseMaterial myOverlay = new DiffuseMaterial();
        DiffuseMaterial myEyes = new DiffuseMaterial();
        DiffuseMaterial myUnderwear = new DiffuseMaterial();
        MaterialGroup myMaterials = new MaterialGroup();

        public SkintonePreviewer()
        {
            InitializeComponent();
            rot_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            rot_y = new AxisAngleRotation3D( new Vector3D(0, 1, 0), 0);

            cameraTransform = new Transform3DGroup();
            cameraTransform.Children.Add(zoom);
           // cameraTransform.Children.Add(center);
            modelTransform = new Transform3DGroup();
            modelTransform.Children.Add(new RotateTransform3D(rot_y));
            modelTransform.Children.Add(new RotateTransform3D(rot_x));

            DirLight1.Color = Colors.White;
            DirLight1.Direction = new Vector3D(.5, -.5, -1.25);
            DirLight2.Color = Colors.White;
            DirLight2.Direction = new Vector3D(-.5, -.5, -1.25);
            Ambient1.Color = Color.FromArgb(255, 25, 25, 25);

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

            myEyes = new DiffuseMaterial(GetImageBrush(Properties.Resources.BrownEyes));

            myViewport.Camera = Camera1;
            myViewport.Children.Add(modelsVisual);
            myViewport.Height = 480;
            myViewport.Width = 480;
            myViewport.Camera.Transform = cameraTransform;
            this.canvas1.Children.Insert(0, myViewport);

            Canvas.SetTop(myViewport, 0);
            Canvas.SetLeft(myViewport, 0);
            this.Width = myViewport.Width;
            this.Height = myViewport.Height;
        }

        internal static MeshGeometry3D SimMesh(GEOM simgeom)
        {
            if (simgeom == null) return null;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection verts = new Point3DCollection();
            Vector3DCollection normals = new Vector3DCollection();
            PointCollection uvs = new PointCollection();
            Int32Collection facepoints = new Int32Collection();
            int indexOffset = 0;
            float centerOffset = 0.80f;

           // foreach (GEOM g in simgeom)
           // {
            GEOM g = simgeom;

                for (int i = 0; i < g.numberVertices; i++)
                {
                    float[] pos = g.getPosition(i);
                    verts.Add(new Point3D(pos[0], pos[1] - centerOffset, pos[2]));
                    float[] norm = g.getNormal(i);
                    normals.Add(new Vector3D(norm[0], norm[1], norm[2]));
                    float[] uv = g.getUV(i, 0);
                    uvs.Add(new Point(uv[0], uv[1]));
                }

                for (int i = 0; i < g.numberFaces; i++)
                {
                    int[] face = g.getFaceIndices(i);
                    facepoints.Add(face[0] + indexOffset);
                    facepoints.Add(face[1] + indexOffset);
                    facepoints.Add(face[2] + indexOffset);
                }

                indexOffset += g.numberVertices;
           // }

            mesh.Positions = verts;
            mesh.TriangleIndices = facepoints;
            mesh.Normals = normals;
            mesh.TextureCoordinates = uvs;
            return mesh;
        }

        internal static ImageBrush GetImageBrush(System.Drawing.Image image)
        {
            BitmapImage bmpImg = new BitmapImage();
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            bmpImg.BeginInit();
            bmpImg.StreamSource = ms;
            bmpImg.EndInit();
            ImageBrush img = new ImageBrush();
            img.ImageSource = bmpImg;
            img.Stretch = Stretch.Fill;
            img.TileMode = TileMode.None;
            img.ViewportUnits = BrushMappingMode.Absolute;
            return img;
        }

        public void Start_Mesh(XmodsEnums.Age age, XmodsEnums.Gender gender, System.Drawing.Image skinTexture, System.Drawing.Image overlayTexture,
            bool showLight1, bool showLight2, bool showUndies)
        {
            GetPartGeometries(age, gender);
            myUnderwear = GetUnderwearTexture(age, gender);

            myMaterials.Children.Clear();
            mySkin = new DiffuseMaterial(GetImageBrush(skinTexture));
            myMaterials.Children.Add(mySkin);
          //  myMaterials.Children.Add(mySkinDetails);

            if (overlayTexture != null)
            {
                myOverlay = new DiffuseMaterial(GetImageBrush(overlayTexture));
                myMaterials.Children.Add(myOverlay);
            }
            myMaterials.Children.Add(myEyes);
            if (showUndies) myMaterials.Children.Add(myUnderwear);

            myHead.Material = myMaterials;
            myTop.Material = myMaterials;
            myBottom.Material = myMaterials;
            myFeet.Material = myMaterials;
            
            modelGroup.Children.Clear();
            if (showLight1) modelGroup.Children.Add(DirLight1);
            if (showLight2) modelGroup.Children.Add(DirLight2);
            modelGroup.Children.Add(Ambient1);
            modelGroup.Children.Add(myHead);
            modelGroup.Children.Add(myTop);
            modelGroup.Children.Add(myBottom);
            modelGroup.Children.Add(myFeet);
        }

        private void GetPartGeometries(XmodsEnums.Age age, XmodsEnums.Gender gender)
        {
            if ((age & XmodsEnums.Age.TeenToElder) > 0)
            {
                if (gender == XmodsEnums.Gender.Female)
                {
                    myHead.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfHead_lod0))));
                    myTop.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfTop_lod0))));
                    myBottom.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfBottom_lod0))));
                    myFeet.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.yfFeet_lod0))));
                }
                else
                {
                    myHead.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymHead_lod0))));
                    myTop.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymTop_lod0))));
                    myBottom.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymBottom_lod0))));
                    myFeet.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.ymFeet_lod0))));
                }
            }
            else if (age == XmodsEnums.Age.Infant)
            {
                myHead.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuHead_lod0))));
                myTop.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuTop_lod0))));
                myBottom.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuBottom_lod0))));
                myFeet.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.iuFeet_lod0))));
            }
            else if (age == XmodsEnums.Age.Toddler)
            {
                myHead.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puHead_lod0))));
                myTop.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puTop_lod0))));
                myBottom.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puBottom_lod0))));
                myFeet.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.puFeet_lod0))));
            }
            else
            {
                myHead.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuHead_lod0))));
                myTop.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuTop_lod0))));
                myBottom.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuBottom_lod0))));
                myFeet.Geometry = SimMesh(new GEOM(new BinaryReader(new MemoryStream(Properties.Resources.cuFeet_lod0))));
            }
        }

        internal static DiffuseMaterial GetUnderwearTexture(XmodsEnums.Age age, XmodsEnums.Gender gender)
        {
            if ((age & XmodsEnums.Age.TeenToElder) > 0)
            {
                if (gender == XmodsEnums.Gender.Female)
                    return new DiffuseMaterial(GetImageBrush(Properties.Resources.FemaleUnderwear));
                else
                    return new DiffuseMaterial(GetImageBrush(Properties.Resources.MaleUnderwear));
            }
            else
            {
                return new DiffuseMaterial(GetImageBrush(Properties.Resources.ChildUnderwear));
            }
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

        public void SetOverlay(bool showOverlay)
        {
            if (showOverlay && !myMaterials.Children.Contains(myOverlay))
            {
                myMaterials.Children.Insert(1, myOverlay);
            }
            else if (!showOverlay && myMaterials.Children.Contains(myOverlay))
            {
                myMaterials.Children.Remove(myOverlay);
            }
        }
        
        public void SetLights(bool light1On, bool light2On)
        {
            if (light1On && !modelGroup.Children.Contains(DirLight1))
            {
                modelGroup.Children.Insert(0, DirLight1);
            }
            else if (!light1On && modelGroup.Children.Contains(DirLight1))
            {
                modelGroup.Children.Remove(DirLight1);
            }
            if (light2On && !modelGroup.Children.Contains(DirLight2))
            {
                modelGroup.Children.Insert(0, DirLight2);
            }
            else if (!light2On && modelGroup.Children.Contains(DirLight2))
            {
                modelGroup.Children.Remove(DirLight2);
            }
        }

        public void SetAgeGender(XmodsEnums.Age age, XmodsEnums.Gender gender, System.Drawing.Image displayImage, System.Drawing.Image overlayImage)
        {
            GetPartGeometries(age, gender);
            bool hasUnderwear = false;
            myMaterials.Children.Remove(mySkin);
            if (myMaterials.Children.Contains(myUnderwear))
            {
                myMaterials.Children.Remove(myUnderwear);
                hasUnderwear = true;
            }
            mySkin = new DiffuseMaterial(GetImageBrush(displayImage));
            myMaterials.Children.Insert(0, mySkin);
            myMaterials.Children.Remove(myOverlay);
            if (overlayImage != null)
            {
                myOverlay = new DiffuseMaterial(GetImageBrush(overlayImage));
                myMaterials.Children.Insert(1, myOverlay);
            }
            myUnderwear = GetUnderwearTexture(age, gender);
            if (hasUnderwear)
            {
                myMaterials.Children.Add(myUnderwear);
            }
        }

        public void SetSkinTexture(System.Drawing.Image displayImage)
        {
            myMaterials.Children.Remove(mySkin);
            mySkin = new DiffuseMaterial(GetImageBrush(displayImage));
            myMaterials.Children.Insert(0, mySkin);
        }

    }
}
