using Kursach.Models;
using Kursach.Models.RealtyDataBase.Context;
using Kursach.Models.RealtyDataBase.TableModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


namespace Kursach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private RealtyDbContext db;
        private readonly IWebHostEnvironment _appEnv;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, RealtyDbContext _db)
        {
            db = _db;
            _logger = logger;
            _appEnv = env;
        }

        public IActionResult Index()
        {
            ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Searching()
        {
            return View(new SearchingModel(db.ad_types.ToList(), db.realty_types.ToList()));
        }

        [HttpPost]
        public IActionResult AdShow(FilterOptions options)
        {
            List<AnnouncementModel> searched_ad = new List<AnnouncementModel>();
            List<AdViewInfo> AdToView = new List<AdViewInfo>();
            foreach(var realty_type in options.ChosenTypes)
            {
                searched_ad.AddRange(db.announcements.Where(an =>
                    an.realty_type_id == realty_type
                        && an.rooms_num == options.RoomsNum
                        && (an.address.Contains(options.Address) || options.Address == null || options.Address == "")
                        && an.ad_type_id == options.Action)
                    .ToList());
            }
            foreach(var ad in searched_ad)
            {
                try
                {
                    string MainImagePath = db.ad_images.Where(im => im.ad_id == ad.id).First().path;
                    AdToView.Add(new AdViewInfo(ad.id, ad.name, MainImagePath, ad.rooms_num, ad.flour, ad.total_flours, ad.square, ad.price, ad.address));
                }
                catch (InvalidOperationException) { }
                catch (ArgumentNullException) { }
            }
            return View(AdToView);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Test()
        {
            List<AdImageModel> images = db.ad_images.Where(im => im.ad_id == 1).ToList();
            TestViewModel model = new TestViewModel(images);
            return View(model);
        }

        [HttpPost]
        public RedirectResult Upload(IFormFile FileToUpload)
        {
            int new_id = 1;
            try
            {
                new_id = db.ad_images.Max(im => im.id) + 1;
            }
            catch (InvalidOperationException) { }

            string name = "Image" + new_id.ToString() + ".jpg";

            string path = "/images/" + name;
            using (var fileStream = new FileStream(_appEnv.WebRootPath + path, FileMode.Create))
            {
                FileToUpload.CopyTo(fileStream);
            }
            AdImageModel image = new AdImageModel
            {
                id = new_id,
                ad_id = 1,
                name = name,
                path = path
            };
            db.ad_images.Add(image);
            db.SaveChanges();
            return Redirect("Test");
        }
    }
}
