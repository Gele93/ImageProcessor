using ImageProcessor.Data;

namespace ImageProcessor.Services
{
    public static class Utils
    {
        public static string GetContentType(EncodingType encoding) => encoding switch
        {
            EncodingType.JPG => "image/jpg",
            EncodingType.PNG => "image/png",
            _=> "application/octet-stream"
        };
    }
}
