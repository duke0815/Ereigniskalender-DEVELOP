using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace Ereigniskalender.Models
{
    public static class CsvService
    {
        private const string FileName = "birthdays.csv";

        public static List<BirthdayEntry> LoadAll()
        {
            if (!File.Exists(FileName))
                return new List<BirthdayEntry>();

            return File.ReadAllLines(FileName)
                       .Skip(1) // Header
                       .Select(line =>
                       {
                           var parts = line.Split(',');
                           return new BirthdayEntry
                           {
                               Name = parts[0],
                               Birthday = DateTime.ParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                               Comment = parts.Length >= 3 ? parts[2] : string.Empty
                           };
                       })
                       .ToList();
        }

        public static void SaveAll(IEnumerable<BirthdayEntry> entries)
        {
            using var w = new StreamWriter(FileName);
            w.WriteLine("Name,Birthday,Comment");
            foreach (var e in entries)
                w.WriteLine($"{e.Name},{e.Birthday:yyyy-MM-dd},{e.Comment}");
        }
    }
}
