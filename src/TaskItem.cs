class TaskItem
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

enum Status
{
    Todo,
    InProgress,
    Done,
}
