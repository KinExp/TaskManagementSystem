using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Api.Authorization
{
    public class TaskAccessHandler
        : AuthorizationHandler<TaskAccessRequirement, TaskItem>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TaskAccessRequirement requirement,
            TaskItem resource)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId, out var parsetUserId)
                && resource.UserId == parsetUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
