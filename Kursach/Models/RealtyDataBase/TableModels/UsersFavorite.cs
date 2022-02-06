using System.ComponentModel.DataAnnotations;

namespace Kursach.Models.RealtyDataBase.TableModels
{
    public class UsersFavorite
    {
        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
        public int ad_id { get; set; }
        public UsersFavorite() { }

        public UsersFavorite(int id, int user_id, int ad_id)
        {
            this.id = id;
            this.user_id = user_id;
            this.ad_id = ad_id;
        }
    }
}
