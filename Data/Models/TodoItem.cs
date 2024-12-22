namespace Data.Models;

public class TodoItem
{
    public long Id { get; set; }
    public required string Description { get; set; }
    public bool IsComplete { get; set; }
    public required Guid UID { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public required TodoList List { get; set; }
}