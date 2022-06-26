using SkiaSharp;

namespace Sang.IoT.SSD1306
{
    public class SSD1306_128_64 : SSD1306_Base
    {
        /// <summary>
        /// SSD1306_128_64
        /// </summary>
        /// <param name="i2c_bus">I2C总线ID</param>
        public SSD1306_128_64(int i2c_bus) : base(128, 64, i2c_bus)
        {

        }

        public override void Initialize()
        {

            Command(SSD1306_DISPLAYOFF);            // 0xAE
            Command(SSD1306_SETDISPLAYCLOCKDIV);    // 0xD5
            Command(0x80);                          // 建议的比率 0x80
            Command(SSD1306_SETMULTIPLEX);          //0xA8
            Command(0x3F);
            Command(SSD1306_SETDISPLAYOFFSET);      // 0xD3
            Command(0x0);                           // 无偏移量
            Command(SSD1306_SETSTARTLINE | 0x0);    // line #0
            Command(SSD1306_CHARGEPUMP);            // 0x8D
            if (this._vccstate == SSD1306_EXTERNALVCC)
                Command(0x10);
            else
                Command(0x14);
            Command(SSD1306_MEMORYMODE);            // 0x20
            Command(0x00);                          // 0x0 act like ks0108
            Command(SSD1306_SEGREMAP | 0x1);
            Command(SSD1306_COMSCANDEC);
            Command(SSD1306_SETCOMPINS);            // 0xDA
            Command(0x12);
            Command(SSD1306_SETCONTRAST);           // 0x81
            if (this._vccstate == SSD1306_EXTERNALVCC)
                Command(0x9F);
            else
                Command(0xCF);
            Command(SSD1306_SETPRECHARGE);          // 0xd9
            if (this._vccstate == SSD1306_EXTERNALVCC)
                Command(0x22);
            else
                Command(0xF1);
            Command(SSD1306_SETVCOMDETECT);         // 0xDB
            Command(0x40);
            Command(SSD1306_DISPLAYALLON_RESUME);   // 0xA4
            Command(SSD1306_NORMALDISPLAY);         // 0xA6
        }

        /// <summary>
        /// 设置图片数据 128*64
        /// </summary>
        /// <param name="filepath">图片路径</param>
        public void Image(string filepath) {
            Image(SKBitmap.Decode(filepath));
        }

        /// <summary>
        /// 设置图片数据 128*64
        /// </summary>
        /// <param name="imageData">图片数据</param>
        public void Image(byte[] imageData)
        {
            Image(SKBitmap.Decode(imageData));
        }

        private void Image(SKBitmap bitmap)
        {
            if (bitmap.Width != 128 || bitmap.Height != 64) return;

            byte[] Data = new byte[this.width * this.pages];
            int index = 0;
            for (int page = 0; page < this.pages; page++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    int bits = 0;
                    for (int bit = 0; bit < 8; bit++)
                    {
                        bits = bits << 1;
                        if (bitmap.GetPixel(x, page * 8 + 7 - bit).ToString() == "#ff000000")
                        {
                            bits = bits | 0;
                        }
                        else
                        {
                            bits = bits | 1;
                        }
                    }
                    Data[index] = (byte)bits;
                    index++;
                }
            }
            this.SetBuffer(Data);
        }
    }
}
