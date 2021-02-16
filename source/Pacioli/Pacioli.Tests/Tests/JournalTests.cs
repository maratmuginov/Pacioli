using System;
using Pacioli.Lib.Models;
using Pacioli.Tests.Helpers;
using Xunit;

namespace Pacioli.Tests.Tests
{
    public class JournalTests
    {
        private static readonly JournalItem[] DebitItems = { new() { Amount = 0m } };
        private static readonly JournalItem[] CreditItems = { new() { Amount = 1m } };
        public static readonly JournalEntryTheoryData UnbalancedJournalEntries = new (DebitItems, CreditItems);

        [Theory, MemberData(nameof(UnbalancedJournalEntries))]
        public void JournalDoesNotAcceptUnbalancedEntries(JournalItem[] debits, JournalItem[] credits)
        {
            var journal = new Journal();

            journal.AddDebits(debits);
            journal.AddCredits(credits);

            Assert.Throws<Exception>(journal.Commit);
        }

        //TODO : Write a test to enforce double-entry. 
    }
}
