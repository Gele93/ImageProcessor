using ImageProcessor.Data;
using System.Drawing;

namespace ImageProcessor.Services.Converters
{
    public interface IImageConverter
    {
        public byte[] GetRawRgbBytes(Image img, out int width, out int height);
        public byte[] EncodeRgbBytes(byte[] rgbBytes, int width, int height, EncodingType encodingType);


    }
}
