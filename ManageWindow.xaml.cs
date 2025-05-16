using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            // Alle Edits übernehmen
            AllGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            AllGrid.CommitEdit(DataGridEditingUnit.Row, true);

            CsvService.SaveAll(_entries);
            DialogResult = true;
            Close();
        }

        private void OnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Ereigniskalender: Version 0.5 (Mai 2025)\n" +
                "Verwalte Geburtstage, Hochzeitstage und andere wichtige Meilensteine.\n\n" +
                "Entwickler: Manuel Kasser\n" +
                "MK Computer Services e.U.\n" +
                "Sonnwendgasse 30/4, 9020 Klagenfurt\n" +
                "kasser@mk-cs.at\n" +
                "0463 203311",
                "Über Ereigniskalender",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

        }
    }
}
