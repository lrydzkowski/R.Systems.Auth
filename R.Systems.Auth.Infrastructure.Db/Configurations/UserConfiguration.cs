using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Models.Users;
using System.Collections.Generic;

namespace R.Systems.Auth.Infrastructure.Db.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable(name: "user", schema: "user");
        builder.HasKey(user => user.Id);
        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasMany(user => user.Roles)
            .WithMany(role => role.Users)
            .UsingEntity<Dictionary<string, object>>(
                "user_role",
                x => x.HasOne<RoleEntity>().WithMany().HasForeignKey("role_id"),
                x => x.HasOne<UserEntity>().WithMany().HasForeignKey("user_id"),
                x => x.ToTable(name: "user_role", schema: "user")
                    .HasData(new Dictionary<string, object>()
                    {
                            { "user_id", 1L },
                            { "role_id", 1L }
                    })
            );
        ConfigureColumns(builder);
        InitializeData(builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<UserEntity> builder)
    {
        builder.Property(user => user.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(user => user.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(user => user.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(user => user.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(user => user.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(200);

        builder.Property(user => user.RefreshToken)
            .HasColumnName("refresh_token")
            .HasMaxLength(200);

        builder.Property(user => user.RefreshTokenExpireDateTimeUtc)
            .HasColumnName("refresh_token_expire_date_time_utc");

        builder.Property(user => user.NumOfIncorrectSignIn)
            .HasColumnName("num_of_incorrect_sign_in");

        builder.Property(user => user.LastIncorrectSignInDateTimeUtc)
            .HasColumnName("last_incorrect_sign_in_date_time_utc");
    }

    private void InitializeData(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasData(
            new UserEntity[]
            {
                    new UserEntity()
                    {
                        Id = 1,
                        Email = "admin@lukaszrydzkowski.pl",
                        FirstName = "Lukasz",
                        LastName = "Rydzkowski",
                        PasswordHash = null
                    }
            }
        );
        builder.Property(user => user.Id).HasIdentityOptions(startValue: 2);
    }
}
