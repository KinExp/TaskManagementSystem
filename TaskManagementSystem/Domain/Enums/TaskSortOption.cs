namespace TaskManagement.Domain.Enums
{
    public enum TaskSortOption
    {
        /// <summary>
        /// Newest tasks first
        /// </summary>
        CreatedAtDesc = 0,

        /// <summary>
        /// Oldest tasks first
        /// </summary>
        CreatedAtAsc = 1,

        /// <summary>
        /// Priority descending
        /// </summary>
        PriorityDesc = 2,

        /// <summary>
        /// Priority ascending
        /// </summary>
        PriorityAsc = 3
    }
}
