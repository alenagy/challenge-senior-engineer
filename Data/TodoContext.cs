using Microsoft.EntityFrameworkCore;
using Data.Models;

namespace Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public DbSet<TodoList> TodoList { get; set; } = default!;

    public DbSet<TodoItem> TodoItems { get; set; } = default!;

    public DbSet<LastSyncProcess> LastSyncProcess { get; set; } = default!;
}
