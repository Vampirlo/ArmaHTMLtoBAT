using ArmaHTMLtoBAT.tools;

namespace ArmaHTMLtoBAT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ArmaGameId = "107410";

            // получение пути arma.html. В данный момент находится в директории исполняемого файла
            string FileName = "arma.html";
            string Filepath = tools.tools.GetFilePath(FileName);
            List<string> modname = new List<string>();
            List<string> modID = new List<string>();
            List<string> LocalModNames = new List<string>();

            // parameter initialization
            string iniFileName = "settings.ini";
            INIManager manager = new INIManager(tools.tools.GetFilePath(iniFileName));

            string modsPath = manager.GetPrivateString("SETTINGS", "modsPath");
            string SteamCMDLocation = manager.GetPrivateString("SETTINGS", "SteamCmdPath");
            string Steamlogin = manager.GetPrivateString("SETTINGS", "SteamLogin");
            string SteamPassword = manager.GetPrivateString("SETTINGS", "SteamPassword");
            string profiles = manager.GetPrivateString("SETTINGS", "profiles");

            modID = Arma_methods.GetModID(Filepath);

            LocalModNames = Arma_methods.GetLocalModNameList(Filepath);
        notCorrectSymbol:
            Console.WriteLine("Need to create bat ?\n Y or N");
            
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            char symbol = keyInfo.KeyChar;
            string str = symbol.ToString();

            if(str.ToUpper() == "Y")
            { 
                modname = Arma_methods.GetModNameList(Filepath);
                
            _notCorrectSymbol:
                Console.WriteLine("Create Bat Full Path ?\n Y or N");

                ConsoleKeyInfo _keyInfo = Console.ReadKey();
                char _symbol = _keyInfo.KeyChar;
                string _str = _symbol.ToString();

                if (_str.ToUpper() == "Y")
                    Arma_methods.CreateBatFullPath(modID, SteamCMDLocation, profiles, LocalModNames);
                else if (_str.ToUpper() == "N")
                    Arma_methods.CreateBat(modname, profiles);
                else
                    goto _notCorrectSymbol;

                Environment.Exit(0);
            }
            else if (str.ToUpper() == "N")
            {
                Console.WriteLine("Validate ?\n Y or N");

                ConsoleKeyInfo keyInfo_ = Console.ReadKey();
                char symbol_ = keyInfo.KeyChar;
                string str_ = symbol.ToString();

                if (str.ToUpper() == "Y")
                {
                    Arma_methods.ValidateSteamCMDDownloadMultipleMods(modID, SteamCMDLocation, ArmaGameId, Steamlogin, SteamPassword);
                }
                else if (str.ToUpper() == "N")
                {
                    Arma_methods.SteamCMDDownloadMultipleMods(modID, SteamCMDLocation, ArmaGameId, Steamlogin, SteamPassword);
                }
                Environment.Exit(0);
            }
            goto notCorrectSymbol;
        }
    }
}