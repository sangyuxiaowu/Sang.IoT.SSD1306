using Sang.IoT.SSD1306;
using SkiaSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        BaseTest();
    }

    private static void BaseTest()
    {
        using (var oled = new SSD1306_128_64(1))
        {

            oled.Begin();
            oled.Clear();

            using (var bitmap = new SKBitmap(128, 64, true))
            {
                SKCanvas canvas = new SKCanvas(bitmap);
                SKPaint paint = new SKPaint()
                {
                    Color = new SKColor(255, 255, 255),
                    StrokeWidth = 1, //画笔宽度
                    Style = SKPaintStyle.Fill,
                };
                SKFont font = new SKFont(SKTypeface.Default, 13);

                canvas.DrawText("Sang.IoT.SSD1306 ", 0, 13, font, paint);
                font.Size = 30;
                canvas.DrawText("桑榆肖物 ", 0, 50, font, paint);
                using (FileStream fs = new FileStream("tmp.png", FileMode.Create))
                    bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);
                oled.Image(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray());
            }

            oled.Display();
        }
    }
}