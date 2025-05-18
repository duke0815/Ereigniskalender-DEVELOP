using System;
using System.ComponentModel;

namespace Ereigniskalender.Models
{
    public class BirthdayEntry : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _birthday;
        private string _comment;
        private bool _isModified;

        /// <summary>
        /// Zeigt, ob dieser Eintrag seit dem letzten Speichern geändert wurde.
        /// </summary>
        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    OnPropertyChanged(nameof(IsModified));
                }
            }
        }

        /// <summary>
        /// Name der Person oder des Events.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    MarkModified();
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Geburtsdatum bzw. Datum des Events.
        /// </summary>
        public DateTime Birthday
        {
            get => _birthday;
            set
            {
                if (_birthday != value)
                {
                    _birthday = value;
                    MarkModified();
                    OnPropertyChanged(nameof(Birthday));
                    // Benachrichtige UI, dass sich auch abhängige Werte geändert haben:
                    OnPropertyChanged(nameof(AgeCurrent));
                    OnPropertyChanged(nameof(DaysUntil));
                }
            }
        }

        /// <summary>
        /// Freitext-Kommentar zum Eintrag.
        /// </summary>
        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    MarkModified();
                    OnPropertyChanged(nameof(Comment));
                }
            }
        }

        /// <summary>
        /// Aktuelles Alter basierend auf dem heutigen Datum.
        /// </summary>
        public int AgeCurrent
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - Birthday.Year;
                var thisYearBirthday = new DateTime(today.Year, Birthday.Month, Birthday.Day);
                if (thisYearBirthday > today)
                    age--;
                return age;
            }
        }

        /// <summary>
        /// Anzahl der Tage bis zum nächsten Geburtstag.
        /// </summary>
        public int DaysUntil
        {
            get
            {
                var today = DateTime.Today;
                var next = new DateTime(today.Year, Birthday.Month, Birthday.Day);
                if (next < today)
                    next = next.AddYears(1);
                return (next - today).Days;
            }
        }

        /// <summary>
        /// Markiert den Eintrag als geändert.
        /// </summary>
        private void MarkModified()
        {
            IsModified = true;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
