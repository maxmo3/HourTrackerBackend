using HourTrackerBackend.Helpers;
using HourTrackerBackend.Modals.Database;
using HourTrackerBackend.Modals.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HourTrackerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public ActionResult LoginCheck()
        {
            var helper = new GeneralHelper(_userManager);
            return Ok(helper.FirstLoadData(HttpContext?.User?.Identity?.Name!));
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserLogin loginInfo)
        {
            var authHelper = new AuthHelper(_userManager, _signInManager);
            await authHelper.Login(loginInfo.username, loginInfo.password);
            var helper = new GeneralHelper(_userManager);
            return Ok(helper.FirstLoadData(loginInfo.username));
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var authHelper = new AuthHelper(_userManager, _signInManager);
            await authHelper.Logout();
            return Ok();
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] UserLogin loginInfo)
        {
            try
            {
                var authHelper = new AuthHelper(_userManager, _signInManager);
                var user = await authHelper.CreateUser(loginInfo.username, loginInfo.password, loginInfo.registrationCode);

                await authHelper.Login(loginInfo.username, loginInfo.password);

                var helper = new GeneralHelper(_userManager);
                return Ok(helper.FirstLoadData(user.UserName!));
            }
            catch (Exception ex) when (ex.Message == "InvalidCode")
            {
                return BadRequest("InvalidCode");
            }
            catch (Exception ex) when (ex.Message == "Duplicate")
            {
                return Conflict("Duplicate");
            }
            catch
            {
                return BadRequest("BadRequest");
            }
        }
    }
}