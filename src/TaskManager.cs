static class TaskManager
{
    private static readonly string _filePath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "tasks.json"
    );

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter<Status>(JsonNamingPolicy.KebabCaseLower) },
    };

    public static void Add(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description required.");
            return;
        }

        if (!TryLoadTasks(out var tasks))
        {
            return;
        }

        int taskId = tasks.Count > 0 ? tasks.Max(task => task.Id) + 1 : 1;
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var task = new TaskItem
        {
            Id = taskId,
            Description = description.Trim(),
            Status = Status.Todo,
            CreatedAt = now,
            UpdatedAt = now,
        };

        tasks.Add(task);

        if (!TrySaveTasks(tasks))
        {
            return;
        }

        Console.WriteLine($"Added task {taskId}.");
    }

    public static void List(string? statusFilter)
    {
        statusFilter = string.IsNullOrWhiteSpace(statusFilter)
            ? null
            : statusFilter.ToLowerInvariant();

        Status? status = null;
        switch (statusFilter)
        {
            case "todo":
                status = Status.Todo;
                break;

            case "in-progress":
                status = Status.InProgress;
                break;

            case "done":
                status = Status.Done;
                break;

            case null:
                break;

            default:
                Console.WriteLine("Status must be todo, in-progress or done.");
                return;
        }

        if (!TryLoadTasks(out var tasks))
        {
            return;
        }

        if (status.HasValue)
        {
            tasks = tasks.Where(task => task.Status == status.Value).ToList();
        }

        if (tasks.Count == 0)
        {
            Console.WriteLine(statusFilter is null ? "No tasks." : "No matching tasks.");
            return;
        }

        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id} | {FormatStatus(task.Status)} | {task.Description}");
        }
    }

    public static void Update(int taskId, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description required.");
            return;
        }

        if (!TryLoadTasks(out var tasks))
        {
            return;
        }

        var task = tasks.Find(task => task.Id == taskId);

        if (task is null)
        {
            Console.WriteLine($"Task {taskId} not found.");
            return;
        }

        task.Description = description.Trim();
        task.UpdatedAt = DateTimeOffset.UtcNow;

        if (!TrySaveTasks(tasks))
        {
            return;
        }

        Console.WriteLine($"Updated task {taskId}.");
    }

    public static void Delete(int taskId)
    {
        if (!TryLoadTasks(out var tasks))
        {
            return;
        }

        int removedCount = tasks.RemoveAll(task => task.Id == taskId);

        if (removedCount == 0)
        {
            Console.WriteLine($"Task {taskId} not found.");
            return;
        }

        if (!TrySaveTasks(tasks))
        {
            return;
        }

        Console.WriteLine($"Deleted task {taskId}.");
    }

    public static void ChangeStatus(int taskId, Status status)
    {
        if (!TryLoadTasks(out var tasks))
        {
            return;
        }

        var task = tasks.Find(task => task.Id == taskId);

        if (task is null)
        {
            Console.WriteLine($"Task {taskId} not found.");
            return;
        }

        task.Status = status;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        if (!TrySaveTasks(tasks))
        {
            return;
        }

        Console.WriteLine($"Task {taskId} marked {FormatStatus(status)}.");
    }

    private static string FormatStatus(Status status)
    {
        return status switch
        {
            Status.Todo => "todo",
            Status.InProgress => "in-progress",
            Status.Done => "done",
            _ => status.ToString(),
        };
    }

    private static bool TryLoadTasks(out List<TaskItem> tasks)
    {
        tasks = new List<TaskItem>();

        if (!File.Exists(_filePath))
        {
            return true;
        }

        string content;
        try
        {
            content = File.ReadAllText(_filePath);
        }
        catch (IOException)
        {
            Console.WriteLine("Cannot read tasks.json.");
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Cannot read tasks.json.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return true;
        }

        try
        {
            tasks = JsonSerializer.Deserialize<List<TaskItem>>(content, _jsonOptions)
                ?? new List<TaskItem>();
            return true;
        }
        catch (JsonException)
        {
            Console.WriteLine("Cannot read tasks.json: invalid JSON.");
            return false;
        }
    }

    private static bool TrySaveTasks(List<TaskItem> tasks)
    {
        try
        {
            var json = JsonSerializer.Serialize(tasks, _jsonOptions);
            File.WriteAllText(_filePath, json);
            return true;
        }
        catch (IOException)
        {
            Console.WriteLine("Cannot save tasks.json.");
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Cannot save tasks.json.");
            return false;
        }
    }
}
