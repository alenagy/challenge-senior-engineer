namespace Data.Models;

public class TodoList
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required Guid UID { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}
