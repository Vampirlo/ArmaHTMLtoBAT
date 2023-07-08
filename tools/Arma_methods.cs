using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.IO;

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
                // строка может быть пуста и далее происходит исключительная ситуация. избегаем этого
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
        public static void CreateBat(List<string> modname, string profiles)
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
            writer.Write($"\" -port=2302 \"-profiles={profiles}\" -config=CONFIG_server.cfg -world=empty");
            writer.Close();
        }

        // для создания батника с полным путём к модам (если используется SteamCMD)
        public static void CreateBatFullPath(List<string> modname, string SteamCMDLocation, string profiles)
        {
            if (File.Exists("server" + modname.Count() + "mods.bat"))
                File.Delete("server" + modname.Count() + "mods.bat");

            using (File.Create("server" + modname.Count() + "mods.bat")) ;
            StreamWriter writer = new StreamWriter("server" + modname.Count() + "mods.bat");

            writer.Write("start arma3server_x64.exe \"-mod=");
            // нужно все папки прописать по пути 
            string SpeamCMDLocationMods = SteamCMDLocation + @"\steamapps\workshop\content\107410";
            // получаем полные пути всех модов
            string[] directories = Directory.GetDirectories(SpeamCMDLocationMods);
            
            //теперь необходимо 


            for (int i = 0; i < directories.Length; i++)
            {
                if ((i) == (directories.Length - 1))
                    writer.Write(directories[i]);
                if ((i) != (directories.Length - 1))
                    writer.Write(directories[i] + ";");
            }
            writer.Write($"\" -port=2302 \"-profiles={profiles}\" -config=CONFIG_server.cfg -world=empty");
            writer.Close();
        }

        public static List<string> GetModID(string Filepath)
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
                // строка может быть пуста и далее происходит исключительная ситуация. избегаем этого
                if ((line == null))
                    goto invalidIndex;
                splitstr = line.Split("?id=");
                if (splitstr.Length == 1)
                    goto invalidIndex;

                strForSubstring = splitstr[2];
                strForSubstring = strForSubstring.Substring(0, splitstr[2].Length - 4);
                modname.Add(strForSubstring);
            }
        _endOfStream:
            reader.Close();

            return modname;
        }


        public static void SteamCMDDownload(List<string> modID, string SteamCMDLocation, string GameId, string Steamlogin, string SteamPassword)
        {
            string steamCmdPath = SteamCMDLocation + "/steamcmd.exe";

            string modsIDToInstall = string.Empty;

            for (int i = 0; i < modID.Count; i++)
            {
                if (i == modID.Count - 1)
                {
                    modsIDToInstall += modID[i];
                }
                else
                {
                    modsIDToInstall += modID[i] + ",";
                }
            }
            string downloadCommand = $"workshop_download_item {GameId} {modsIDToInstall} validate";

            // Создаем процесс SteamCMD
            // нужен аккаунт с армой, для того, чтобы все моды успешно скачались
            Process steamCmdProcess = new Process();
            steamCmdProcess.StartInfo.FileName = steamCmdPath;
            steamCmdProcess.StartInfo.Arguments = $"+login {Steamlogin} {SteamPassword} +{downloadCommand} +quit";

            steamCmdProcess.Start();
            steamCmdProcess.WaitForExit();
        }


    }
}
