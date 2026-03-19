using CatShelter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CatShelter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // конструктор на контекста с опции за конфигурация
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // конфигуриране на моделите и релациите между тях
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // настройка на връзката котка -> порода
            modelBuilder.Entity<Cat>()
                .HasOne(c => c.Breed)
                .WithMany(b => b.Cats)
                .HasForeignKey(c => c.BreedId)
                .OnDelete(DeleteBehavior.Cascade);

            // настройка на връзката грижа за котка -> котка
            modelBuilder.Entity<CatCare>()
                .HasOne(cc => cc.Cat)
                .WithMany(c => c.CatCares)
                .HasForeignKey(cc => cc.CatId)
                .OnDelete(DeleteBehavior.Cascade);

            // настройка на връзката грижа за котка -> грижа
            modelBuilder.Entity<CatCare>()
                .HasOne(cc => cc.Care)
                .WithMany(c => c.CatCares)
                .HasForeignKey(cc => cc.CareId)
                .OnDelete(DeleteBehavior.Cascade);

            // настройка на връзката грижа за котка -> потребител
            // използва restrict за изтриване
            modelBuilder.Entity<CatCare>()
                .HasOne(cc => cc.User)
                .WithMany(u => u.CatCares)
                .HasForeignKey(cc => cc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // настройка на връзката осиновяване -> потребител
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.User)
                .WithMany(u => u.Adoptions)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // настройка на връзката осиновяване -> котка
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Cat)
                .WithMany(c => c.Adoptions)
                .HasForeignKey(async => async.CatId)
                .OnDelete(DeleteBehavior.Cascade);

            // добавяне на начални данни за грижи
            modelBuilder.Entity<Care>().HasData(
                new Care { Id = 1, CareName = "Food", Description = "Cats will get food and water" },
                new Care { Id = 2, CareName = "Medical Exam", Description = "Each cat will have an opportunity to be medically examined" },
                new Care { Id = 3, CareName = "Playtime", Description = "You can come to our shelter to cheer the cats with play time" },
                new Care { Id = 4, CareName = "Special Treatment", Description = "Disabled or sick cats could get treatment based on their health problem" }
            );
        }
        // DbSet за котки
        public DbSet<CatShelter.Models.Cat> Cat { get; set; } = default!;
        // DbSet за породи
        public DbSet<CatShelter.Models.Breed> Breed { get; set; } = default!;
        // DbSet за грижи
        public DbSet<CatShelter.Models.Care> Care { get; set; } = default!;
        // DbSet за грижи за котки
        public DbSet<CatShelter.Models.CatCare> CatCare { get; set; } = default!;
        // DbSet за осиновявания
        public DbSet<CatShelter.Models.Adoption> Adoption { get; set; } = default!;
    }
}
