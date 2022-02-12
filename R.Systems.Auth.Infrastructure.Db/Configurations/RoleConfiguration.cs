using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R.Systems.Auth.Core.Models.Roles;

namespace R.Systems.Auth.Infrastructure.Db.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable(name: "role", schema: "user");
        builder.HasKey(role => role.Id);
        ConfigureColumns(builder);
        InitializeData(builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<RoleEntity> builder)
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

    private void InitializeData(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasData(new()
            {
                Id = 1,
                RoleKey = "admin",
                Name = "Administrator",
                Description = "System administrator."
            }, new()
            {
                Id = 2,
                RoleKey = "user",
                Name = "User",
                Description = "Standard user."
            });
        builder.Property(role => role.Id).HasIdentityOptions(startValue: 3);
    }
}
