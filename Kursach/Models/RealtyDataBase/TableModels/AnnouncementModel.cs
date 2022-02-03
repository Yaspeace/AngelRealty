using System.ComponentModel.DataAnnotations;

namespace Kursach.Models.RealtyDataBase.TableModels
{
    public class AnnouncementModel
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int user_id { get; set; }
        public int realty_type_id { get; set; }
        public int ad_type_id { get; set; }
        public int rooms_num { get; set; }
        public double square { get; set; }
        public int? flour { get; set; }
        public int total_flours { get; set; }
        public decimal price { get; set; }
        public AnnouncementModel() { }

        public AnnouncementModel(int id, string name, string description, int user_id, int realty_type_id, int ad_type_id, int rooms_num, double square, int? flour, int total_flours, decimal price)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.user_id = user_id;
            this.realty_type_id = realty_type_id;
            this.ad_type_id = ad_type_id;
            this.rooms_num = rooms_num;
            this.square = square;
            this.flour = flour;
            this.total_flours = total_flours;
            this.price = price;
        }
    }
}
