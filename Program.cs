using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace PopupTwitch
{
    static class Program
    {
        /// <summary>
        ///  Ponto de entrada principal do aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Define o idioma salvo no config.json
                string idioma = AppConfig.GetIdioma();
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(idioma);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(idioma);
            }
            catch
            {
                // Se der erro, força o padrão pt-BR
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            }

            // Inicializa a aplicação
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
