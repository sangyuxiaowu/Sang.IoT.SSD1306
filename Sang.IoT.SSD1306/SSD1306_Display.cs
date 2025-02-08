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

            byte[] data = new byte[this.width * this.pages];
            int index = 0;
            for (int page = 0; page < this.pages; page++)
            {
                for (int i = 0; i < this.width; i++)
                {
                    data[index++] = GetByteForColumn(bitmap, i , page);
                }
            }
            this.SetBuffer(data, startX, startY, regionWidth, regionHeight);
        }

        private byte GetByteForColumn(SKBitmap bitmap, int x, int page)
        {
            int bits = 0;
            for (int bit = 0; bit < 8; bit++)
            {
                bits <<= 1;
                if (bitmap.GetPixel(x, page * 8 + 7 - bit).ToString() == "#ff000000")
                {
                    bits = bits | 0;
                }
                else
                {
                    bits = bits | 1;
                }
            }
            return (byte)bits;
        }
    }
}