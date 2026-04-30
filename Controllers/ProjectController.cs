using HourTrackerBackend.Helpers;
using HourTrackerBackend.Modals.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HourTrackerBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : BaseController
    {
        private readonly ProjectHelper _projectHelper;
        public ProjectController(IHttpContextAccessor ctx, ProjectHelper projectHelper) : base(ctx)
        {
            _projectHelper = projectHelper;
        }

        [HttpGet]
        public ActionResult Get() =>
            Ok(_projectHelper.GetProjects());

        [HttpPost]
        public ActionResult Post([FromBody] ProjectMessage project) =>
            Ok(_projectHelper.AddProject(project));


        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ProjectMessage project) =>
            Ok(_projectHelper.UpdateProject(project, id));

        [HttpPut("{id}/extras")]
        public ActionResult PutExtras(int id, [FromBody] ProjectExtrasMessage extras) {
            _projectHelper.UpdateExtras(id, extras);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id) {
            _projectHelper.RemoveProject(id);
            return Ok();
        }

        [HttpPost("{projectId}/types")]
        public ActionResult PostType(int projectId, [FromBody] ProjectTypeMessage msg) =>
            Ok(_projectHelper.AddProjectType(projectId, msg));

        [HttpPut("{projectId}/types/{typeId}")]
        public ActionResult PutType(int projectId, int typeId, [FromBody] ProjectTypeMessage msg) {
            _projectHelper.UpdateProjectType(projectId, typeId, msg);
            return Ok();
        }

        [HttpDelete("{projectId}/types/{typeId}")]
        public ActionResult DeleteType(int projectId, int typeId) {
            _projectHelper.RemoveProjectType(projectId, typeId);
            return Ok();
        }
    }
}