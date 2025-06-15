using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.CompilerServices;
using ImageProcessor.Services.Converters;


namespace ImageProcessor.Services.Modifiers
{
    public class ImageModifier :IImageModifier
    {
        private IImageConverter _imageConverter;

        public ImageModifier(IImageConverter imageConverter)
        {
            _imageConverter = imageConverter;
        }


        public async Task<(byte[], int, int)> UseGaussianBlur(byte[] inputImage, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream(inputImage);
            using var img = Image.FromStream(ms);
            var rgbBytes = _imageConverter.GetRawRgbBytes(img, out int width, out int height);
            var size = rgbBytes.Length;


            byte[] outputImage = new byte[size];

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ExternalServices.ApplyGaussianBlur(rgbBytes, outputImage, width, height);
            }, cancellationToken);

            return (outputImage, width, height);
        }
    }
}
