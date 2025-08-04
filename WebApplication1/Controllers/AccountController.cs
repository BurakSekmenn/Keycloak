using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() // Bu metot kullanıcıyı kimlik doğrulama için yönlendirir"
        {
            return Challenge(new AuthenticationProperties // Burası authentication işlemini başlatır
            {
                RedirectUri = "/" //Giriş başarılı olduktan sonra yönlendirilecek URL
            }, OpenIdConnectDefaults.AuthenticationScheme); // OpenID Connect kimlik doğrulama şemasını kullanır
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties // Bu metot oturumu kapatır
            {
                RedirectUri = "/" // Giriş sonrası yönlendirilecek URL
            },
            CookieAuthenticationDefaults.AuthenticationScheme, // Cookie kimlik doğrulama şemasını kullanır
            OpenIdConnectDefaults.AuthenticationScheme); // OpenID Connect kimlik doğrulama şemasını kullanır
        }
    }
}
