using System.Collections.Generic;
using System.Reflection.Emit;
using UserManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace UserManagmentSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }     

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder
    .Entity<User>()
    .ToTable(tb => tb.HasTrigger("trg_AuditUsers"));
        }
        //public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, System.Threading.CancellationToken cancellationToken = default)
        //{
        //    var entries = ChangeTracker.Entries()
        //        .Where(e => e.Entity is User &&
        //                    (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

        //    foreach (var entry in entries)
        //    {
        //        var audit = new AuditLog
        //        {
        //            UserId = ((User)entry.Entity).Id,
        //            ActionType = entry.State.ToString(),
        //            UpdatedBy = "System", // You might want to replace this with the actual user performing the action
        //            UpdatedOn = DateTime.UtcNow
        //        };

        //        if (entry.State == EntityState.Added)
        //        {
        //            audit.NewValue = JsonConvert.SerializeObject(entry.Entity);
        //        }
        //        else if (entry.State == EntityState.Modified)
        //        {
        //            var original = entry.OriginalValues.Properties
        //                .ToDictionary(p => p, p => entry.OriginalValues[p]?.ToString());

        //            var current = entry.CurrentValues.Properties
        //                .ToDictionary(p => p, p => entry.CurrentValues[p]?.ToString());

        //            audit.OldValue = JsonConvert.SerializeObject(original);
        //            audit.NewValue = JsonConvert.SerializeObject(current);
        //        }
        //        else if (entry.State == EntityState.Deleted)
        //        {
        //            audit.OldValue = JsonConvert.SerializeObject(entry.OriginalValues.Properties
        //                .ToDictionary(p => p, p => entry.OriginalValues[p]?.ToString()));
        //        }

        //        AuditLogs.Add(audit);
        //    }

        //    return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

    }
}
