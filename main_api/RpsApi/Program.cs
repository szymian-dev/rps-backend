using Microsoft.EntityFrameworkCore;
using RpsApi.Database;
using RpsApi.Models.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("RpsDatabaseConnection")));

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors config
const string allowedOriginsPolicy = "_allowedOriginsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOriginsPolicy,
        policy =>
        {
            policy.AllowAnyOrigin() // TODO-maybe: Change to AllowSpecificOrigins
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandler>();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
