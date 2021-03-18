using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pacioli.Lib.Contracts.Models;

namespace Pacioli.Lib.Models
{
    public record JournalEntry
    {
        public JournalEntry(string userId, DateTime date, string description, 
            [NotNull] ICollection<JournalEntryDebitLine> debits, 
            [NotNull] ICollection<JournalEntryCreditLine> credits) : 
            this(userId, date, debits, credits)
        {
            this.Description = description;
        }

        public JournalEntry(string userId, DateTime date, 
            [NotNull] ICollection<JournalEntryDebitLine> debits, 
            [NotNull] ICollection<JournalEntryCreditLine> credits)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null", nameof(userId));

            EnsureArgsNotNull(debits, nameof(debits), "Debits should not be null.");
            EnsureArgsNotNull(credits, nameof(credits), "Credits should not be null.");
            
            if (date == DateTime.MinValue)
                throw new ArgumentException("Please specify a valid date", nameof(date));

            EnsureArgNotEmpty(debits, nameof(debits), "Debits should not be empty");
            EnsureArgNotEmpty(credits, nameof(credits), "Credits should not be empty");

            bool hasCommonAccount = debits.Any(dr => credits.Any(cr => dr.Account.Equals(cr.Account)));
            if (hasCommonAccount)
                throw new ArgumentException("Accounts should be exclusive to debit or credit side of the journal entry.");

            EnsureDebitsAndCreditsBalance(debits, credits);

            UserId = userId;
            Date = date;
            Debits = new List<JournalEntryDebitLine>(debits);
            Credits = new List<JournalEntryCreditLine>(credits);
        }

        public string UserId { get; }
        public DateTime Date { get; }
        public string Description { get; }
        public List<JournalEntryDebitLine> Debits { get; }
        public List<JournalEntryCreditLine> Credits { get; }
        private bool closed = false;
        public bool Closed { get => closed; }

        public ImmutableList<Review> Reviews { get; private set; } = ImmutableList.Create<Review>();

        public void AddReview(Review review)
        {
            if (Closed)
                throw new InvalidOperationException("Journal is already approved and closed");

            if (UserId == review.Reviewer)
                throw new ArgumentException("Accountant cannot review and approve own journal entry", nameof(review.Reviewer));

            if (review.Approved)
                closed = true;

            Reviews.Add(review);
        }

        private static void EnsureArgsNotNull(object param, string paramName, string exceptionMessage)
        {
            if (param is null)
                throw new ArgumentNullException(paramName, exceptionMessage);
        }

        private static void EnsureArgNotEmpty<T>(IEnumerable<T> collection, string paramName, string exceptionMessage)
        {
            if (!collection.Any())
                throw new ArgumentException(exceptionMessage, paramName);
        }

        private static void EnsureDebitsAndCreditsBalance(IEnumerable<JournalEntryDebitLine> debits,
            IEnumerable<JournalEntryCreditLine> credits)
        {
            var lines = credits.Cast<IJournalEntryLine>().Union(debits);
            bool linesAreBalanced = lines.Sum(line => line.Amount) is 0m;
            if (linesAreBalanced is false)
                throw new ArgumentException("Debits and Credits should be balanced.");
        }
    }
}
