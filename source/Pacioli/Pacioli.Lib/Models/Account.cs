namespace Pacioli.Lib.Models
{
    public record Account
    {
        public Account(string name, NormalBalance normalBalance)
        {
            Name = name;
            NormalBalance = normalBalance;
        }

        public string Name { get; init;  }
        public NormalBalance NormalBalance { get; init;  }
    }
}
