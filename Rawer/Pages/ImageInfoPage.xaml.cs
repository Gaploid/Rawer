using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Globalization;

namespace Rawer
{
    public partial class ImageInfoPage : PhoneApplicationPage
    {
        public ImageInfoPage()
        {
            InitializeComponent();
            FillProperties((App.Current as App).BundleImage);
            this.Loaded += ImageInfoPage_Loaded;
        }

        void ImageInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("ImageInfoPage");
            
            chart.Series[2].ItemsSource = (App.Current as App).BundleImage.histogramData.Red;
            chart.Series[1].ItemsSource = (App.Current as App).BundleImage.histogramData.Green;
            chart.Series[0].ItemsSource = (App.Current as App).BundleImage.histogramData.Blue;
            //throw new NotImplementedException();
        }

        private void FillProperties(Models.BundleImage bundleImage)
        {
            Models.PropertyModelList list = new Models.PropertyModelList();
            list.LoadPropertieList(bundleImage);
            PropertieLongList.DataContext = list;
            
            PropertieLongList.UpdateLayout();

            //PropertieLongList.LayoutUpdated += PropertieLongList_LayoutUpdated;
            //PropertieLongList.Tap += PropertieLongList_Tap;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            MapsTask task = new MapsTask();
            task.Center = new System.Device.Location.GeoCoordinate((App.Current as App).BundleImage.Exif.gps_latitude,
                                                                        (App.Current as App).BundleImage.Exif.gps_longtitude);
            task.SearchTerm = (App.Current as App).BundleImage.Exif.gps_latitude.ToString().Replace(",", ".") + ", " + (App.Current as App).BundleImage.Exif.gps_longtitude.ToString().Replace(",", "."); 
            task.ZoomLevel = 18;
            task.Show();
           

        }

        //void PropertieLongList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        //void PropertieLongList_LayoutUpdated(object sender, EventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}
    }
}