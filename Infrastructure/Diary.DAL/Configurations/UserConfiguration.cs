using Diary.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Diary.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Login).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Password).IsRequired();

            builder.HasMany<Report>(x => x.Reports)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .HasPrincipalKey(x => x.Id);

            builder.HasMany(x => x.Roles)
                .WithMany(x => x.Users)
                .UsingEntity<UserRole>(x => x.HasOne<Role>().WithMany().HasForeignKey(role => role.RoleId),
                    x => x.HasOne<User>().WithMany().HasForeignKey(role => role.UserId));
            builder.HasData(new List<User>()
            {
                new User()
                {
                    Id = 1,
                    Login = "svyatmtk",
                    Password = "password",
                    CreatedAt = DateTime.UtcNow,
                }
            });
        }
    }
}