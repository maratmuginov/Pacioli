using Pacioli.Lib.Contracts.Models;
using Pacioli.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Pacioli.Tests.Unit
{
    public partial class JournalTests
    {
        [Theory, MemberData(nameof(NormalBalance_TestData))]
        public void JournalEntriesAreBalancedByNetZeroNormalBalance(DateTime date, 
            List<JournalEntryDebitLine> debits, List<JournalEntryCreditLine> credits)
        {
            var sut = new JournalEntry(date, debits, credits);
            const decimal expectedVariance = 0m;

            decimal debitsSum = sut.Debits.Sum(dr => dr.Amount);
            decimal creditsSum = sut.Credits.Sum(cr => cr.Amount);
            var actualVariance = debitsSum + creditsSum;

            Assert.Equal(expectedVariance, actualVariance);
        }

        [Fact]
        public void JournalEntryPropertiesAreImmutable()
        {
            var sut = typeof(JournalEntry);
            var properties = sut.GetProperties();

            var anyPropertyIsMutable = AnyPropertyIsMutable(properties);

            Assert.False(anyPropertyIsMutable);
        }

        private static bool AnyPropertyIsMutable(IEnumerable<PropertyInfo> properties)
        {
            return properties.Any(prop =>
            {
                var genericTypeArgs = prop.PropertyType.GenericTypeArguments;
                if (genericTypeArgs.Any())
                {
                    //Check if generic type T is also mutable. 
                    var genericTypeProperties = genericTypeArgs.SelectMany(type => type.GetProperties());
                    return prop.CanWrite && AnyPropertyIsMutable(genericTypeProperties);
                }
                return prop.CanWrite;
            });
        }

        [Fact]
        public void AccountsAreComparedByValueSemantics()
        {
            const string accountName = "Account";
            var account = new Account(accountName, NormalBalance.Debit);
            var sameAccount = new Account(accountName, NormalBalance.Debit);

            Assert.Equal(account, sameAccount);
        }

        [Theory, MemberData(nameof(SameAccountsOnCreditAndDebit_TestData))]
        public void AccountsAreExclusiveToDebitOrCreditSide(DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(date, debits, credits);
            
            Assert.Throws<ArgumentException>(CreateJournalEntry);
        }
        
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
            var sut = typeof(IJournalEntryLine);
            var properties = sut.GetProperties();

            bool propertiesCanBeRead = properties.All(prop => prop.CanRead);

            Assert.True(propertiesCanBeRead);
        }

        [Fact]
        public void JournalEntryItemPropertiesAreImmutable()
        {
            var sut = typeof(IJournalEntryLine);
            var properties = sut.GetProperties();

            bool propertiesAreImmutable = properties.All(prop => prop.CanWrite is false);

            Assert.True(propertiesAreImmutable);
        }

        [Theory, MemberData(nameof(Unbalanced_TestData))]
        public void JournalDoesNotAcceptUnbalancedEntries(DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(date, debits, credits);

            Assert.ThrowsAny<Exception>(CreateJournalEntry);
        }

        [Theory, MemberData(nameof(NullAndEmpty_TestData))]
        public void JournalEntryThrowsExceptionOnInvalidArgument(DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(date, debits, credits);
            
            Assert.ThrowsAny<Exception>(CreateJournalEntry);
        }


        [Theory, MemberData(nameof(NormalBalance_TestData))]
        public void JournalEntryDoesNotThrowExceptionWithValidArguments(DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(date, debits, credits);

            var exception = Record.Exception(CreateJournalEntry);

            Assert.Null(exception);
        }

        [Theory, MemberData(nameof(NormalBalance_TestData))]
        public void JournalEntryMembersValuesAreTheSameAsConstructorArguments(DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry sut = new(date, debits, credits);
            
            var debitsNotInDebits = debits.Except(sut.Debits);
            var creditsNotInCredits = credits.Except(sut.Credits);
            
            Assert.True(sut.Date == date);
            Assert.True(debitsNotInDebits.Any() is false && creditsNotInCredits.Any() is false);
        }
    }
}
