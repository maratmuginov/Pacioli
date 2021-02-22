using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pacioli.WebApi.Models;

namespace Pacioli.WebApi.Data
{
    public class AccountantDbContext : IdentityDbContext<Accountant>
    {
        public AccountantDbContext(DbContextOptions<AccountantDbContext> option) : base(option)
        {

        }
    }
}
