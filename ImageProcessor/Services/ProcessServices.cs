using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ImageProcessor.Services
{
    public static class ProcessServices
    {
        [DllImport(@"D:\Gele\Codecool\projects\HW\3DHISTECH\ImageProcessors\x64\Debug\ImageProcessors.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ApplyGaussianBlur(byte[] inputImage, byte[] outputImage, int width, int height);

        public async static Task<(byte[], int, int)> UseGaussianBlur(byte[] inputImage, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream(inputImage);
            using var img = Image.FromStream(ms);
            var rgbBytes = ImageConverter.GetRawRgbBytes(img, out int width, out int height);
            var size = rgbBytes.Length;

            if (size != width * height * 3)
                throw new ArgumentException($"Input image size does not match width*height*3 input.length: {inputImage.Length} size: {size}");

            byte[] outputImage = new byte[size];

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ApplyGaussianBlur(rgbBytes, outputImage, width, height);
            }, cancellationToken);

            return (outputImage, width, height);
        }
    }
}
