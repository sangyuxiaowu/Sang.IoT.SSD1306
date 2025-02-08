using SkiaSharp;

namespace Sang.IoT.SSD1306
{
    public partial class SSD1306_Base
    {
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