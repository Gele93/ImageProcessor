using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using ImageProcessor.Data;

namespace ImageProcessor.Services
{
    public static class ImageConverter
    {
        public static byte[] GetRawRgbBytes(Image img, out int width, out int height)
        {
            using var bitmap = new Bitmap(img);
            width = bitmap.Width;
            height = bitmap.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int size = Math.Abs(bmpData.Stride) * height;
            byte[] rawRgbBytes = new byte[size];

            Marshal.Copy(bmpData.Scan0, rawRgbBytes, 0, size);
            bitmap.UnlockBits(bmpData);

            return rawRgbBytes;
        }

        public static byte[] EncodeRgbBytes(byte[] rgbBytes, int width, int height, EncodingType encodingType)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            Marshal.Copy(rgbBytes, 0, bmpData.Scan0, rgbBytes.Length);
            bmp.UnlockBits(bmpData);

            using var ms = new MemoryStream();

            switch (encodingType)
            {
                case EncodingType.PNG:
                    bmp.Save(ms, ImageFormat.Png);
                    break;

                case EncodingType.JPG:
                    bmp.Save(ms, ImageFormat.Jpeg);
                    break;

                default:
                    throw new Exception("Invalid encoding type");
            }

            return ms.ToArray();
        }

    }
}
