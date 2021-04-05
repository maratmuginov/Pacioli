using System;

namespace Pacioli.Lib.Models
{
    public record Review
    {
        public Review(string userId, bool approved, string comment) : this(userId, approved)
        {
            Comment = comment;
        }

        public Review(string userId, bool approved)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null", nameof(userId));
            
            Reviewer = userId;
            Approved = approved;
            ReviewDate = DateTime.UtcNow;
        }

        public string Reviewer { get; }
        public bool Approved { get; }
        public string Comment { get; }
        public DateTime ReviewDate { get; }
    }
}
