using AutoMapper;
using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.Constants;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager
{
    public class TestBase
    {
        public static DbContextOptions<AppDbContext> GetDbContextOptions(string dbName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(dbName)
                .Options;
        }

        public static IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return configuration.CreateMapper();
        }

        public static UserManager<User> CreateUserManager(AppDbContext dbContext)
        {
            var userStore = new UserStore<User>(dbContext);
            var passwordHasher = new PasswordHasher<User>();
            var options = new IdentityOptions();
            var userValidators = new List<IUserValidator<User>>
            {
                new UserValidator<User>()
            };
            var passwordValidators = new List<IPasswordValidator<User>>
            {
                new PasswordValidator<User>()
            };

            return new UserManager<User>(
                userStore,
                null,
                passwordHasher,
                userValidators,
                passwordValidators,
                null,
                new IdentityErrorDescriber(),
                null,
                new Logger<UserManager<User>>(new LoggerFactory())
            );
        }

        public static IPasswordHasher<User> CreatePasswordHasher()
        {
            return new PasswordHasher<User>();
        }

        public static IConfiguration CreateConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:Secret", "keyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy"},
                {"JWT:ValidAudience", "http://localhost:3000"},
                {"JWT:ValidIssuer", "https://localhost:7104"},
                {"JWT:ExpireDays", "1"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return configuration;
        }

        // Scenario

        public static async Task SeedRoleTestBase(AppDbContext dbContext)
        {
            var roleStore = new RoleStore<IdentityRole>(dbContext);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);
            var roles = new List<string> { Role.Admin, Role.User };
            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    var _role = new IdentityRole(role);
                    await roleManager.CreateAsync(_role);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedUserTestBase(AppDbContext dbContext, UserManager<User> userManager)
        {
            // users
            dbContext.Users.AddRange(new List<User>
            {
                new User { Id = "admin0", Email = "admin@admin.com", UserName = "admin@admin.com", PasswordHash = "Admin1!", FirstName = "Admin F.", LastName = "Admin L.", Location = "Admin Loc.", PhoneNumber = "000000000" },
                new User { Id = "unknown9", Email = "unknown@unknown.com", UserName = "unknown@unknown.com", PasswordHash = "Uuuuu1!",FirstName = "Unknown F.", LastName = "Unknown L.", Location = "Unknown Loc.", PhoneNumber = "999999999" },
                new User { Id = "leomessi", Email = "lm10@gmail.com", UserName = "lm10@gmail.com", PasswordHash = "Leooo1!",FirstName = "Leo", LastName = "Messi", Location = "Miami", PhoneNumber = "101010101" },
                new User { Id = "pepguardiola", Email = "pg8@gmail.com", UserName = "pg8@gmail.com", PasswordHash = "Peppp1!",FirstName = "Pep", LastName = "Guardiola", Location = "Manchester", PhoneNumber = "868686868" }
            });
            await dbContext.SaveChangesAsync();

            // user roles
            dbContext.UserRoles.AddRange(new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> { UserId = "admin0", RoleId = dbContext.Roles.First(r => r.Name == Role.Admin).Id },
                new IdentityUserRole<string> { UserId = "unknown9", RoleId = dbContext.Roles.First(r => r.Name == Role.User).Id },
                new IdentityUserRole<string> { UserId = "leomessi", RoleId = dbContext.Roles.First(r => r.Name == Role.User).Id },
                new IdentityUserRole<string> { UserId = "pepguardiola", RoleId = dbContext.Roles.First(r => r.Name == Role.User).Id },
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedOfferStatusTestBase(AppDbContext dbContext)
        {
            var statuses = new List<string> { OfferStatusName.Offered, OfferStatusName.Accepted, OfferStatusName.Rejected };

            foreach (var status in statuses)
            {
                if (!await dbContext.OfferStatuses.AnyAsync(s => s.StatusName == status))
                {
                    OfferStatus newStatus = new OfferStatus
                    {
                        StatusName = status,
                    };
                    dbContext.OfferStatuses.Add(newStatus);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedPlayerPositionTestBase(AppDbContext dbContext)
        {
            var positions = new List<string> { Position.Goalkeeper, Position.RightBack, Position.CenterBack, Position.LeftBack, Position.RightWingBack, Position.LeftWingBack, Position.CentralDefensiveMidfield, Position.CentralMidfield, Position.CentralAttackingMidfield, Position.RightMidfield, Position.RightWing, Position.LeftMidfield, Position.LeftWing, Position.CentreForward, Position.Striker };

            foreach (var position in positions)
            {
                if (!await dbContext.PlayerPositions.AnyAsync(p => p.PositionName == position))
                {
                    PlayerPosition newPosition = new PlayerPosition
                    {
                        PositionName = position,
                    };
                    dbContext.PlayerPositions.Add(newPosition);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedPlayerFootTestBase(AppDbContext dbContext)
        {
            var feet = new List<string> { Foot.Left, Foot.Right, Foot.TwoFooted };

            foreach (var foot in feet)
            {
                if (!await dbContext.PlayerFeet.AnyAsync(p => p.FootName == foot))
                {
                    PlayerFoot newFoot = new PlayerFoot
                    {
                        FootName = foot,
                    };
                    dbContext.PlayerFeet.Add(newFoot);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedClubHistoryTestBase(AppDbContext dbContext)
        {
            // achievements
            dbContext.Achievements.AddRange(new List<Achievements>
            {
                new Achievements { NumberOfMatches = 750, Goals = 678, Assists = 544, AdditionalAchievements = "LM" },
                new Achievements { NumberOfMatches = 40, Goals = 27, Assists = 24, AdditionalAchievements = "No info" },
            });
            await dbContext.SaveChangesAsync();

            // club history
            dbContext.ClubHistories.AddRange(new List<ClubHistory>
            {
                new ClubHistory { PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "FC Barcelona", League = "La Liga", Region = "Spain", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(150), AchievementsId = 1, PlayerId = "leomessi"},
                new ClubHistory { PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "PSG", League = "Ligue1", Region = "France", StartDate = DateTime.UtcNow.AddDays(150), EndDate = DateTime.UtcNow.AddDays(300), AchievementsId = 2, PlayerId = "leomessi"},
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedProblemTestBase(AppDbContext dbContext)
        {
            // club history
            dbContext.Problems.AddRange(new List<Problem>
            {
                new Problem { Title = "Problem 1", Description = "Desc 1", CreationDate = DateTime.UtcNow, IsSolved = true, RequesterId = "leomessi" },
                new Problem { Title = "Problem 2", Description = "Desc 2", CreationDate = DateTime.UtcNow.AddDays(150), IsSolved = false, RequesterId = "pepguardiola" },
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedChatTestBase(AppDbContext dbContext)
        {
            // chat
            dbContext.Chats.AddRange(new List<Chat>
            {
                new Chat { User1Id = "leomessi", User2Id = "pepguardiola" },
                new Chat { User1Id = "admin0", User2Id = "leomessi" },
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedMessageTestBase(AppDbContext dbContext)
        {
            // messages
            dbContext.Messages.AddRange(new List<Message>
            {
                new Message { ChatId = 1, Content = "Hey", SenderId = "pepguardiola", ReceiverId = "leomessi" , Timestamp = DateTime.UtcNow },
                new Message { ChatId = 1, Content = "Hello", SenderId = "leomessi", ReceiverId = "pepguardiola" , Timestamp = DateTime.UtcNow },
                new Message { ChatId = 2, Content = "a b c", SenderId = "admin0", ReceiverId = "leomessi" , Timestamp = DateTime.UtcNow },
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedPlayerAdvertisementTestBase(AppDbContext dbContext)
        {
            // salary range
            dbContext.SalaryRanges.AddRange(new List<SalaryRange>
            {
                new SalaryRange { Min = 150, Max = 200 },
                new SalaryRange { Min = 145, Max = 195 },
            });
            await dbContext.SaveChangesAsync();

            // player advertisement
            dbContext.PlayerAdvertisements.AddRange(new List<PlayerAdvertisement>
            {
                new PlayerAdvertisement { PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, League = "Premier League", Region = "England", Age = 37, Height = 167, PlayerFootId = 3, PlayerFoot = new PlayerFoot { Id = 3, FootName = "Right" }, SalaryRangeId = 1, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), PlayerId = "leomessi" },
                new PlayerAdvertisement { PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, League = "La Liga", Region = "Spain", Age = 37, Height = 167, PlayerFootId = 3, PlayerFoot = new PlayerFoot { Id = 3, FootName = "Right" }, SalaryRangeId = 2, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), PlayerId = "leomessi" },
            });
            await dbContext.SaveChangesAsync();

            // favorite player advertisement
            dbContext.FavoritePlayerAdvertisements.AddRange(new List<FavoritePlayerAdvertisement>
            {
                new FavoritePlayerAdvertisement { PlayerAdvertisementId = 1, UserId = "pepguardiola" },
                new FavoritePlayerAdvertisement { PlayerAdvertisementId = 2, UserId = "pepguardiola" },
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedClubOfferTestBase(AppDbContext dbContext)
        {
            // club offer
            dbContext.ClubOffers.AddRange(new List<ClubOffer>
            {
                new ClubOffer { PlayerAdvertisementId = 1, OfferStatusId = 1, OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" }, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "Manchester City", League = "Premier League", Region = "England", Salary = 160, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, ClubMemberId = "pepguardiola" },
                new ClubOffer { PlayerAdvertisementId = 2, OfferStatusId = 2, OfferStatus = new OfferStatus { Id = 2, StatusName = "Accepted" }, PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, ClubName = "Real Madrid", League = "La Liga", Region = "Spain", Salary = 155, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, ClubMemberId = "pepguardiola" },
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedClubAdvertisementTestBase(AppDbContext dbContext)
        {
            // salary range
            dbContext.SalaryRanges.AddRange(new List<SalaryRange>
            {
                new SalaryRange { Min = 150, Max = 200 },
                new SalaryRange { Min = 145, Max = 195 },
            });
            await dbContext.SaveChangesAsync();

            // club advertisement
            dbContext.ClubAdvertisements.AddRange(new List<ClubAdvertisement>
            {
                new ClubAdvertisement { PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, ClubName = "Manchester City", League = "Premier League", Region = "England", SalaryRangeId = 3, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), ClubMemberId = "pepguardiola" },
                new ClubAdvertisement { PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, ClubName = "Manchester City", League = "Premier League", Region = "England", SalaryRangeId = 4, CreationDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), ClubMemberId = "pepguardiola" },
            });
            await dbContext.SaveChangesAsync();

            // favorite club advertisement
            dbContext.FavoriteClubAdvertisements.AddRange(new List<FavoriteClubAdvertisement>
            {
                new FavoriteClubAdvertisement { ClubAdvertisementId = 1, UserId = "leomessi" },
                new FavoriteClubAdvertisement { ClubAdvertisementId = 2, UserId = "leomessi" },
            });
            await dbContext.SaveChangesAsync();
        }

        public static async Task SeedPlayerOfferTestBase(AppDbContext dbContext)
        {
            // player offer
            dbContext.PlayerOffers.AddRange(new List<PlayerOffer>
            {
                new PlayerOffer { ClubAdvertisementId = 1, OfferStatusId = 1, OfferStatus = new OfferStatus { Id = 1, StatusName = "Offered" }, PlayerPositionId = 15, PlayerPosition = new PlayerPosition { Id = 15, PositionName = "Striker" }, Age = 37, Height = 167, PlayerFootId = 1, PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" }, Salary = 160, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, PlayerId = "leomessi" },
                new PlayerOffer { ClubAdvertisementId = 2, OfferStatusId = 2, OfferStatus = new OfferStatus { Id = 2, StatusName = "Accepted" }, PlayerPositionId = 14, PlayerPosition = new PlayerPosition { Id = 14, PositionName = "Centre-Forward" }, Age = 37, Height = 167, PlayerFootId = 1, PlayerFoot = new PlayerFoot { Id = 1, FootName = "Left" }, Salary = 155, AdditionalInformation = "no info", CreationDate = DateTime.UtcNow, PlayerId = "leomessi" },
            });
            await dbContext.SaveChangesAsync();
        }
    }
}