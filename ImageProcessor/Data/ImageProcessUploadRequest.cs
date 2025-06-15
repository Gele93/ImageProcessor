using System.ComponentModel.DataAnnotations;

namespace ImageProcessor.Data
{
    public record ImageProcessUploadRequest(
        [Required]
        IFormFile File,
        [Required]
        EncodingType Encoding
        );
}
