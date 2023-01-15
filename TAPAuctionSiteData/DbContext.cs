using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TAP22_23.AuctionSite.Interface;

namespace Crea
{
    public class DbContext : TapDbContext
    {
        public DbSet<SiteTable> Sites { get; set; }
        public DbSet<AuctionTable> Auctions { get; set; }
        public DbSet<UserTable> Users { get; set; }
        public DbSet<SessionTable> Sessions { get; set; }

        public DbContext(string connectionString) : base(new DbContextOptionsBuilder<DbContext>().UseSqlServer(connectionString).Options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.LogTo(Console.WriteLine).EnableSensitiveDataLogging(); 
            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<UserTable>();
            user.HasMany(u => u.AuctionSeller).WithOne(a => a.CreatedBy!).OnDelete(DeleteBehavior.NoAction);
            user.HasMany(u => u.AuctionBidder).WithOne(a => a.CurrentWinner!).OnDelete(DeleteBehavior.SetNull);
            user.HasMany(u => u.Sessions).WithOne(s => s.Owner!).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            
            var session = modelBuilder.Entity<SessionTable>();
            session.HasOne(s => s.Site).WithMany(u => u!.Sessions).HasForeignKey(s => s.SiteId).OnDelete(DeleteBehavior.NoAction);
        }


        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (SqlException e)
            {
                throw new AuctionSiteUnavailableDbException("Unavailable Db", e);
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.InnerException as SqlException;
                if (sqlException == null)
                    throw new AuctionSiteInvalidOperationException("Missing information from DB", e);
                switch (sqlException.Number)
                {
                    case < 54: throw new AuctionSiteUnavailableDbException("Not available DB", e);
                    case < 122: throw new AuctionSiteInvalidOperationException("Sql: sintax error", e);
                    case 2601: throw new AuctionSiteNameAlreadyInUseException(null, $"{sqlException.Message}", e);
                    case 2627: throw new AuctionSiteNameAlreadyInUseException(null, "Primary key already in use", e);
                    case 547: throw new AuctionSiteInvalidOperationException("Foreign key not found", e);
                    default: throw new AuctionSiteInvalidOperationException("Query error", e);
                }
            }
        }
    }
}