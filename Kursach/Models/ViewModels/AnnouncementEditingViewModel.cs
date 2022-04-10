using Kursach.Models.RealtyDataBase.TableModels;
using System.Collections.Generic;

namespace Kursach.Models
{
    public class AnnouncementEditingViewModel : AnnouncementAddingViewModel
    {
        public AddingAnnouncementForm AdInfo { get; set; }
        public List<AdImageModel> AdImages { get; set; }
        public AnnouncementEditingViewModel(List<AdTypeModel> adTypes, List<RealtyTypeModel> realtyTypes, AddingAnnouncementForm adInfo) : base(adTypes, realtyTypes)
        {
            AdInfo = adInfo;
            AdImages = new List<AdImageModel>();
        }
    }
}
