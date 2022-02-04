using System.Collections.Generic;
using Kursach.Models.RealtyDataBase.TableModels;

namespace Kursach.Models
{
    public class SearchingModel
    {
        public List<AdTypeModel> AdTypes { get; set; }
        public List<RealtyTypeModel> RealtyTypes { get; set; }
        public SearchingModel(List<AdTypeModel> adTypes, List<RealtyTypeModel> realtyTypes)
        {
            AdTypes = adTypes;
            RealtyTypes = realtyTypes;
        }
    }
}
