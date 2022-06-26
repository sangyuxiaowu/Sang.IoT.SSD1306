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
using (var oled = new SSD1306_128_64(1)) {
    oled.Begin();
    oled.Clear();
    oled.Display();
}
```