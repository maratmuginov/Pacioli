using Pacioli.Lib.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pacioli.Tests.Unit
{
    public partial class JournalTests
    {
        public static TheoryData<DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> NormalBalance_TestData =>
        new()
        {
            {
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 100m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -100m) }
            },
            {
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 5.23m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Debit), -5.23m) }
            },
            {
                new DateTime(2020, 2, 15),
                new List<JournalEntryDebitLine> { new(new Account("Account1", NormalBalance.Debit), 10_101m) },
                new List<JournalEntryCreditLine> { new(new Account("Account2", NormalBalance.Credit), -10_101m) }
            }
        };

        public static TheoryData<DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> SameAccountsOnCreditAndDebit_TestData =>
        new()
        {
            {
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> 
                { 
                    new(new Account("Account1", NormalBalance.Debit), 100m),
                    new(new Account("Account2", NormalBalance.Debit), 100m)
                },
                new List<JournalEntryCreditLine> 
                { 
                    new(new Account("Account1", NormalBalance.Credit), -200m) 
                }
            },
            {
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> 
                { 
                    new(new Account("Test", NormalBalance.Debit), 6.23m) 
                },
                new List<JournalEntryCreditLine> 
                { 
                    new(new Account("Test", NormalBalance.Debit), -5.23m),
                    new(new Account("Test", NormalBalance.Debit), -1.00m)
                }
            },
        };

        public static TheoryData<DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> Unbalanced_TestData =>
        new()
        {
            {
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account", NormalBalance.Debit), 10m) },
                new List<JournalEntryCreditLine> { new(new Account("Another Account", NormalBalance.Credit), -20m) }
            }
        };

        public static TheoryData<DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> NullAndEmpty_TestData =>
        new()
        {
            {
                new DateTime(),
                new List<JournalEntryDebitLine>(),
                new List<JournalEntryCreditLine>()
            },
            {
                new DateTime(),
                new List<JournalEntryDebitLine> { new(new Account("Account", NormalBalance.Debit), 1m) },
                new List<JournalEntryCreditLine>()
            },
            {
                new DateTime(),
                new List<JournalEntryDebitLine>(),
                new List<JournalEntryCreditLine> { new(new Account("Another Account", NormalBalance.Credit), -1m) }
            }
        };
    }
}
