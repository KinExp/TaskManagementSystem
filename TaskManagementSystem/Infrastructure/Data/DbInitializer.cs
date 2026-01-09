using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            var user = new User(
                email: "test@example.com",
                passwordHash: "TEST_HASH");

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
