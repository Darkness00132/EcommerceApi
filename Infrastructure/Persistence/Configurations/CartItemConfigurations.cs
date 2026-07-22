using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CartItemConfigurations : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => new { ci.CartId, ci.ProductId });

            builder.Property(ci => ci.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}
