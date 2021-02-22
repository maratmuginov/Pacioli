namespace Pacioli.Lib.Models
{
    public record JournalEntryLine
    {
        public JournalEntryLine(Account account, decimal amount)
        {
            Account = account;
            Amount = amount;
        }

        public Account Account { get; }
        public decimal Amount { get; }
    }
}
