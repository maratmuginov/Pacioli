﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
                throw new ArgumentException($"Debits and credits should contain at least one {nameof(JournalEntryLine)}.");

            bool hasCommonAccount = debits.Any(dr => credits.Any(cr => dr.Account == cr.Account));
            if (hasCommonAccount)
                throw new ArgumentException("Accounts should be exclusive to debit or credit side of the journal entry.");
            
            Date = date;
            Debits = new List<JournalEntryLine>(debits);
            Credits = new List<JournalEntryLine>(credits);
        }

        public DateTime Date { get; }
        public string Description { get; }
        public List<JournalEntryLine> Debits { get; }
        public List<JournalEntryLine> Credits { get; }
    }
}
