using EcoGuardian_Backend.IAM.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoGuardian_Backend.Shared.Infrastructure.Persistence.EFC.Configuration.Builders;

public class UserConfigurationBuilder : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(u => u.Email).IsRequired();
        builder.Property(u => u.Auth0UserId).HasMaxLength(255);
        builder.HasIndex(u => u.Auth0UserId).IsUnique();
    }
}