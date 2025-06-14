using System.ComponentModel.DataAnnotations;

namespace ImageProcessor.Data
{
    public record ImageProcessRequest(
        [Required]
        string Base64,
        [Required]
        EncodingType Encoding
        );
}
