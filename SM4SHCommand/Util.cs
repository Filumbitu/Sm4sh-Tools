using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sm4shCommand
{
    public static class Util
    {
        public static string CanonicalizePath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
