using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clock
{
    public partial class Schedule : Form
    {
        public Schedule()
        {
            InitializeComponent();
        }

        #region 窗体靠近边缘隐藏事件

        /// <summary>
        /// 窗体靠近边缘隐藏事件
        /// </summary>

        internal AnchorStyles StopAnhor = AnchorStyles.None;

        void Timer1Tick(object sender, EventArgs e)
        {
            if (this.Bounds.Contains(Cursor.Position))
            {
                switch (this.StopAnhor)
                {
                    case AnchorStyles.Top:
                        this.Location = new Point(this.Location.X, 0);
                        break;
                    case AnchorStyles.Left:
                        this.Location = new Point(0, this.Location.Y);
                        break;
                    case AnchorStyles.Right:
                        this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, this.Location.Y);
                        break;
                }
            }
            else
            {
                switch (this.StopAnhor)
                {
                    case AnchorStyles.Top:
                        this.Location = new Point(this.Location.X, (this.Height - 4) * (-1));
                        break;
                    case AnchorStyles.Left:
                        this.Location = new Point((this.Width - 4) * (-1), this.Location.Y);
                        break;
                    case AnchorStyles.Right:
                        this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 4, this.Location.Y);
                        break;
                }
            }
        }

        private void mStopAnhor()
        {
            if (this.Top <= 0)
            {
                StopAnhor = AnchorStyles.Top;
            }
            else if (this.Left <= 0)
            {
                StopAnhor = AnchorStyles.Left;
            }
            else if (this.Right >= Screen.PrimaryScreen.Bounds.Width)
            {
                StopAnhor = AnchorStyles.Right;
            }
            else
            {
                StopAnhor = AnchorStyles.None;
            }
        }

        void MainFormLocationChanged(object sender, EventArgs e)
        {
            this.mStopAnhor();
        }

        #endregion

        My_Class MC = new My_Class();

        private void Schedule_Load(object sender, EventArgs e)
        {
            Width = 255;
            Height = Screen.PrimaryScreen.WorkingArea.Height - 130;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width-Width,120);
            pictureBox1.BackgroundImage = MC.Paint_Schedule1_BackGround(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = MC.Paint_Schedule1_TimeLine(MC.Paint_Schedule1_String(pictureBox1.BackgroundImage, MC.Read_All_String()), DateTime.Now);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            DateTime Time = MC.PointToTime(Height, e.Y);
            label1.Text = MC.TimeToString(Time);
            //MessageBox.Show("");
        }

    }

}
