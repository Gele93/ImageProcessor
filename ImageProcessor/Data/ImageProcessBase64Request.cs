using System.ComponentModel.DataAnnotations;

namespace ImageProcessor.Data
{
    public record ImageProcessBase64Request(
        [Required]
        string Base64,
        [Required]
        EncodingType Encoding
        );
}
