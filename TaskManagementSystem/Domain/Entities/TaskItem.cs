using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }

        public TaskState State { get; private set; } = TaskState.New;
        public TaskPriority Priority { get; private set; } = TaskPriority.Medium;

        public DateTime? Deadline { get; private set; }

        public Guid UserId { get; private set; }

        protected TaskItem() { }

        public TaskItem(
            string title,
            Guid userId,
            string? description = null,
            TaskPriority priority = TaskPriority.Medium,
            DateTime? deadline = null)
        {
            Title = title;
            UserId = userId;
            Description = description;
            Priority = priority;
            Deadline = deadline;
        }

        public void MarkInProgress()
        {
            State = TaskState.InProgress;
        }

        public void MarkCompleted()
        {
            State = TaskState.Completed;
        }
    }
}
