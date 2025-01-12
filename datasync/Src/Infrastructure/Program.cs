using Datasync.Extensions;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.ConfigureProjectors();
builder.Services.ConfigureMassTransit();
builder.Services.ConfigureSwagger();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthorization();
app.UseSwagger();

app.MapGet("api/datasync/health", () => Results.Ok("ok"));
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();