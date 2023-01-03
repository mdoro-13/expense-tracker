using Api.Infrastructure.Data;
using Api.Infrastructure.Data.Seed;
using Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

const string APP_TITLE = "Expense Tracker";
const string AUTHENTICATION_SCHEME = JwtBearerDefaults.AuthenticationScheme;

string firebaseSecureTokenURL = config.GetSection("Firebase").GetValue<string>("SecureTokenURL");
string firebaseProjectId = config.GetSection("Firebase").GetValue<string>("ProjectId");


// Add services to the container.

builder.Services
    .AddAuthentication(AUTHENTICATION_SCHEME)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{firebaseSecureTokenURL}/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{firebaseSecureTokenURL}/{firebaseProjectId}",
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };
    });

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services
    .AddFluentValidationAutoValidation(config =>
    {
        config.DisableDataAnnotationsValidation = true;
    });

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument(configure => configure.Title = APP_TITLE);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(config.GetConnectionString("ExpenseTracker")));

builder.Services.AddScoped<IExpenseManager, ExpenseManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            await context.Database.MigrateAsync();
            await CategoriesSeed.SeedCategoriesAsync(context);
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error has occurred during migration");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(config["ClientUrl"]));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
