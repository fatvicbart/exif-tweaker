using ExifTweaker.Infrastructure;
using Velopack;

namespace ExifTweaker
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Must be the first startup operation so install/update hooks can exit quickly.
            VelopackApp.Build().SetAutoApplyOnStartup(false).Run();

            ApplicationConfiguration.Initialize();
            ThemeService.SetMode(AppSettings.Load().Theme);
            Application.Run(new Form1());
        }
    }
}
