using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System;

namespace Pacioli.Lib.Models
{
    public record JournalEntry
    {
        public JournalEntry(DateTime date, string description, 
            [NotNull] List<JournalEntryLine> debits, [NotNull] List<JournalEntryLine> credits) : 
            this(date, debits, credits)
        {
            this.Description = description;
        }

        public JournalEntry(DateTime date, [NotNull] List<JournalEntryLine> debits, [NotNull] List<JournalEntryLine> credits)
        {
            var referenceParams = new object[] { debits, credits };
            if (referenceParams.Any(param => param is null))
                throw new ArgumentNullException("Debits and credits should not be null");

            if (date == DateTime.MinValue)
                throw new ArgumentException("Please specify a valid date");

            if (!debits.Any() || !credits.Any())
                throw new ArgumentException("Debits and credits should contain at least one JournalEntryItem.");

            Date = date;
            Debits = new List<JournalEntryLine>(debits);
            Credits = new List<JournalEntryLine>(credits);
        }

        public DateTime Date { get; set; }
        public string Description { get; set; }
        public List<JournalEntryLine> Debits { get; set; }
        public List<JournalEntryLine> Credits { get; set; }
    }
}
