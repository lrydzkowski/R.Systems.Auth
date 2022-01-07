﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using R.Systems.Auth.Infrastructure.Db;

namespace R.Systems.Auth.Infrastructure.Db.Migrations
{
    [DbContext(typeof(AuthDbContext))]
    [Migration("20211123183511_add_refresh_token_cols")]
    partial class add_refresh_token_cols
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("R.Systems.Auth.Core.Models.Role", b =>
                {
                    b.Property<long>("RecId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("role_id")
                        .HasIdentityOptions(3L, null, null, null, null, null)
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<string>("RoleKey")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("role_key");

                    b.HasKey("RecId");

                    b.ToTable("role", "user");

                    b.HasData(
                        new
                        {
                            RecId = 1L,
                            Description = "System administrator.",
                            Name = "Administrator",
                            RoleKey = "admin"
                        },
                        new
                        {
                            RecId = 2L,
                            Description = "Standard user.",
                            Name = "User",
                            RoleKey = "user"
                        });
                });

            modelBuilder.Entity("R.Systems.Auth.Core.Models.User", b =>
                {
                    b.Property<long>("RecId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("user_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("last_name");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("password_hash");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("refresh_token");

                    b.Property<DateTime?>("RefreshTokenExpireDateTimeUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("refresh_token_expire_date_time_utc");

                    b.HasKey("RecId");

                    b.ToTable("user", "user");

                    b.HasData(
                        new
                        {
                            RecId = 1L,
                            Email = "admin@lukaszrydzkowski.pl",
                            FirstName = "Lukasz",
                            LastName = "Rydzkowski"
                        });
                });

            modelBuilder.Entity("user_role", b =>
                {
                    b.Property<long>("role_id")
                        .HasColumnType("bigint");

                    b.Property<long>("user_id")
                        .HasColumnType("bigint");

                    b.HasKey("role_id", "user_id");

                    b.HasIndex("user_id");

                    b.ToTable("user_role", "user");

                    b.HasData(
                        new
                        {
                            role_id = 1L,
                            user_id = 1L
                        });
                });

            modelBuilder.Entity("user_role", b =>
                {
                    b.HasOne("R.Systems.Auth.Core.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("R.Systems.Auth.Core.Models.User", null)
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
