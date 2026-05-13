using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiProducto.Domain.Entities;

namespace MiProducto.Infrastructure.Configuration;

public class RubroConfiguration : IEntityTypeConfiguration<Rubro>
{
    public void Configure(EntityTypeBuilder<Rubro> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.IsActive).IsRequired();
        builder.HasMany(r => r.Subrubros).WithOne(s => s.Rubro).HasForeignKey(s => s.RubroId);
    }
}