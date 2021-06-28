using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Rawer.Resources;

namespace Rawer.Pages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
       
        public SettingsPage()
        {
            InitializeComponent();

            //Init File formats
            InitFileFormats();


            //init delete files
            InitDeleteFileTypes();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateCacheSizeinUI();
            base.OnNavigatedTo(e);
        }

        async void UpdateCacheSizeinUI() {

            TempSizeTB.Text = String.Format(new FileSizeFormatProvider(), "{0:fs}", await StorageExplorer.GetTotalLocalStorageSize());
            
        
        }

        private void InitDeleteFileTypes()
        {
            List<KeyValuePair<int, string>> deletetypes = new List<KeyValuePair<int, string>>();
            deletetypes.Add(new KeyValuePair<int, string>(0, ".DNG"));
            deletetypes.Add(new KeyValuePair<int, string>(0, ".JPEG + .DNG"));
            radListPickerDelete.ItemsSource = deletetypes;
        }

        private void InitFileFormats()
        {
            List<KeyValuePair<ImageEncoder.FileFormat, string>> formats = new List<KeyValuePair<ImageEncoder.FileFormat, string>>();

            foreach (ImageEncoder.FileFormat format in Enum.GetValues(typeof(ImageEncoder.FileFormat)))
            {
                formats.Add(new KeyValuePair<ImageEncoder.FileFormat, string>(format, format.ToString()));

            }
            this.radListPicker.ItemsSource = formats;
        }

        ~SettingsPage()
        {

            System.Diagnostics.Debug.WriteLine("SettingsPage destroed");
        
        }

        private void radListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyValuePair<ImageEncoder.FileFormat, string> f = (KeyValuePair<ImageEncoder.FileFormat, string>)e.AddedItems[0];


            if (f.Key == Rawer.ImageEncoder.FileFormat.TIFF)
            {
                jpegq.IsEnabled = false;
            }
            else {

                jpegq.IsEnabled = true;            
            }

        }

        private void radListPicker_SelectionChanged_Delete(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void ClearCache_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //string filename = "";
            //if ((App.Current as App).BundleImage != null)
            //{
            //    filename = (App.Current as App).BundleImage.PixMapFile.Name.Substring(0, (App.Current as App).BundleImage.PixMapFile.Name.Length - ".pixmap".Length);            
            //}
            PhoneHelper.ShowProgressBar(AppResources.SettingsClearCacheProgressText);
            await StorageExplorer.ClearCache();
            UpdateCacheSizeinUI();
            PhoneHelper.HideProgressBar(1500);
        }
    }
}