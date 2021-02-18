namespace Pacioli.Lib.Models
{
    public record JournalEntryItem
    {
        public JournalEntryItem(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; init; }
    }
}
