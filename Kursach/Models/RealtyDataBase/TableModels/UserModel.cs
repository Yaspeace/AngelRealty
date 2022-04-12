using System.ComponentModel.DataAnnotations;

namespace Kursach.Models.RealtyDataBase.TableModels
{
    public class UserModel
    {
        [Key]
        public int id { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool? vip { get; set; }
    }
}
