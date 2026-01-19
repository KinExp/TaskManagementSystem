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
        public User User { get; private set; } = null!;

        public bool IsDeleted { get; private set; }

        protected TaskItem() { }

        public TaskItem(
            string title,
            Guid userId,
            string? description = null,
            TaskPriority priority = TaskPriority.Medium,
            DateTime? deadline = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title;
            UserId = userId;
            Description = description;
            Priority = priority;
            Deadline = deadline;
            IsDeleted = false;
        }

        public void SoftDelete()
        {
            if (IsDeleted) 
                return;

            IsDeleted = true;
        }

        public void UpdateTitle(string title) => Title = title;
        public void UpdateDescription(string? description) => Description = description;
        public void UpdatePriority(TaskPriority priority) => Priority = priority;
        public void UpdateDeadline(DateTime? deadline) => Deadline = deadline;

        public void MarkInProgress()
        {
            if (State == TaskState.Completed)
                throw new InvalidOperationException("Cannot mark a completed task as InProgress");

            State = TaskState.InProgress;
        }

        public void MarkCompleted()
        {
            if (State == TaskState.Completed)
                throw new InvalidOperationException("Task is already completed");

            State = TaskState.Completed;
        }
    }
}
