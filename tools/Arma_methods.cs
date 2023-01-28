using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaHTMLtoBAT.tools
{
    internal class Arma_methods
    {
        public static List<string> GetModNameList(string Filepath)
        {
            string line = String.Empty;
            string[] splitstr;
            string strForSubstring = string.Empty;
            List<string> modname = new List<string>();

            StreamReader reader = new StreamReader(Filepath);

            while (!reader.EndOfStream)
            {
            invalidIndex:
                // из-за проверки на "null" приходится вводить дополнительную, чтобы выйти из цикла
                if (reader.EndOfStream)
                    goto _endOfStream;
                line = reader.ReadLine();
                // строка может быть пустаю и далее происходит исключительная ситуация. избегаем этого
                if ((line == null))
                    goto invalidIndex;
                splitstr = line.Split("\"");
                if (splitstr.Length == 1)
                    goto invalidIndex;

                if (splitstr[1] == "DisplayName")
                {
                    strForSubstring = splitstr[2];
                    strForSubstring = strForSubstring.Substring(1, splitstr[2].Length - 6);
                    modname.Add(strForSubstring.Insert(0, "@"));
                    //добавить в лист название мода и обрезать первый символ, и последние 5. Добавить в начале строки @
                    // (1) >A-10 Warthog</td> (5)
                }
            }
            _endOfStream:
            reader.Close();

            return modname;
        }
        public static void CreateBat(List<string> modname)
        {
            if (File.Exists("server" + modname.Count() + "mods.bat"))
                File.Delete("server" + modname.Count() + "mods.bat");

            using (File.Create("server" + modname.Count() + "mods.bat")) ;
            StreamWriter writer = new StreamWriter("server" + modname.Count() + "mods.bat");

            writer.Write("start arma3server_x64.exe \"-mod=");

            for (int i = 0; i < modname.Count; i++)
            {
                if ((i) == (modname.Count - 1))
                    writer.Write(modname[i]);
                if ((i) != (modname.Count - 1))
                    writer.Write(modname[i] + ";");
            }
            writer.Write("\" -port=2302 \"-profiles=c:\\server\\Games\\ArmA3\\A3Master\" -config=CONFIG_server.cfg -world=empty");
            writer.Close();
        }
    }
}
