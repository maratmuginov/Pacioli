using Pacioli.Lib.Contracts.Models;
using System;

namespace Pacioli.Lib.Models
{
    public record JournalEntryDebitLine : IJournalEntryLine
    {
        public JournalEntryDebitLine(Account account, decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("A debit cannot be negative.", nameof(amount));

            Account = account;
            Amount = amount;
        }

        public Account Account { get; }
        public decimal Amount { get; }
    }
}
