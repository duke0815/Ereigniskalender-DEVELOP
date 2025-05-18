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
