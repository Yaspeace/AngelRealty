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
            return View("Searching", new SearchingModel(db.ad_types.ToList(), db.realty_types.ToList()));
        }

        [HttpPost]
        public IActionResult AdShow(FilterOptions options)
        {
            if (options.ChosenTypes == null)
                return Searching();
            List<AnnouncementModel> searched_ad = new List<AnnouncementModel>();
            foreach (var realty_type in options.ChosenTypes)
            {
                searched_ad.AddRange(db.announcements.Where(an =>
                    an.realty_type_id == realty_type
                        && (options.RoomsNum == 0 || an.rooms_num == options.RoomsNum)
                        && (an.address.Contains(options.Address) || options.Address == null || options.Address == "") //Применение фильтров района поиска, если таковые есть
                        && (options.Action == 0 || an.ad_type_id == options.Action))
                    .ToList());
            }
            AdsViewModel model = new AdsViewModel("Объявления по запросу", DataSelectionToViewInfo(searched_ad), true);
            return Ads(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserLK()
        {
            return View(db.users.Where(u => u.email == HttpContext.User.Identity.Name).First());
        }

        [Authorize]
        [HttpPost]
        public IActionResult UserLK(int uid, string name, string surname, string phone)
        {
            UserModel u = db.users.Find(uid);
            u.name = name;
            u.surname = surname;
            u.phone = phone;
            db.SaveChanges();
            return View(u);
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
            UsersFavorite usfav = db.users_favorites.Where(uf => uf.ad_id == ad_id && uf.user_id == uid).FirstOrDefault();
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
                    string MainImagePath = db.ad_images.Any(i => i.ad_id == ad.id) ? db.ad_images.Where(im => im.ad_id == ad.id).First().path : "";
                    string RealtType = db.realty_types.Find(ad.realty_type_id).name;
                    bool isFavorite = false;
                    UserModel ad_owner = db.users.Find(ad.user_id);
                    bool vip = ad_owner.vip ?? false;
                    if (HttpContext.User.Identity.IsAuthenticated)
                    {
                        UserModel user = db.users.Where(u => u.email == HttpContext.User.Identity.Name).FirstOrDefault();
                        if (user != null)
                        {
                            isFavorite = db.users_favorites.Any(uf => uf.user_id == user.id && uf.ad_id == ad.id);
                        }
                    }
                    result.Add(new AdViewInfo(ad.id, ad.name, MainImagePath, ad.rooms_num, ad.flour, ad.total_flours, ad.square, ad.price, ad.address, RealtType, isFavorite, ad.views_num, vip));
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

        public IActionResult Announcement(int ad_id)
        {
            AnnouncementModel ad = db.announcements.Find(ad_id);
            UserModel seller = db.users.Find(ad.user_id);
            
            if (!HttpContext.User.Identity.IsAuthenticated || seller.id != db.users.Where(u => u.email == HttpContext.User.Identity.Name).First().id)
            {
                ad.views_num++;
                db.SaveChanges();
            }
            string RealtType = db.realty_types.Find(ad.realty_type_id).name;
            AnnouncementViewInfo anInfo = new AnnouncementViewInfo(ad.id, 
                                                                    ad.name,
                                                                    db.ad_images.Where(adim => adim.ad_id == ad.id).First().path, 
                                                                    ad.rooms_num, 
                                                                    ad.flour, 
                                                                    ad.total_flours, 
                                                                    ad.square, 
                                                                    ad.price, 
                                                                    ad.address, 
                                                                    RealtType, 
                                                                    false, 
                                                                    ad.views_num, 
                                                                    db.ad_images.Where(adim => adim.ad_id == ad.id).ToList(), 
                                                                    ad.description);
            SellerViewInfo selInfo = new SellerViewInfo(seller.name, seller.surname, seller.phone, seller.email);
            return View(new AnnouncementViewModel(anInfo, selInfo));
        }

        [Authorize]
        [HttpGet]
        public IActionResult AnnouncementAdding()
        {
            return View(new AnnouncementAddingViewModel(db.ad_types.ToList(), db.realty_types.ToList()));
        }

        [Authorize]
        [HttpPost]
        public IActionResult AnnouncementAdding(AddingAnnouncementForm adToAdd)
        {
            string realtyName = db.realty_types.Find(adToAdd.RealtyType).name;
            int new_id = 1;
            if (db.announcements.Any())
                new_id = db.announcements.Max(ad => ad.id) + 1;
            AnnouncementModel ad = new AnnouncementModel
            {
                id = new_id,
                address = adToAdd.Address,
                ad_type_id = adToAdd.AdType,
                description = adToAdd.Description,
                flour = adToAdd.Flour,
                name = $"{adToAdd.RoomsNum} - комн. {realtyName.ToLower()}," + ((adToAdd.Flour == null || adToAdd.Flour < 1) ? $"этажей: {adToAdd.TotalFlours}" : $"этаж: {adToAdd.Flour}/{adToAdd.TotalFlours}"),
                price = adToAdd.Price,
                realty_type_id = adToAdd.RealtyType,
                rooms_num = adToAdd.RoomsNum,
                square = adToAdd.Square,
                total_flours = adToAdd.TotalFlours,
                user_id = db.users.Where(u => u.email == HttpContext.User.Identity.Name).First().id,
                views_num = 0
            };
            db.announcements.Add(ad);
            foreach(var img in adToAdd.Images)
                UploadFile(img, ad.id);
            db.SaveChanges();
            return UsersAd();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AnnouncementEditing(int adId)
        {
            AnnouncementModel ad = db.announcements.Find(adId);
            AddingAnnouncementForm forminfo = new AddingAnnouncementForm
            {
                Address = ad.address,
                AdType = ad.ad_type_id,
                Description = ad.description,
                Flour = ad.flour,
                Images = null,
                Price = ad.price,
                RealtyType = ad.realty_type_id,
                RoomsNum = ad.rooms_num,
                Square = ad.square,
                TotalFlours = ad.total_flours,
                Id = ad.id
            };
            AnnouncementEditingViewModel model = new AnnouncementEditingViewModel(db.ad_types.ToList(), db.realty_types.ToList(), forminfo);
            model.AdImages = db.ad_images.Where(i => i.ad_id == adId).ToList();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AnnouncementEditing(AddingAnnouncementForm adToEdit, int[] imagesToDelete)
        {
            AnnouncementModel ad = db.announcements.Find(adToEdit.Id);
            ad.address = adToEdit.Address;
            ad.description = adToEdit.Description;
            ad.ad_type_id = adToEdit.AdType;
            ad.flour = adToEdit.Flour;
            ad.total_flours = adToEdit.TotalFlours;
            ad.realty_type_id = adToEdit.RealtyType;
            ad.price = adToEdit.Price;
            ad.rooms_num = adToEdit.RoomsNum;
            ad.square = adToEdit.Square;
            ad.name = $"{adToEdit.RoomsNum} - комн. {db.realty_types.Find(ad.realty_type_id).name.ToLower()}," + ((adToEdit.Flour == null || adToEdit.Flour < 1) ? $"этажей: {adToEdit.TotalFlours}" : $"этаж: {adToEdit.Flour}/{adToEdit.TotalFlours}");
            
            for(int i = 0; i < imagesToDelete.Length; i++)
            {
                int img_id = imagesToDelete[i];
                AdImageModel img = db.ad_images.Find(img_id);
                System.IO.File.Delete(_appEnv.WebRootPath + img.path);
                db.ad_images.Remove(img);
            }
            
            foreach (var img in adToEdit.Images)
                UploadFile(img, ad.id);

            db.SaveChanges();
            return UsersAd();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AnnouncementRemove(int adId)
        {
            foreach (var image in db.ad_images.Where(im => im.ad_id == adId))
                System.IO.File.Delete(_appEnv.WebRootPath + image.path);
            db.ad_images.RemoveRange(db.ad_images.Where(im => im.ad_id == adId));
            db.announcements.Remove(db.announcements.Find(adId));
            db.SaveChanges();
            return UsersAd();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Payment()
        {
            return View();
        }
        

        [Authorize]
        [HttpPost]
        public RedirectResult PaymentProcess()
        {
            UserModel u = db.users.FirstOrDefault(u => u.email == HttpContext.User.Identity.Name);
            u.vip = true;
            db.SaveChanges();
            return Redirect("UserLK");
        }

        private void UploadFile(IFormFile FileToUpload, int adId)
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
                ad_id = adId,
                name = name,
                path = path
            };
            db.ad_images.Add(image);
            db.SaveChanges();
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
