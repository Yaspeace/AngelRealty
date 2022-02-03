using System.ComponentModel.DataAnnotations;

namespace Kursach.Models.RealtyDataBase.TableModels
{
    public class AdImageModel
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public int ad_id { get; set; }
        public AdImageModel() { }

        public AdImageModel(int id, string name, string path, int ad_id)
        {
            this.id = id;
            this.name = name;
            this.path = path;
            this.ad_id = ad_id;
        }
    }
}
