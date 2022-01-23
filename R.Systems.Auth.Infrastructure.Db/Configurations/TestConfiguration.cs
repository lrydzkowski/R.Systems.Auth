using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R.Systems.Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Configurations;

internal class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable(name: "test", schema: "user");
        builder.HasKey(test => test.Id);
        ConfigureColumns(builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<Test> builder)
    {
        builder.Property(test => test.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(test => test.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);
    }
}
