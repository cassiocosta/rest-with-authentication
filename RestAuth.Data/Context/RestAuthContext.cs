using Microsoft.EntityFrameworkCore;
using RestAuth.Data.Context.SeedConfiguration;
using RestAuth.Domain.Entities;

namespace RestAuth.Data.Context
{
    public class RestAuthContext : DbContext
    {
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }

        public RestAuthContext()
        {
        }

        public RestAuthContext(DbContextOptions<RestAuthContext> options)
            : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is Entity entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreationDate = now;
                            entity.LastChangeDate = now;
                            break;

                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreationDate).IsModified = false;
                            entity.LastChangeDate = now;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<UserHasRoles>()
                .HasKey(x => new { x.UserId, x.RoleId });

            base.OnModelCreating(modelBuilder);

            SeedDatabase(modelBuilder);
        }

        private static void SeedDatabase(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AddNewRoleSeedConfiguration());
        }
    }
}