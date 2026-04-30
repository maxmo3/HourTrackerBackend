using HourTrackerBackend.Modals;
using HourTrackerBackend.Modals.Database;
using HourTrackerBackend.Modals.Request;
using Microsoft.EntityFrameworkCore;

namespace HourTrackerBackend.Helpers
{
    public class LinkHelper
    {
        private readonly TrackerContext _context;

        public LinkHelper(TrackerContext context)
        {
            _context = context;
        }

        public void RemoveLink(int linkId)
        {
            var link = _context.ProjectMecanicLinks.Where(l => l.Id == linkId).Include(l => l.WeekData).FirstOrDefault();
            if (link == null)
            {
                throw new Exception("Link not found");
            }
            _context.WeekData.RemoveRange(link.WeekData);

            _context.ProjectMecanicLinks.Remove(link);
            _context.SaveChanges();
        }

        public void AddLink(int projectId, int mechanicId)
        {
            var project = _context.Projects.Include(p => p.Links).FirstOrDefault(p => p.Id == projectId);

            if (project == null) throw new Exception("Project not found");

            var link = project.Links.FirstOrDefault(l => l.MechanicId == mechanicId);
            if (link != null) throw new Exception("Link already exists");

            project.Links.Add(new ProjectMecanicLink
            {
                ProjectId = projectId,
                MechanicId = mechanicId,
                Created = DateTime.UtcNow
            });

            _context.SaveChanges();
        }

        public List<ProjectMecanicLink> GetLinks(int projectId)
        {
            return _context.ProjectMecanicLinks.Where(l => l.ProjectId == projectId).ToList();
        }

        public void AddWeekData(int linkId, WeekDataMessage msg, int week)
        {
            var link = _context.ProjectMecanicLinks.Include(l => l.WeekData).FirstOrDefault(l => l.Id == linkId);
            if (link == null)
            {
                throw new Exception("Link not found");
            }
            var weekData = new WeekData
            {
                WeekNumber = week,
                Year = msg.Year,
                SecondsWorked = msg.SecondsWorked,
                Weight = msg.Weight,
                ProjectTypeId = msg.ProjectTypeId,
                Created = DateTime.UtcNow
            };

            link.WeekData.Add(weekData);
            _context.SaveChanges();
        }

        public void EditWeekData(int weekId, WeekDataMessage msg)
        {
            var weekData = _context.WeekData.FirstOrDefault(l => l.Id == weekId);
            if (weekData == null)
            {
                throw new Exception("Week data not found");
            }

            weekData.Weight = msg.Weight;
            weekData.SecondsWorked = msg.SecondsWorked;
            weekData.ProjectTypeId = msg.ProjectTypeId;
            _context.SaveChanges();
        }

        internal void RemoveLinks(List<ProjectMecanicLink> links)
        {
            foreach (var link in links)
            {
                var fetchedLink = _context.ProjectMecanicLinks.Include(l => l.WeekData).FirstOrDefault(l => l.Id == link.Id)!;
                _context.WeekData.RemoveRange(fetchedLink.WeekData);
            }

            _context.ProjectMecanicLinks.RemoveRange(links);
        }
    }
}
