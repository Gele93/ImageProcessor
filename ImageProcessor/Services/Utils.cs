using ImageProcessor.Data;

namespace ImageProcessor.Services
{
    public static class Utils
    {
        public static string GetContentTypeString(EncodingType encoding) => encoding switch
        {
            EncodingType.JPG => "image/jpg",
            EncodingType.PNG => "image/png",
            _ => "application/octet-stream"
        };

        public static bool IsValidJpgOrPng(IFormFile file)
        {
            if (file == null)
                throw new Exception("Input file was not found");

            if (file.ContentType == null)
                throw new Exception("Input file content type was not found");

            if (!(file.ContentType == "image/jpeg" || file.ContentType == "image/png"))
                throw new Exception($"Ivalid file content type:{file.ContentType}  (upload .jpg or .png)");

            return true;
        }
    }
}
