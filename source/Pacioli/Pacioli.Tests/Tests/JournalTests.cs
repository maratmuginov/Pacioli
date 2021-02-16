using Pacioli.Lib.Models;
using Xunit;

namespace Pacioli.Tests.Tests
{
    public class JournalTests
    {
        [Theory, InlineData(10, 0), InlineData(210, 853)]
        public void JournalDoesNotAcceptUnbalancedEntries(decimal debit, decimal credit)
        {
            var journal = new Journal();
            var journalEntry = new JournalEntry();

            journalEntry.Debits.Add(new JournalEntryItem { Amount = debit });
            journalEntry.Credits.Add(new JournalEntryItem { Amount = credit });
            bool postSuccessful = journal.PostEntry(journalEntry);
            
            Assert.False(postSuccessful);
        }
    }
}
