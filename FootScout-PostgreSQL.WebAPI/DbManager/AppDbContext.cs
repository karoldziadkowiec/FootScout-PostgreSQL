using FootScout_PostgreSQL.WebAPI.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FootScout_PostgreSQL.WebAPI.DbManager
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Achievements> Achievements { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ClubAdvertisement> ClubAdvertisements { get; set; }
        public DbSet<ClubHistory> ClubHistories { get; set; }
        public DbSet<ClubOffer> ClubOffers { get; set; }
        public DbSet<FavoriteClubAdvertisement> FavoriteClubAdvertisements { get; set; }
        public DbSet<FavoritePlayerAdvertisement> FavoritePlayerAdvertisements { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OfferStatus> OfferStatuses { get; set; }
        public DbSet<PlayerAdvertisement> PlayerAdvertisements { get; set; }
        public DbSet<PlayerFoot> PlayerFeet { get; set; }
        public DbSet<PlayerOffer> PlayerOffers { get; set; }
        public DbSet<PlayerPosition> PlayerPositions { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<SalaryRange> SalaryRanges { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClubAdvertisement>()
                .HasOne(ca => ca.ClubMember)
                .WithMany()
                .HasForeignKey(ca => ca.ClubMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClubAdvertisement>()
               .Property(ca => ca.PlayerPosition)
               .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PlayerPosition>(v)
                )
               .HasColumnType("jsonb");

            modelBuilder.Entity<ClubAdvertisement>()
                .HasOne(ca => ca.SalaryRange)
                .WithMany()
                .HasForeignKey(ca => ca.SalaryRangeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteClubAdvertisement>()
                .HasOne(caf => caf.ClubAdvertisement)
                .WithMany()
                .HasForeignKey(caf => caf.ClubAdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteClubAdvertisement>()
                .HasOne(caf => caf.User)
                .WithMany()
                .HasForeignKey(caf => caf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClubOffer>()
                .HasOne(co => co.PlayerAdvertisement)
                .WithMany()
                .HasForeignKey(co => co.PlayerAdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubOffer>()
                .Property(ca => ca.OfferStatus)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<OfferStatus>(v)
                )
                .HasColumnType("jsonb");

            modelBuilder.Entity<ClubOffer>()
                .Property(ca => ca.PlayerPosition)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PlayerPosition>(v)
                )
                .HasColumnType("jsonb");

            modelBuilder.Entity<ClubOffer>()
                .HasOne(co => co.ClubMember)
                .WithMany()
                .HasForeignKey(co => co.ClubMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubHistory>()
                .HasOne(ch => ch.Achievements)
                .WithOne()
                .HasForeignKey<ClubHistory>(ch => ch.AchievementsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubHistory>()
               .Property(ca => ca.PlayerPosition)
               .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PlayerPosition>(v)
                )
               .HasColumnType("jsonb");

            modelBuilder.Entity<ClubHistory>()
                .HasOne(ch => ch.Player)
                .WithMany()
                .HasForeignKey(ch => ch.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoritePlayerAdvertisement>()
               .HasOne(paf => paf.PlayerAdvertisement)
               .WithMany()
               .HasForeignKey(paf => paf.PlayerAdvertisementId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoritePlayerAdvertisement>()
                .HasOne(paf => paf.User)
                .WithMany()
                .HasForeignKey(paf => paf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany()
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAdvertisement>()
                .HasOne(pa => pa.Player)
                .WithMany()
                .HasForeignKey(pa => pa.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAdvertisement>()
                .Property(ca => ca.PlayerPosition)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PlayerPosition>(v)
                )
                .HasColumnType("jsonb");

            modelBuilder.Entity<PlayerAdvertisement>()
                .Property(ca => ca.PlayerFoot)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PlayerFoot>(v)
                )
                .HasColumnType("jsonb");

            modelBuilder.Entity<PlayerAdvertisement>()
                .HasOne(pa => pa.SalaryRange)
                .WithMany()
                .HasForeignKey(pa => pa.SalaryRangeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerOffer>()
                .HasOne(po => po.ClubAdvertisement)
                .WithMany()
                .HasForeignKey(po => po.ClubAdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerOffer>()
                .Property(ca => ca.OfferStatus)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<OfferStatus>(v)
                )
                .HasColumnType("jsonb");

            modelBuilder.Entity<PlayerOffer>()
                .Property(ca => ca.PlayerPosition)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PlayerPosition>(v)
                )
                .HasColumnType("jsonb");

            modelBuilder.Entity<PlayerOffer>()
                .Property(ca => ca.PlayerFoot)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<PlayerFoot>(v)
                )
                .HasColumnType("jsonb");

            modelBuilder.Entity<PlayerOffer>()
                .HasOne(po => po.Player)
                .WithMany()
                .HasForeignKey(po => po.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Problem>()
                .HasOne(p => p.Requester)
                .WithMany()
                .HasForeignKey(p => p.RequesterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}