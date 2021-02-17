namespace Pacioli.Lib.Models
{
    public record Account
    {
        public Account(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
