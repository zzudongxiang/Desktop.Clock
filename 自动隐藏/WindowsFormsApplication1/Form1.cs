using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        int num = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TopMost = true;
            label1.Text = DateTime.Now.ToString("HH:mm");
            label2.Text = DateTime.Now.ToString("yy年MM月dd日");
            label3.Text = DateTime.Now.DayOfWeek.ToString();
            int R, B, G;
            IntPtr hdc = GetDC(IntPtr.Zero); 
            uint pixel = GetPixel(hdc, this.Location.X+96,this .Location.Y+32);
            ReleaseDC(IntPtr.Zero, hdc);
            G = 255 - ((int)(pixel & 0x000000FF));
            B = 255 - ((int)(pixel & 0x0000FF00) >> 8);
            R = 255 - ((int)(pixel & 0x00FF0000) >> 16);
            Color color1 = Color.FromArgb(G,B,R);
            if (R - 5 < 0)
                R = R + 6;
            Color color2 = Color.FromArgb(G,B,R-5);
            this.BackColor = color2;
            this.TransparencyKey = color2;
            label1.ForeColor = color1;
            label2.ForeColor = color1;
            label3.ForeColor = color1;
        }

        public Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero); uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel & 0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            this.Opacity = 0;
            timer2.Enabled = true;
        }

        public void timer2_Tick(object sender, EventArgs e)
        {
            if (num <= 2)
                num++;
            else
            {
                num = 0;
                timer2.Enabled = false;
                this.Opacity = 1;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = 0;
        }
    }
}
