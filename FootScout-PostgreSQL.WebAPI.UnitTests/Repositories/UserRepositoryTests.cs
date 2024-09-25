using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.Constants;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Repositories
{
    public class UserRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetUser_ReturnsCorrectUser()
        {
            // Arrange
            var options = GetDbContextOptions("GetUser_ReturnsCorrectUser");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUser("leomessi");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("leomessi", result.Id);
                Assert.Equal("Leo", result.FirstName);
                Assert.Equal("Messi", result.LastName);
            }
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            // Arrange
            var options = GetDbContextOptions("GetUsers_ReturnsAllUsers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUsers();

                // Assert
                Assert.Equal(4, result.Count());
            }
        }

        [Fact]
        public async Task GetOnlyUsers_ReturnsUsersWithUserRole()
        {
            // Arrange
            var options = GetDbContextOptions("GetOnlyUsers_ReturnsUsersWithUserRole");

            var mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, new PasswordHasher<User>(), null, null, null, null, null, null
            );

            var users = new List<User>
            {
                new User { Id = "leomessi", UserName = "leomessi", CreationDate = DateTime.UtcNow },
                new User { Id = "pepguardiola", UserName = "pepguardiola", CreationDate = DateTime.UtcNow.AddDays(-1) }
            };

            mockUserManager.Setup(um => um.GetUsersInRoleAsync("User")).ReturnsAsync(users);

            var mapper = CreateMapper();
            var _userRepository = new UserRepository(null, mapper, mockUserManager.Object, null);

            // Act
            var result = await _userRepository.GetOnlyUsers();

            // Assert
            Assert.NotNull(result);
            var userList = result.ToList();
            Assert.Contains(userList, u => u.Id == "leomessi");
            Assert.Contains(userList, u => u.Id == "pepguardiola");
            Assert.Equal(2, userList.Count);
        }

        [Fact]
        public async Task GetOnlyAdmins_ReturnsUsersWithAdminRole()
        {
            // Arrange
            var options = GetDbContextOptions("GetOnlyAdmins_ReturnsUsersWithAdminRole");

            var mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, new PasswordHasher<User>(), null, null, null, null, null, null
            );

            var users = new List<User>
            {
                new User { Id = "admin0", UserName = "admin0", CreationDate = DateTime.UtcNow },
                new User { Id = "admin1", UserName = "admin1", CreationDate = DateTime.UtcNow.AddDays(-1) }
            };

            mockUserManager.Setup(um => um.GetUsersInRoleAsync("Admin")).ReturnsAsync(users);

            var mapper = CreateMapper();
            var _userRepository = new UserRepository(null, mapper, mockUserManager.Object, null);

            // Act
            var result = await _userRepository.GetOnlyAdmins();

            // Assert
            Assert.NotNull(result);
            var userList = result.ToList();
            Assert.Contains(userList, u => u.Id == "admin0");
            Assert.Contains(userList, u => u.Id == "admin1");
            Assert.Equal(2, userList.Count);
        }

        [Fact]
        public async Task GetUserRole_ReturnsCorrectRole()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserRole_ReturnsCorrectRole");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);

                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserRole("leomessi");

                // Assert
                Assert.Equal(Role.User, result);
            }
        }

        [Fact]
        public async Task GetUserCount_ReturnsCorrectNumberOfUsers()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserCount_ReturnsCorrectNumberOfUsers");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserCount();

                // Assert
                Assert.Equal(4, result);
            }
        }

        [Fact]
        public async Task UpdateUser_UpdatesUserDetails()
        {
            // Arrange
            var options = GetDbContextOptions("UpdateUser_UpdatesUserDetails");
            var dto = new UserUpdateDTO
            {
                FirstName = "Updated FirstName"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);

                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                await _userRepository.UpdateUser("leomessi", dto);

                // Assert
                var updatedUser = await dbContext.Users.FindAsync("leomessi");
                Assert.NotNull(updatedUser);
                Assert.Equal("Updated FirstName", updatedUser.FirstName);
            }
        }

        [Fact]
        public async Task ResetUserPassword_ResetsPasswordSuccessfully()
        {
            // Arrange
            var options = GetDbContextOptions("ResetUserPassword_ResetsPasswordSuccessfully");
            var dto = new UserResetPasswordDTO
            {
                PasswordHash = "NewPassword123!",
                ConfirmPasswordHash = "NewPassword123!"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);

                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                await _userRepository.ResetUserPassword("leomessi", dto);

                // Assert
                var updatedUser = await dbContext.Users.FindAsync("leomessi");
                Assert.NotNull(updatedUser);
                Assert.True(passwordHasher.VerifyHashedPassword(updatedUser, updatedUser.PasswordHash, "NewPassword123!") == PasswordVerificationResult.Success);
            }
        }

        [Fact]
        public async Task DeleteUser_RemovesUserAndRelatedEntities()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteUser_RemovesUserAndRelatedEntities");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                var user = await dbContext.Users.FindAsync(userId);
                if (user == null)
                    throw new Exception("Test user not found");

                // Act
                await _userRepository.DeleteUser(userId);

                // Assert
                var deletedUser = await dbContext.Users.FindAsync(userId);
                Assert.Null(deletedUser);

                var clubHistories = await dbContext.ClubHistories
                    .Where(ch => ch.PlayerId == userId)
                    .ToListAsync();
                Assert.Empty(clubHistories);

                var chats = await dbContext.Chats
                    .Where(c => c.User1Id == userId || c.User2Id == userId)
                    .ToListAsync();
                Assert.Empty(chats);

                var playerFavorites = await dbContext.FavoritePlayerAdvertisements
                    .Where(fpa => fpa.UserId == userId)
                    .ToListAsync();
                Assert.Empty(playerFavorites);

                var clubFavorites = await dbContext.FavoriteClubAdvertisements
                    .Where(fca => fca.UserId == userId)
                    .ToListAsync();
                Assert.Empty(clubFavorites);

                var playerAdvertisements = await dbContext.PlayerAdvertisements
                    .Where(pa => pa.PlayerId == userId)
                    .ToListAsync();
                Assert.All(playerAdvertisements, pa => Assert.Equal("unknownUserId", pa.PlayerId));

                var clubOffers = await dbContext.ClubOffers
                    .Where(co => co.ClubMemberId == userId)
                    .ToListAsync();
                Assert.All(clubOffers, co => Assert.Equal("unknownUserId", co.ClubMemberId));

                var clubAdvertisements = await dbContext.ClubAdvertisements
                    .Where(ca => ca.ClubMemberId == userId)
                    .ToListAsync();
                Assert.All(clubAdvertisements, ca => Assert.Equal("unknownUserId", ca.ClubMemberId));

                var playerOffers = await dbContext.PlayerOffers
                    .Where(po => po.PlayerId == userId)
                    .ToListAsync();
                Assert.All(playerOffers, po => Assert.Equal("unknownUserId", po.PlayerId));

                var problems = await dbContext.Problems
                    .Where(p => p.RequesterId == userId)
                    .ToListAsync();
                Assert.Empty(problems);
            }
        }

        [Fact]
        public async Task GetUserClubHistory_ReturnsUserClubHistories()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserClubHistory_ReturnsUserClubHistories");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubHistoryTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserClubHistory(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ch => Assert.Equal(userId, ch.PlayerId));
                Assert.True(result.All(ch => ch.StartDate != default(DateTime)));
            }
        }

        [Fact]
        public async Task GetUserPlayerAdvertisements_ReturnsUserPlayerAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserPlayerAdvertisements_ReturnsUserPlayerAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserPlayerAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, pa => Assert.Equal(userId, pa.PlayerId));
                Assert.True(result.All(pa => pa.EndDate != default(DateTime)));
            }
        }

        [Fact]
        public async Task GetUserActivePlayerAdvertisements_ReturnsActivePlayerAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserActivePlayerAdvertisements_ReturnsActivePlayerAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserActivePlayerAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, pa => Assert.Equal(userId, pa.PlayerId));
                Assert.All(result, pa => Assert.True(pa.EndDate >= DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetUserInactivePlayerAdvertisements_ReturnsInactivePlayerAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserInactivePlayerAdvertisements_ReturnsInactivePlayerAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserInactivePlayerAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, pa => Assert.Equal(userId, pa.PlayerId));
                Assert.All(result, pa => Assert.True(pa.EndDate < DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetUserFavoritePlayerAdvertisements_ReturnsFavoriteAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserFavoritePlayerAdvertisements_ReturnsFavoriteAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserFavoritePlayerAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, pa => Assert.Equal(userId, pa.UserId));
                Assert.True(result.All(pa => pa.PlayerAdvertisement != null));
            }
        }

        [Fact]
        public async Task GetUserActiveFavoritePlayerAdvertisements_ReturnsActiveFavoriteAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserActiveFavoritePlayerAdvertisements_ReturnsActiveFavoriteAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserActiveFavoritePlayerAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, pa => Assert.Equal(userId, pa.UserId));
                Assert.All(result, pa => Assert.True(pa.PlayerAdvertisement.EndDate >= DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetUserInactiveFavoritePlayerAdvertisements_ReturnsInactiveFavoriteAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserInactiveFavoritePlayerAdvertisements_ReturnsInactiveFavoriteAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserInactiveFavoritePlayerAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, pa => Assert.Equal(userId, pa.UserId));
                Assert.All(result, pa => Assert.True(pa.PlayerAdvertisement.EndDate < DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetReceivedClubOffers_ReturnsReceivedClubOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetReceivedClubOffers_ReturnsReceivedClubOffers");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetReceivedClubOffers(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, co => Assert.Equal(userId, co.PlayerAdvertisement.PlayerId));
                Assert.True(result.All(co => co.OfferStatus != null));
            }
        }

        [Fact]
        public async Task GetSentClubOffers_ReturnsSentClubOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetSentClubOffers_ReturnsSentClubOffers");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedPlayerAdvertisementTestBase(dbContext);
                await SeedClubOfferTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetSentClubOffers(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, co => Assert.Equal(userId, co.ClubMemberId));
                Assert.True(result.All(co => co.OfferStatus != null));
            }
        }

        [Fact]
        public async Task GetUserClubAdvertisements_ReturnsClubAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserClubAdvertisements_ReturnsClubAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserClubAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ca => Assert.Equal(userId, ca.ClubMemberId));
                Assert.True(result.All(ca => ca.PlayerPosition != null));
            }
        }

        [Fact]
        public async Task GetUserActiveClubAdvertisements_ReturnsActiveClubAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserActiveClubAdvertisements_ReturnsActiveClubAdvertisements");
            var userId = "leomessi";
            
            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserActiveClubAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ca => Assert.Equal(userId, ca.ClubMemberId));
                Assert.All(result, ca => Assert.True(ca.EndDate >= DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetUserInactiveClubAdvertisements_ReturnsInactiveClubAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserInactiveClubAdvertisements_ReturnsInactiveClubAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserInactiveClubAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ca => Assert.Equal(userId, ca.ClubMemberId));
                Assert.All(result, ca => Assert.True(ca.EndDate < DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetUserFavoriteClubAdvertisements_ReturnsFavoriteClubAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserFavoriteClubAdvertisements_ReturnsFavoriteClubAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserFavoriteClubAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ca => Assert.Equal(userId, ca.UserId));
                Assert.True(result.All(ca => ca.ClubAdvertisement != null));
            }
        }

        [Fact]
        public async Task GetUserActiveFavoriteClubAdvertisements_ReturnsActiveFavoriteClubAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserActiveFavoriteClubAdvertisements_ReturnsActiveFavoriteClubAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserActiveFavoriteClubAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ca => Assert.Equal(userId, ca.UserId));
                Assert.All(result, ca => Assert.True(ca.ClubAdvertisement.EndDate >= DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetUserInactiveFavoriteClubAdvertisements_ReturnsInactiveFavoriteClubAdvertisements()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserInactiveFavoriteClubAdvertisements_ReturnsInactiveFavoriteClubAdvertisements");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedClubAdvertisementTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserInactiveFavoriteClubAdvertisements(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, ca => Assert.Equal(userId, ca.UserId));
                Assert.All(result, ca => Assert.True(ca.ClubAdvertisement.EndDate < DateTime.UtcNow));
            }
        }

        [Fact]
        public async Task GetReceivedPlayerOffers_ReturnsReceivedPlayerOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetReceivedPlayerOffers_ReturnsReceivedPlayerOffers");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetReceivedPlayerOffers(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, po => Assert.Equal(userId, po.ClubAdvertisement.ClubMemberId));
                Assert.True(result.All(po => po.OfferStatus != null));
            }
        }

        [Fact]
        public async Task GetSentPlayerOffers_ReturnsSentPlayerOffers()
        {
            // Arrange
            var options = GetDbContextOptions("GetSentPlayerOffers_ReturnsSentPlayerOffers");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedPlayerPositionTestBase(dbContext);
                await SeedPlayerFootTestBase(dbContext);
                await SeedOfferStatusTestBase(dbContext);
                await SeedClubAdvertisementTestBase(dbContext);
                await SeedPlayerOfferTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetSentPlayerOffers(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, po => Assert.Equal(userId, po.PlayerId));
                Assert.True(result.All(po => po.OfferStatus != null));
            }
        }

        [Fact]
        public async Task GetUserChats_ReturnsChatsOrderedByLastMessage()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserChats_ReturnsChatsOrderedByLastMessage");
            var userId = "leomessi";

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var result = await _userRepository.GetUserChats(userId);

                // Assert
                Assert.NotNull(result);
                Assert.All(result, chat => Assert.True(chat.User1Id == userId || chat.User2Id == userId));
            }
        }

        [Fact]
        public async Task ExportUsersToCsv_ReturnsValidCsvStream()
        {
            // Arrange
            var options = GetDbContextOptions("ExportUsersToCsv_ReturnsValidCsvStream");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                var mapper = CreateMapper();
                var passwordHasher = CreatePasswordHasher();
                var _userRepository = new UserRepository(dbContext, mapper, userManager, passwordHasher);

                // Act
                var csvStream = await _userRepository.ExportUsersToCsv();
                csvStream.Position = 0;

                using (var reader = new StreamReader(csvStream))
                {
                    var csvContent = await reader.ReadToEndAsync();

                    // Assert
                    Assert.NotEmpty(csvContent);
                    Assert.Contains("E-mail,First Name,Last Name,Phone Number,Location,Creation Date", csvContent);
                    Assert.Contains("lm10@gmail.com,Leo,Messi,101010101", csvContent);
                }
            }
        }
    }
}