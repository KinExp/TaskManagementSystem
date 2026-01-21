namespace TaskManagement.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public Guid? CreatedBy { get; private set; }
        public Guid? UpdatedBy { get; private set; }
        public Guid? DeletedBy { get; private set; }

        protected void SetCreated(DateTime at, Guid? by)
        {
            CreatedAt = at;
            CreatedBy = by;
        }

        protected void SetUpdated(DateTime at, Guid? by)
        {
            UpdatedAt = at;
            UpdatedBy = by;
        }

        protected void SetDeleted(DateTime at, Guid? by)
        {
            DeletedAt = at;
            DeletedBy = by;
        }

        internal void MarkCreated(DateTime at, Guid? by)
            => SetCreated(at, by);

        internal void MarkUpdated(DateTime at, Guid? by)
            => SetUpdated(at, by);

        internal void MarkDeleted(DateTime at, Guid? by)
            => SetDeleted(at, by);
    }
}
