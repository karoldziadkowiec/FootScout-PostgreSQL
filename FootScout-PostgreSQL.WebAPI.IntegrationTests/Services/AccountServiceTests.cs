using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager;
using FootScout_PostgreSQL.WebAPI.Models.Constants;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Services.Classes;
using FootScout_PostgreSQL.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.Services
{
    public class AccountServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly AccountService _accountService;

        public AccountServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            var roleStore = new RoleStore<IdentityRole>(_dbContext);
            _roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);
            _tokenService = new TokenService(TestBase.CreateConfiguration(), TestBase.CreateUserManager(_dbContext));
            var cookieServiceMock = new Mock<ICookieService>();
            _cookieService = cookieServiceMock.Object;

            _accountService = new AccountService(_dbContext, TestBase.CreateUserManager(_dbContext), TestBase.CreateMapper(), _roleManager, _tokenService, _cookieService);
        }

        [Fact]
        public async Task Register_SuccessfulRegistration_CreatesUser()
        {
            // Arrange
            var registerDTO = new RegisterDTO
            {
                Email = "new@user.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = "First Name",
                LastName = "Last Name",
                Location = "Location",
                PhoneNumber = "PhoneNumber",
            };

            // Act
            await _accountService.Register(registerDTO);

            // Assert
            var result = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == registerDTO.Email);
            Assert.NotNull(result);
            Assert.Equal(registerDTO.Email, result.Email);
        }

        [Fact]
        public async Task Login_SuccessfulLogin_ReturnsToken()
        {
            // Arrange
            var registerDTO = new RegisterDTO
            {
                Email = "cr7@gmail.com",
                Password = "Cr7771!",
                ConfirmPassword = "Cr7771!",
                FirstName = "Cristiano",
                LastName = "Ronaldo",
                Location = "Madrid",
                PhoneNumber = "707070707"
            };

            // Act
            await _accountService.Register(registerDTO);
            await _dbContext.SaveChangesAsync();

            var loginDTO = new LoginDTO
            {
                Email = "cr7@gmail.com",
                Password = "Cr7771!"
            };

            // Act
            var result = await _accountService.Login(loginDTO);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetRoles_SuccessfulReturnsRoles()
        {
            // Arrange & Act
            var roles = await _accountService.GetRoles();

            // Assert
            var result = await _dbContext.Roles.SingleOrDefaultAsync(u => u.Name == Role.User);
            Assert.NotNull(result);
            Assert.Equal(Role.User, result.Name);
        }

        [Fact]
        public async Task MakeAnAdmin_Successfully_MakesUserAnAdmin()
        {
            // Arrange
            var user = await _dbContext.Users.FindAsync("leomessi");

            // Act
            await _accountService.MakeAnAdmin(user.Id);

            // Assert
            var adminRoleId = await _dbContext.Roles
                .Where(r => r.Name == Role.Admin)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var userInAdminRole = await _dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id && ur.RoleId == adminRoleId)
                .FirstOrDefaultAsync();

            Assert.NotNull(userInAdminRole);

            await _accountService.MakeAnUser(user.Id);
        }

        [Fact]
        public async Task MakeAnUser_Successfully_MakesAdminAnUser()
        {
            // Arrange
            var user = await _dbContext.Users.FindAsync("admin0");

            // Act
            await _accountService.MakeAnUser(user.Id);

            // Assert
            var userRoleId = await _dbContext.Roles
                .Where(r => r.Name == Role.User)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var adminInAdminRole = await _dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id && ur.RoleId == userRoleId)
                .FirstOrDefaultAsync();

            Assert.NotNull(adminInAdminRole);

            await _accountService.MakeAnAdmin(user.Id);
        }
    }
}