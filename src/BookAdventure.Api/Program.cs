using System.Text;
using BookAdventure.Api.Filters;
using BookAdventure.Dto.Response;
using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Persistence.Seeders;
using BookAdventure.Services.Profiles;
using BookAdventure.Services.Interfaces;
using BookAdventure.Services.Implementation;
using BookAdventure.Repositories.Interfaces;
using BookAdventure.Repositories.Implementation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logPath,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

try
{
    builder.Logging.AddSerilog(logger);
    logger.Information($"LOG INITIALIZED in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NO ENV"}");
    //CORS
    var corsConfiguration = "MusicStoreCors";
    builder.Services.AddCors(setup =>
    {
        setup.AddPolicy(corsConfiguration, policy =>
        {
            policy.AllowAnyOrigin(); // Que cualquiera pueda consumir el API
            policy.AllowAnyHeader().WithExposedHeaders(new string[] { "TotalRecordsQuantity" });
            policy.AllowAnyMethod();
        });
    });

    builder.Services.AddControllers(options => { options.Filters.Add(typeof(FilterExceptions)); });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new BaseResponse
            {
                Success = false,
                ErrorMessage = string.Join("; ", errors) // Une los mensajes de error en un solo string.
            };

            return new BadRequestObjectResult(response);
        };
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "BookAdventure API", Version = "v1" });
        
        // JWT Authentication in Swagger
        c.AddSecurityDefinition("Bearer", new()
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
        });
        
        c.AddSecurityRequirement(new()
        {
            {
                new()
                {
                    Reference = new()
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Configuring context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    builder.Services.Configure<AppSettings>(builder.Configuration);

    // Registering the BookAdventure Repositories
    builder.Services.AddScoped<IBookRepository, BookRepository>();
    builder.Services.AddScoped<IGenreRepository, GenreRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IRentalOrderRepository, RentalOrderRepository>();
    builder.Services.AddScoped<IRentalOrderDetailRepository, RentalOrderDetailRepository>();

    // Registering the BookAdventure Services
    builder.Services.AddScoped<IBookService, BookService>();
    builder.Services.AddScoped<IGenreService, GenreService>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IRentalOrderService, RentalOrderService>();
    builder.Services.AddScoped<IRentalOrderDetailService, RentalOrderDetailService>();
    builder.Services.AddScoped<IRentalQueryService, RentalQueryService>();
    builder.Services.AddScoped<IUserService, UserService>();

    // Registering Seeders
    builder.Services.AddScoped<UserDataSeeder>();
    builder.Services.AddScoped<GenreSeeder>();
    builder.Services.AddScoped<BookSeeder>();
    builder.Services.AddScoped<CustomerSeeder>();
    builder.Services.AddScoped<RentalOrderSeeder>();
    builder.Services.AddScoped<MasterSeeder>();
    builder.Services.AddHealthChecks()
        .AddCheck("selfcheck", () => HealthCheckResult.Healthy())
        .AddDbContextCheck<ApplicationDbContext>();

    // builder.Services.AddScoped<UserDataSeeder>();
    // builder.Services.AddScoped<GenreSeeder>();

    builder.Services.AddHttpContextAccessor();

    //Identity
    builder.Services.AddIdentity<BookAdventureUserIdentity, IdentityRole>(policies =>
        {
            policies.Password.RequireDigit = true;
            policies.Password.RequiredLength = 6;
            policies.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    //JWT Authentication
    var jwtKey = builder.Configuration["JWT:JWTKey"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("JWT:JWTKey is not configured in appsettings.json");
    }

    var jwtLifetimeConfig = builder.Configuration["JWT:LifetimeInSeconds"];
    if (string.IsNullOrEmpty(jwtLifetimeConfig) || !int.TryParse(jwtLifetimeConfig, out var jwtLifetime))
    {
        throw new InvalidOperationException("JWT:LifetimeInSeconds is not configured or invalid in appsettings.json");
    }

    logger.Information("JWT Configuration validated successfully");

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        var key = Encoding.UTF8.GetBytes(jwtKey);
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Not using issuer validation for simplicity
            ValidateAudience = false, // Not using audience validation for simplicity
            ValidateLifetime = true, // Always validate token expiration
            ValidateIssuerSigningKey = true, // Always validate signing key
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // No tolerance for clock differences
        };
        
        // Optional: Add events for debugging JWT issues
        x.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                logger.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                logger.Information("JWT Token validated successfully for user: {User}", 
                    context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        // Add custom authorization policies if needed
        options.AddPolicy("RequireAdminRole", policy =>
            policy.RequireRole("Admin"));
        
        options.AddPolicy("RequireUserRole", policy =>
            policy.RequireRole("User"));
    });

    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<BookProfile>();
        config.AddProfile<GenreProfile>();
        config.AddProfile<CustomerProfile>();
        config.AddProfile<RentalOrderProfile>();
        config.AddProfile<RentalOrderDetailProfile>();
    });

    var app = builder.Build();

    // Applying migrations and seeding data
    await ApplyMigrationsAndSeedDataAsync(app);

    //if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors(corsConfiguration);
    // app.MapReports();
    // app.MapHomeEndpoints();
    app.MapControllers();

    //Configuring health checks
    app.MapHealthChecks("/healthcheck", new()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "An unhandled exception occurred during the API initialization.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static async Task ApplyMigrationsAndSeedDataAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Only apply migrations if we have pending ones and not running in migrations context
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
            await dbContext.Database.MigrateAsync();
        }

        // Only seed data if not running migrations command
        var args = Environment.GetCommandLineArgs();
        var isMigrationCommand = args.Any(arg => arg.Contains("migrations") || arg.Contains("database"));
        
        if (!isMigrationCommand)
        {
            var masterSeeder = scope.ServiceProvider.GetRequiredService<MasterSeeder>();
            await masterSeeder.SeedAllAsync();
            Console.WriteLine("Data seeding completed successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during migration/seeding: {ex.Message}");
        // Don't throw here to allow the app to continue
    }
}