using System.ComponentModel.DataAnnotations;

namespace Kursach.Models.RealtyDataBase.TableModels
{
    public class AdTypeModel
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public AdTypeModel() { }

        public AdTypeModel(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
