# Sang.IoT.SSD1306

 .NET library to use SSD1306-based 128x64 pixel OLED displays with a IoT device.Passed the test on the Jetson nano device. 

 .NET 类库，用于驱动 IOT 设备基于 SSD1306 分辨率 128*64 的 OLED 显示器，已在 Jetson Nano 设备上测试通过。

## Instructions:

##### Step 1 

Create a new .NET Console App

```
dotnet new console -o i2c_oled
```

##### Step 2

Add the [Sang.IoT.SSD1306](https://www.nuget.org/packages/Sang.IoT.SSD1306/) to the project. 

```
dotnet add package Sang.IoT.SSD1306
```

#### Step 3

Replace the contents of Program.cs with the following code:

```csharp
using Sang.IoT.SSD1306;

using (var oled = new SSD1306_128_64(1)) {
    oled.Begin();
    // set data to oled
    byte[] c = new byte[128*64]{...};
    oled.SetBuffer(c);
    oled.Display();
}
```

## Display Image

```csharp
using Sang.IoT.SSD1306;

using (var oled = new SSD1306_128_64(1)) {
    oled.Begin();
    oled.Image("assets/test.png");
    oled.Display();
}
```

## Display Text

```csharp
using Sang.IoT.SSD1306;
using SkiaSharp;

using (var oled = new SSD1306_128_64(1)) {

    oled.Begin();
    oled.Clear();

    using(var bitmap = new SKBitmap(128, 64, true)){
        SKCanvas canvas = new SKCanvas(bitmap);
        SKPaint paint = new SKPaint() { 
            Color = new SKColor(255, 255, 255),
            StrokeWidth = 1, //画笔宽度
            Typeface = SKTypeface.FromFile("/home/sangsq/i2c_led/SourceHanSansCN-Normal.ttf"),
            TextSize = 13,  //字体大小
            Style = SKPaintStyle.Fill,
        };
        canvas.DrawText("公众号：sangxiao99 ", 0, 13, paint);
        paint.TextSize = 30;
        canvas.DrawText("桑榆肖物 ", 0, 50, paint);
        oled.Image(bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray());
    }

    oled.Display();
}
```


## Clear

Use `oled.Clear();`.
