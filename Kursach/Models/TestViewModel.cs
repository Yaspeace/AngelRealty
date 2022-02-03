using System.Collections.Generic;
using Kursach.Models.RealtyDataBase.TableModels;

namespace Kursach.Models
{
    public class TestViewModel
    {
        public List<AdImageModel> Images { get; set; }
        public TestViewModel(List<AdImageModel> images)
        {
            Images = images;
        }
    }
}
