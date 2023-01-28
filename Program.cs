using ArmaHTMLtoBAT.tools;

namespace ArmaHTMLtoBAT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string FileName = "arma.html";
            string exepath = Environment.CurrentDirectory;
            string Filepath = exepath + "\\" + FileName;
            List<string> modname = new List<string>();

            modname = Arma_methods.GetModNameList(Filepath);
            
            Arma_methods.CreateBat(modname);
        }
    }
}