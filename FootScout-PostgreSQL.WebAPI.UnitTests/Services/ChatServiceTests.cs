using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Services.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Services
{
    public class ChatServiceTests : TestBase
    {
        [Fact]
        public async Task GetChatById_ReturnsCorrectChat()
        {
            // Arrange
            var options = GetDbContextOptions("GetChatById_ReturnsCorrectChat");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                var _chatService = new ChatService(dbContext);

                // Act
                var result = await _chatService.GetChatById(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("leomessi", result.User1Id);
            }
        }

        [Fact]
        public async Task GetChats_ReturnsAllChats()
        {
            // Arrange
            var options = GetDbContextOptions("GetChats_ReturnsAllChats");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                var _chatService = new ChatService(dbContext);

                // Act
                var result = await _chatService.GetChats();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetChatCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetChatCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                var _chatService = new ChatService(dbContext);

                // Act
                var result = await _chatService.GetChatCount();

                // Assert
                Assert.Equal(2, result);
            }
        }

        [Fact]
        public async Task GetChatIdBetweenUsers_ReturnsCorrectChatId()
        {
            // Arrange
            var options = GetDbContextOptions("GetChatIdBetweenUsers_ReturnsCorrectChatId");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                var _chatService = new ChatService(dbContext);

                // Act
                var result = await _chatService.GetChatIdBetweenUsers("leomessi", "pepguardiola");

                // Assert
                Assert.Equal(1, result);
            }
        }

        [Fact]
        public async Task CreateChat_AddsNewChat()
        {
            // Arrange
            var options = GetDbContextOptions("CreateChat_AddsNewChat");

            using (var dbContext = new AppDbContext(options))
            {
                var _chatService = new ChatService(dbContext);

                var newChat = new Chat
                {
                    User1Id = "user3Id",
                    User2Id = "user4Id"
                };

                // Act
                await _chatService.CreateChat(newChat);
                var result = await dbContext.Chats.FindAsync(newChat.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("user3Id", result.User1Id);
                Assert.Equal("user4Id", result.User2Id);
            }
        }

        [Fact]
        public async Task DeleteChat_RemovesChatAndMessages()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteChat_RemovesChatAndMessages");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                var _chatService = new ChatService(dbContext);

                // Act
                await _chatService.DeleteChat(1);
                var chatResult = await dbContext.Chats.FindAsync(1);
                var messagesResult = await dbContext.Messages.Where(m => m.ChatId == 1).ToListAsync();

                // Assert
                Assert.Null(chatResult);
                Assert.Empty(messagesResult);
            }
        }

        [Fact]
        public async Task ExportChatsToCsv_ReturnsValidCsvStream()
        {
            // Arrange
            var options = GetDbContextOptions("ExportChatsToCsv_ReturnsValidCsvStream");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                var _chatService = new ChatService(dbContext);

                // Act
                var csvStream = await _chatService.ExportChatsToCsv();
                var csvContent = Encoding.UTF8.GetString(csvStream.ToArray());

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("Chat Id,User1 E-mail,User1 First Name,User1 Last Name,User2 E-mail,User2 First Name,User2 Last Name", csvContent);
                Assert.Contains("1,lm10@gmail.com,Leo,Messi,pg8@gmail.com,Pep,Guardiola", csvContent);
            }
        }
    }
}