using System.ComponentModel;

namespace WifiSyncDesktop.Model
{
    public class CopyProgressModel : INotifyPropertyChanged
    {
        string from, to, size;
        int percentage;

        public string From 
        {
            get { return from; }
            set { from = value; PropertyChanged(this, new PropertyChangedEventArgs("From")); }
        }

        public string To
        {
            get { return to; }
            set { to = value; PropertyChanged(this, new PropertyChangedEventArgs("To")); }
        }

        public string Size
        {
            get { return size; }
            set { size = value; PropertyChanged(this, new PropertyChangedEventArgs("Size")); }
        }

        public int Percentage
        {
            get { return percentage; }
            set { percentage = value; PropertyChanged(this, new PropertyChangedEventArgs("Percentage")); }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
