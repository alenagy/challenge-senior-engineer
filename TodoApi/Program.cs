using Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TodoApi.Interceptors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(serviceProvider =>
{
    var rabbitMqService = new RabbitMqService();
    Task.Run(async () => {
        await rabbitMqService.InitializeAsync();
    }).GetAwaiter().GetResult();
    return rabbitMqService;
});

builder.Services.AddDbContextWithInterceptors(serviceProvider =>
{
    var rabbitMqService = serviceProvider.GetRequiredService<RabbitMqService>();
    return new List<IInterceptor>
    {
        new RabbitMqSaveChangesInterceptor(rabbitMqService)
    };
})
.AddEndpointsApiExplorer()
.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
