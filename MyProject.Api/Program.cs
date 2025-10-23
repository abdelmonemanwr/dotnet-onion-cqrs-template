using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyProject.Api.Middleware;
using MyProject.Application;
using MyProject.Infrastructure;
using MyProject.Infrastructure.Persistence;
using System.Security.Claims;
using System.Text;

namespace MyProject.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register basic API services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Register services of (Application, Infrastructure) layers
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddHangfireServer();

            builder.Services.AddAuthorization();

            #region Configure Swagger
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.EnableAnnotations();
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "SwaggerComments.xml");
                opt.IncludeXmlComments(xmlPath);
            });
            #endregion

            #region Add Authentication
            var googleSettings = builder.Configuration.GetSection("GoogleSettings");

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var jwtKey = jwtSettings["Key"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
            }

            builder.Services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        // استخدام المتغيرات اللي قرأناها
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = googleSettings["ClientId"]!;
                options.ClientSecret = googleSettings["ClientSecret"]!;

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
                options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                // ... (Rest of claims)
                options.SaveTokens = true;
                options.CallbackPath = "/signin-google";
            });
            #endregion

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            #endregion

            #region ForwardedHeaders
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            #endregion

            var app = builder.Build();

            RunDatabaseMigrations(app);

            // 1. Custom Request Logging Middleware
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<CustomAuthenticationMiddleware>();
            
            // 2. Enable Forwarded Headers Middleware
            app.UseForwardedHeaders();

            // 3. Redirect HTTP to HTTPS if SSL is configured to enforce secure connections.
            app.UseHttpsRedirection();

            // 4. Serve static files (if any) from wwwroot folder.
            app.UseStaticFiles();

            // 5. Enable routing.
            app.UseRouting();

            // 6. Enable CORS
            app.UseCors();

            // 7. Enable Hangfire Dashboard
            app.UseHangfireDashboard("/hangfire");

            // 8. Custom Authentication Middleware
            //app.UseMiddleware<CustomAuthenticationMiddleware>();
            app.UseAuthentication();

            // 9. Enable Authorization
            app.UseAuthorization();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI V1");
                });
            }

            // 12. Map controller endpoints.
            app.MapControllers();
            app.MapHangfireDashboard();

            app.Run();
        }

        private static void RunDatabaseMigrations(IHost app)
        {
            var connectionString = app.Services
                .GetRequiredService<IConfiguration>()
                .GetConnectionString("DefaultConnection");

            DbUp.EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader =
                DbUp.DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(
                        typeof(ApplicationDbContext).Assembly)
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Database migration failed:");
                Console.WriteLine(result.Error);
                Console.ResetColor();

                throw new Exception("Database migration failed. See console for details.");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database migration successful!");
            Console.ResetColor();
        }
    }
}
