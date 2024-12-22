using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RabbitMqService>();

builder
    .Services.AddDbContext<TodoContext>(async (serviceProvider, opt) => {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext"));
        var rabbitMqService = serviceProvider.GetRequiredService<RabbitMqService>();
        await rabbitMqService.InitializeAsync();
        var interceptor = new RabbitMqSaveChangesInterceptor(rabbitMqService);
        opt.AddInterceptors(interceptor);
    })
    .AddEndpointsApiExplorer()
    .AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
