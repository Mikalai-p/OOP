using System;
using System.Windows.Forms;

namespace UniversityLab3   // должно совпадать с пространством имён Form1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}