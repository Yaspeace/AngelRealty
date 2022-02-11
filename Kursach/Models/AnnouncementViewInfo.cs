using System.Collections.Generic;
using Kursach.Models.RealtyDataBase.TableModels;

namespace Kursach.Models
{
    public class AnnouncementViewInfo : AdViewInfo
    {
        public List<AdImageModel> Images { get; set; }
        public string Description { get; set; }
        public AnnouncementViewInfo(int id, string name, string imagePath, int roomsNum, int? flour, int totalFlours, double square, decimal price, string address, string realtyType, bool isFavorite, int viewsNum, List<AdImageModel> images, string desc)
            : base(id, name, imagePath, roomsNum, flour, totalFlours, square, price, address, realtyType, isFavorite, viewsNum)
        {
            Images = images;
            Description = desc;
        }
    }
}
