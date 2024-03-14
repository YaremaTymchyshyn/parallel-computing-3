using System;
using System.Drawing;
using System.Windows.Forms;

namespace Task_04
{
    public partial class Form4 : Form
    {
        static Timer myTimer = new Timer();
        private float angle = 0;

        public Form4()
        {
            myTimer.Enabled = true;
            myTimer.Interval = 10;
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
            angle += (float)1;
            Graphics g = panel4.CreateGraphics();
            Pen pen = new Pen(Color.Black, 1);

            g.Clear(Color.White);
            g.TranslateTransform(panel4.Width / 2, panel4.Height / 2);
            g.RotateTransform(angle);
            g.DrawLine(pen, 0, 0, 77, 77);
            g.Dispose();
            pen.Dispose();
        }
    }
}