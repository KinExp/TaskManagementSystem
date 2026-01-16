namespace TaskManagement.Domain.Enums
{
    public enum TaskState
    {
        /// <summary>
        /// Newly created task
        /// </summary>
        New = 0,

        /// <summary>
        /// Task is currently in progress
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Task has been completed
        /// </summary>
        Completed = 2
    }
}
