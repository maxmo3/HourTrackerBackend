using HourTrackerBackend.Helpers;
using HourTrackerBackend.Modals.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HourTrackerBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("links")]
    public class LinkController : BaseController
    {
        private readonly LinkHelper _linkHelper;
        public LinkController(IHttpContextAccessor ctx, LinkHelper linkHelper) : base(ctx)
        {
            _linkHelper = linkHelper;
        }

        [HttpPost("{projectId}/{mechanicId}")]
        public ActionResult Post(int projectId, int mechanicId, [FromQuery] int? typeId = null) {
            _linkHelper.AddLink(projectId, mechanicId, typeId);
            return Ok();
        }

        [HttpDelete("{linkId}")]
        public ActionResult Delete(int linkId) {
            _linkHelper.RemoveLink(linkId);
            return Ok();
        }

        [HttpPost("{linkId}/week/{weekNumber}")]
        public ActionResult AddWeekData(int linkId, int weekNumber, [FromBody] WeekDataMessage weekData) {
            _linkHelper.AddWeekData(linkId, weekData, weekNumber);
            return Ok();
        }

        [HttpPut("{linkId}/week/{weekId}")]
        public ActionResult UpdateWeekData(int linkId, int weekId, [FromBody] WeekDataMessage weekData) {
            _linkHelper.EditWeekData(weekId, weekData);
            return Ok();
        }

    }
}