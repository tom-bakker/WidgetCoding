using Microsoft.EntityFrameworkCore;
using WidgetCoding.Infrastructure.Data;
using WidgetCoding.Infrastructure.Seeder;

const string DefaultCorsPolicy = "default_cors";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Local");

if (string.IsNullOrWhiteSpace(connectionString)) {
    return;
}

builder.Services.AddDbContext<ApiDataContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<WidgetSeeder>();

builder.Services.AddControllers();

builder.Services.AddCors(options => {

    options.AddPolicy(DefaultCorsPolicy, policy => {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Ensure the existence of the database and tables.
using (var scope = app.Services.CreateScope()) {

    var context = scope.ServiceProvider.GetRequiredService<ApiDataContext>();
    await context.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<WidgetSeeder>();
    seeder.SeedWidgets();

}

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors(DefaultCorsPolicy);

app.UseAuthorization();

app.MapControllers();



app.MapFallbackToFile("/index.html");

app.Run();
