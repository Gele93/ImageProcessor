using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using ImageProcessor.Data;
using System.Threading;

namespace ImageProcessor.Services.Converters
{
    public class ImageConverter : IImageConverter
    {
        //In what format we are expecting input images
        private static readonly PixelFormat _pixelFormat = PixelFormat.Format32bppArgb;
        private readonly ILogger<ImageConverter> _logger;

        public ImageConverter(ILogger<ImageConverter> logger)
        {
            _logger = logger;
        }


        public byte[] GetRawRgbBytes(Image img, out int width, out int height)
        {
            Bitmap bitmap = new Bitmap(img);
            width = bitmap.Width;
            height = bitmap.Height;

            if (_pixelFormat != bitmap.PixelFormat)
            {
                _logger.LogError($"Pixel format missmatch: input:{bitmap.PixelFormat.ToString()} expected: {_pixelFormat.ToString()}");
                throw new ArgumentException($"Unsupported pixel format: {bitmap.PixelFormat}. Expected: {_pixelFormat}");
            }

            //getting the bitmapdata for the full image
            Rectangle fullImg = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bitmap.LockBits(fullImg, ImageLockMode.ReadOnly, _pixelFormat);

            //inits the array for managed image data
            int size = Math.Abs(bmpData.Stride) * height;
            byte[] rawRgbBytes = new byte[size];

            //saves img data from unmanaged memory into managed byte[]
            Marshal.Copy(bmpData.Scan0, rawRgbBytes, 0, size);
            bitmap.UnlockBits(bmpData);

            return rawRgbBytes;
        }


        public byte[] EncodeRgbBytes(byte[] rgbBytes, int width, int height, EncodingType encodingType)
        {
            if (rgbBytes.Length < 1)
                throw new ArgumentException("Can not encode empty rgbByte array");

            var bmp = new Bitmap(width, height, _pixelFormat);

            Rectangle fullImg = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(fullImg, ImageLockMode.WriteOnly, _pixelFormat);

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
                    _logger.LogError($"Unsupported encoding type: {encodingType}");
                    throw new ArgumentException($"Unsupported encoding type: {encodingType}");
            }

            return ms.ToArray();
        }

        public async Task<byte[]> ConvertImageToBytes(IFormFile file, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
        }


    }
}
