using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Services.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Services
{
    public class MessageServiceTests : TestBase
    {
        [Fact]
        public async Task GetAllMessages_ReturnsAllMessages()
        {
            // Arrange
            var options = GetDbContextOptions("GetAllMessages_ReturnsAllMessages");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var _messageService = new MessageService(dbContext);

                // Act
                var result = await _messageService.GetAllMessages();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.Count());
                Assert.All(result, message =>
                {
                    Assert.NotNull(message.Chat);
                    Assert.NotNull(message.Sender);
                    Assert.NotNull(message.Receiver);
                });
            }
        }

        [Fact]
        public async Task GetAllMessagesCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetAllMessagesCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var _messageService = new MessageService(dbContext);

                // Act
                var result = await _messageService.GetAllMessagesCount();

                // Assert
                Assert.Equal(3, result);
            }
        }

        [Fact]
        public async Task GetMessagesForChat_ReturnsMessagesForChat()
        {
            // Arrange
            var options = GetDbContextOptions("GetMessagesForChat_ReturnsMessagesForChat");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var _messageService = new MessageService(dbContext);

                // Act
                var result = await _messageService.GetMessagesForChat(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.All(result, message => Assert.Equal(1, message.ChatId));
            }
        }

        [Fact]
        public async Task GetMessagesForChatCount_ReturnsCorrectCount()
        {
            // Arrange
            var options = GetDbContextOptions("GetMessagesForChatCount_ReturnsCorrectCount");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var _messageService = new MessageService(dbContext);

                // Act
                var result = await _messageService.GetMessagesForChatCount(1);

                // Assert
                Assert.Equal(2, result);
            }
        }

        [Fact]
        public async Task GetLastMessageDateForChat_ReturnsCorrectLastMessageDate()
        {
            // Arrange
            var options = GetDbContextOptions("GetLastMessageDateForChat_ReturnsCorrectLastMessageDate");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var _messageService = new MessageService(dbContext);

                // Act
                var result = await _messageService.GetLastMessageDateForChat(2);

                // Assert
                Assert.Equal(new DateTime(2024, 09, 09), result);
            }
        }

        [Fact]
        public async Task SendMessage_AddsNewMessage()
        {
            // Arrange
            var options = GetDbContextOptions("SendMessage_AddsNewMessage");

            var messageSendDto = new MessageSendDTO
            {
                ChatId = 1,
                SenderId = "leomessi",
                ReceiverId = "pepguardiola",
                Content = "message"
            };

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var _messageService = new MessageService(dbContext);

                // Act
                var result = await _messageService.SendMessage(messageSendDto);
                var savedMessage = await dbContext.Messages.FindAsync(result.Id);

                // Assert
                Assert.NotNull(savedMessage);
                Assert.Equal(messageSendDto.ChatId, savedMessage.ChatId);
                Assert.Equal(messageSendDto.SenderId, savedMessage.SenderId);
                Assert.Equal(messageSendDto.ReceiverId, savedMessage.ReceiverId);
                Assert.Equal(messageSendDto.Content, savedMessage.Content);
            }
        }

        [Fact]
        public async Task DeleteMessage_RemovesMessage()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteMessage_RemovesMessage");

            using (var dbContext = new AppDbContext(options))
            {
                var userManager = CreateUserManager();
                await SeedUserTestBase(dbContext, userManager);
                await SeedChatTestBase(dbContext);
                await SeedMessageTestBase(dbContext);
                var _messageService = new MessageService(dbContext);

                // Act
                await _messageService.DeleteMessage(1);
                var result = await dbContext.Messages.FindAsync(1);

                // Assert
                Assert.Null(result);
            }
        }
    }
}