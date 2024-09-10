using FootScout_PostgreSQL.WebAPI.Services.Classes;
using FootScout_PostgreSQL.WebAPI.UnitTests.TestManager;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FootScout_PostgreSQL.WebAPI.UnitTests.Services
{
    public class CookieServiceTests : TestBase
    {
        [Fact]
        public async Task SetCookies_AppendCookieWithCorrectOptions()
        {
            // Arrange
            var token = new JwtSecurityToken(
                issuer: "http://localhost",
                audience: "http://localhost",
                expires: DateTime.Now.AddDays(1),
                claims: new List<Claim>(),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test_key")),
                    SecurityAlgorithms.HmacSha256)
            );
            var tokenString = "test_token_string";

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpResponse = new Mock<HttpResponse>();
            var mockCookieCollection = new Mock<IResponseCookies>();

            // Configure mock responses
            mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpResponse.Setup(r => r.Cookies).Returns(mockCookieCollection.Object);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            var _cookieService = new CookieService(
                mockHttpContextAccessor.Object
            );

            // Act
            await _cookieService.SetCookies(token, tokenString);

            // Assert
            mockCookieCollection.Verify(
                c => c.Append(
                    "AuthToken",
                    tokenString,
                    It.Is<CookieOptions>(opt =>
                        opt.HttpOnly &&
                        opt.Secure &&
                        opt.SameSite == SameSiteMode.Strict &&
                        opt.Expires == token.ValidTo
                    )
                ),
                Times.Once
            );
        }
    }
}