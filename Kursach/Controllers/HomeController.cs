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
            foreach (var realty_type in options.ChosenTypes)
            {
                searched_ad.AddRange(db.announcements.Where(an =>
                    an.realty_type_id == realty_type
                        && an.rooms_num == options.RoomsNum
                        && (an.address.Contains(options.Address) || options.Address == null || options.Address == "") //Применение фильтров района поиска, если таковые есть
                        && an.ad_type_id == options.Action)
                    .ToList());
            }
            AdsViewModel model = new AdsViewModel("Объявления по запросу", DataSelectionToViewInfo(searched_ad), true);
            return Ads(model);
        }

        [Authorize]
        public IActionResult UserLK()
        {
            return View(db.users.Where(u => u.email == HttpContext.User.Identity.Name).First());
        }

        [Authorize]
        public IActionResult UsersAd()
        {
            int uid = db.users.Where(u => u.email == HttpContext.User.Identity.Name).First().id;
            AdsViewModel model = new AdsViewModel("Ваши объявления", DataSelectionToViewInfo(db.announcements.Where(an => an.user_id == uid).ToList()), false, true, true);
            return Ads(model);
        }

        [Authorize]
        public IActionResult UsersFavorite()
        {
            int uid = db.users.Where(u => u.email == HttpContext.User.Identity.Name).First().id;
            List<AnnouncementModel> ads = db.users_favorites.Where(uf => uf.user_id == uid)
                .Join(
                    db.announcements,
                    uf => uf.ad_id,
                    ad => ad.id,
                    (uf, ad) => new AnnouncementModel(ad.id, ad.name, ad.description, ad.user_id, ad.realty_type_id, ad.ad_type_id, ad.rooms_num, ad.square, ad.flour, ad.total_flours, ad.price, ad.address, ad.views_num)).ToList();
            AdsViewModel model = new AdsViewModel("Ваши избранные", DataSelectionToViewInfo(ads), true);
            return Ads(model);
        }

        [HttpPost]
        [Authorize]
        public void Liking(int ad_id)
        {
            int uid = db.users.Where(u => u.email == HttpContext.User.Identity.Name).First().id;
            if(db.users_favorites.Any(uf => uf.user_id == uid && uf.ad_id == ad_id))
                RemoveFromFavorite(ad_id);
            else
                AddToFavorite(ad_id);
        }

        public void AddToFavorite(int ad_id)
        {
            int new_id = 1;
            if(db.users_favorites.Any())
                new_id = db.users_favorites.Max(uf => uf.id) + 1;
            int uid = db.users.Where(u => u.email == HttpContext.User.Identity.Name).First().id;
            db.users_favorites.Add(new UsersFavorite(new_id, uid, ad_id));
            db.SaveChanges();
        }

        public void RemoveFromFavorite(int ad_id)
        {
            int uid = db.users.Where(u => u.email == HttpContext.User.Identity.Name).First().id;
            UsersFavorite? usfav = db.users_favorites.Where(uf => uf.ad_id == ad_id && uf.user_id == uid).FirstOrDefault();
            if(usfav != null)
            {
                db.users_favorites.Remove(usfav);
                db.SaveChanges();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Test()
        {
            List<AdImageModel> images = db.ad_images.Where(im => im.ad_id == 1).ToList();
            TestViewModel model = new TestViewModel(images);
            return View(model);
        }

        private List<AdViewInfo> DataSelectionToViewInfo(List<AnnouncementModel> ads)
        {
            List<AdViewInfo> result = new List<AdViewInfo>();
            foreach (var ad in ads)
            {
                try
                {
                    string MainImagePath = db.ad_images.Where(im => im.ad_id == ad.id).First().path;
                    string RealtType = db.realty_types.Find(ad.realty_type_id).name;
                    bool isFavorite = false;
                    if (HttpContext.User.Identity.IsAuthenticated)
                    {
                        UserModel user = db.users.Where(u => u.email == HttpContext.User.Identity.Name).FirstOrDefault();
                        if (user != null)
                            isFavorite = db.users_favorites.Any(uf => uf.user_id == user.id && uf.ad_id == ad.id);
                    }
                    result.Add(new AdViewInfo(ad.id, ad.name, MainImagePath, ad.rooms_num, ad.flour, ad.total_flours, ad.square, ad.price, ad.address, RealtType, isFavorite, ad.views_num));
                }
                catch (InvalidOperationException) { _logger.LogError($"Cant load image for announcement with id {ad.id} (InvalidOperation)"); }
                catch (ArgumentNullException) { _logger.LogError($"Cant load image for announcement with id {ad.id} (ArgumentNull)"); }
            }
            return result;
        }

        private IActionResult Ads(AdsViewModel model)
        {
            return View("Ads", model);
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
