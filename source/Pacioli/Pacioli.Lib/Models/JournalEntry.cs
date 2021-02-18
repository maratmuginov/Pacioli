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

        public JournalEntry([NotNull]Account account, DateTime date, [NotNull] List<JournalEntryItem> debits, [NotNull] List<JournalEntryItem> credits)
        {
            var referenceParams = new object[] { debits, credits, account };
            if (referenceParams.Any(param => param is null))
                throw new ArgumentNullException("Debits, credits and account should not be null");

            if (date == DateTime.MinValue)
                throw new ArgumentException("Please specify a valid date");

            if (!debits.Any() || !credits.Any())
                throw new ArgumentException("Debits and credits should contain at least one JournalEntryItem.");

            Account = new(account.Name);
            _date = date;
            Debits = new List<JournalEntryItem>(debits);
            Credits = new List<JournalEntryItem>(credits);
        }

        public Account Account { get; init; }

        private DateTime _date;
        public DateTime Date 
        {
            get => _date.Date;
        }

        public List<JournalEntryItem> Debits { get; set; }
        public List<JournalEntryItem> Credits { get; set; }
    }
}
