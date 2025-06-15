using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.CompilerServices;
using ImageProcessor.Services.Converters;


namespace ImageProcessor.Services.Modifiers
{
    public class ImageModifier :IImageModifier
    {
        private const int BlurStrength = 255;
        private IImageConverter _imageConverter;
        private ILogger<ImageModifier> _logger;
        public ImageModifier(IImageConverter imageConverter, ILogger<ImageModifier> logger)
        {
            _imageConverter = imageConverter;
            _logger = logger;
        }

        public async Task<(byte[], int, int)> UseGaussianBlur(byte[] inputImage, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream(inputImage);
            using var img = Image.FromStream(ms);
            var rgbBytes = _imageConverter.GetRawRgbBytes(img, out int width, out int height);
            var size = rgbBytes.Length;

            _logger.LogInformation($"image size: {size}, width: {width}, height: {height}");

            byte[] outputImage = new byte[size];

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ExternalServices.ApplyGaussianBlur(rgbBytes, outputImage, width, height, BlurStrength);
            }, cancellationToken);

            return (outputImage, width, height);
        }
    }
}
