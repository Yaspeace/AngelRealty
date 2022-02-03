using Microsoft.EntityFrameworkCore;
using Kursach.Models.RealtyDataBase.TableModels;

namespace Kursach.Models.RealtyDataBase.Context
{
    public class RealtyDbContext : DbContext
    {
        public DbSet<AdTypeModel> ad_types { get; set; }
        public DbSet<AnnouncementModel> announcements { get; set; }
        public DbSet<RealtyTypeModel> realty_types { get; set; }
        public DbSet<UserModel> users { get; set; }
        public DbSet<AdImageModel> ad_images { get; set; }
        
        public RealtyDbContext(DbContextOptions opt) : base(opt)
        {
            Database.EnsureCreated();
        }
    }
}
