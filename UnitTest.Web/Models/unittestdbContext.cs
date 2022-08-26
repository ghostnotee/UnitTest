using Microsoft.EntityFrameworkCore;

namespace UnitTest.Web.Models
{
    public partial class unittestdbContext : DbContext
    {
        public unittestdbContext()
        {
        }

        public unittestdbContext(DbContextOptions<unittestdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb3_turkish_ci")
                .HasCharSet("utf8mb3");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.HasComment("						");

                entity.Property(e => e.Color).HasMaxLength(45);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .UseCollation("utf8mb3_general_ci");

                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
