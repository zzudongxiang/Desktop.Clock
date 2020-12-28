using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Clock
{
    public partial class Main_Form : Form
    {
        public Main_Form()
        {
            InitializeComponent();
        }

        #region My_Class MC = new My_Class();

        /// <summary>
        /// 引用My_Class类，可调用类内函数
        /// </summary>

        My_Class MC = new My_Class();

        #endregion

        #region private void Form1_Load(object sender, EventArgs e)

        /// <summary>
        /// 窗口初始化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Form1_Load(object sender, EventArgs e)
        {
            //可穿透
            MC.Penetrate(this.Handle);
            //位置初始化
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = 0;
            //系统初始化
            timer_Clock_Tick(null, null);
            Timer_Lable_Tick(null, null);
            Timer_Flow_Tick(null, null);
            Timer_Lable.Interval = 5 * 60 * 1000;
        }

        #endregion

        #region private void timer_Clock_Tick(object sender, EventArgs e)

        /// <summary>
        /// 每秒钟执行一次，用于更新所显示的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void timer_Clock_Tick(object sender, EventArgs e)
        {
            this.TopMost = true;
            string[] WeekDays = { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            label1.Text = DateTime.Now.ToString("HH:mm");
            label2.Text = DateTime.Now.ToString("yyyy年MM月dd日");
            label2.Text += "  " + WeekDays[(int)DateTime.Now.DayOfWeek];
            Color[] SC = MC.Get_Inverse_Color(this.Location.X + this.Width / 2, this.Location.Y + 38);
            label1.ForeColor = SC[0];
            label2.ForeColor = SC[0];
            label3.ForeColor = SC[2];
            BackColor = SC[1];
            TransparencyKey = SC[1];
        }

        #endregion

        #region private void Timer_Lable_Tick(object sender, EventArgs e)

        /// <summary>
        /// 每5分钟检查一次提醒内容的更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Timer_Lable_Tick(object sender, EventArgs e)
        {
            MC.Flow_Time = MC.Read_Next_String(DateTime.Now)[0];
            MC.Flow_String = MC.Read_Next_String(DateTime.Now)[1];
        }

        #endregion

        #region private void Timer_Flow_Tick(object sender, EventArgs e)

        /// <summary>
        /// 每0.5秒更新一次，提示牌流水效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Timer_Flow_Tick(object sender, EventArgs e)
        {
            if (Timer_Flow.Interval == 500)
            {
                string TMP = MC.Flow_String;
                if (TMP != "")
                {
                    label3.Text = MC.Flow_Time + " " + TMP;
                }
                else
                {
                    label3.Text = MC.Flow_Time + " " + MC.Flow_String; ;
                    Timer_Flow.Interval = 2000;
                }
            }
            else
            {
                Timer_Flow.Interval = 500;
            }
        }

        #endregion

    }
}
