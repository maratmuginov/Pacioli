using Pacioli.Lib.Contracts.Models;
using System;

namespace Pacioli.Lib.Models
{
    public record JournalEntryCreditLine : IJournalEntryLine
    {
        public JournalEntryCreditLine(Account account, decimal amount)
        {
            if (amount > 0)
                throw new ArgumentException("A credit cannot be positive.", nameof(amount));

            Account = account;
            Amount = amount;
        }

        public Account Account { get; }
        public decimal Amount { get; }
    }
}
