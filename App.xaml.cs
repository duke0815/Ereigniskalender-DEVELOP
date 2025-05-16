using System.Globalization;
using System.Windows;

namespace Ereigniskalender
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Deutsche Kultur für Zahlen, Datumsformate etc.
            var culture = new CultureInfo("de-DE");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            base.OnStartup(e);
        }
    }
}
