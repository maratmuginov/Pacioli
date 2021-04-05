using Pacioli.Lib.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pacioli.Tests.Unit
{
    public partial class JournalTests
    {
        public static TheoryData<string, DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> NormalBalance_TestData =>
        new()
        {
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 100m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -100m) }
            },
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 5.23m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Debit), -5.23m) }
            },
            {
                "FakeUser",
                new DateTime(2020, 2, 15),
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 10_101m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -10_101m) }
            }
        };

        public static TheoryData<string, DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> SameAccountsOnCreditAndDebit_TestData =>
        new()
        {
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> 
                { 
                    new(new Account("Account1", NormalBalance.Debit), 200m),
                },
                new List<JournalEntryCreditLine> 
                { 
                    new(new Account("Account1", NormalBalance.Debit), -200m) 
                }
            },
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> 
                { 
                    new(new Account("Test", NormalBalance.Credit), 6.23m) 
                },
                new List<JournalEntryCreditLine> 
                { 
                    new(new Account("Test", NormalBalance.Credit), -6.23m),
                }
            },
        };

        public static TheoryData<string, DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> Unbalanced_TestData =>
        new()
        {
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account", NormalBalance.Debit), 10m) },
                new List<JournalEntryCreditLine> { new(new Account("Another Account", NormalBalance.Credit), -20m) }
            }
        };

        public static TheoryData<string, DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> NullAndEmpty_TestData =>
        new()
        {
            {
                "",
                new DateTime(),
                new List<JournalEntryDebitLine>(),
                new List<JournalEntryCreditLine>()
            },
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account", NormalBalance.Debit), 1m) },
                new List<JournalEntryCreditLine>()
            },
            {
                "FakeUser",
                new DateTime(),
                new List<JournalEntryDebitLine>(),
                new List<JournalEntryCreditLine> { new(new Account("Another Account", NormalBalance.Credit), -1m) }
            }
        };

        public static TheoryData<string, DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>, Review, bool> EntryCreatorAndReviewer_TestData =>
        new()
        {
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 100m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -100m) },
                new Review("FakeUser", true),
                true
            },
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 100m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -100m) },
                new Review("FakeReviewer", true),
                false
            }
        };

        public static TheoryData<string, DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>, Review[]> NewReviewNotPermittedOnClosedJournalEntries_TestData =>
        new()
        {
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 100m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -100m) },
                new Review[]
                {
                    new Review("FakeReviewer", false, "not true"),
                    new Review("FakeReviewer", false, "not faked data"),
                    new Review("FakeReviewer", true),
                    new Review("FakeReviewer", true, "follow up review"),
                }
            },
            {
                "FakeUser",
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 100m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -100m) },
                new Review[]
                {
                    new Review("FakeReviewer", false, "not true"),
                    new Review("FakeReviewer2", false, "faked data"),
                    new Review("FakeReviewer", false, "we will get in trouble with IRS"),
                    new Review("FakeReviewer", true),
                    new Review("FakeReviewer2", false, "follow up review"),
                }
            },
        };
    }
}
