using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Domain.Common;
using WillyNet.JunSlack.Core.Domain.Entities;

namespace WillyNet.JunSlack.Infraestructure.Persistence.Contexts
{
    public class ApplicationDbContext  : IdentityDbContext<AppUser>
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;

        public ApplicationDbContext(
            DbContextOptions options,
             IDateTimeService dateTime, IAuthenticatedUserService authenticatedUser
            ) : base(options)
        {

            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;

        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<TypingNotification> TypingNotifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = _dateTime.NowUtc;
                        entry.Entity.CreatedBy = _authenticatedUser.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = _dateTime.NowUtc;
                        entry.Entity.LastModifiedBy = _authenticatedUser.UserId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasKey(p => p.ChannelId);
                entity.ToTable("Channel");
                entity.Property(p => p.Name)
                        .HasMaxLength(50);
                entity.Property(p => p.Description)
                        .HasMaxLength(350);
                entity.HasOne(o => o.TypingNotification)
                        .WithOne(o => o.Channel)
                        .HasForeignKey<TypingNotification>(f => f.ChannelId);
            });

            modelBuilder.Entity<Message>(entity => {
                entity.HasKey(p => p.MessageId);
                entity.ToTable("Message");
                entity.HasOne(o => o.Sender)
                        .WithMany(m => m.Messages)
                        .HasForeignKey(f => f.SenderId);
                entity.HasOne(o => o.Channel)
                        .WithMany(m => m.Messages)
                        .HasForeignKey(f => f.ChannelId);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasOne(o => o.TypingNotification)
                        .WithOne(o => o.Sender)
                        .HasForeignKey<TypingNotification>(f => f.Id);
                        
            });

            modelBuilder.Entity<TypingNotification>(entity =>
            {
                entity.HasKey(p => p.TypingNotificationId);
                entity.ToTable("TypingNotification");               
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(p => p.RefreshTokenId);
                entity.ToTable("RefreshToken");
                entity.HasOne(o => o.UserApp)
                    .WithMany(m => m.RefreshTokens)
                    .HasForeignKey(f => f.UserAppId);
            });
        }

    }
}
