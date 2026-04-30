using HourTrackerBackend.Modals.Database;
using Microsoft.AspNetCore.Identity;

namespace HourTrackerBackend.Helpers
{
    public class AuthHelper
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public AuthHelper(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        internal async Task<User> CreateUser(string username, string password, string? registrationCode)
        {
            if (registrationCode != "4338") throw new Exception("InvalidCode");
            if (username == null || password == null) throw new Exception("BadRequest");
            var u = await userManager.FindByNameAsync(username.ToLower());
            if (u != null) throw new Exception("Duplicate");

            if (!(password.Count() > 5 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit))) throw new Exception("BadRequest", new Exception("Bad password"));

            var user = new User { UserName = username.ToLower().Trim() };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                await userManager.DeleteAsync(user);
                throw new Exception("BadRequest");
            }
            return user;
        }

        internal async Task<bool> Login(string username, string password)
        {
            var signIn = await signInManager.PasswordSignInAsync(username, password, true, true);
            if (!signIn.Succeeded)
            {
                throw new Exception("unauthorized");
            }
            return signIn.Succeeded;
        }

        internal async Task Logout()
        {
            await signInManager.SignOutAsync();
        }
    }
}
