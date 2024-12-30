using Datasync.Core;
using DataSync.Infrastructure;
using DotNetEnv;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.AddSingleton<UserProjector>();
builder.Services.AddSingleton<SupplierCompanyProjector>();
builder.Services.AddScoped<IProjector, UserProjector>();
builder.Services.AddScoped<IProjector, SupplierCompanyProjector>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Datasync API", Version = "v1" });
});

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<DatasyncController>();
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI")!), h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")!);
            h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!);
        });

        configurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Datasync v1");
    c.RoutePrefix = string.Empty;
});

app.MapGet("api/datasync/health", () => Results.Ok("ok"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();