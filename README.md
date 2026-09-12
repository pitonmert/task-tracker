# Task Tracker

A .NET 10 command-line app to manage pending, in-progress and completed tasks.

Based on [roadmap.sh Task Tracker](https://roadmap.sh/projects/task-tracker).

## Features

- Add, update and delete tasks.
- Mark tasks as in progress or done.
- List all tasks or filter by status.
- Store tasks in a local JSON file using built-in .NET APIs.

## Run

Requires the .NET 10 SDK. No external packages.

From the project directory:

```bash
dotnet run -- add "Buy groceries"
dotnet run -- list
```

To use `task-cli` on macOS/Linux, publish and create a command link once:

```bash
dotnet publish -c Release -o bin/publish
mkdir -p ~/.dotnet/tools
ln -s "$PWD/bin/publish/task-cli" ~/.dotnet/tools/task-cli
```

Ensure `~/.dotnet/tools` is on your PATH. Publish again after source changes.

## Usage

Replace `1` with the ID returned when adding a task.

```bash
task-cli add "Buy groceries"
task-cli update 1 "Buy groceries and cook dinner"
task-cli mark-in-progress 1
task-cli mark-done 1
task-cli list
task-cli list done
task-cli list todo
task-cli list in-progress
task-cli delete 1
```

List output: `1 | done | Buy groceries and cook dinner`

Commands and filters are case-insensitive. IDs must be positive integers.
Descriptions are trimmed and cannot be blank. An empty filter lists all tasks.

## Task Data

Tasks are saved to `tasks.json` in the current working directory.
The first save creates the file.

| Field         | Description                     |
| ------------- | ------------------------------- |
| `id`          | Unique task ID                  |
| `description` | Task description                |
| `status`      | `todo`, `in-progress` or `done` |
| `createdAt`   | Creation time in UTC            |
| `updatedAt`   | Last update time in UTC         |

New tasks start as `todo` with equal timestamps. Updating a description or
status changes `updatedAt` and preserves `createdAt`.

Invalid input, missing tasks, malformed JSON and file access errors produce
short messages. Failed loads cancel the operation; success is reported only
after saving completes.
