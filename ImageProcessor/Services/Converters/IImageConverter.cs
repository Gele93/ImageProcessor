using ImageProcessor.Data;
using System.Drawing;

namespace ImageProcessor.Services.Converters
{
    public interface IImageConverter
    {

        /// <summary>
        /// Returns managed byte[] of raw BGRA image data.
        /// </summary>
        /// <param name="img">System.Drawing.Image refering to the image</param>
        /// <param name="width">Width of image in pixels</param>
        /// <param name="height">Height of image in pixels.</param>
        /// <returns>Managed byte[] of BGRA image data </returns>
        public byte[] GetRawRgbBytes(Image img, out int width, out int height);
        /// <summary>
        /// Returns encoded byte[] of raw BGRA image data.
        /// </summary>
        /// <param name="rgbBytes">blured BGRA formated byte []</param>
        /// <param name="width">Width of image in pixels</param>
        /// <param name="height">Height of image in pixels.</param>
        /// <param name="encodingType">encoding format eg.: .png / .jpg</param>
        /// <returns> encoded byte[] of raw BGRA image data </returns>
        public byte[] EncodeRgbBytes(byte[] rgbBytes, int width, int height, EncodingType encodingType);
        public Task<byte[]> ConvertImageToBytes(IFormFile file, CancellationToken cancellationToken);

    }
}
