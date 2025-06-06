using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ereigniskalender.Models;

namespace Ereigniskalender
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUpcoming();
        }

        private void LoadUpcoming()
        {
            var all = CsvService.LoadAll();
            var today = DateTime.Today;
            var cutoff = today.AddMonths(3);

            var upcoming = all
                .Select(e =>
                {
                    // nächstes Geburtstagsdatum berechnen
                    var next = e.Birthday.WithYear(today.Year);
                    if (next < today)
                        next = next.AddYears(1);

                    return new
                    {
                        e.Name,
                        Birthday = next,
                        Age = next.Year - e.Birthday.Year,  // Alter
                        DaysUntil = (next - today).Days
                    };
                })
                .Where(x => x.DaysUntil <= (cutoff - today).Days)
                .OrderBy(x => x.DaysUntil)
                .ToList();

            UpcomingGrid.ItemsSource = upcoming;
        }

        private void UpcomingGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Item kann dein anonymes Objekt oder eine Klasse mit DaysUntil sein
            dynamic item = e.Row.Item;

            // Hervorhebung für Ereignisse von heute
            if (item.DaysUntil == 0)
            {
                e.Row.Background = Brushes.LightCoral;
            }
            // Ereignisse innerhalb der nächsten Woche
            else if (item.DaysUntil < 7)
            {
                e.Row.Background = Brushes.LightGoldenrodYellow;
            }
            else
            {
                // Standard-Hintergrund zurücksetzen
                e.Row.Background = Brushes.White;
            }
        }


        private void OnShowAll_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ManageWindow();
            if (dlg.ShowDialog() == true)
                LoadUpcoming();
        }
    }

    public static class DateExtensions
    {
        public static DateTime WithYear(this DateTime dt, int year) =>
            new DateTime(year, dt.Month, dt.Day);
    }
}
