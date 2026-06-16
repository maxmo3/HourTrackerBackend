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

        [HttpPost("{projectId}/notes")]
        public ActionResult PostNote(int projectId, [FromBody] ProjectNoteMessage msg) =>
            Ok(_projectHelper.AddNote(projectId, msg));

        [HttpPut("{projectId}/notes/{noteId}")]
        public ActionResult PutNote(int projectId, int noteId, [FromBody] ProjectNoteMessage msg) =>
            Ok(_projectHelper.UpdateNote(projectId, noteId, msg));

        [HttpDelete("{projectId}/notes/{noteId}")]
        public ActionResult DeleteNote(int projectId, int noteId) {
            _projectHelper.DeleteNote(projectId, noteId);
            return Ok();
        }
    }
}