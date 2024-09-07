using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootScout_PostgreSQL.WebAPI.DbManager
{
    public static class AppSeeder
    {
        public static async Task Seed(IServiceProvider services)
        {
            using (var dbContext = services.GetRequiredService<AppDbContext>())
            {
                await SeedRoles(services);
                await SeedAdminRole(services);
                await SeedPlayerPositions(services, dbContext);
                await SeedPlayerFeet(services, dbContext);
                await SeedOfferStatuses(services, dbContext);
                await SeedUnknownUser(services);
            }
        }

        private static async Task SeedRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new List<string> { Role.Admin, Role.User };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedAdminRole(IServiceProvider services)
        {
            string adminEmail = "admin@admin.com";
            string adminPassword = "Admin1!";

            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FirstName = "Admin",
                    LastName = "Admin",
                    PhoneNumber = "000000000",
                    Location = "Admin",
                    CreationDate = DateTime.UtcNow,
                };
                await userManager.CreateAsync(admin, adminPassword);
                await userManager.AddToRoleAsync(admin, Role.Admin);
            }
        }

        private static async Task SeedOfferStatuses(IServiceProvider services, AppDbContext dbContext)
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

        private static async Task SeedPlayerPositions(IServiceProvider services, AppDbContext dbContext)
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

        private static async Task SeedPlayerFeet(IServiceProvider services, AppDbContext dbContext)
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

        private static async Task SeedUnknownUser(IServiceProvider services)
        {
            string unknownUserEmail = "unknown@unknown.com";
            string unknownUserPassword = "Unknown1!";

            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var unknownUser = await userManager.FindByEmailAsync(unknownUserEmail);
            if (unknownUser == null)
            {
                unknownUser = new User
                {
                    Email = unknownUserEmail,
                    UserName = unknownUserEmail,
                    FirstName = "Unknown",
                    LastName = "Unknown",
                    PhoneNumber = "000000000",
                    Location = "Unknown",
                    CreationDate = DateTime.UtcNow,
                };
                await userManager.CreateAsync(unknownUser, unknownUserPassword);
                await userManager.AddToRoleAsync(unknownUser, Role.User);
            }
        }
    }
}