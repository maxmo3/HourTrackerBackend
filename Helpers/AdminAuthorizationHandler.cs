using HourTrackerBackend.Modals;
using Microsoft.AspNetCore.Authorization;

namespace HourTrackerBackend.Helpers
{
    public class AdminRequirement : IAuthorizationRequirement { }

    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly TrackerContext _context;

        public AdminAuthorizationHandler(TrackerContext context)
        {
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            var username = context.User.Identity?.Name;
            if (username == null) return Task.CompletedTask;

            var user = _context.Users.FirstOrDefault(u => u.UserName!.ToLower() == username.ToLower());
            if (user?.IsAdmin == true)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
