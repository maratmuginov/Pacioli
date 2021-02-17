using Pacioli.Lib.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [Theory, ClassData(typeof(JournalEntryHasToHaveCreditAndDebit_TestData))]
        public void JournalEntry_Throws_Exception_On_Empty_Credit_Or_Debit(List<JournalEntryItem> debits, List<JournalEntryItem> credits)
        {
            Assert.ThrowsAny<Exception>(() => new JournalEntry(debits: debits, credits: credits));
        }

        private record JournalEntryHasToHaveCreditAndDebit_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new List<JournalEntryItem>(),
                    new List<JournalEntryItem>()
                };
                yield return new object[]
                {
                    new List<JournalEntryItem>{ new JournalEntryItem { Amount = 1m } },
                    new List<JournalEntryItem>()
                };
                yield return new object[]
                {
                    new List<JournalEntryItem>(),
                    new List<JournalEntryItem>{ new JournalEntryItem { Amount = 1m } }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
