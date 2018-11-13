using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.MetaData.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Watermark
{
    public class Photo
    {
        private Image<Rgba32> originImage;
        private Image<Rgba32> watermarkImage;

        public string FileName;

        public Photo(string path)
        {
            originImage = Image.Load(path);
            watermarkImage = Image.Load(Properties.Resources.dxpressWatermark);
        }

        public void Resize()
        {
            ResizePic(originImage, 2000, 2000);
        }

        public void Watermark(ImagePosition position = ImagePosition.LeftBottom, int width = 50, int height = 50, float opacity = 1f)
        {
            ResizePic(watermarkImage, 450, 450);

            int originWidth = originImage.Width;
            int originHeight = originImage.Height;
            int wmWidth = watermarkImage.Width;
            int wmHeight = watermarkImage.Height;

            int wmPosiX, wmPosiY;

            switch (position)
            {
                case ImagePosition.LeftBottom:
                    wmPosiX = width;
                    wmPosiY = originHeight - wmHeight - height;
                    break;
                default:
                    throw new NotImplementedException();
            }
            originImage.Mutate(i => { i.DrawImage(watermarkImage, opacity, new Point(wmPosiX, wmPosiY)); });
        }

        public void SaveImage(string savePath, PicFormat saveFormat)
        {
            string saveName = FileName + "." + saveFormat;
            string filepath = Path.Combine(savePath, saveName);
            switch (saveFormat)
            {
                case PicFormat.png:
                    originImage.Save(filepath, new PngEncoder());
                    break;
                case PicFormat.gif:
                    originImage.Save(filepath, new GifEncoder());
                    break;
                case PicFormat.jpg:
                    originImage.Save(filepath, new JpegEncoder
                    {
                        Quality = 80
                    });
                    break;
            }
        }

        private void ResizePic(Image<Rgba32> image, int maxWidth, int maxHeight)
        {
            ResizeOptions resizeOptions = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, maxHeight)
            };
            image.Mutate(x => x.Resize(resizeOptions));
        }

        public void AddCopyright(string copyRight)
        {
            var newExifProfile = originImage.MetaData.ExifProfile == null ? new ExifProfile() : new ExifProfile(originImage.MetaData.ExifProfile.ToByteArray());
            newExifProfile.SetValue(ExifTag.Copyright, copyRight);
            originImage.MetaData.ExifProfile = newExifProfile;
        }

        public enum ImagePosition
        {
            LeftTop,
            LeftBottom,
            RightTop,
            RigthBottom,
            TopMiddle,
            BottomMiddle,
            Center
        }

        public enum PicFormat
        {
            jpg,
            png,
            gif
        }
    }
}