using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Web253502Nikolaychik.UI.Models;
using Web253502Nikolaychik.UI.Services.Authorization;

namespace Web253502Nikolaychik.UI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View(new RegisterUserViewModel());
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel user,
        [FromServices] IAuthService authService)
        {
            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    return BadRequest();
                }

                var (Result, ErrorMessage) = await authService.RegisterUserAsync(user.Email, user.Password, user.Avatar);
                if (Result)
                {
                    return Redirect(Url.Action("Index", "Home"));
                }

                else return BadRequest(ErrorMessage);
            }
            return View(user);
        }
        
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(
                OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Index", "Home")
                }
            );
        }
        [HttpPost]
        public async Task Logout()
        {
            await
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, 
                new AuthenticationProperties 
                { 
                    RedirectUri = Url.Action("Index", "Home") 
                }
            );
        }
    }
}

