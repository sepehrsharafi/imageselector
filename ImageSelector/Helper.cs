﻿using DarkUI.Docking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageSelector
{
    class Helper
    {
        string[] images;
        public void FillImageList(string path, FlowLayoutPanel panel)
        {
        }

        public static void FillSelectedImages(string path, DataGridView dg)
        {
            if (Directory.Exists(path))
            {
                dg.DataSource = new DirectoryInfo(path).GetFiles().Where(s => s.FullName.ToLower().EndsWith(".jpeg") || s.FullName.ToLower().EndsWith(".jpg") || s.FullName.ToLower().EndsWith(".png")).ToArray();
                dg.Update();
            }
        }

        public static void DeleteSelectedImages(string path, DataGridView dg)
        {

        }

        public static Image CreateThumbnailImage(string imageFileName)
        {
            const int thumbnailSize = 100;

            using (var image = Image.FromFile(imageFileName))
            {
                var imageHeight = image.Height;
                var imageWidth = image.Width;
                if (imageHeight > imageWidth)
                {
                    imageWidth = (int)(((float)imageWidth / (float)imageHeight) * thumbnailSize);
                    imageHeight = thumbnailSize;
                }
                else
                {
                    imageHeight = (int)(((float)imageHeight / (float)imageWidth) * thumbnailSize);
                    imageWidth = thumbnailSize;
                }

                return image.GetThumbnailImage(imageWidth, imageHeight, () => false, IntPtr.Zero);
            }
        }

        public static string[] GetImages(string path)
        {

            return Directory.GetFiles(path).Where(s => s.ToLower().EndsWith(".jpeg") || s.ToLower().EndsWith(".jpg") || s.ToLower().EndsWith(".png")).ToArray();

        }

        public static void CopyFiles(DirectoryInfo source,
                      DirectoryInfo destination,
                      bool overwrite,
                      string searchPattern)
        {
            FileInfo[] files = source.GetFiles(searchPattern);

            string DestPath;

            //this section is what's really important for your application.
            foreach (FileInfo file in files)
            {
                DestPath = destination.FullName + "\\" + file.Name;
                if (!File.Exists(DestPath))
                {
                    file.CopyTo(DestPath, overwrite);
                }
            }
        }

        public static void DeleteFiles(DirectoryInfo directory, string searchPattern)
        {
            FileInfo[] files = directory.GetFiles(searchPattern);

            //this section is what's really important for your application.
            foreach (FileInfo file in files)
            {
                file.Delete();
            }
        }

        public static void SelectImage(string SelectedFolderPath, int CurrentIndex, string txtNotes, string txtAddress, string currentImagePath, Image[] images, DataGridView dgSelectedImages)
        {
            if (!Directory.Exists(SelectedFolderPath))
            {
                Directory.CreateDirectory(SelectedFolderPath);
            }

            currentImagePath = images[CurrentIndex].ToString();

            DirectoryInfo source = new DirectoryInfo(txtAddress);
            DirectoryInfo dest = new DirectoryInfo(SelectedFolderPath);

            Helper.CopyFiles(source, dest, true, Path.GetFileNameWithoutExtension(currentImagePath) + ".*");

            Helper.FillSelectedImages(SelectedFolderPath, dgSelectedImages);

            string NotesPath = SelectedFolderPath + @"\" + Path.GetFileNameWithoutExtension(currentImagePath) + ".txt";


            if (!String.IsNullOrEmpty(txtNotes))
            {
                File.Create(NotesPath).Close(); // Create file
                using (StreamWriter sw = File.AppendText(NotesPath))
                {
                    sw.Write(txtNotes); // Write text to .txt file
                    sw.Close();
                }

                txtNotes = " ";
            }
        }

        //Change Image to Correct Orientation When displaying to PictureBox
        public static RotateFlipType Rotate(Image bmp)
        {
            PropertyItem pi = bmp.PropertyItems.Select(x => x)
                                 .FirstOrDefault(x => x.Id == 0x0112);
            if (pi == null)
                return RotateFlipType.RotateNoneFlipNone;

            byte o = pi.Value[0];

           // Orientations
            if (o == 2) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            if (o == 3) bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            if (o == 4) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (o == 5) bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
            if (o == 6) bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (o == 7) bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
            if (o == 8) bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);

            return RotateFlipType.RotateNoneFlipNone; //TopLeft (what the image looks by default) [or] Unknown

        }

        //public void Rotate(Image bmp)
        //{
        //    PropertyItem pi = bmp.PropertyItems.Select(x => x)
        //                         .FirstOrDefault(x => x.Id == 0x0112);

        //    if (pi == null) return;

        //    byte o = pi.Value[0];

        //    if (o == 2) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
        //    if (o == 3) bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);
        //    if (o == 4) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //    if (o == 5) bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
        //    if (o == 6) bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
        //    if (o == 7) bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
        //    if (o == 8) bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
        //}

        PropertyItem getPropertyItemByID(Image img, int Id)
        {
            return
              img.PropertyItems.Select(x => x).FirstOrDefault(x => x.Id == Id);
        }
    }
}