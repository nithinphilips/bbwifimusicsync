using System.ComponentModel;
using WifiSyncDesktop.Helpers;

namespace WifiSyncDesktop.Model
{
    [NotifyPropertyChanged]
    public class CopyProgressModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Size { get; set; }
        public int Percentage { get; set; }
    }
}
