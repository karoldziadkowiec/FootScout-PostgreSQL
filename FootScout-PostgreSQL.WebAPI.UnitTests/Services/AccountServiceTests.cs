using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.Constants;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;
using FootScout_PostgreSQL.WebAPI.Services.Classes;
using FootScout_PostgreSQL.WebAPI.Services.Interfaces;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Services
{
    public class AccountServiceTests : TestBase
    {
        [Fact]
        public async Task Register_SuccessfulRegistration_CreatesUser()
        {
            // Arrange
            var options = GetDbContextOptions("Register_SuccessfulRegistration_CreatesUser");

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

            using (var dbContext = new AppDbContext(options))
            {
                var userStore = new UserStore<User>(dbContext);
                var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

                var roleStore = new RoleStore<IdentityRole>(dbContext);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

                // Seed roles
                await SeedRoleTestBase(dbContext, roleManager);

                var mapper = CreateMapper();
                var _accountService = new AccountService(dbContext, userManager, mapper, roleManager, Mock.Of<ITokenService>(), Mock.Of<ICookieService>());

                // Act
                await _accountService.Register(registerDTO);

                // Assert
                var result = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == registerDTO.Email);
                Assert.NotNull(result);
                Assert.Equal(registerDTO.Email, result.Email);
                Assert.True(await userManager.IsInRoleAsync(result, Role.User));
            }
        }

        private async Task SeedRoleTestBase(AppDbContext dbContext, RoleManager<IdentityRole> roleManager)
        {
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
        }

        [Fact]
        public async Task Login_SuccessfulLogin_ReturnsToken()
        {
            // Arrange
            var options = GetDbContextOptions("Login_SuccessfulLogin_ReturnsToken");

            var loginDTO = new LoginDTO
            {
                Email = "user@domain.com",
                Password = "Password1!"
            };

            var user = new User { Email = "user@domain.com" };

            using (var dbContext = new AppDbContext(options))
            {
                var token = new JwtSecurityToken();
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Mock UserManager
                var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, new PasswordHasher<User>(), null, null, null, null, null, null);

                userManagerMock.Setup(um => um.FindByEmailAsync(loginDTO.Email)).ReturnsAsync(user);

                userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginDTO.Password)).ReturnsAsync(true);

                // Mock TokenService
                var tokenServiceMock = new Mock<ITokenService>();
                tokenServiceMock.Setup(ts => ts.CreateTokenJWT(user)).ReturnsAsync(token);

                // Mock CookieService
                var cookieServiceMock = new Mock<ICookieService>();
                cookieServiceMock.Setup(cs => cs.SetCookies(token, tokenString)).Returns(Task.CompletedTask);

                // Mock RoleManager
                var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                    Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

                roleManagerMock.Setup(rm => rm.Roles).Returns(Enumerable.Empty<IdentityRole>().AsQueryable());

                var accountService = new AccountService(dbContext, userManagerMock.Object, Mock.Of<IMapper>(), roleManagerMock.Object, tokenServiceMock.Object, cookieServiceMock.Object);

                // Act
                var result = await accountService.Login(loginDTO);

                // Assert
                Assert.Equal(tokenString, result);
            }
        }

        [Fact]
        public async Task GetRoles_SuccessfulReturnsRoles()
        {
            // Arrange
            var options = GetDbContextOptions("GetRoles_SuccessfulReturnsRole");

            using (var dbContext = new AppDbContext(options))
            {
                var userStore = new UserStore<User>(dbContext);
                var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

                var roleStore = new RoleStore<IdentityRole>(dbContext);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

                // Seed roles
                await SeedRoleTestBase(dbContext);

                var mapper = CreateMapper();
                var _accountService = new AccountService(dbContext, userManager, mapper, roleManager, Mock.Of<ITokenService>(), Mock.Of<ICookieService>());

                // Act
                var roles = await _accountService.GetRoles();

                // Assert
                var result = await dbContext.Roles.SingleOrDefaultAsync(u => u.Name == Role.User);
                Assert.NotNull(result);
                Assert.Equal(Role.User, result.Name);
            }
        }

        [Fact]
        public async Task MakeAnAdmin_Successfully_MakesUserAnAdmin()
        {
            // Arrange
            var options = GetDbContextOptions("MakeAnAdmin_Successfully_MakesUserAnAdmin");

            using (var dbContext = new AppDbContext(options))
            {
                var userStore = new UserStore<User>(dbContext);
                var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

                var roleStore = new RoleStore<IdentityRole>(dbContext);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

                // Seed roles and users
                await SeedRoleTestBase(dbContext, roleManager);
                await SeedUserTestBase(dbContext, userManager);

                var user = await userManager.FindByIdAsync("leomessi");

                var mapper = CreateMapper();
                var _accountService = new AccountService(dbContext, userManager, mapper, roleManager, Mock.Of<ITokenService>(), Mock.Of<ICookieService>());

                // Act
                await _accountService.MakeAnAdmin(user.Id);

                // Assert
                var adminRoleId = await dbContext.Roles
                    .Where(r => r.Name == Role.Admin)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                var userInAdminRole = await dbContext.UserRoles
                    .Where(ur => ur.UserId == user.Id && ur.RoleId == adminRoleId)
                    .FirstOrDefaultAsync();

                Assert.NotNull(userInAdminRole);
            }
        }

        [Fact]
        public async Task MakeAnUser_Successfully_MakesAdminAnUser()
        {
            // Arrange
            var options = GetDbContextOptions("MakeAnUser_Successfully_MakesAdminAnUser");

            using (var dbContext = new AppDbContext(options))
            {
                var userStore = new UserStore<User>(dbContext);
                var userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

                var roleStore = new RoleStore<IdentityRole>(dbContext);
                var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

                // Seed roles and users
                await SeedRoleTestBase(dbContext, roleManager);
                await SeedUserTestBase(dbContext, userManager);

                var user = await userManager.FindByIdAsync("admin0");

                var mapper = CreateMapper();
                var _accountService = new AccountService(dbContext, userManager, mapper, roleManager, Mock.Of<ITokenService>(), Mock.Of<ICookieService>());

                // Act
                await _accountService.MakeAnUser(user.Id);

                // Assert
                var userRoleId = await dbContext.Roles
                    .Where(r => r.Name == Role.User)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                var adminInAdminRole = await dbContext.UserRoles
                    .Where(ur => ur.UserId == user.Id && ur.RoleId == userRoleId)
                    .FirstOrDefaultAsync();

                Assert.NotNull(adminInAdminRole);
            }
        }
    }
}