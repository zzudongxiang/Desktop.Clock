using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Clock
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //使用多线程初始化两窗体
            //Thread Run_Form1 = new Thread(Run_Schedule);
            //Run_Form1.Start();

            Application.Run(new Main_Form());
        }

        static void Run_Schedule()
        {
            Application.Run(new Schedule());
        }
    }
}
