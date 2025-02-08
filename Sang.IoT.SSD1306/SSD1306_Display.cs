using SkiaSharp;

namespace Sang.IoT.SSD1306
{
    public partial class SSD1306_Base
    {

        /// <summary>
        /// 绘制文本
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="fontFile">字体文件</param>
        /// <param name="fontSize">字体大小，默认13</param>
        /// <param name="bgHeight">背景高度，默认字体大小+3</param>
        /// <param name="bgWidth">背景宽度，默认剩余空间</param>
        /// <param name="drawOffset">绘制偏移量，默认0</param>
        public void DrawText(string text, int x, int y, string fontFile, int fontSize = 13, int bgHeight = 0, int bgWidth = 0, int drawOffset = 0)
        {
            bgWidth = bgWidth <= 0 ? this.width - x : bgWidth;
            bgHeight = bgHeight <= 0 ? fontSize + 3 : bgHeight;

            var regionWidth = Math.Min(bgWidth, this.width - x);
            var regionHeight = Math.Min(bgHeight, this.height - y);

            using (var bitmap = new SKBitmap(regionWidth, regionHeight, true))
            {
                SKCanvas canvas = new(bitmap);
                SKPaint paint = new()
                {
                    Color = new SKColor(255, 255, 255),
                    StrokeWidth = 1,
                    Style = SKPaintStyle.Fill,
                };
                SKFont font = new(SKTypeface.FromFile(fontFile), fontSize);
                canvas.DrawText(text, 0, fontSize + drawOffset, font, paint);
                Image(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray(), x, y, regionWidth, regionHeight);
            }
        }


        /// <summary>
        /// 设置图片数据
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="regionWidth">区域宽度</param>
        /// <param name="regionHeight">区域高度</param>
        public void Image(string filePath, int startX = 0, int startY = 0, int regionWidth = 128, int regionHeight = 64)
        {
            Image(SKBitmap.Decode(filePath), startX, startY, regionWidth, regionHeight);
        }

        /// <summary>
        /// 设置图片数据
        /// </summary>
        /// <param name="imageData">图片数据</param>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="regionWidth">区域宽度</param>
        /// <param name="regionHeight">区域高度</param>
        public void Image(byte[] imageData, int startX = 0, int startY = 0, int regionWidth = 128, int regionHeight = 64)
        {
            Image(SKBitmap.Decode(imageData), startX, startY, regionWidth, regionHeight);
        }

        /// <summary>
        /// 设置图片数据
        /// </summary>
        /// <param name="bitmap">图片数据</param>
        /// <param name="startX">起始X坐标</param>
        /// <param name="startY">起始Y坐标</param>
        /// <param name="regionWidth">区域宽度</param>
        /// <param name="regionHeight">区域高度</param>
        public void Image(SKBitmap bitmap, int startX = 0, int startY = 0, int regionWidth = 128, int regionHeight = 64)
        {
            // 边界检查
            if (startX < 0 || startY < 0 || startX >= this.width || startY >= this.height) return;

            // 调整区域大小确保不超出显示范围
            regionWidth = Math.Min(regionWidth, this.width - startX);
            regionHeight = Math.Min(regionHeight, this.height - startY);

            if (bitmap.Width < regionWidth || bitmap.Height < regionHeight) return;

            // 计算需要更新的页数
            int startPage = startY / 8;
            int endPage = (startY + regionHeight + 7) / 8;
            int pageCount = endPage - startPage;

            byte[] data = new byte[regionWidth * pageCount];
            int index = 0;

            for (int page = 0; page < pageCount; page++)
            {
                for (int x = 0; x < regionWidth; x++)
                {
                    data[index++] = GetByteForColumn(bitmap, x, page, startY % 8);
                }
            }

            this.SetBuffer(data, startX, startY, regionWidth, regionHeight);
        }

        private byte GetByteForColumn(SKBitmap bitmap, int x, int page, int yOffset)
        {
            int bits = 0;
            for (int bit = 0; bit < 8; bit++)
            {
                bits <<= 1;
                int y = page * 8 + 7 - bit + yOffset;
                if (y < bitmap.Height)
                {
                    if (bitmap.GetPixel(x, y).ToString() == "#ff000000")
                    {
                        bits = bits | 0;
                    }
                    else
                    {
                        bits = bits | 1;
                    }
                }
            }
            return (byte)bits;
        }
    }
}