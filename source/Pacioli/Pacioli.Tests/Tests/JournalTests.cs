using Pacioli.Lib.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            journalEntry.Debits.Add(new JournalEntryItem(debit));
            journalEntry.Credits.Add(new JournalEntryItem(credit));
            bool postSuccessful = journal.PostEntry(journalEntry);

            Assert.False(postSuccessful);
        }

        [Theory, ClassData(typeof(JournalEntryThrowsExceptionOnInvalidArgument_TestData))]
        public void JournalEntryThrowsExceptionOnInvalidArgument(Account account, DateTime date, List<JournalEntryItem> debits, List<JournalEntryItem> credits)
        {
            Action sut = () => new JournalEntry(account: account, date: date, debits: debits, credits: credits);
            Assert.ThrowsAny<Exception>(sut);
        }

        private record JournalEntryThrowsExceptionOnInvalidArgument_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    null,
                    new DateTime(),
                    new List<JournalEntryItem>(),
                    new List<JournalEntryItem>()
                };
                yield return new object[]
                {
                    null,
                    null,
                    new List<JournalEntryItem>{ new JournalEntryItem(1m) },
                    new List<JournalEntryItem>()
                };
                yield return new object[]
                {
                    new Account("Test Account"),
                    null,
                    new List<JournalEntryItem>(),
                    new List<JournalEntryItem>{ new JournalEntryItem(1m) }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory, ClassData(typeof(JournalEntryDoesNotThrowExceptionWithValidArguments_TestData))]
        public void JournalEntryDoesNotThrowExceptionWithValidArguments(Account account, DateTime date, List<JournalEntryItem> debits, List<JournalEntryItem> credits)
        {
            var exception = Record.Exception(() => new JournalEntry(account: account, date: date, debits: debits, credits: credits));
            Assert.Null(exception);
        }

        private record JournalEntryDoesNotThrowExceptionWithValidArguments_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new Account("SomeAccount"),
                    DateTime.UtcNow,
                    new List<JournalEntryItem>{ new JournalEntryItem(1m) },
                    new List<JournalEntryItem>{ new JournalEntryItem(1m) }
                };
                yield return new object[]
                {
                    new Account("AnotherAccount"),
                    new DateTime(2020, 2, 15),
                    new List<JournalEntryItem>{ new JournalEntryItem(10_101m) },
                    new List<JournalEntryItem>{ new JournalEntryItem(111_111m) }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
