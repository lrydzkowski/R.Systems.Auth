using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R.Systems.Auth.Core.Models;

namespace R.Systems.Auth.Db.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(name: "role", schema: "user");
            builder.HasKey(role => role.RoleId);
            ConfigureColumns(builder);
            InitializeData(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<Role> builder)
        {
            builder.Property(role => role.RoleId)
                .HasColumnName("role_id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(role => role.RoleKey)
                .HasColumnName("role_key")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(role => role.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(role => role.Description)
                .HasColumnName("description")
                .HasMaxLength(200);
        }

        private void InitializeData(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role[]
                {
                    new Role()
                    {
                        RoleId = 1,
                        RoleKey = "admin",
                        Name = "Administrator",
                        Description = "System administrator."
                    },
                    new Role()
                    {
                        RoleId = 2,
                        RoleKey = "user",
                        Name = "User",
                        Description = "Standard user."
                    }
                }
            );
            builder.Property(role => role.RoleId).HasIdentityOptions(startValue: 3);
        }
    }
}
