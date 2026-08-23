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
            Application.SetColorMode(SystemColorMode.System);
            Application.Run(new Form1());
        }
    }
}
