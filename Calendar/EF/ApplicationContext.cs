using Calendar.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.EF
{
    public class ApplicationContext :DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events{ get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            // Database.EnsureDeleted();   // удаляем бд со старой схемой

            if (!Database.CanConnect())
            {
                Database.EnsureCreated();   // создаем бд с новой схемой
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.OwnerId);
        }
    }
}
