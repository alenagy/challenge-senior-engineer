using Data;
using Data.Models;

namespace Data.SeedData;

public class SeedLastSyncProcess
{
    public static void Initialize(TodoContext context)
    {
        // Check if data already exists
        if (!context.LastSyncProcess.Any())
        {
            context.LastSyncProcess.Add(new LastSyncProcess
            {
                Id = 1,
                LastSyncTime = DateTimeOffset.UtcNow
            });

            context.SaveChanges();
        }
    }
}