const string APP_TITLE = "Expense Tracker";


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


// Add services to the container.

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
