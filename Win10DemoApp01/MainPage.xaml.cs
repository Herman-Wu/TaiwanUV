﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Win10DemoApp01.ViewModels;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Win10DemoApp01.Services;
using Win10DemoApp01.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10DemoApp01
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            UpdateTile();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainViewModel mvm = new MainViewModel();
            mvm.SetSplitFrame(SplitViewFrame);
            mvm.InitialMenuItems();
            rootGrid.DataContext = mvm;
        }

        private void clickBackBtn(object sender, RoutedEventArgs e)
        {

        }

        private void clickHamburgerBtn(object sender, RoutedEventArgs e)
        {

            this.mainSplitView.IsPaneOpen = !this.mainSplitView.IsPaneOpen;
        }

        private async void UpdateTile()
        {
            TaiwanUVOpenDataService twUVDataSer = new TaiwanUVOpenDataService();
            List<TaiwanCityUV> twCityUVData = await twUVDataSer.GetTaiwanUVData();

            // create the instance of Tile Updater, which enables you to change the appearance of the calling app's tile
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();

            // enables the tile to queue up to five notifications
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // get the XML content of one of the predefined tile templates, so that, you can customize it
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Block);

            if (twCityUVData.Count > 0)
            {
                System.Random rnd = new System.Random();

                TaiwanCityUV tileCityUV = twCityUVData[rnd.Next(0, twCityUVData.Count-1)];

                tileXml.GetElementsByTagName("text")[0].InnerText =" "+ tileCityUV.UVI.ToString();
                tileXml.GetElementsByTagName("text")[1].InnerText = tileCityUV.SiteName + " 紫外線";

                // Create a new tile notification. 

            }
            else
            {
                tileXml.GetElementsByTagName("text")[0].InnerText = "未知";
                tileXml.GetElementsByTagName("text")[1].InnerText ="未取得資料";


            }

            updater.Update(new TileNotification(tileXml));
        }

    }
}
