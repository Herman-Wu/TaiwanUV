using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win10DemoApp01.Models;
using Win10DemoApp01.Services;

namespace Win10DemoApp01.ViewModels
{
    public class UVMapViewModel
    {
        public ObservableCollection<TaiwanCityUV> _taiwanUVData;

        public ObservableCollection<TaiwanCityUV> TaiwanUVData
        {
            get { return _taiwanUVData; }
            set
            {
                if (Equals(_taiwanUVData, value)) return;
                _taiwanUVData = value;
                NotifyPropertyChanged("TaiwanUVData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task LoadData()
        {
            TaiwanUVOpenDataService tUV = new TaiwanUVOpenDataService();
            TaiwanUVData = new ObservableCollection<TaiwanCityUV>(await tUV.GetTaiwanUVData());
        }

    }
}
