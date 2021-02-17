using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System;

namespace Pacioli.Lib.Models
{
    public record JournalEntry
    {
        public JournalEntry()
        {
            Debits = new List<JournalEntryItem>();
            Credits = new List<JournalEntryItem>();
        }

        public JournalEntry([NotNull]Account account, [NotNull] DateTime date, [NotNull] List<JournalEntryItem> debits, [NotNull] List<JournalEntryItem> credits)
        {
            if (account is null)
                throw new ArgumentNullException("Account should not be null");

            if (date == DateTime.MinValue)
                throw new ArgumentException("Please specify a valid date");

            if (debits is null || credits is null || account is null || date == DateTime.MinValue)
                throw new ArgumentNullException("Debits and credits should not be null");

            if (!debits.Any() || !credits.Any())
                throw new ArgumentException("Debits and credits length should not be empty.");
        }

        public Account Account { get; }

        private DateTime _date;
        public DateTime Date 
        {
            get => _date.Date;
        }

        public List<JournalEntryItem> Debits { get; set; }
        public List<JournalEntryItem> Credits { get; set; }
    }
}
