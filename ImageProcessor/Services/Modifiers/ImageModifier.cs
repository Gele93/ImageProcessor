﻿using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.CompilerServices;
using ImageProcessor.Services.Converters;


namespace ImageProcessor.Services.Modifiers
{
    public class ImageModifier : IImageModifier
    {
        //Strength of cv::GaussianBlur (must be odd)
        private const int BlurStrength = 101;
        private readonly IImageConverter _imageConverter;
        private readonly ILogger<ImageModifier> _logger;
        public ImageModifier(IImageConverter imageConverter, ILogger<ImageModifier> logger)
        {
            _imageConverter = imageConverter;
            _logger = logger;
        }

        public async Task<(byte[], int, int)> UseGaussianBlur(byte[] encodedBytes, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream(encodedBytes);
            using var img = Image.FromStream(ms);
            var BGRAbytes = _imageConverter.GetRawRgbBytes(img, out int width, out int height);
            var size = BGRAbytes.Length;

            _logger.LogInformation($"image size: {size}, width: {width}, height: {height}");

            /* outputImage:
             * Creates an empty byte[] with the same size as the input image in BGRA byte[]             
             * This will be filled with the blured BGRA data by ApllyGaussianBlur native module
             */
            byte[] bluredBGRAbytes = new byte[size];

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                ExternalServices.ApplyGaussianBlur(BGRAbytes, bluredBGRAbytes, width, height, BlurStrength);
            }, cancellationToken);

            return (bluredBGRAbytes, width, height);
        }
    }
}
