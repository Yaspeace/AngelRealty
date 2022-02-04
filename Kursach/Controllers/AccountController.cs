using Microsoft.AspNetCore.Mvc;
using Kursach.Models.RealtyDataBase.Context;
using Kursach.Models.RealtyDataBase.TableModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;


namespace Kursach.Controllers
{
    public class AccountController : Controller
    {
        private RealtyDbContext db;
        public AccountController(RealtyDbContext _db)
        {
            db = _db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<RedirectResult> Login(string email, string password)
        {
            if (db.users.Any(usr => usr.email == email && usr.password == password))
            {
                var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, email) };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return Redirect("/Home/Index");
            }
            else
            {
                return Redirect("Login");
            }
        }
    }
}
