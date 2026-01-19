namespace TaskManagement.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; internal set; }
        public DateTime? UpdatedAt { get; internal set; }

        public Guid? CreatedBy { get; internal set; }
        public Guid? UpdatedBy { get; internal set; }
    }
}
