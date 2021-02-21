using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pacioli.Lib.Models
{
    public record JournalEntry
    {
        public JournalEntry(DateTime date, string description, 
            [NotNull] ICollection<JournalEntryLine> debits, 
            [NotNull] ICollection<JournalEntryLine> credits) : 
            this(date, debits, credits)
        {
            this.Description = description;
        }

        public JournalEntry(DateTime date, 
            [NotNull] ICollection<JournalEntryLine> debits, 
            [NotNull] ICollection<JournalEntryLine> credits)
        {
            EnsureParamNotNull(debits, nameof(debits), "Debits should not be null.");
            EnsureParamNotNull(credits, nameof(credits), "Credits should not be null.");
            
            if (date == DateTime.MinValue)
                throw new ArgumentException("Please specify a valid date", nameof(date));

            EnsureParamNotEmpty(debits, nameof(debits), "Debits should not be empty");
            EnsureParamNotEmpty(credits, nameof(credits), "Credits should not be empty");

            bool hasCommonAccount = debits.Any(dr => credits.Any(cr => dr.Account.Equals(cr.Account)));
            if (hasCommonAccount)
                throw new ArgumentException("Accounts should be exclusive to debit or credit side of the journal entry.");

            EnsureNormalBalances(debits, credits);

            Date = date;
            Debits = new List<JournalEntryLine>(debits);
            Credits = new List<JournalEntryLine>(credits);
        }

        public DateTime Date { get; }
        public string Description { get; }
        public List<JournalEntryLine> Debits { get; }
        public List<JournalEntryLine> Credits { get; }

        private static void EnsureNormalBalances(ICollection<JournalEntryLine> debits, 
            ICollection<JournalEntryLine> credits)
        {
            //TODO : Write helpful exception messages.
            if (debits.Any(dr => dr.Account.NormalBalance is not NormalBalance.Debit || dr.Amount < 0))
                throw new ArgumentException();

            if (credits.Any(cr => cr.Account.NormalBalance is not NormalBalance.Credit || cr.Amount > 0))
                throw new ArgumentException();
        }

        private static void EnsureParamNotNull(object param, string paramName, string exceptionMessage)
        {
            if (param is null)
                throw new ArgumentNullException(paramName, exceptionMessage);
        }

        private static void EnsureParamNotEmpty<T>(IEnumerable<T> collection, string paramName, string exceptionMessage)
        {
            if (!collection.Any())
                throw new ArgumentException(exceptionMessage, paramName);
        }
    }
}
