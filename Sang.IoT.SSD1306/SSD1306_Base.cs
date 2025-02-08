using System.Device.I2c;

namespace Sang.IoT.SSD1306
{
    public partial class SSD1306_Base : IDisposable
    {
        #region 常量
        public const byte SSD1306_I2C_ADDRESS = 0x3C;    // 011110+SA0+RW - 0x3C or 0x3D
        public const byte SSD1306_SETCONTRAST = 0x81;
        public const byte SSD1306_DISPLAYALLON_RESUME = 0xA4;
        public const byte SSD1306_DISPLAYALLON = 0xA5;
        public const byte SSD1306_NORMALDISPLAY = 0xA6;
        public const byte SSD1306_INVERTDISPLAY = 0xA7;
        public const byte SSD1306_DISPLAYOFF = 0xAE;
        public const byte SSD1306_DISPLAYON = 0xAF;
        public const byte SSD1306_SETDISPLAYOFFSET = 0xD3;
        public const byte SSD1306_SETCOMPINS = 0xDA;
        public const byte SSD1306_SETVCOMDETECT = 0xDB;
        public const byte SSD1306_SETDISPLAYCLOCKDIV = 0xD5;
        public const byte SSD1306_SETPRECHARGE = 0xD9;
        public const byte SSD1306_SETMULTIPLEX = 0xA8;
        public const byte SSD1306_SETLOWCOLUMN = 0x00;
        public const byte SSD1306_SETHIGHCOLUMN = 0x10;
        public const byte SSD1306_SETSTARTLINE = 0x40;
        public const byte SSD1306_MEMORYMODE = 0x20;
        public const byte SSD1306_COLUMNADDR = 0x21;
        public const byte SSD1306_PAGEADDR = 0x22;
        public const byte SSD1306_COMSCANINC = 0xC0;
        public const byte SSD1306_COMSCANDEC = 0xC8;
        public const byte SSD1306_SEGREMAP = 0xA0;
        public const byte SSD1306_CHARGEPUMP = 0x8D;
        public const byte SSD1306_EXTERNALVCC = 0x1;
        public const byte SSD1306_SWITCHCAPVCC = 0x2;
        #endregion

        #region 滚动常量
        public const byte SSD1306_ACTIVATE_SCROLL = 0x2F;
        public const byte SSD1306_DEACTIVATE_SCROLL = 0x2E;
        public const byte SSD1306_SET_VERTICAL_SCROLL_AREA = 0xA3;
        public const byte SSD1306_RIGHT_HORIZONTAL_SCROLL = 0x26;
        public const byte SSD1306_LEFT_HORIZONTAL_SCROLL = 0x27;
        public const byte SSD1306_VERTICAL_AND_RIGHT_HORIZONTAL_SCROLL = 0x29;
        public const byte SSD1306_VERTICAL_AND_LEFT_HORIZONTAL_SCROLL = 0x2A;
        #endregion

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public int width { get; private set; }

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public int height { get; private set; }

        /// <summary>
        /// I2C总线
        /// </summary>
        private readonly I2cBus _bus;

        /// <summary>
        /// I2C设备
        /// </summary>
        private readonly I2cDevice _i2c;


        /// <summary>
        /// 页数
        /// </summary>
        public int pages { get; private set; }

        /// <summary>
        /// 待写入的buffer
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// vcc 状态
        /// </summary>
        public byte _vccstate;

        /// <summary>
        /// SSD1306_Base
        /// </summary>
        /// <param name="width">屏幕宽度</param>
        /// <param name="height">屏幕高度</param>
        /// <param name="i2c_bus">I2C总线ID</param>
        /// <param name="i2c_address">I2C设备地址</param>
        public SSD1306_Base(int width, int height, int i2c_bus, byte i2c_address = SSD1306_I2C_ADDRESS)
        {
            this.width = width;
            this.height = height;
            this.pages = (int)Math.Floor(height / 8.0);
            this._buffer = new byte[width * this.pages];

            this._bus = I2cBus.Create(i2c_bus);
            this._i2c = _bus.CreateDevice(i2c_address);

        }


        /// <summary>
        /// 针对不同分辨率设备的初始化
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 发送byte指令到显示器
        /// </summary>
        /// <param name="c"></param>
        public void Command(byte c)
        {
            // Co = 0, DC = 0
            _i2c.Write(new byte[] { 0x00, c });
        }

        /// <summary>
        /// 发送data数据到显示器
        /// </summary>
        /// <param name="c"></param>
        public void Data(byte[] c)
        {
            byte[] data = new byte[c.Length + 1];
            data[0] = 0x40;
            Array.Copy(c, 0, data, 1, c.Length);
            _i2c.Write(data);
        }


        /// <summary>
        /// 初始化库
        /// </summary>
        public void Begin(byte vccstate = SSD1306_SWITCHCAPVCC)
        {
            //保存vcc状态
            this._vccstate = vccstate;
            this.Initialize();
            this.Command(SSD1306_DISPLAYON);
        }

        /// <summary>
        /// 设置显示器buffer
        /// </summary>
        public void SetBuffer(byte[] c) {
            if (c.Length != _buffer.Length) return;
            _buffer = c;
        }

        /// <summary>
        /// 设置显示器buffer的特定区域
        /// </summary>
        public void SetBuffer(byte[] c, int x, int y, int regionWidth, int regionHeight) {
            int pages = (regionHeight + 7) / 8;
            int bufferWidth = this.width;
            for (int page = 0; page < pages; page++) {
                int bufferIndex = (y / 8 + page) * bufferWidth + x;
                int sourceIndex = page * regionWidth;
                Array.Copy(c, sourceIndex, _buffer, bufferIndex, regionWidth);
            }
        }

        /// <summary>
        /// 将buffer数据写入到显示器
        /// </summary>
        public void Display()
        {
            Command(SSD1306_COLUMNADDR);
            Command(0);              // 列起始地址 (0 = reset)
            Command((byte)(this.width - 1));  // 列结束地址
            Command(SSD1306_PAGEADDR);
            Command(0);              // 开始页 (0 = reset)
            Command((byte)(this.pages - 1));  // 结束页
            for (int i = 0; i < this._buffer.Length; i += 16)
            {
                Data(this._buffer[i..(i + 16)]);
            }
        }

        /// <summary>
        /// 清除buffer内容
        /// </summary>
        public void Clear()
        {
            this._buffer = new byte[width * this.pages];
            Display();
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this._i2c != null)
            {
                _i2c.Dispose();
            }
            if (this._bus != null)
            {
                _bus.Dispose();
            }
        }
    }
}