using SkiaSharp;

namespace Sang.IoT.SSD1306
{
    public partial class SSD1306_Base
    {
        /// <summary>
        /// 设置图片数据
        /// </summary>
        /// <param name="filePath">图片路径</param>
        public void Image(string filePath)
        {
            Image(SKBitmap.Decode(filePath));
        }

        /// <summary>
        /// 设置图片数据
        /// </summary>
        /// <param name="imageData">图片数据</param>
        public void Image(byte[] imageData)
        {
            Image(SKBitmap.Decode(imageData));
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
            if (bitmap.Width < startX + regionWidth || bitmap.Height < startY + regionHeight) return;

            byte[] data = new byte[regionWidth * ((regionHeight + 7) / 8)];
            int index = 0;
            for (int page = 0; page < (regionHeight + 7) / 8; page++)
            {
                for (int i = 0; i < regionWidth; i++)
                {
                    data[index++] = GetByteForColumn(bitmap, i + startX, startY + page * 8);
                }
            }
            this.SetBuffer(data, startX, startY, regionWidth, regionHeight);
        }

        private byte GetByteForColumn(SKBitmap bitmap, int x, int startY)
        {
            int bits = 0;
            for (int bit = 0; bit < 8; bit++)
            {
                bits <<= 1;
                int pixelY = startY + 7 - bit;
                if (pixelY < bitmap.Height && bitmap.GetPixel(x, pixelY).Alpha == 255)
                {
                    bits |= 1;
                }
            }
            return (byte)bits;
        }
    }
}