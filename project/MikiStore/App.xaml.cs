using Microsoft.Toolkit.Uwp.Notifications;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MikiStore
{
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                if (args.TryGetValue("action", out string action) && action == "launchApp")
                {
                    string appId = args["appId"];
                    LaunchInstalledApp(appId);
                }
            };
        }
        private void LaunchInstalledApp(string appId)
        {
            string appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                MikiStore.MainWindow.appDataFolder, "apps", appId);

            if (!Directory.Exists(appFolder)) return;

            var exeFiles = Directory.GetFiles(appFolder, "*.exe", SearchOption.AllDirectories);

            string targetExe = exeFiles.FirstOrDefault(f => !f.Contains("UnityCrashHandler") && !f.Contains("install"));

            if (!string.IsNullOrEmpty(targetExe))
            {
                string processName = Path.GetFileNameWithoutExtension(targetExe);

                var existingProcesses = Process.GetProcessesByName(processName);
                if (existingProcesses.Length > 0)
                {
                    return;
                }

                Process.Start(new ProcessStartInfo(targetExe)
                {
                    WorkingDirectory = Path.GetDirectoryName(targetExe),
                    UseShellExecute = true
                });
            }
        }
    }
}
