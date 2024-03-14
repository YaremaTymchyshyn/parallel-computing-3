using System;
using System.Drawing;
using System.Windows.Forms;

namespace Task_04
{
    public partial class Form3 : Form
    {
        static Timer myTimer = new Timer();
        private double k = 0;
        private double passed = 0;

        public Form3()
        {
            myTimer.Enabled = true;
            myTimer.Interval = 5;
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
            Graphics g = panel3.CreateGraphics();

            g.FillRectangle(Brushes.Brown, Convert.ToInt32(k), Convert.ToInt32(Math.Round(Math.Cos(passed / 50) * 100)) + 111, 1, 1);

            if (k >= panel3.Width)
            {
                g.Clear(SystemColors.Control);
                k = 0;
            }

            passed++;
            k++;
            g.Dispose();
        }
    }
}