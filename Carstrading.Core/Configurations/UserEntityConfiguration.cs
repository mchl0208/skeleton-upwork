using Carstrading.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carstrading.Core.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(u => u.EmailAddress).IsRequired();
            builder.ToTable("Users");

            builder.Property(u => u.CreatedDate).HasDefaultValueSql("getutcdate()");
            builder.Property(u => u.ModifiedDate).HasDefaultValueSql("getutcdate()");

            //builder.Property(u => u.CreatedBy).IsRequired(false);
        }
    }
}
