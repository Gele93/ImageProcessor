using ImageProcessor.Data;
using ImageProcessor.Services;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Buffers.Text;
using System.Collections;
using System.IO;
using System.ComponentModel.DataAnnotations;
using ImageProcessor.Services.Converters;
using ImageProcessor.Services.Modifiers;

namespace ImageProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private ILogger<ImageController> _logger;
        private IImageConverter _imageConverter;
        private IImageModifier _imageModifier;
        public ImageController(ILogger<ImageController> logger, IImageConverter imageConverter, IImageModifier imageModifier)
        {
            _logger = logger;
            _imageConverter = imageConverter;
            _imageModifier = imageModifier;
        }

        [HttpPost("gaussian-blur")]
        public async Task<IActionResult> ApplyGaussianBlurToImage([FromBody][Required] ImageProcessRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Base64))
            {
                return BadRequest("The Base64 input is empty.");
            }

            var contentType = Utils.GetContentType(request.Encoding);

            byte[] bytes = null;
            try
            {
                bytes = Convert.FromBase64String(request.Base64);
            }
            catch (FormatException ex)
            {
                _logger.LogError($"Error while converting to byte[] from base64: {ex.Message}");
                return BadRequest("The input was not valid BASE64.");
            }

            byte[] bluredBytes = null;
            int width, height;
            try
            {
                (bluredBytes, width, height) = await _imageModifier.UseGaussianBlur(bytes, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while blurring the image: {ex.Message}");
                return StatusCode(500, "An internal error occured while blurring the image.");
            }

            var encodedImage = _imageConverter.EncodeRgbBytes(bluredBytes, width, height, request.Encoding);

            var stream = new MemoryStream(encodedImage);
            return new FileStreamResult(stream, contentType);
        }
    }
}
