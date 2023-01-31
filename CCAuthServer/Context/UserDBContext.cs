using Microsoft.EntityFrameworkCore;

namespace CCAuthServer.Context
{
    public class UserDBContext : DbContext
    {
        public UserDBContext(DbContextOptions<UserDBContext> options)
            : base(options)
        {
        }

        public UserDBContext() { }

        public virtual DbSet<UserData> UserData { get; set; }
        public virtual DbSet<UserTenant> UserTenant { get; set; }
        public virtual DbSet<SystemSettings> SystemSettings { get; set; }
        public virtual DbSet<Tenant> Tenant { get; set; }

    }
}
