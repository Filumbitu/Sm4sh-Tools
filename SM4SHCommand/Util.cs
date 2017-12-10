using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;
using System.Runtime.InteropServices;

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
                    delegate
                    {
                        var messageColor = consoleColor.DrawingColor();
                        int length = MainForm.Instance.richTextBox1.TextLength;  // at end of text
                        MainForm.Instance.richTextBox1.AppendText($">   {message}\n");
                        MainForm.Instance.richTextBox1.SelectionStart = length;
                        MainForm.Instance.richTextBox1.SelectionLength = 5 + message.Length;
                        MainForm.Instance.richTextBox1.SelectionColor = messageColor;
                        MainForm.Instance.richTextBox1.SelectionLength = 0;
                    }));
        }

        /// <summary>
        /// Registers a file extension with the given executible
        /// </summary>
        /// <param name="Extension">Extension to register</param>
        /// <param name="KeyName">Registry key to register as</param>
        /// <param name="OpenWith">Executible to associate this extension with</param>
        /// <param name="FileDescription">Description of the file</param>
        public static void RegisterFileAssociation(string Extension, string KeyName, string OpenWith, string FileDescription)
        {
            RegistryKey BaseKey;
            RegistryKey OpenMethod;
            RegistryKey Shell;
            RegistryKey CurrentUser;


            BaseKey = Registry.ClassesRoot.CreateSubKey(Extension);
            BaseKey.SetValue("", KeyName);


            OpenMethod = Registry.ClassesRoot.CreateSubKey(KeyName);
            OpenMethod.SetValue("", FileDescription);
            OpenMethod.CreateSubKey("DefaultIcon").SetValue("", "\"" + OpenWith + "\",1");

            Shell = OpenMethod.CreateSubKey("Shell");
            Shell.CreateSubKey("edit").CreateSubKey("command").SetValue("", $"\"{OpenWith}\" \"-f\" \"%1\"");
            Shell.CreateSubKey("open").CreateSubKey("command").SetValue("", $"\"{OpenWith}\" \"-f\" \"%1\"");

            BaseKey.Close();
            OpenMethod.Close();
            Shell.Close();

            // Delete the key instead of trying to change it
            CurrentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + Extension, true);
            CurrentUser.DeleteSubKey("UserChoice", false);
            CurrentUser.Close();

            // Tell explorer the file association has been changed
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
