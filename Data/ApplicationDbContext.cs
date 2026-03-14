using CatShelter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CatShelter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cat>()
                .HasOne(c => c.Breed)
                .WithMany(b => b.Cats)
                .HasForeignKey(c => c.BreedId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CatCare>()
                .HasOne(cc => cc.Cat)
                .WithMany(c => c.CatCares)
                .HasForeignKey(cc => cc.CatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CatCare>()
                .HasOne(cc => cc.Care)
                .WithMany(c => c.CatCares)
                .HasForeignKey(cc => cc.CareId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CatCare>()
                .HasOne(cc => cc.User)
                .WithMany(u => u.CatCares)
                .HasForeignKey(cc => cc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.User)
                .WithMany(u => u.Adoptions)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Cat)
                .WithMany(c => c.Adoptions)
                .HasForeignKey(async => async.CatId)
                .OnDelete(DeleteBehavior.Cascade);

        }
        public DbSet<CatShelter.Models.Cat> Cat { get; set; } = default!;
        public DbSet<CatShelter.Models.Breed> Breed { get; set; } = default!;
        public DbSet<CatShelter.Models.Care> Care { get; set; } = default!;
        public DbSet<CatShelter.Models.CatCare> CatCare { get; set; } = default!;
        public DbSet<CatShelter.Models.Adoption> Adoption { get; set; } = default!;
    }
}
