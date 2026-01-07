
using EasyGames.Class.DATA;
using EasyGames.Class.NewFolder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EasyGames.Class
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<StockItem> StockItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockItem>(entity =>
            {
                entity.Property(e => e.Price)
                      .HasPrecision(18, 2);

                entity.HasIndex(e => e.Name)
                      .IsUnique();

                // ✅ Business rule:
                // AvailableQuantity <= Quantity
                entity.HasCheckConstraint(
                    "CK_StockItems_AvailableQuantity_LessOrEqual_Quantity",
                    "[AvailableQuantity] <= [Quantity]"
                );
            });

        }

    }

}
