using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaHTMLtoBAT.tools
{
    internal class tools
    {
        public static string GetFilePath(string FileName)
        {
            string exepath = Environment.CurrentDirectory;
            string Filepath = exepath + "\\" + FileName;
            return Filepath;
        }
    }
}
