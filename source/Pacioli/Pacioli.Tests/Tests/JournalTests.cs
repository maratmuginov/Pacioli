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
        [Fact]
        public void JournalEntryMayHaveDescription()
        {
            var sut = typeof(JournalEntry);
            var properties = sut.GetProperties();

            var descriptionProperty = properties
                .FirstOrDefault(prop => prop.PropertyType == typeof(string)
                                        && prop.Name == "Description");

            Assert.NotNull(descriptionProperty);
        }

        [Fact]
        public void JournalEntryItemPropertiesCanBeRead()
        {
            var sut = typeof(JournalEntryLine);
            var properties = sut.GetProperties();

            bool propertiesCanBeRead = properties.All(prop => prop.CanRead);

            Assert.True(propertiesCanBeRead);
        }

        [Fact]
        public void JournalEntryItemPropertiesAreImmutable()
        {
            var sut = typeof(JournalEntryLine);
            var properties = sut.GetProperties();

            bool propertiesAreImmutable = properties.All(prop => prop.CanWrite is false);

            Assert.True(propertiesAreImmutable);
        }

        [Theory, ClassData(typeof(JournalDoesNotAcceptUnbalancedEntries_TestData))]
        public void JournalDoesNotAcceptUnbalancedEntries(DateTime date, List<JournalEntryLine> debits, List<JournalEntryLine> credits)
        {
            var sut = new Journal();
            var journalEntry = new JournalEntry(date, debits, credits);
            
            bool postSuccessful = sut.PostEntry(journalEntry);

            Assert.False(postSuccessful);
        }

        private class JournalDoesNotAcceptUnbalancedEntries_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    DateTime.UtcNow, 
                    new List<JournalEntryLine> { new JournalEntryLine(new Account("Account"), 10m) },
                    new List<JournalEntryLine> { new JournalEntryLine(new Account("Another Account"), -20m) }
                };
                yield return new object[]
                {
                    DateTime.UtcNow,
                    new List<JournalEntryLine> { new JournalEntryLine(new Account("Account"), -8324m) },
                    new List<JournalEntryLine> { new JournalEntryLine(new Account("Another Account"), 2313m) }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory, ClassData(typeof(JournalEntryThrowsExceptionOnInvalidArgument_TestData))]
        public void JournalEntryThrowsExceptionOnInvalidArgument(DateTime date, List<JournalEntryLine> debits, List<JournalEntryLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(date, debits, credits);
            
            Assert.ThrowsAny<Exception>(CreateJournalEntry);
        }

        private class JournalEntryThrowsExceptionOnInvalidArgument_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new DateTime(),
                    new List<JournalEntryLine>(),
                    new List<JournalEntryLine>()
                };
                yield return new object[]
                {
                    null,
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Account"), 1m) },
                    new List<JournalEntryLine>()
                };
                yield return new object[]
                {
                    null,
                    new List<JournalEntryLine>(),
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Another Account"), 1m) }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory, ClassData(typeof(JournalEntryDoesNotThrowExceptionWithValidArguments_TestData))]
        public void JournalEntryDoesNotThrowExceptionWithValidArguments(DateTime date, List<JournalEntryLine> debits, List<JournalEntryLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(date, debits, credits);

            var exception = Record.Exception(CreateJournalEntry);

            Assert.Null(exception);
        }

        private class JournalEntryDoesNotThrowExceptionWithValidArguments_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    DateTime.UtcNow,
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Account"), 1m) },
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Another Account"), 1m) }
                };
                yield return new object[]
                {
                    new DateTime(2020, 2, 15),
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Account"), 10_101m) },
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Another Account"), 111_111m) }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory, ClassData(typeof(JournalEntryMembersValuesAreTheSameAsConstructorArguments_TestData))]
        public void JournalEntryMembersValuesAreTheSameAsConstructorArguments(DateTime date, List<JournalEntryLine> debits, List<JournalEntryLine> credits)
        {
            JournalEntry sut = new(date, debits, credits);
            
            var debitsNotInDebits = debits.Except(sut.Debits);
            var creditsNotInCredits = credits.Except(sut.Credits);
            
            Assert.True(sut.Date == date);
            Assert.True(debitsNotInDebits.Any() is false && creditsNotInCredits.Any() is false);
        }

        private class JournalEntryMembersValuesAreTheSameAsConstructorArguments_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    DateTime.UtcNow,
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Account"), 1m) },
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Another Account"), 1m) }
                };
                yield return new object[]
                {
                    new DateTime(2020, 2, 15),
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Account"), 10_101m) },
                    new List<JournalEntryLine>{ new JournalEntryLine(new Account("Another Account"), 111_111m) }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
