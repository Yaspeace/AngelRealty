namespace Kursach.Models
{
    public class AdViewInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int RoomsNum { get; set; }
        public int? Flour { get; set; }
        public int TotalFlours { get; set; }
        public double Square { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string RealtyType { get; set; }
        public AdViewInfo() { }

        public AdViewInfo(int id, string name, string imagePath, int roomsNum, int? flour, int totalFlours, double square, decimal price, string address, string realtyType)
        {
            Id = id;
            Name = name;
            ImagePath = imagePath;
            RoomsNum = roomsNum;
            Flour = flour;
            TotalFlours = totalFlours;
            Square = square;
            Price = price;
            Address = address;
            RealtyType = realtyType;
        }
    }
}
