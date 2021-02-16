using System.Diagnostics.Contracts;
using System.Linq;
using Pacioli.Lib.Models;
using Xunit;

namespace Pacioli.Tests.Helpers
{
    public class JournalEntryTheoryData : TheoryData<JournalItem[], JournalItem[]>
    {
        public JournalEntryTheoryData(JournalItem[] debitItems, JournalItem[] creditItems)
        {
            //Not sure if these should be handled in the test or the attribute. 
            Contract.Assert(debitItems is not null && debitItems.Any());
            Contract.Assert(creditItems is not null && creditItems.Any());

            Add(debitItems, creditItems);
        }
    }
}
