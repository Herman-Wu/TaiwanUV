using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win10DemoApp01.Models
{   
    //Taiwan UV 資料格式
    public class TaiwanCityUV: INotifyPropertyChanged
    {
        public string County { get; set; }
        public string PublishAgency { get; set; }

        private string _publishTime;
        public string PublishTime
        {
            get
            {
                return _publishTime;
            }
            set
            {
                if (value != _publishTime)
                {
                    _publishTime = value;
                    NotifyPropertyChanged("PublishTime");
                }
            }
        }

        public string SiteName { get; set; }
        public string TWD97Lat { get; set; }
        public string TWD97Lon { get; set; }

        private string _uvi;
        public string UVI {
            get
            {
                return _uvi;
            }
            set
            {
                if (value != _uvi)
                {
                    _uvi = value;
                    NotifyPropertyChanged("UVI");
                }
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

    }
}
