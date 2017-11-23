using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sm4shCommand
{
    public static class Util
    {
        public static string CanonicalizePath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
        public static void LogMessage(string message)
        {
            MainForm.Instance.Invoke(
                new MethodInvoker(
                    delegate { MainForm.Instance.richTextBox1.AppendText($">   {message}\n"); }));
        }
    }
}
