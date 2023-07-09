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
        public static void CreateBatFullPath(List<string> modID, string SteamCMDLocation, string profiles)
        {
            if (File.Exists("server" + modID.Count() + "mods.bat"))
                File.Delete("server" + modID.Count() + "mods.bat");

            using (File.Create("server" + modID.Count() + "mods.bat")) ;
            StreamWriter writer = new StreamWriter("server" + modID.Count() + "mods.bat");

            writer.Write("start arma3server_x64.exe \"-mod=");
            // нужно все папки прописать по пути 
            string SpeamCMDLocationMods = SteamCMDLocation + @"\steamapps\workshop\content\107410";
            // получаем полные пути всех модов
            string[] directories = Directory.GetDirectories(SpeamCMDLocationMods);

            //необходимо делать проверку. Лист с id модами сравнивать с id, которые тут, которых нет - удалять из массива
            // есть массив с всеми путями к модам. Есть массив с id загруженных модов. Нужно находить в элементах массива с путями id из второго массива
            // если совпадают, то добавлять в новый список

            // теперь мы имеем не массив string[], а лист List<string>. необходимо изменить добавления ниже.
            // ну также ебать сюда функцию вставь дебил. CurrentModsToLoad
            List<string> CurMods = new List<string>();
            CurMods = CurrentModsToLoad(directories, modID);

            for (int i = 0; i < CurMods.Count; i++)
            {
                if ((i) == (CurMods.Count - 1))
                    writer.Write(CurMods[i]);
                if ((i) != (CurMods.Count - 1))
                    writer.Write(CurMods[i] + ";");
            }
            writer.Write($"\" -port=2302 \"-profiles={profiles}\" -config=CONFIG_server.cfg -world=empty");
            writer.Close();
        }

        public static List<string> CurrentModsToLoad(string[] directories, List<string> modID)
        {
            List<string> CurrentMods = new List<string>();

            foreach (string id in modID)
            {
                // Для каждого числа мы используем метод Any, который проверяет, содержится ли хотя бы один элемент массива directories, 
                // удовлетворяющий условию. Внутри метода Any мы используем метод Contains, чтобы проверить, содержит ли элемент массива directories число id.
                bool found = directories.Any(directory => directory.Contains(id));

                if (found)
                {
                    //добавляем найденную папку (matchingDirectory) в CurrentMods, используя метод Add.
                    //Мы используем метод First для получения первого элемента из directories, который содержит подстроку id. Затем мы добавляем эту папку в CurrentMods.
                    string matchingDirectory = directories.First(directory => directory.Contains(id));
                    CurrentMods.Add(matchingDirectory);
                }
            }
            
            return CurrentMods;
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
            string downloadCommand;
            string arguments;

            for (int i = 0; i < modID.Count; i++)
            {
                downloadCommand = $"workshop_download_item {GameId} {modID[i]} validate";
                arguments = $"+login {Steamlogin} {SteamPassword} +{downloadCommand} +quit";
                ExecuteCommand(steamCmdPath, arguments);
            }
        }

        public static void ExecuteCommand(string steamCmdPath, string arguments)
        {
            Process steamCmdProcess = new Process();
            steamCmdProcess.StartInfo.FileName = steamCmdPath;
            steamCmdProcess.StartInfo.Arguments = arguments;
            steamCmdProcess.Start();
            steamCmdProcess.WaitForExit();
        }
    }
}
