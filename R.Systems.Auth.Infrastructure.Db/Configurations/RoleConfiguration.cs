using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R.Systems.Auth.Core.Models;

namespace R.Systems.Auth.Infrastructure.Db.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(name: "role", schema: "user");
        builder.HasKey(role => role.Id);
        ConfigureColumns(builder);
        InitializeData(builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<Role> builder)
    {
        builder.Property(role => role.Id)
            .HasColumnName("id")
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
                        Id = 1,
                        RoleKey = "admin",
                        Name = "Administrator",
                        Description = "System administrator."
                    },
                    new Role()
                    {
                        Id = 2,
                        RoleKey = "user",
                        Name = "User",
                        Description = "Standard user."
                    }
            }
        );
        builder.Property(role => role.Id).HasIdentityOptions(startValue: 3);
    }
}
