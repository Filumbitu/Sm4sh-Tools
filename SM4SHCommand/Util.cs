using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Sm4shCommand
{
    public static class Util
    {
        public static string CanonicalizePath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar).Trim();
        }
        public static void LogMessage(string message)
        {
            LogMessage(message, ConsoleColor.Green);
        }
        public static void LogMessage(string message, ConsoleColor consoleColor)
        {
            MainForm.Instance.Invoke(
                new MethodInvoker(
                    delegate {
                        var messageColor = consoleColor.DrawingColor();
                        int length = MainForm.Instance.richTextBox1.TextLength;  // at end of text
                        MainForm.Instance.richTextBox1.AppendText($">   {message}\n");
                        MainForm.Instance.richTextBox1.SelectionStart = length;
                        MainForm.Instance.richTextBox1.SelectionLength = 5 + message.Length;
                        MainForm.Instance.richTextBox1.SelectionColor = messageColor;
                        MainForm.Instance.richTextBox1.SelectionLength = 0;
                    }));
        }
    }
}
