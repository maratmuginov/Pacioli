using Pacioli.Lib.Models;

namespace Pacioli.Lib.Contracts.Models
{
    public interface IJournalEntryLine
    {
        Account Account { get; }
        decimal Amount { get; }
    }
}