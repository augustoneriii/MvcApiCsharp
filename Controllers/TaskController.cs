public static class TaskController
{
    public static List<Task> Tasks { get; set; }

    public static void Add(Task task)
    {
        if (Tasks == null)
        {
            Tasks = new List<Task>();
        }

        Tasks.Add(task);
    }

    public static Task GetBy(int id)
    {
        return Tasks.FirstOrDefault(t => t.Id == id);
    }

    public static void Remove(Task Task)
    {
        Tasks.Remove(Task);
    }
}
