using FootballDashboardAPI.Data;
using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true; // ADD THIS
    });

// Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
}

builder.Services.AddDbContext<FootballContext>(options =>
    options.UseSqlServer(connectionString));

// ---------- PostgreSQL ----------

var postgresConnectionString =
    builder.Configuration.GetConnectionString("PostgresConnection");

if (string.IsNullOrEmpty(postgresConnectionString))
{
    throw new InvalidOperationException(
        "Connection string 'PostgresConnection' not found"
    );
}

// ── Identity DbContext (auth schema in PostgreSQL) ───────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<PostgresConnectionProvider>(
    _ => new PostgresConnectionProvider(postgresConnectionString)
);


// Register Repositories
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IReviewSkillDetailRepository, ReviewSkillDetailRepository>();
builder.Services.AddScoped<IReviewRatingRepository, ReviewRatingRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IScoutRepository, ScoutRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICompanyProfileRepository, CompanyProfileRepository>();

// Register Services
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IClubContactService, ClubContactService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IScoutService, ScoutService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// CORS for React App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",  "https://localhost:3000",
            "http://localhost:5173",  "https://localhost:5173",
            "http://localhost:8080",  "https://localhost:8080"
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("Authorization");
    });
});

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured in appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Email notification services
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddHostedService<NotificationBackgroundService>();

var app = builder.Build();

await IdentitySeedService.SeedAsync(app.Services, app.Configuration);

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<FootballContext>();
//    var scriptsPath = Path.Combine(app.Environment.ContentRootPath, "Database", "StoredProcedures");

//    if (Directory.Exists(scriptsPath))
//    {
//        var scriptFiles = Directory.GetFiles(scriptsPath, "*.sql", SearchOption.TopDirectoryOnly)
//            .OrderBy(f => f)
//            .ToList();

//        foreach (var file in scriptFiles)
//        {
//            var sql = await File.ReadAllTextAsync(file);
//            var batches = Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
//                .Where(b => !string.IsNullOrWhiteSpace(b));

//            foreach (var batch in batches)
//            {
//                await db.Database.ExecuteSqlRawAsync(batch);
//            }
//        }
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();



Console.WriteLine("Application starting...");
app.Run();
