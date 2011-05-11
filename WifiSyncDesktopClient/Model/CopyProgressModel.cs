using System.ComponentModel;
using WifiSyncDesktop.Helpers;

namespace WifiSyncDesktop.Model
{
    public class CopyProgressModel : INotifyPropertyChanged
    {
        private string _from;
        public string From
        {
            get { return _from; }
            set { _from = value; OnPropertyChanged("From"); }
        }

        private string _to;
        public string To
        {
            get { return _to; }
            set { _to = value; OnPropertyChanged("To"); }
        }

        private string _size;
        public string Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged("Size"); }
        }

        private int _percentage;
        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = value; OnPropertyChanged("Percentage"); }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
