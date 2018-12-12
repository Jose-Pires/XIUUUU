using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using XIUNetworkingLib;
using RSALib;
namespace SiUi
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //Substituir pelo nome da entidade existente

            //XIUNetworking Networking = new XIUNetworking("Pedro");
            //RSA myRsa = new RSA("Pedro");
            //myRsa.DeleteAllFromContainer();
            //

            //ChavesPrePartilhada Cha = new ChavesPrePartilhada(new List<string>());
        }
    }
}
