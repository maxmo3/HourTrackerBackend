using HourTrackerBackend.Modals;
using HourTrackerBackend.Modals.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HourTrackerBackend.Helpers
{
    public class GeneralHelper
    {
        private UserManager<User> _userManager;

        public GeneralHelper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        internal object FirstLoadData(string username)
        {
            using var context = new TrackerContext();
            var user = context.Users.AsNoTracking().FirstOrDefault(x => x.UserName!.ToLower() == username.ToLower());

            // AsNoTracking + separate queries so each graph has its own instances,
            // then null the deep back-reference so System.Text.Json can't walk Project↔Mechanic forever.
            var projects = context.Projects
                .AsNoTracking()
                .Include(p => p.Links).ThenInclude(l => l.Mechanic)
                .Include(p => p.Links).ThenInclude(l => l.WeekData)
                .Include(p => p.Todos)
                .Include(p => p.Common).ThenInclude(c => c.Comments)
                .ToList();

            foreach (var p in projects)
                foreach (var l in p.Links)
                    if (l.Mechanic != null)
                        l.Mechanic.Links = new List<ProjectMecanicLink>();

            var mechanics = context.Mechanics
                .AsNoTracking()
                .Include(m => m.Links).ThenInclude(l => l.Project)
                .Include(m => m.Links).ThenInclude(l => l.WeekData)
                .Include(m => m.Common).ThenInclude(c => c.Comments)
                .ToList();

            foreach (var m in mechanics)
                foreach (var l in m.Links)
                    if (l.Project != null)
                        l.Project.Links = new List<ProjectMecanicLink>();

            return new
            {
                user,
                projects,
                mechanics
            };
        }
    }
}
