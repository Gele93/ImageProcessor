using ImageProcessor.Data;
using ImageProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ImageProcessor.Services.Converters;
using ImageProcessor.Services.Modifiers;
using System.Threading;

namespace ImageProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IImageConverter _imageConverter;
        private readonly IImageModifier _imageModifier;
        public ImageController(ILogger<ImageController> logger, IImageConverter imageConverter, IImageModifier imageModifier)
        {
            _logger = logger;
            _imageConverter = imageConverter;
            _imageModifier = imageModifier;
        }

        /// <summary>
        /// Applies a Gaussian blur to a Base64-encoded image and returns the result.
        /// </summary>
        /// <param name="request">The request object containing Base64 image and encoding format.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>A blurred image stream in the requested format.</returns>
        [HttpPost("gaussian-blur/base-64")]
        public async Task<IActionResult> ApplyGaussianBlurToBase64Image([FromBody][Required] ImageProcessBase64Request request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (string.IsNullOrWhiteSpace(request.Base64))
                return BadRequest("The Base64 input is empty.");

            var contentType = Utils.GetContentTypeString(request.Encoding);

            //Get binary data of the image
            byte[] encodedBytes;
            try
            {
                encodedBytes = Convert.FromBase64String(request.Base64);
            }
            catch (FormatException ex)
            {
                _logger.LogError($"Error while converting to byte[] from base64: {ex.Message}");
                return BadRequest("The input was not valid BASE64.");
            }

            //blurredBytes will be filled with raw BGRA data of blured image
            byte[] blurredBGRAbytes;
            int width, height;
            try
            {
                (blurredBGRAbytes, width, height) = await _imageModifier.UseGaussianBlur(encodedBytes, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while blurring the image: {ex.Message}");
                return StatusCode(500, $"An internal error occurred while blurring the image: {ex.Message}");
            }

            //Encodes BGRA data to binary byte[]
            byte[] encodedBluredBytes;
            try
            {
                encodedBluredBytes = _imageConverter.EncodeRgbBytes(blurredBGRAbytes, width, height, request.Encoding);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Failed to encode blurred image: {ex.Message}. Supported encoding types: .jpg, .png");
                return BadRequest($"Failed to encode blurred image: {ex.Message}. Supported encoding types: .jpg, .png");
            }

            var stream = new MemoryStream(encodedBluredBytes);
            return new FileStreamResult(stream, contentType);
        }

        /// <summary>
        /// Applies a Gaussian blur to an uploaded .jpg or .png image and returns the result.
        /// </summary>
        /// <param name="request">The request object containing IFormFile file and encoding format.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>A blurred image stream in the requested format.</returns>
        [HttpPost("gaussian-blur/upload")]
        public async Task<IActionResult> ApplyGaussianBlurToUploadedImage([FromForm][Required] ImageProcessUploadRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contentType = Utils.GetContentTypeString(request.Encoding);

            //Validates file format
            try
            {
                Utils.IsValidJpgOrPng(request.File);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid file uploaded: {ex.Message}");
                return BadRequest("Invalid file uploaded");
            }

            //Get binary data of the image
            byte[] encodedBytes;
            try
            {
                encodedBytes = await _imageConverter.ConvertImageToBytes(request.File, cancellationToken);
            }
            catch (FormatException ex)
            {
                _logger.LogError($"Error while converting the image to byte[]: {ex.Message}");
                return BadRequest("The uploaded image could not be converted to byte[].");
            }

            //blurredBytes will be filled with raw BGRA data of blurred image
            byte[] blurredBGRAbytes;
            int width, height;
            try
            {
                (blurredBGRAbytes, width, height) = await _imageModifier.UseGaussianBlur(encodedBytes, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while blurring the image: {ex.Message}");
                return StatusCode(500, $"An internal error occurred while blurring the image: {ex.Message}");
            }

            //Encodes BGRA data to binary byte[]
            byte[] encodedBluredBytes;
            try
            {
                encodedBluredBytes = _imageConverter.EncodeRgbBytes(blurredBGRAbytes, width, height, request.Encoding);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Failed to encode blurred image: {ex.Message}. Supported encoding types: .jpg, .png");
                return BadRequest($"Failed to encode blurred image: {ex.Message}. Supported encoding types: .jpg, .png");
            }
            var stream = new MemoryStream(encodedBluredBytes);
            return new FileStreamResult(stream, contentType);
        }


    }
}
