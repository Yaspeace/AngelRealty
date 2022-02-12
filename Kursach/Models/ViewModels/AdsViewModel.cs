using System.Collections.Generic;

namespace Kursach.Models
{
    public class AdsViewModel
    {
        public string Title { get; set; } = "Default";
        public List<AdViewInfo> Ads { get; set; }
        public bool ShowHearts { get; set; } = false;
        public bool ShowViews { get; set; } = false;
        public bool EnableCrud { get; set; } = false;

        public AdsViewModel(string title, List<AdViewInfo> ads, bool showHearts = false, bool showViews = false, bool enableCrud = false)
        {
            Title = title;
            Ads = ads;
            ShowHearts = showHearts;
            ShowViews = showViews;
            EnableCrud = enableCrud;
        }
    }
}
