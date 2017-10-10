using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sm4shCommand
{
    public static class GLOBALS
    {
        static GLOBALS()
        {
            MyDocumentsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SM4SHCommand");
            DefaultProjectDirectory = Path.Combine(MyDocumentsDirectory, "Projects");
        }
        public static readonly string DefaultProjectDirectory;
        public static readonly string MyDocumentsDirectory;
            
    }
}
