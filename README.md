# 在桌面右上角显示当前的时间

这是一个桌面小挂件, 会在桌面的右上角实时显示当前的时钟和日期

`开发平台`: VS2019

`开发技术`: C#, WinForm

`使用到的技术`: WinForm无边框, 透明背景, 鼠标穿透等

---

### **效果图：**

![img](.image/1.png)![img](.image/2.png)

### 几个用到的关键代码：

```c#
//务必在程序using部分添加该引用
using System.Runtime.InteropServices;
using System.Drawing;

//在public Form1()之前添加以下代码
[DllImport("user32.dll")]
public static extern IntPtr GetDC(IntPtr hwnd);

[DllImport("user32.dll")]
public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

[DllImport("gdi32.dll")]
public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

//获取指定位置(屏幕的x，y位置)的颜色并返回C#中的Color类型
public Color GetColor(int x, int y)
{
    IntPtr hdc = GetDC(IntPtr.Zero); uint pixel = GetPixel(hdc, x, y);
    ReleaseDC(IntPtr.Zero, hdc);
    Color color = Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel & 0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);
    return color;
}
```

### 后期优化：

1、由于Windows Form控件自身的原因，虽然设置Transparent属性可以显示透明色，但是并不能达到鼠标穿透的效果，因此我将其设置成：当鼠标移动进入控件时，自动隐藏窗体，以免影响鼠标的正常操作。

更新：可以设置鼠标穿透属性，但是做这个项目的时候没有完全掌握该方法，大家可以参考我的其他博客内容，有牵扯到相关的鼠标穿透属性。

2、由于上一步设置了鼠标进入自动隐藏，因此我们将无法改变时钟显示的位置，我们可以通过Load方法将其自动固定到桌面右上角位置，涉及到代码如下：

```cs
private void Form1_Load(object sender, EventArgs e)
{
    this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
    this.Top = 0;
}
```

### 其他说明：

计时部分采用Windows Form自带的Timer控件实现

 

## 补充：

鼠标穿透使用到的部分有：

```c#
[DllImport("user32", EntryPoint = "SetWindowLong")]
private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

[DllImport("user32", EntryPoint = "GetWindowLong")]
private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

public string Penetrate(IntPtr Handle)
{
    uint intExTemp = GetWindowLong(Handle, GWL_EXSTYLE);
    uint oldGWLEx;
    oldGWLEx = SetWindowLong(Handle, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT |     WS_EX_LAYERED);
    return "设置鼠标穿透成功！";
}


//调用部分
Penetrate(this.Handle);
```

## 其他

1. 本软件于2016年7月左右编写, 2019年才编写README, 很多地方介绍不是很详细

2. 如果有任何疑问欢迎下载源码讨论

3. 有其他疑问也可联系[作者](maileto:zzudongxiang@163.com)

4. 如果需要程序开机自启动, 需要将生成的exe置于以下位置

   ```path
   C:\Users\<YourName>\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
   # 其中<YourName>为当前用户的名称
   ```

   