using System;
using System.Drawing;
using System.Windows.Forms;

namespace Task_04
{
    public partial class Form2 : Form
    {
        private bool Forward = true;
        private Timer myTimer = new Timer();
        private int k = 0;

        public Form2()
        {
            myTimer.Interval = 20;
            myTimer.Tick += new EventHandler(myTimer_Tick);
            myTimer.Start();
            InitializeComponent();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            myTimer.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            myTimer.Start();
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            Graphics g = panel2.CreateGraphics();
            Pen pen = new Pen(Color.LightGreen, 1);

            g.Clear(Color.White);
            g.DrawRectangle(pen, 0, 0, k, k);

            if (k >= panel2.Height)
            {
                Forward = false;
            }
            else if (k == 0)
            {
                Forward = true;
            }
            if (Forward)
            {
                k++;
            }
            else
            {
                k--;
            }

            g.Dispose();
            pen.Dispose();
        }
    }
}