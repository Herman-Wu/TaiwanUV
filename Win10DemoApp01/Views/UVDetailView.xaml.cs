using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Win10DemoApp01.Utils;
using Win10DemoApp01.ViewModels;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win10DemoApp01.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UVDetailView : Page
    {
        UVDetailViewModel UVDetailVM = new UVDetailViewModel();
        DateTime startViewTime;

        public UVDetailView()
        {
            BasicInfo.Initialize();
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadTaiwanCityUVData();
            startViewTime = DateTime.Now;
            this.DataContext = UVDetailVM;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var telemetry = new TelemetryClient();
            PageViewTelemetry pvT = new PageViewTelemetry("ViewedUVDetailView");
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            telemetry.Context.Device.Id = BasicInfo.DeviceID;
            telemetry.Context.User.Id = BasicInfo.UserName;
            telemetry.TrackPageView(pvT);
        }

        private async void LoadTaiwanCityUVData()
        {
             await UVDetailVM.LoadData();
             gv.ItemsSource = UVDetailVM.TaiwanUVData;
        }

    }
}
