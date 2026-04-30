using HourTrackerBackend.Modals;
using HourTrackerBackend.Modals.Database;
using HourTrackerBackend.Modals.Request;
using Microsoft.EntityFrameworkCore;

namespace HourTrackerBackend.Helpers
{
    public class ProjectHelper
    {
        private readonly TrackerContext _context;
        private readonly string? _username;
        private readonly LinkHelper _linkHelper;
        private readonly TodoHelper _todoHelper;

        public ProjectHelper(TrackerContext context, LinkHelper linkHelper, IHttpContextAccessor ctx, TodoHelper todoHelper)
        {
            _context = context;
            _username = ctx.HttpContext?.User.Identity?.Name;
            _linkHelper = linkHelper;
            _todoHelper = todoHelper;
        }

        internal List<Project> GetProjects()
        {
            return _context.Projects.ToList();
        }

        internal Project AddProject(ProjectMessage project)
        {
            var newProject = new Project
            {
                Name = project.Name,
                About = project.About,
                Created = System.DateTime.UtcNow,
                CreatedByUserName = _username,
                Common = new Common(),
                EstimatedTimeInSeconds = project.EstimatedTimeInSeconds,
                MaterialsDelivered = project.MaterialsDelivered,
            };
            _context.Projects.Add(newProject);
            _context.SaveChanges();

            if (project.Types != null)
            {
                foreach (var typeName in project.Types.Where(n => !string.IsNullOrWhiteSpace(n)))
                {
                    _context.ProjectTypes.Add(new ProjectType
                    {
                        Name = typeName.Trim(),
                        ProjectId = newProject.Id,
                        Created = DateTime.UtcNow
                    });
                }
                _context.SaveChanges();
            }

            return newProject;
        }

        internal void RemoveProject(int id)
        {
            var project = _context.Projects.Include(p => p.Links).Include(x=> x.Todos).FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            _linkHelper.RemoveLinks(project.Links);

            _todoHelper.RemoveTodos(project.Todos);

            _context.Projects.Remove(project);
            _context.SaveChanges();
        }

        internal Project UpdateProject(ProjectMessage project, int id)
        {
            var dbProject = _context.Projects.Find(id);

            if (dbProject == null) throw new Exception("Project not found");

            dbProject.Name = project.Name;
            dbProject.About = project.About;
            dbProject.EstimatedTimeInSeconds = project.EstimatedTimeInSeconds;
            dbProject.MaterialsDelivered = project.MaterialsDelivered;
            _context.Projects.Update(dbProject);
            _context.SaveChanges();
            return dbProject;
        }

        internal void AddMechanic(int id, int mechanicId)
        {
            var project = _context.Projects.Include(p => p.Links).ThenInclude(x=> x.Mechanic).Include(p => p.Links).ThenInclude(x=> ((ProjectMecanicLink)x).WeekData).FirstOrDefault(p => p.Id == id);
            if (project == null) throw new Exception("Project not found");
            var link = project.Links.FirstOrDefault(l => l.MechanicId == mechanicId);
            if (link == null)
            {
                project.Links.Add(new ProjectMecanicLink
                {
                    ProjectId = id,
                    MechanicId = mechanicId,
                    WeekData = new List<WeekData>()
                });
                _context.Projects.Update(project);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Mechanic already exists");
            }
        }

        internal ProjectType AddProjectType(int projectId, ProjectTypeMessage msg)
        {
            var project = _context.Projects.Find(projectId);
            if (project == null) throw new Exception("Project not found");

            var type = new ProjectType
            {
                Name = msg.Name,
                ProjectId = projectId,
                Created = DateTime.UtcNow
            };
            _context.ProjectTypes.Add(type);
            _context.SaveChanges();
            return type;
        }

        internal void RemoveProjectType(int projectId, int typeId)
        {
            var type = _context.ProjectTypes.FirstOrDefault(t => t.Id == typeId && t.ProjectId == projectId);
            if (type == null) throw new Exception("Project type not found");
            _context.ProjectTypes.Remove(type);
            _context.SaveChanges();
        }

        internal void UpdateExtras(int id, ProjectExtrasMessage msg)
        {
            var project = _context.Projects.Find(id);
            if (project == null) throw new Exception("Project not found");

            project.MeerwerkSeconds = msg.MeerwerkSeconds;
            project.DhzSeconds = msg.DhzSeconds;
            _context.SaveChanges();
        }

        internal void RemoveMechanic(int id, int mechanicId)
        {
            var project = _context.Projects.Include(p => p.Links).FirstOrDefault(p => p.Id == id);

            if (project == null) throw new Exception("Project not found");

            var link = project.Links.FirstOrDefault(m => m.MechanicId == mechanicId);
            if (link != null)
            {
                _context.ProjectMecanicLinks.Remove(link);

                _context.Projects.Update(project);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Mechanic does not exist");
            }
        }
    }
}
