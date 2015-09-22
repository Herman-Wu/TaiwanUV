using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Win10DemoApp01.ViewModels;
using Win10DemoApp01.Models;
using Win10DemoApp01.Utils;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.Devices.Enumeration;
using Windows.System.UserProfile;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win10DemoApp01.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UVMapView : Page
    {

        string DeviceID = "";
        string UserName = "";
        DateTime startViewTime;

        public UVMapView()
        {
            BasicInfo.Initialize();
            this.InitializeComponent();
            myMap.Center = new Geopoint(new BasicGeoposition
            {
                // Geopoint for San Francisco
                Latitude = 25.038533,
                Longitude = 121.568150,
            });
            myMap.ZoomLevel = 14;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {          
            AddUVDataPinPoint();
            startViewTime = DateTime.Now;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            TelemetryClient telemetry = new TelemetryClient();
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            PageViewTelemetry pvT = new PageViewTelemetry("ViewedMapView");

            telemetry.Context.Device.Id = BasicInfo.DeviceID;
            telemetry.Context.User.Id = BasicInfo.UserName;
            pvT.Duration = DateTime.Now.Subtract(startViewTime);
            telemetry.TrackPageView(pvT);
        }


        private async void AddUVDataPinPoint()
        {
            UVMapViewModel uvMapVM = new UVMapViewModel();
            await uvMapVM.LoadData();

            myMap.MapElements.Clear();
            myMap.Children.Clear();
            foreach (TaiwanCityUV twCityUV in uvMapVM.TaiwanUVData)
            {
                AddPushpin(new BasicGeoposition
                {
                    Longitude = GeoCoordinateTransformer.DMSToDouble(twCityUV.TWD97Lon),
                    Latitude = GeoCoordinateTransformer.DMSToDouble(twCityUV.TWD97Lat)
                }, twCityUV.UVI);
            }
        }

        public void AddPushpin(BasicGeoposition location, string inUVLeve)
        {
            int uvLevel=0;

            if(!String.IsNullOrEmpty(inUVLeve))
            {
                uvLevel =int.Parse( Math.Round(double.Parse(inUVLeve),0).ToString());
            }

            var pin = new Grid()
            {
                Width = 100,
                Height = 100,
                Margin = new Windows.UI.Xaml.Thickness(-12)
            };

#region Set Map Pin Color 
            Color uvColor;
            if (uvLevel <= 2)
            {
                uvColor = Colors.Green;
            }
            else if(uvLevel <= 5)
            {
                uvColor = Colors.Yellow;
            }
            else if (uvLevel <= 7)
            {
                uvColor = Colors.Orange;
            }
            else if (uvLevel <= 8)
            {
                uvColor = Colors.Red;
            }
            else 
            {
                uvColor = Colors.Purple;
            }
            pin.Children.Add(new Ellipse()
            {
                Fill = new SolidColorBrush(uvColor),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 3,
                Width = 100,
                Height = 100
            });

            Color uvTextColor;
            if (uvLevel <= 2)
            {
                uvTextColor = Colors.White;
            }
            else
            {
                uvTextColor = Colors.DarkBlue;
            }

                pin.Children.Add(new TextBlock()
            {
                Text = "紫外線指數 " + uvLevel.ToString(),
                FontSize = 12,
                Foreground = new SolidColorBrush(uvTextColor),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center
            });
#endregion
            MapControl.SetLocation(pin, new Geopoint(location));
            myMap.Children.Add(pin);
        }

    }
}
