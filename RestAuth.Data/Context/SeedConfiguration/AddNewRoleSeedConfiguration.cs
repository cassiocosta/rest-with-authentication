using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestAuth.Domain;
using RestAuth.Domain.Entities;

namespace RestAuth.Data.Context.SeedConfiguration
{
    public class AddNewRoleSeedConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            var userRole = new Role
            {
                Id = Guid.Parse(Constants.RoleIds[1]),
                Name = Constants.RoleNames[1],
                Description = Constants.RoleDescriptions[1],
                CreationDate = new DateTime(2022, 05, 05),
                DeletionDate = null,
                LastChangeDate = null
            };

            builder.HasData(userRole);

            var adminRole = new Role
            {
                Id = Guid.Parse(Constants.RoleIds[2]),
                Name = Constants.RoleNames[2],
                Description = Constants.RoleDescriptions[2],
                CreationDate = new DateTime(2022, 05, 05),
                DeletionDate = null,
                LastChangeDate = null
            };

            builder.HasData(adminRole);
        }
    }
}