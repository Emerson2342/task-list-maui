using SkiaSharp;

namespace TaskListMaui.Source.Domain.Main.Services
{
    public static class PhotoHelper
    {
      public static SKBitmap AutoOrient(SKBitmap bitmap, SKEncodedOrigin origin)
        {
            SKBitmap rotated;
            switch (origin)
            {
                case SKEncodedOrigin.BottomRight:
                    rotated = new SKBitmap(bitmap.Width, bitmap.Height);
                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }
                    return rotated;
                case SKEncodedOrigin.RightTop:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(rotated.Width, 0);
                        surface.RotateDegrees(90);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }
                    return rotated;
                case SKEncodedOrigin.LeftBottom:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(0, rotated.Height);
                        surface.RotateDegrees(270);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }
                    return rotated;
                default:
                    return bitmap;
            }
        }

        public static SKBitmap LoadBitmapWithOrigin(Stream imageStream, out SKEncodedOrigin origin)
        {
            // Reset stream position
            if (imageStream.CanSeek)
                imageStream.Seek(0, SeekOrigin.Begin);

            try
            {
                using (var codec = SKCodec.Create(imageStream))
                {
                    if (codec == null)
                    {
                        origin = SKEncodedOrigin.Default; // Default origin if codec creation failed
                        return null; // Early exit if the image format is unrecognized
                    }

                    origin = codec.EncodedOrigin; // Get the encoded origin
                    var info = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                    var bitmap = SKBitmap.Decode(codec, info);
                    if (bitmap == null)
                    {
                        throw new InvalidOperationException("Failed to decode the bitmap.");
                    }

                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your application's error handling policies
                origin = SKEncodedOrigin.Default; // Set default origin in case of failure
                Console.WriteLine($"Error loading bitmap: {ex.Message}");
                return null;
            }
        }

    }
}
