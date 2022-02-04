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
        public async Task<IActionResult> Login(string email, string password)
        {
            if (db.users.Any(usr => usr.email == email && usr.password == password))
            {
                await Authenticate(email);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserModel user)
        {
            if (user != null && user.name != null && user.name != ""
                && user.surname != null && user.surname != ""
                && user.phone != null && user.phone != ""
                && user.email != null && user.email != ""
                && user.password != null && user.password != "")
            {
                if (!db.users.Any(u => u.email == user.email))
                {
                    user.id = db.users.Max(u => u.id) + 1;
                    db.users.Add(user);
                    db.SaveChanges();
                    await Authenticate(user.email);
                    return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Register", "Account");
            }
            else
                return RedirectToAction("Register", "Account");

        }

        public async Task Authenticate(string email)
        {
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, email) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}
