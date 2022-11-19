using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocument(configure => configure.Title = APP_TITLE);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(config["ClientUrl"]));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
