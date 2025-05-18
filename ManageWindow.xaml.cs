using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Ereigniskalender.Models;
using System.IO;
using Microsoft.Win32;
using System.Globalization;

namespace Ereigniskalender
{
    public partial class ManageWindow : Window
    {
        private ObservableCollection<BirthdayEntry> _entries;

        public ManageWindow()
        {
            InitializeComponent();

            // Daten laden und binden
            _entries = new ObservableCollection<BirthdayEntry>(CsvService.LoadAll());
            AllGrid.ItemsSource = _entries;

            // Standard-Sortierung: Tage übrig aufsteigend
            var view = CollectionViewSource.GetDefaultView(AllGrid.ItemsSource);
            view.SortDescriptions.Add(new SortDescription(nameof(BirthdayEntry.DaysUntil), ListSortDirection.Ascending));
            var daysCol = AllGrid.Columns.FirstOrDefault(c => c.SortMemberPath == nameof(BirthdayEntry.DaysUntil));
            if (daysCol != null)
                daysCol.SortDirection = ListSortDirection.Ascending;
        }

        private void OnNew_Click(object sender, RoutedEventArgs e)
        {
            _entries.Add(new BirthdayEntry
            {
                Name = "Neuer Eintrag",
                Birthday = DateTime.Today,
                Comment = string.Empty
            });
        }
        private void OnExportData_Click(object sender, RoutedEventArgs e)
        {
            // Erst alle offenen Edits committen, damit wir aktuellen Stand exportieren
            AllGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            AllGrid.CommitEdit(DataGridEditingUnit.Row, true);

            // SaveFileDialog öffnen
            var dlg = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "birthdays_export.csv",
                DefaultExt = ".csv"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    using var writer = new StreamWriter(dlg.FileName);
                    writer.WriteLine("Name,Birthday,Comment");
                    foreach (var entry in _entries)
                    {
                        // ISO-Format im Export
                        writer.WriteLine($"{entry.Name},{entry.Birthday:yyyy-MM-dd},{entry.Comment}");
                    }

                    MessageBox.Show(
                        $"Data successfully exported to:\n{dlg.FileName}",
                        "Export successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Export failed:\n{ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void OnSave_Click(object sender, RoutedEventArgs e)
        {
            // Alle offenen Edits übernehmen
            AllGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            AllGrid.CommitEdit(DataGridEditingUnit.Row, true);

            CsvService.SaveAll(_entries);
            DialogResult = true;
            Close();
        }

        private void OnMoreFunctions_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.ContextMenu != null)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.Placement = PlacementMode.Bottom;
                btn.ContextMenu.IsOpen = true;
            }
        }

        private void OnImportData_Click(object sender, RoutedEventArgs e)
        {
            // Offene Edits übernehmen
            AllGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            AllGrid.CommitEdit(DataGridEditingUnit.Row, true);

            var dlg = new OpenFileDialog
            {
                Title = "Importiere Ereignisdaten",
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = ".csv"
            };

            if (dlg.ShowDialog() != true)
                return;

            try
            {
                var lines = File.ReadAllLines(dlg.FileName)
                                .Skip(1) // Header überspringen
                                .Where(l => !string.IsNullOrWhiteSpace(l))
                                .ToArray();

                int added = 0;
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length < 2)
                        continue;

                    var name = parts[0];
                    if (!DateTime.TryParseExact(parts[1], "yyyy-MM-dd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateTime bd))
                        continue;

                    var comment = parts.Length > 2 ? parts[2] : string.Empty;

                    // Einfacher Duplikat-Check: Name + Datum
                    bool exists = _entries.Any(e2 =>
                        e2.Name == name &&
                        e2.Birthday == bd
                    );

                    if (!exists)
                    {
                        _entries.Add(new BirthdayEntry
                        {
                            Name = name,
                            Birthday = bd,
                            Comment = comment
                        });
                        added++;
                    }
                }

                AllGrid.Items.Refresh();
                MessageBox.Show(
                    $"Import abgeschlossen!\n{added} neue Einträge hinzugefügt.",
                    "Import erfolgreich",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Import fehlgeschlagen:\n{ex.Message}",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void OnOpenAppDirectory_Click(object sender, RoutedEventArgs e)
        {
            // öffnet im Explorer das Verzeichnis, in dem die EXE liegt
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            Process.Start(new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
