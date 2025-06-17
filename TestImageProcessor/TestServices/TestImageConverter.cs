using ImageProcessor.Services.Converters;
using Microsoft.Extensions.Logging;
using Moq;
using System.Drawing;
using System.Drawing.Imaging;

namespace TestImageProcessor.TestServices
{
    public class TestImageConverter
    {
        private IImageConverter _imageConverter;
        private Image TestImage;
        private static readonly PixelFormat _pixelFormat = PixelFormat.Format32bppArgb;

        [SetUp]
        public void Setup()
        {
            var loggerMock = new Mock<ILogger<ImageProcessor.Services.Converters.ImageConverter>>();
            _imageConverter = new ImageProcessor.Services.Converters.ImageConverter(loggerMock.Object);

            TestImage = new Bitmap(100, 100, _pixelFormat);
            using (Graphics g = Graphics.FromImage(TestImage))
            {
                g.Clear(Color.Blue);
            }
        }

        [TearDown]
        public void TearDown()
        {
            TestImage.Dispose();
        }

        [Test]
        public void TestGetBGRAofBlueImageReturns255_0_0_255()
        {
            byte[] result = _imageConverter.GetRawRgbBytes(TestImage, out int widthR, out int heightR);

            bool isAllBlue = result
                .Where((b, i) => i % 4 == 0)
                .All(n => n == 255);

            bool isNoneGreen = result
                .Where((b, i) => i % 4 == 1)
                .All(n => n == 0);

            bool isNoneRed = result
                .Where((b, i) => i % 4 == 2)
                .All(n => n == 0);

            bool isAllAlpha255 = result
                .Where((b, i) => i % 4 == 3)
                .All(n => n == 255);

            Assert.That(isAllBlue, Is.True);
            Assert.That(isNoneGreen, Is.True);
            Assert.That(isNoneRed, Is.True);
            Assert.That(isAllAlpha255, Is.True);
        }

        [Test]
        public void Test100x100PixelReturns40000RgbBytes()
        {
            byte[] result = _imageConverter.GetRawRgbBytes(TestImage, out int widthR, out int heightR);
            int channels = 4;
            int expectedSize = widthR * heightR * channels;

            Assert.That(result.Length, Is.EqualTo(expectedSize));
        }

        [Test]
        public void TestEncodeBlueImgToPngReturnsPngSigniture()
        {
            int width = 10;
            int height = 10;
            int channels = 4;
            byte[] pngSigniture = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

            byte[] blueBGRAbytes = createBlueBGRA(width, height, channels);
            byte[] result = _imageConverter.EncodeRgbBytes(blueBGRAbytes, width, height, ImageProcessor.Data.EncodingType.PNG);

            Assert.That(result.Take(8).SequenceEqual(pngSigniture), Is.True);
        }

        [Test]
        public void TestEncodeBlueImgToPngReturnsJpgSigniture()
        {
            int width = 10;
            int height = 10;
            int channels = 4;
            byte[] jpgSignitureStart = { 0xFF, 0xD8 };
            byte[] jpgSignitureEnd = { 0xFF, 0xD9 };

            byte[] blueBGRAbytes = createBlueBGRA(width, height, channels);
            byte[] result = _imageConverter.EncodeRgbBytes(blueBGRAbytes, width, height, ImageProcessor.Data.EncodingType.JPG);

            Assert.That(result.Take(2).SequenceEqual(jpgSignitureStart), Is.True);
            Assert.That(result[result.Length - 2], Is.EqualTo(jpgSignitureEnd[0]));
            Assert.That(result[result.Length - 1], Is.EqualTo(jpgSignitureEnd[1]));
        }

        [Test]
        public void TestEncodingEmptyArrayThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _imageConverter.EncodeRgbBytes(new byte[0], 0, 0, ImageProcessor.Data.EncodingType.JPG));
        }

        private byte[] createBlueBGRA(int width, int height, int channels)
        {
            byte[] BGRAbytes = new byte[width * height * channels];
            for (int i = 0; i < BGRAbytes.Length; i++)
            {
                switch (i % 4)
                {
                    case 0:
                        BGRAbytes[i] = 255;
                        break;
                    case 1:
                        BGRAbytes[i] = 0;
                        break;
                    case 2:
                        BGRAbytes[i] = 0;
                        break;
                    case 3:
                        BGRAbytes[i] = 255;
                        break;
                }
            }
            return BGRAbytes;
        }


    }
}
