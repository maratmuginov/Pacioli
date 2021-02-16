using System.Collections.Generic;

namespace Pacioli.Lib.Models
{
    public class JournalEntry
    {
        public JournalEntry()
        {
            Debits = new List<JournalEntryItem>();
            Credits = new List<JournalEntryItem>();
        }

        public List<JournalEntryItem> Debits { get; set; }
        public List<JournalEntryItem> Credits { get; set; }
    }
}
