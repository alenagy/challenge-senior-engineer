using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class RabbitMqSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly RabbitMqService _rabbitMqService;

    public RabbitMqSaveChangesInterceptor(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context == null)
        {
            throw new InvalidOperationException("The DbContext is null in the SaveChanges interceptor.");
        }

        await PublishChangesAsync(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishChangesAsync(DbContext context)
    {
        var changes = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in changes)
        {
            var message = JsonSerializer.Serialize(new
            {
                Action = entry.State.ToString(),
                Entity = entry.Entity
            });

            await _rabbitMqService.PublishMessageAsync(message);
        }
    }
}