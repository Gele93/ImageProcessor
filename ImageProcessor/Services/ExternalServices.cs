using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.CompilerServices;
using ImageProcessor.Services.Converters;

namespace ImageProcessor.Services
{
    public static class ExternalServices
    {
        [DllImport(@"D:\Gele\Codecool\projects\HW\3DHISTECH\ImageProcessors\x64\Debug\ImageProcessors.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ApplyGaussianBlur(byte[] inputImage, byte[] outputImage, int width, int height);

    }
}
