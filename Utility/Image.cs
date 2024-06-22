using System.Drawing;

namespace Aapi.Utility;

static class ImageUtils {
    public static System.Drawing.Image ImageFromIFormFile(IFormFile file)
    {
        return System.Drawing.Image.FromStream(file.OpenReadStream());
    }

    public static System.Drawing.Image ResizeImage(System.Drawing.Image image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);
        var destImage = new Bitmap(width, height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using var graphics = Graphics.FromImage(destImage);
        graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

        using var wrapMode = new System.Drawing.Imaging.ImageAttributes();
        wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

        return destImage;
    }
}

