using System;
using System.Drawing;
using System.Windows.Forms;

namespace Task_04
{
    public partial class Form1 : Form
    {
        private bool Forward = true;
        static Timer myTimer = new Timer();
        private int k = 0;

        public Form1()
        {
            myTimer.Enabled = true;
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
            Graphics g = panel1.CreateGraphics();
            Brush brush = new SolidBrush(Color.Red);

            g.Clear(Color.White);
            g.FillEllipse(brush, k, 0, 211, 211);

            if (k + 211 == panel1.Width)
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
            brush.Dispose();
        }
    }
}