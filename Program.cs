using DisasterAlert.Forms;

namespace DisasterAlert
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Habilita visual moderno no Windows
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FormDashboard());
        }
    }
}
