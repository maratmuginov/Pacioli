using System.Linq;

namespace Pacioli.Lib.Models
{
    public class Journal
    {
        public bool PostEntry(JournalEntry journalEntry)
        {
            return journalEntry.Credits.Sum(credit => credit.Amount) 
                   == journalEntry.Debits.Sum(debit => debit.Amount);
        }
    }
}
