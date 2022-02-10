namespace Kursach.Models
{
    public class AnnouncementViewModel
    {
        public AnnouncementViewInfo AdInfo { get; set; }
        public SellerViewInfo SellerInfo { get; set; }
        public AnnouncementViewModel(AnnouncementViewInfo adInfo, SellerViewInfo sellerInfo)
        {
            AdInfo = adInfo;
            SellerInfo = sellerInfo;
        }
    }
}
