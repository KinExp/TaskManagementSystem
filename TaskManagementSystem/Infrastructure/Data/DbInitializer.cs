using TaskManagement.Domain.Entities;
using BCrypt.Net;

namespace TaskManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("123");
            var user = new User(
                email: "test@example.com",
                passwordHash: passwordHash);

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
