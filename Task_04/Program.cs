using System;
using System.Threading;
using System.Windows.Forms;

namespace Task_04
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Thread[] threads = new Thread[4];

            threads[0] = new Thread(() => 
            {
                Form1 form1 = new Form1();
                RunForm(form1);
            });
            threads[1] = new Thread(() => 
            {
                Form2 form2 = new Form2();
                RunForm(form2);
            });
            threads[2] = new Thread(() => 
            {
                Form3 form3 = new Form3();
                RunForm(form3);
            });
            threads[3] = new Thread(() => 
            {
                Form4 form4 = new Form4();
                RunForm(form4);
            });

            foreach (var x in threads)
                x.Start();
            foreach (var x in threads)
                x.Join();
        }
        private static void RunForm(Form form)
        {
            Application.Run(form);
        }
    }
}