using Microsoft.Win32;
using SplitImageMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SplitImageMaker.Services
{
    public class FileService
    {
        public bool SaveCombinedImage(BitmapSource finalImage)
        {
            if (finalImage == null) return false;

            var dialog = new SaveFileDialog
            {
                Filter = "PNG Files|*.png|JPEG Files|*.jpg",
                Title = "Save Combined Image",
                FileName = "CombinedImage.png"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    SaveBitmapSource(finalImage, dialog.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

        public bool SaveImages(IEnumerable<PanelInfo> panels, int maxCount)
        {
            var panelsWithImages = panels.Where(p => p.Image != null).Take(maxCount).ToList();
            if (!panelsWithImages.Any()) return false;

            var dialog = new SaveFileDialog
            {
                Filter = "PNG Files|*.png|JPEG Files|*.jpg",
                Title = "Save Individual Images",
                FileName = "Image_01.png"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string basePath = Path.GetDirectoryName(dialog.FileName);
                    string baseName = Path.GetFileNameWithoutExtension(dialog.FileName).Replace("_01", "");

                    for (int i = 0; i < panelsWithImages.Count; i++)
                    {
                        string fileName = Path.Combine(basePath, $"{baseName}_{i + 1:D2}.png");
                        SaveBitmapSource(panelsWithImages[i].Image, fileName);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving files: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

        private void SaveBitmapSource(BitmapSource source, string filePath)
        {
            BitmapEncoder encoder;
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    var jpegEncoder = new JpegBitmapEncoder { QualityLevel = 100 };
                    encoder = jpegEncoder;
                    break;
                default:
                    encoder = new PngBitmapEncoder();
                    break;
            }

            encoder.Frames.Add(BitmapFrame.Create(source));
            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }
    }
}