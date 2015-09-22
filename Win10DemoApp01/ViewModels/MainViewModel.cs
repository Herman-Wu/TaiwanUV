using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Win10DemoApp01.Models;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

using Win10DemoApp01.Views;

namespace Win10DemoApp01.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        const string TaiwanUVOpenDataUrl = @"http://opendata.epa.gov.tw/ws/Data/UV/?format=json";
        const string TaiwanAirConditionOpenDataUrl = @"http://opendata.epa.gov.tw/ws/Data/AQX/?format=json";

        // Variables
        private bool _isPaneOpen;
        private ObservableCollection<MenuItem> _menuItems;
        private MenuItem _selectedMenuItem;

        private Frame appFrame;
        private Frame splitViewFrame;
        private bool isBusy;



        public Frame AppFrame
        {
            get { return appFrame; }
        }
        public Frame SplitViewFrame
        {
            get { return splitViewFrame; }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }


        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set
            {
                _menuItems = value;
                NotifyPropertyChanged("MenuItems");
            }
        }

        public MenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;
                NotifyPropertyChanged("SelectedMenuItem");

                Navigate(_selectedMenuItem.View);
            }
        }


        public MainViewModel()
        {
            this.TaiwanUVData = new ObservableCollection<TaiwanCityUV>();
        }


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

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public bool IsPaneOpen
        {
            get
            {
                return _isPaneOpen;
            }
            set
            {
                _isPaneOpen = value;
                NotifyPropertyChanged("IsPaneOpen");
            }
        }




        private void Navigate(Type view)
        {
            var type = view.Name;

            switch (type)
            {
                case "UVMapView":
                    SplitViewFrame.Navigate(view);
                    break;
                case "UVDetailView":
                    SplitViewFrame.Navigate(view);
                    break;
                case "AboutView":
                    SplitViewFrame.Navigate(view);
                    break;
            }
        }


        //public ICommand HamburgerCommand
        //{
        //    get { return _hamburgerCommand = _hamburgerCommand ?? new DelegateCommand(HamburgerCommandExecute); }
        //}


        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public async void  LoadData()
        {
            if (this.IsDataLoaded == false)
            {
                this.TaiwanUVData.Clear();
                //this.Items.Add(new ItemViewModel() { ID = "0", LineOne = "Please Wait...", LineTwo = "Please wait while the catalog is downloaded from the server.", LineThree = null });

                HttpClient httpClient = new HttpClient();
                try
                {
                    string content = await httpClient.GetStringAsync(new Uri(TaiwanUVOpenDataUrl));


                    //content=content.Replace("\\","");
                    RawContent = content;
            
                    JsonArray jArray = JsonArray.Parse(content);
                    IJsonValue outValue;

                    string testContent = "";
                    foreach (var item in jArray)
                    {
                        JsonObject obj = item.GetObject();
                        // Assume there is a “backgroundImage” column coming back
                        if (obj.TryGetValue("SiteName", out outValue))
                        {
                            testContent += outValue.GetString() + " ";
                           }

                    }
                    RawContent = testContent;

                    DeserializeTaiwanUVJason(content);

                    RawContent = "There are "+TaiwanUVData.Count+" " +RawContent;
                    //JsonObject jo= await Task.Run(() => JsonObject.Parse(content));

                    //if (jo != null)
                    //{
                    //    int a = jo.Count;
                    //}


                }
                catch
                {
                    // Details in ex.Message and ex.HResult.       
                }

                // Once your app is done using the HttpClient object call dispose to 
                // free up system resources (the underlying socket and memory used for the object)
                httpClient.Dispose();

            }
        }


        public void DeserializeTaiwanUVJason(string inTaiwanUVJsonContent)
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IEnumerable<TaiwanCityUV>));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(inTaiwanUVJsonContent)))
            {
                var rootObject = serializer.ReadObject(stream) as IEnumerable<TaiwanCityUV>;
                TaiwanUVData = new ObservableCollection<TaiwanCityUV>(rootObject);
            }
        }



        private string _rawContent;
        public string RawContent
        {
            get
            {
                if (_rawContent == null) _rawContent = "";
          
                return _rawContent;
            }
            set
            {
                if (value != _rawContent)
                {
                    _rawContent = value;
                    NotifyPropertyChanged("RawContent");
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

        public void InitialMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    //Icon = "",
                    Icon ="",
                    Title = "地圖瀏覽模式",
                    View = typeof(UVMapView)
                },
                new MenuItem
                {
                    Icon = "",
                    Title = "列表瀏覽模式",
                    View = typeof(UVDetailView)
                },
                new MenuItem
                {
                    Icon = "",
                    Title = "關於紫外線指數",
                    View = typeof(AboutView)
                }
            };

            SelectedMenuItem = MenuItems.FirstOrDefault();

        }

        internal void SetAppFrame(Frame viewFrame)
        {
            appFrame = viewFrame;
        }

        internal void SetSplitFrame(Frame viewFrame)
        {
            splitViewFrame = viewFrame;
        }
    }
}
