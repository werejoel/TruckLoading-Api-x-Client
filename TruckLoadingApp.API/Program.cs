using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using TruckLoadingApp.API.Configuration;
using TruckLoadingApp.API.Services;
using TruckLoadingApp.Application.Services;
using TruckLoadingApp.Application.Services.Administration;
using TruckLoadingApp.Application.Services.Administration.Interfaces;
using TruckLoadingApp.Application.Services.Authentication;
using TruckLoadingApp.Application.Services.Authentication.Interfaces;
using TruckLoadingApp.Application.Services.DriverManagement;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Prevents automatic 400 responses
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("TruckLoadingApp.Infrastructure").UseNetTopologySuite()
    )
);

// Add rate limiting
builder.Services.AddCustomRateLimiting(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// Redis Configuration
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

// Add SignalR Service
builder.Services.AddSignalR();

// Identity Configuration
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication Configuration
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        RequireExpirationTime = true,
        ValidateLifetime = true,
        RoleClaimType = ClaimTypes.Role
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError($"Authentication failed: {context.Exception}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5094",
            "https://localhost:5049",
            "http://localhost:7094"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Register Services
builder.Services.AddScoped<IUserActivityService, UserActivityService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<TruckLocationService>();
builder.Services.AddSingleton<ITruckLocationService, TruckLocationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IUserLocationService, UserLocationService>();
builder.Services.AddScoped<ITruckService, TruckService>();
builder.Services.AddScoped<ITruckTypeService, TruckTypeService>();
builder.Services.AddScoped<ITruckHistoryService, TruckHistoryService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<ILoadService, LoadService>();
builder.Services.AddScoped<ILoadTagService, LoadTagService>();
builder.Services.AddScoped<ILoadTemperatureService, LoadTemperatureService>();
builder.Services.AddScoped<IDriverPerformanceService, DriverPerformanceService>();
builder.Services.AddScoped<IDriverDocumentService, DriverDocumentService>();

// Add Authentication Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add ComplianceCheckerService first to break circular dependency
builder.Services.AddScoped<ComplianceCheckerService>();
// Then register services that depend on it
builder.Services.AddScoped<IDriverComplianceService, DriverComplianceService>();
builder.Services.AddScoped<IDriverPayrollService, DriverPayrollService>();
builder.Services.AddScoped<IDriverRoutePreferenceService, DriverRoutePreferenceService>();
builder.Services.AddScoped<IDriverScheduleService, DriverScheduleService>();

// Authorization Configuration
builder.Services.AddAuthorization();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/unauthorized";
    options.AccessDeniedPath = "/api/auth/forbidden";
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Truck Loading API",
        Description = "API for Truck Loading Application"
    });

    // Configure Swagger to handle file uploads
    c.OperationFilter<SwaggerFileOperationFilter>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer"
            },
            new string[] {}
        }
    });
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed Database on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedDatabaseAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Middleware Configuration
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter(); // Add rate limiting middleware
app.MapControllers();

await app.RunAsync();

// Database Seeder Function
async Task SeedDatabaseAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    await context.Database.MigrateAsync();

    string[] roleNames = { "Admin", "Trucker", "Shipper", "Company" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    string adminEmail = "admin@truckloadingapp.com";
    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        var adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            UserType = UserType.Admin,
            CreatedDate = DateTime.UtcNow
        };

        var adminUserResult = await userManager.CreateAsync(adminUser, "P@$$wOrd123!");
        if (adminUserResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            foreach (var error in adminUserResult.Errors)
            {
                Console.WriteLine(error.Description);
            }
        }
    }
}