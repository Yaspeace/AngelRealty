using Microsoft.AspNetCore.Http;

namespace Kursach.Models
{
    public class AddingAnnouncementForm
    {
        //public IFormFileCollection Images { get; set; }
        public int RealtyType { get; set; }
        public int AdType { get; set; }
        public string Description { get; set; }
        public double Square { get; set; }
        public int? Flour { get; set; } = null;
        public int TotalFlours { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int RoomsNum { get; set; }

        public AddingAnnouncementForm(/*IFormFileCollection images, */int realtyType, int adType, string description, double square, int totalFlours, string address, decimal price, int roomsNum, int? flour = null)
        {
            //Images = images;
            RealtyType = realtyType;
            AdType = adType;
            Description = description;
            Square = square;
            Flour = flour;
            TotalFlours = totalFlours;
            Address = address;
            Price = price;
            RoomsNum = roomsNum;
        }
    }
}
