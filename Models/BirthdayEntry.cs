// Models/BirthdayEntry.cs
using System;

namespace Ereigniskalender.Models
{
    public class BirthdayEntry
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Comment { get; set; }

        public int AgeCurrent
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - Birthday.Year;
                if (new DateTime(today.Year, Birthday.Month, Birthday.Day) > today)
                    age--;
                return age;
            }
        }

        /// <summary>
        /// Tage bis zum nächsten Geburtstag
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
    }
}
