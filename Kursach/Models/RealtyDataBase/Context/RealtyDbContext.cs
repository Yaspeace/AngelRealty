using Microsoft.EntityFrameworkCore;

namespace Kursach.Models.RealtyDataBase.Context
{
    public class RealtyDbContext : DbContext
    {
        /*DbSet<...*/

        public RealtyDbContext(DbContextOptions opt) : base(opt)
        {
            Database.EnsureCreated();
        }
    }
}
