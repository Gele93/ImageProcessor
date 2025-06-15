namespace ImageProcessor.Services.Modifiers
{
    public interface IImageModifier
    {
        public Task<(byte[], int, int)> UseGaussianBlur(byte[] inputImage, CancellationToken cancellationToken);

    }
}
