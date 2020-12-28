using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Threading;
using System.ComponentModel;
namespace Clock
{
    class My_Class
    {

        #region  using DLL

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        #endregion

        #region Statement

        private const uint WS_EX_LAYERED = 0x80000;

        private const int WS_EX_TRANSPARENT = 0x20;

        private const int GWL_STYLE = (-16);

        private const int GWL_EXSTYLE = (-20);

        private int Y_Step = 0;

        private string FlowString = "";

        private int FlowIndex = 0;

        private string FlowTime = "";

        #endregion

        #region get/set:public string Flow_String

        /// <summary>
        /// get得到字符串，每500MS去一次可达到动态效果
        /// set设置新的字符串
        /// </summary>

        public string Flow_String
        {
            get
            {
                if (FlowString == null || FlowString == "")
                    return "";
                int TMP = FlowString.Length;
                if (FlowIndex > TMP - 1)
                    FlowIndex = 0;
                else
                    FlowIndex++;
                if (TMP < 12)
                    return FlowString;
                else if (TMP - FlowIndex > 20)
                    return FlowString.Substring(FlowIndex, 20);
                else
                    return FlowString.Substring(FlowIndex, FlowString.Length - FlowIndex);
            }
            set
            {
                FlowString = value;
            }
        }

        #endregion

        #region get/set:public string Flow_Time

        /// <summary>
        /// 用于记录任务的时间
        /// </summary>
       
        public string Flow_Time
        {
            get { return FlowTime; }
            set { FlowTime = value; }
        }

        #endregion

        #region public void Penetrate(IntPtr Handle)

        /// <summary>
        /// 鼠标穿透与透明
        /// </summary>
        /// <param name="Handle"></param>
        
        public string Penetrate(IntPtr Handle)
        {
            uint intExTemp = GetWindowLong(Handle, GWL_EXSTYLE);
            uint oldGWLEx;
            oldGWLEx = SetWindowLong(Handle, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            return "设置鼠标穿透成功！";
        }

        #endregion

        #region public Color[] Get_Inverse_Color(int X, int Y)

        /// <summary>
        /// SetColor[0]  X,Y点颜色的反色
        /// SetColor[1]  X,Y点颜色的反色的相近色，作为背景剔除
        /// SetColor[2]  X,Y点颜色的反色偏红色
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        /// 

        public Color[] Get_Inverse_Color(int X, int Y)
        {
            Color[] _OutColor = new Color[3];
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, X, Y);
            ReleaseDC(IntPtr.Zero, hdc);
            int R = 255 - ((int)(pixel & 0x000000FF)),
                G = 255 - ((int)(pixel & 0x0000FF00) >> 8),
                B = 255 - ((int)(pixel & 0x00FF0000) >> 16);
            _OutColor[0] = Color.FromArgb(R, G, B);
            if (R < 5)
                R += 6;
            _OutColor[1] = Color.FromArgb(R - 5, G, B);
            _OutColor[2] = Color.FromArgb(255, G / 2, B / 2);
            return _OutColor;
        }

        #endregion

        #region public string[,] Read_All_String()

        /// <summary>
        /// 根据本地记录获取文本返回记录中的时间和内容
        /// 第一列数据为时间，第二列数据为内容
        /// </summary>
        /// <returns></returns>

        public string[,] Read_All_String()
        {
            if (File.Exists("Data.DB"))
            {
                try
                {
                    StreamReader SR = new StreamReader("Data.DB", Encoding.UTF8);
                    List<string> _Out_List = new List<string>();
                    string TMP = "";
                    while ((TMP = SR.ReadLine()) != null)
                        _Out_List.Add(TMP);
                    string[] _Out_Temp = _Out_List.ToArray();
                    int Total_Line = _Out_List.Count();
                    string[,] _Out = new string[Total_Line, 2];
                    for (int i = 0; i < Total_Line; i++)
                    {
                        string[] Temp_For = _Out_Temp[i].Split('_');
                        try
                        {
                            _Out[i, 0] = Temp_For[0];
                            _Out[i, 1] = Temp_For[1];
                        }
                        catch (Exception Ex)
                        {
                            _Out[i, 0] = "--:--";
                            _Out[i, 1] = Ex.Message;
                        }

                    }
                    return _Out;
                }
                catch (Exception Ex)
                {
                    return new string[,] { { "--:--" }, { Ex.Message } };
                }
            }
            else
            {
                File.Create("Data.DB");
                return new string[,] { { "--:--" }, { "暂无工作提醒。" } };
            }
        }

        #endregion

        #region public string[] Read_Next_String(DateTime Time)

        /// <summary>
        /// 输入当前时间，返回下一个将要提醒的工作
        /// _Out[0]表示时间
        /// _Out[1]表示内容
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>

        public string[] Read_Next_String(DateTime Time)
        {
            string[] _Out = new string[2];
            string[,] Temp_String = Read_All_String();
            int Total_Line = Temp_String.Length / 2;
            for (int i = 0; i < Total_Line; i++)
            {
                string[] Split_Time = Temp_String[i, 0].Split(':');
                if (Split_Time[0] == "--")
                    continue;
                int Hour = Convert.ToInt16(Split_Time[0]);
                int Minute = Convert.ToInt16(Split_Time[1]);
                if ((Hour > Time.Hour) || (Hour == Time.Hour && Minute >= Time.Minute))
                {
                    _Out[0] = Hour.ToString("00:") + Minute.ToString("00");
                    _Out[1] = Temp_String[i, 1];
                    break;
                }
            }
            return _Out;
        }

        #endregion

        #region public string Write_All_String(string[] Str)

        /// <summary>
        /// 把数组Str中的数据写入本地记录
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>

        public string Write_All_String(string[] Str)
        {
            FileStream FS = null;
            StreamWriter SW = null;
            try
            {
                FS = new FileStream("Data.DB", FileMode.Create);
                SW = new StreamWriter(FS);
                int Lines_Count = Str.Count();
                for (int i = 0; i < Lines_Count; i++)
                    SW.WriteLine(Str[i], Encoding.UTF8);
                SW.Flush();
                SW.Close();
                FS.Close();
                return "保存成功！";
            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
        }

        #endregion

        #region public String TimeToString(DateTime Time)

        /// <summary>
        /// 根据时间获取跟此时刻最相近的消息
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>

        public String TimeToString(DateTime Time)
        {
            string _Out = "";
            string[,] Temp_String = Read_All_String();
            int Total_Line = Temp_String.Length / 2;
            double Tmp = 24 * 60;
            for (int i = 0; i < Total_Line; i++)
            {
                string[] Split_Time = Temp_String[i, 0].Split(':');
                if (Split_Time[0] == "--")
                    continue;
                int Hour = Convert.ToInt16(Split_Time[0]);
                int Minute = Convert.ToInt16(Split_Time[1]);
                DateTime Date = new DateTime(Time.Year, Time.Month, Time.Day, Hour, Minute, 0);
                double a = Math.Abs((Date - Time).TotalMinutes);
                if (a < 5)
                {
                    if (a < Tmp)
                    {
                        _Out = Temp_String[i, 1];
                        Tmp = a;
                    }
                }
            }
            return _Out;
        }

        #endregion

        #region public int Cycle_SUM(int Frist_Arge, int Second_Arge, int Cyc)

        /// <summary>
        /// 以Cyc为周期的环形加减算法
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>

        public int Cycle_SUM(int Frist_Arge, int Second_Arge, int Cyc)
        {
            if (Frist_Arge + Second_Arge > Cyc - 1)
                return Frist_Arge + Second_Arge - Cyc;
            else if (Frist_Arge + Second_Arge < 0)
                return Frist_Arge + Second_Arge + Cyc;
            else
                return Frist_Arge + Second_Arge;
        }

        #endregion

        #region public int TimeToPoint(int Heigh, DateTime Time)

        /// <summary>
        /// 输入总长度和时间，可计算得其结果
        /// </summary>
        /// <param name="Heigh"></param>
        /// <param name="Time"></param>
        /// <returns></returns>

        public int TimeToPoint(int Heigh, DateTime Time)
        {
            int H = Time.Hour;
            int M = Time.Minute;
            int Step = Heigh / 25;
            return (int)((H + (double)M / 60 + 0.5) * Y_Step);
        }

        public int TimeToPoint(int Heigh, int H, int M)
        {
            int Step = Heigh / 25;
            return (int)((H + (double)M / 60 + 0.5) * Y_Step);
        }

        #endregion

        #region public DateTime PointToTime(int Height, int Point)

        /// <summary>
        /// 根据鼠标所得的Y点计算时间
        /// </summary>
        /// <param name="Height"></param>
        /// <param name="Point"></param>
        /// <returns></returns>

        public DateTime PointToTime(int Height, int Point)
        {
            int Step = Height / 25;
            int H = (int)((double)Point / Step - 0.5);
            if(H==24)
            {
                return new DateTime(2000, 1, 1, 0, 0, 0);
            }
            int Lenght1 = (int)(Point - (H + 0.5) * Step);
            int M = Cycle_SUM((int)((double)Lenght1 / Step * 60), 0, 60);
            return new DateTime(2000, 1, 1, H, M, 0);
        }

        #endregion

        #region public Bitmap Paint_Schedule1_BackGround(int Width, int Heigh)

        /// <summary>
        /// 输入画布的长宽，输出带有时间轴的基础背景图案
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Heigh"></param>
        /// <returns></returns>

        public Bitmap Paint_Schedule1_BackGround(int Width, int Heigh)
        {
            Bitmap BackGround = new Bitmap(Width, Heigh);
            Graphics BG_G = Graphics.FromImage(BackGround);
            //设置背景颜色和边框
            BG_G.Clear(Color.FromArgb(0, 0, 0));
            Point[] Points = new Point[5] { 
                new Point(0, 0), new Point(Width, 0), new Point(Width, Heigh), new Point(0, Heigh) ,new Point(0, 0)};
            BG_G.DrawLines(new Pen(Color.White, 2), Points);
            //声明常用变量
            Pen Line_Pen = new Pen(Color.White);
            Font String_Font = new Font("宋体", 11, FontStyle.Bold);
            //画整体时间轴
            int Center_X = 55;
            Y_Step = Heigh / 25;
            BG_G.DrawLine(Line_Pen, Center_X, 0, Center_X, Heigh);
            //画刻度线
            for (int i = 0; i < 25; i++)
            {
                int Temp_Y = (int)((i + 0.5) * Y_Step);
                if (i == 24)
                {
                    BG_G.DrawLine(Line_Pen, Center_X, Temp_Y, Center_X - 5, Temp_Y);
                    BG_G.DrawLine(Line_Pen, 0, Temp_Y, 5, Temp_Y);
                    BG_G.DrawString("次晨", String_Font, Brushes.YellowGreen, 11, Temp_Y - 13);
                    BG_G.DrawString("00:00", String_Font, Brushes.YellowGreen, 5, Temp_Y );
                    break;
                }               
                BG_G.DrawLine(Line_Pen, Center_X, Temp_Y, Center_X - 5, Temp_Y);
                BG_G.DrawLine(Line_Pen, 0, Temp_Y, 5, Temp_Y);
                BG_G.DrawString((i).ToString("00") + ":00", String_Font, Brushes.YellowGreen, 5, Temp_Y - 7);
                Temp_Y += Y_Step / 2;
                BG_G.DrawLine(Line_Pen, Center_X, Temp_Y, Center_X - 8, Temp_Y);
                BG_G.DrawLine(Line_Pen, 0, Temp_Y, 8, Temp_Y);
            }
            return BackGround;
        }

        #endregion

        #region public Bitmap Paint_Schedule1_String(Bitmap BackGround, string[,] Str)

        /// <summary>
        /// 在原始图片上写入字符串
        /// </summary>
        /// <param name="BackGround"></param>
        /// <param name="Str"></param>
        /// <returns></returns>

        public Bitmap Paint_Schedule1_String(Bitmap BackGround, string[,] Str)
        {
            int Total_Line = Str.Length / 2;
            Graphics BG_G = Graphics.FromImage(BackGround);
            int Center_X = BackGround.Width - 55;
            for (int i = 0; i < Total_Line; i++)
            {
                string[] TMP = Str[i, 0].Split(':');
                if (TMP[0] == "--")
                    continue;
                int H = Convert.ToInt16(TMP[0]);
                int M = Convert.ToInt16(TMP[1]);
                int Temp_Y = (int)((H + (double)M / 60 + 0.5) * Y_Step);
                BG_G.DrawLine(new Pen(Color.Yellow, 8), 0, Temp_Y, 8, Temp_Y);
                BG_G.DrawLine(new Pen(Color.Yellow, 8), BackGround.Width, Temp_Y, BackGround.Width - 8, Temp_Y);
                BG_G.DrawLine(new Pen(Color.Yellow, 1), 0, Temp_Y, BackGround.Width, Temp_Y);
            }
            return BackGround;
        }

        public Bitmap Paint_Schedule1_String(Image Img, string[,] Str)
        {
            Bitmap BackGround = new Bitmap(Img);
            return Paint_Schedule1_String(BackGround, Str);
        }

        #endregion

        #region public Bitmap Paint_Schedule1_TimeLine(Bitmap BackGround, DateTime Time)

        /// <summary>
        /// 画出Time所在的时间线
        /// </summary>
        /// <param name="BackGround"></param>
        /// <param name="Time"></param>
        /// <returns></returns>

        public Bitmap Paint_Schedule1_TimeLine(Bitmap BackGround, DateTime Time)
        {
            Graphics BG_G = Graphics.FromImage(BackGround);
            int Time_Y = TimeToPoint(BackGround.Height, Time);
            BG_G.DrawLine(new Pen(Color.Red, 3), 0, Time_Y, BackGround.Width, Time_Y);
            return BackGround;
        }

        #endregion

    }
}
