using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
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

        public void Watermark(ImagePosition position = ImagePosition.LeftBottom, int width = 60, int height = 0, float opacity = 1f)
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
            if (!savePath.EndsWith("\\"))
            {
                savePath += "\\";
            }
            string filepath = savePath + FileName + "." + saveFormat;
            switch (saveFormat)
            {
                case PicFormat.png:
                    originImage.Save(filepath, new PngEncoder());
                    break;
                case PicFormat.gif:
                    originImage.Save(filepath, new GifEncoder());
                    break;
                case PicFormat.jpg:
                    originImage.Save(filepath, new JpegEncoder());
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