using Pacioli.Lib.Contracts.Models;
using Pacioli.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Pacioli.Tests.Unit
{
    public partial class JournalTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public JournalTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory, MemberData(nameof(NormalBalance_TestData))]
        public void JournalEntriesAreBalancedByNetZeroNormalBalance(string userId, DateTime date, 
            List<JournalEntryDebitLine> debits, List<JournalEntryCreditLine> credits)
        {
            var sut = new JournalEntry(userId, date, debits, credits);
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

        private bool AnyPropertyIsMutable(IEnumerable<PropertyInfo> properties)
        {
            return properties.Any(prop =>
            {
                var genericTypeArgs = prop.PropertyType.GenericTypeArguments;
                if (genericTypeArgs.Any())
                {
                    //Check if generic type T is also mutable. 
                    var genericTypeProperties = genericTypeArgs.SelectMany(type => type.GetProperties());
                    _testOutputHelper.WriteLine(prop.Name);
                    return prop.CanWrite && AnyPropertyIsMutable(genericTypeProperties);
                }
                _testOutputHelper.WriteLine(prop.Name);
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
        public void AccountsAreExclusiveToDebitOrCreditSide(string userId, DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(userId, date, debits, credits);
            
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
        public void JournalDoesNotAcceptUnbalancedEntries(string userId, DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(userId, date, debits, credits);

            Assert.ThrowsAny<Exception>(CreateJournalEntry);
        }

        [Theory, MemberData(nameof(NullAndEmpty_TestData))]
        public void JournalEntryThrowsExceptionOnInvalidArgument(string userId, DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(userId, date, debits, credits);
            
            Assert.ThrowsAny<Exception>(CreateJournalEntry);
        }


        [Theory, MemberData(nameof(NormalBalance_TestData))]
        public void JournalEntryDoesNotThrowExceptionWithValidArguments(string userId, DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry CreateJournalEntry() => new JournalEntry(userId, date, debits, credits);

            var exception = Record.Exception(CreateJournalEntry);

            Assert.Null(exception);
        }

        [Theory, MemberData(nameof(NormalBalance_TestData))]
        public void JournalEntryMembersValuesAreTheSameAsConstructorArguments(string userId, DateTime date, 
            List<JournalEntryDebitLine> debits, 
            List<JournalEntryCreditLine> credits)
        {
            JournalEntry sut = new(userId, date, debits, credits);
            
            var debitsNotInDebits = debits.Except(sut.Debits);
            var creditsNotInCredits = credits.Except(sut.Credits);

            Assert.True(sut.UserId == userId);
            Assert.True(sut.Date == date);
            Assert.True(debitsNotInDebits.Any() is false && creditsNotInCredits.Any() is false);
        }

        [Theory, MemberData(nameof(EntryCreatorAndReviewer_TestData))]
        public void EntryCreatorCannotBeSameAsReviewer(string userId, DateTime date,
            List<JournalEntryDebitLine> debits,
            List<JournalEntryCreditLine> credits, 
            Review review,
            bool expected)
        {
            JournalEntry newJournalEntry = new(userId, date, debits, credits);

            var sut = Record.Exception(() => newJournalEntry.AddReview(review));

            Assert.Equal(expected, sut is not null);
        }

        [Theory, MemberData(nameof(NewReviewNotPermittedOnClosedJournalEntries_TestData))]
        public void CannotAddReviewOnApprovedReviews(string userId, DateTime date,
            List<JournalEntryDebitLine> debits,
            List<JournalEntryCreditLine> credits,
            Review[] reviews)
        {
            JournalEntry newJournalEntry = new(userId, date, debits, credits);

            void sut()
            {
                foreach (var review in reviews)
                    newJournalEntry.AddReview(review);
            };

            Assert.Throws<InvalidOperationException>(sut);
        }
    }
}
