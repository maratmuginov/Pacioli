using Pacioli.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pacioli.Tests.Tests
{
    public partial class JournalTests
    {
        public static TheoryData<DateTime, List<JournalEntryDebitLine>, List<JournalEntryCreditLine>> NormalBalance_TestData => 
        new()
        {
            {
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Account", NormalBalance.Debit), 100m) },
                new List<JournalEntryCreditLine> { new(new Account("Another Account", NormalBalance.Credit), -100m) }
            },
            {
                DateTime.UtcNow,
                new List<JournalEntryDebitLine> { new(new Account("Another Account", NormalBalance.Debit), 5.23m) },
                new List<JournalEntryCreditLine> { new(new Account("Another Account", NormalBalance.Debit), -5.23m) }
            },
            {
                new DateTime(2020, 2, 15),
                new List<JournalEntryDebitLine> { new(new Account("Account", NormalBalance.Debit), 10_101m) },
                new List<JournalEntryCreditLine> { new(new Account("Another Account", NormalBalance.Credit), -10_101m) }
            }
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
