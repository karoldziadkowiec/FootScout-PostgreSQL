using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FootScout_PostgreSQL.WebAPI.DbManager;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.Constants;
using FootScout_PostgreSQL.WebAPI.Repositories.Interfaces;
using FootScout_PostgreSQL.WebAPI.Services.Interfaces;
using FootScout_PostgreSQL.WebAPI.Services.Classes;
using FootScout_PostgreSQL.WebAPI.Repositories.Classes;
using FootScout_PostgreSQL.WebAPI.HubManager;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql;

namespace FootScout_PostgreSQL.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // PostgreSQL database connection
            var connectionString = configuration.GetConnectionString("PostgreSQLConnectionString")
                ?? throw new InvalidOperationException("PostgreSQL connection string is not found!");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(dataSource);
            });

            // Identity with support for roles
            builder.Services.AddIdentity<User, IdentityRole>()
            .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Default authentication scheme
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // JWT Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });

            // Authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRights", policy =>
                    policy.RequireRole(Role.Admin));
                options.AddPolicy("UserRights", policy =>
                policy.RequireRole(Role.User));
                options.AddPolicy("AdminOrUserRights", policy =>
                    policy.RequireRole(Role.Admin, Role.User));
            });

            // Services
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICookieService, CookieService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IMessageService, MessageService>();

            // Repositories
            builder.Services.AddScoped<IPlayerPositionRepository, PlayerPositionRepository>();
            builder.Services.AddScoped<IPlayerFootRepository, PlayerFootRepository>();
            builder.Services.AddScoped<IOfferStatusRepository, OfferStatusRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IClubHistoryRepository, ClubHistoryRepository>();
            builder.Services.AddScoped<IAchievementsRepository, AchievementsRepository>();
            builder.Services.AddScoped<ISalaryRangeRepository, SalaryRangeRepository>();
            builder.Services.AddScoped<IPlayerAdvertisementRepository, PlayerAdvertisementRepository>();
            builder.Services.AddScoped<IFavoritePlayerAdvertisementRepository, FavoritePlayerAdvertisementRepository>();
            builder.Services.AddScoped<IClubOfferRepository, ClubOfferRepository>();
            builder.Services.AddScoped<IClubAdvertisementRepository, ClubAdvertisementRepository>();
            builder.Services.AddScoped<IFavoriteClubAdvertisementRepository, FavoriteClubAdvertisementRepository>();
            builder.Services.AddScoped<IPlayerOfferRepository, PlayerOfferRepository>();
            builder.Services.AddScoped<IProblemRepository, ProblemRepository>();

            // AutoMapper service
            builder.Services.AddAutoMapper(typeof(Program));

            // Password hasher
            builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

            // Accessing HttpContext property (cookies)
            builder.Services.AddHttpContextAccessor();

            // Real time chat (SignalR)
            builder.Services.AddSignalR();

            // Controller handler
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger authentication
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FootScout API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            // CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactDevClient",
                    b =>
                    {
                        b.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin => true)
                            .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Using CORS policy
            app.UseCors("AllowReactDevClient");
            app.UseHttpsRedirection();

            // Auth middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpoints
            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");

            // Seeders
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await AppSeeder.Seed(services);
            }

            app.Run();
        }
    }
}