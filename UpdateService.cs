using System;
using System.IO;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Ereigniskalender
{
    public static class UpdateService
    {
        private const string VersionUrl = "https://example.com/version.txt"; // TODO replace with actual URL
        private const string DownloadUrl = "https://example.com/app.zip"; // TODO replace with actual download URL

        public static async Task CheckForUpdatesAsync(Window owner)
        {
            try
            {
                using var http = new HttpClient();
                var latest = (await http.GetStringAsync(VersionUrl)).Trim();
                if (latest != VersionInfo.CurrentVersion)
                {
                    var result = MessageBox.Show(owner,
                        $"Eine neue Version ({latest}) ist verf\xFCgbar. Jetzt herunterladen?",
                        "Update verf\xFCgbar", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        var tempFile = Path.Combine(Path.GetTempPath(), "Ereigniskalender_Update.exe");
                        using var stream = await http.GetStreamAsync(DownloadUrl);
                        using var fs = File.Create(tempFile);
                        await stream.CopyToAsync(fs);

                        Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception)
            {
                // Fehler ignorieren, Anwendung startet trotzdem
            }
        }
    }
}
