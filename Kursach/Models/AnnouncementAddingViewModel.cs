using Kursach.Models.RealtyDataBase.TableModels;
using System.Collections.Generic;

namespace Kursach.Models
{
    public class AnnouncementAddingViewModel
    {
        public List<AdTypeModel> AdTypes { get; set; }
        public List<RealtyTypeModel> RealtyTypes { get; set; }

        public AnnouncementAddingViewModel(List<AdTypeModel> adTypes, List<RealtyTypeModel> realtyTypes)
        {
            AdTypes = adTypes;
            RealtyTypes = realtyTypes;
        }
    }
}
