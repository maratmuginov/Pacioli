using System;
using System.Collections.Generic;
using System.Linq;

namespace Pacioli.Lib.Models
{
    public class Journal
    {
        private readonly List<JournalItem> _debits = new List<JournalItem>();
        private readonly List<JournalItem> _credits = new List<JournalItem>();

        public void AddDebits(JournalItem[] debits)
        {
            _debits.AddRange(debits);
        }

        public void AddCredits(JournalItem[] credits)
        {
            _credits.AddRange(credits);
        }

        public void Commit()
        {
            //bool hasDebitsAndCredits = _debits.Count > 0 && _credits.Count > 0;
            //if (hasDebitsAndCredits is false)
            //    throw new Exception("Entry is not compliant with double-entry accounting.");

            bool entryIsBalanced = _debits.Sum(item => item.Amount) == _credits.Sum(item => item.Amount);
            if (entryIsBalanced is false)
                throw new Exception("Entry is not balanced.");
        }
    }
}
