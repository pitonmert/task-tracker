class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: <command> [arguments]");
            return;
        }

        string command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "add":
                if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
                {
                    Console.WriteLine("Description required.");
                    return;
                }

                TaskManager.Add(args[1]);
                break;

            case "list":
                string? statusFilter = args.Length > 1 ? args[1] : null;
                TaskManager.List(statusFilter);
                break;

            case "update":
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: update <id> \"<description>\"");
                    return;
                }

                if (!int.TryParse(args[1], out int updateId) || updateId <= 0)
                {
                    Console.WriteLine("ID must be a positive integer.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(args[2]))
                {
                    Console.WriteLine("Description required.");
                    return;
                }

                TaskManager.Update(updateId, args[2]);
                break;

            case "delete":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: delete <id>");
                    return;
                }

                if (!int.TryParse(args[1], out int deleteId) || deleteId <= 0)
                {
                    Console.WriteLine("ID must be a positive integer.");
                    return;
                }

                TaskManager.Delete(deleteId);
                break;

            case "mark-in-progress":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: mark-in-progress <id>");
                    return;
                }

                if (!int.TryParse(args[1], out int inProgressId) || inProgressId <= 0)
                {
                    Console.WriteLine("ID must be a positive integer.");
                    return;
                }

                TaskManager.ChangeStatus(inProgressId, Status.InProgress);
                break;

            case "mark-done":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: mark-done <id>");
                    return;
                }

                if (!int.TryParse(args[1], out int doneId) || doneId <= 0)
                {
                    Console.WriteLine("ID must be a positive integer.");
                    return;
                }

                TaskManager.ChangeStatus(doneId, Status.Done);
                break;

            default:
                Console.WriteLine("Unknown command.");
                break;
        }
    }
}
