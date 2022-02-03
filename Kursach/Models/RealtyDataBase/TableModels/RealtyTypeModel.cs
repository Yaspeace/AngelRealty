using System.ComponentModel.DataAnnotations;

namespace Kursach.Models.RealtyDataBase.TableModels
{
    public class RealtyTypeModel
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public RealtyTypeModel() { }

        public RealtyTypeModel(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
