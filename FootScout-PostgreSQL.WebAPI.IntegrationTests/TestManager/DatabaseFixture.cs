using FootScout_PostgreSQL.WebAPI.DbManager;

namespace FootScout_PostgreSQL.WebAPI.IntegrationTests.TestManager
{
    public class DatabaseFixture : IDisposable
    {
        public AppDbContext DbContext { get; private set; }
        public string UserTokenJWT { get; private set; }
        public string AdminTokenJWT { get; private set; }

        public DatabaseFixture()
        {
            var dbName = $"FootScoutTests_{Guid.NewGuid()}";
            var options = TestBase.GetDbContextOptions($"Host=localhost;Database={dbName};Username=postgres;Password=postgres;Persist Security Info=True");
            DbContext = new AppDbContext(options);

            UserTokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjZhMDY0NjVmLWNhYmItNDRkNy1hMTkyLWU4YzkxYTQyZTgwMiIsImp0aSI6IjQ2ZmQxYzRlLTY4OWItNGMxNC04MTUxLWY1NDg1Mzc1ZDM3NiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MjczNzA3NTMsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMDQiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjMwMDAifQ.t_iWpHHg3YOz_kqguAU3ss_emHtfT1ZFWbWRTCTbWu4";
            AdminTokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImIzNjA1MTA3LTRiY2MtNDhkNS05NjdlLTVlM2NkNTJiN2I2NCIsImp0aSI6ImRkNTA2ZWJhLTRlZGQtNDRlMS04ZjY3LTkzNzFhNGVjMDM1ZiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzI3MzcwNzE5LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MTA0IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDozMDAwIn0.CJLeeo2r_ITisXZ8OHmr5_dOpwAOwokeJhFaON20_JU";

            InitializeDatabase().GetAwaiter().GetResult();
        }

        private async Task InitializeDatabase()
        {
            await DbContext.Database.EnsureCreatedAsync();

            await TestBase.SeedRoleTestBase(DbContext);
            await TestBase.SeedPlayerPositionTestBase(DbContext);
            await TestBase.SeedPlayerFootTestBase(DbContext);
            await TestBase.SeedOfferStatusTestBase(DbContext);
            await TestBase.SeedUserTestBase(DbContext, TestBase.CreateUserManager(DbContext));
            await TestBase.SeedClubHistoryTestBase(DbContext);
            await TestBase.SeedProblemTestBase(DbContext);
            await TestBase.SeedChatTestBase(DbContext);
            await TestBase.SeedMessageTestBase(DbContext);
            await TestBase.SeedPlayerAdvertisementTestBase(DbContext);
            await TestBase.SeedClubOfferTestBase(DbContext);
            await TestBase.SeedClubAdvertisementTestBase(DbContext);
            await TestBase.SeedPlayerOfferTestBase(DbContext);
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}
