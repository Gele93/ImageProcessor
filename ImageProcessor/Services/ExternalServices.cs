using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.CompilerServices;
using ImageProcessor.Services.Converters;

namespace ImageProcessor.Services
{
    public static class ExternalServices
    {
        [DllImport("ImageProcessors.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ApplyGaussianBlur(byte[] inputImage, byte[] outputImage, int width, int height, int blurStrength);
    }
}
