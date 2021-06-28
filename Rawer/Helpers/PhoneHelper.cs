using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.IO;
using System.Windows;
using Windows.Storage;
using System.Net;
using Windows.Phone.Storage.SharedAccess;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Rawer.Resources;
using Microsoft.Phone.Shell;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.ApplicationModel;
using System.Windows.Threading;

namespace Rawer
{
    public class PhoneHelper
    {
        public async static void ShareMedia(StorageFile _UnPackedfile, int w, int h)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            ShowProgressBar(AppResources.ViewerPageProgressSharingTitle);
            //var jpgfile = await ImageEncoder.EncodeToJpegFromPixMap(_UnPackedfile, w, h);

            var jpgfile = await ImageEncoder.SaveFile(_UnPackedfile, ImageEncoder.FileFormat.JPEG, w, h);
            Picture pic = GetIfFileExistInLibrary(jpgfile.Name);

            using (MediaLibrary library = new MediaLibrary())
            { 
                //Checking for existance 
                if (pic == null)
                {
                    using (Stream str = StorageExplorer.GetStreamFile(jpgfile))
                    {
                        pic = library.SavePicture(Path.GetFileName(jpgfile.Path), str);
                        await str.FlushAsync();
                        str.Close();
                        str.Dispose();
                    }
                    StorageExplorer.DeleteFileFromAppRootFolder(jpgfile.Name);
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        
            HideProgressBar(0);
            ShareMediaTask shareMediaTask = new ShareMediaTask();
            shareMediaTask.FilePath = pic.GetPath();
            shareMediaTask.Show();
            
            //!!!!!!!!We need to delete this temp jpeg file but i dont know when
            
        }

        public async static void SaveMedia(StorageFile _UnPackedfile, int w, int h)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            //ADDD PROGRESS BAR
            ShowProgressBar(AppResources.ViewerPageProgressSavingTitle);

            AppSettings settings = new AppSettings();

            //if fileformat tiff or other non standart
            if (settings.SaveFileFormatSetting == 1)
            {

                FilePickerConverter.OpenSaveDialog(_UnPackedfile,ImageEncoder.FileFormat.TIFF,w,h);
            }
            //if fileformat jpeg
            else
            {               

                //var jpgfile = await ImageEncoder.EncodeToJpegFromPixMap(_UnPackedfile, w, h);
                var jpgfile = await ImageEncoder.SaveFile(_UnPackedfile, ImageEncoder.FileFormat.JPEG, w, h);


                using (MediaLibrary library = new MediaLibrary())
                using (Stream str = StorageExplorer.GetStreamFile(jpgfile))
                {

                    library.SavePicture(Path.GetFileName(jpgfile.Path), str);
                    HideProgressBar(0);
                    MessageBox.Show(AppResources.ViewerPageSaveCompleted);

                    await str.FlushAsync();
                    str.Close();
                    str.Dispose();
                }
                StorageExplorer.DeleteFileFromAppRootFolder(jpgfile.Name);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static void ShowFeedbackEmail() {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = AppResources.AboutPageEmailSubject;
            emailComposeTask.To = AppResources.AboutPageEmail;
            emailComposeTask.Show();
        
        }


        public async static void UpgradeApp()
        {

            if (!InAppHelper.CheckUserHaveProduct())
            {
               InAppHelper.AskToBuyWithBuyScreen();
            }

        }



        public static void NavigateToAboutPage() {

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative)); 
        
        }

        public static void NavigateToUpgradePage()
        {

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/UpgradePage.xaml", UriKind.Relative));

        }


        public static void NavigateToPolicyPage()
        {

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/PolicyPage.xaml", UriKind.Relative));

        }


        public static void NavigateToMainPage()
        {

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));

        }

        public async static void NavigateToEditPage()
        {
            if (!InAppHelper.CheckUserHaveProduct())
            {
                InAppHelper.AskToBuyWithBuyScreen();
            }
            else
            {

                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/EditPage.xaml", UriKind.Relative));
            }
        }

        public static void NavigateToInfoPage()
        {

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/ImageInfoPage.xaml", UriKind.Relative));

        }

        public static void NavigateToLoadingPageFromListBox()
        {

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/LoadingPage.xaml?fromListBox", UriKind.Relative));

        }


        public static void NavigateToSettingsPage()
        {

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));

        }

        
        static Picture GetIfFileExistInLibrary(string filename){
            MediaLibrary library = new MediaLibrary();

            Picture file = null;

            foreach (var a in library.Pictures) {
               
                if (a.Name == filename) { file = a;  break; }            
            
            }

            return file;
        
        }
 
        public static List<string> GetFileExtensionsFromManifest()
        {
            List<string> wmData = new List<string>();
            System.Xml.Linq.XElement appxml = System.Xml.Linq.XElement.Load("WMAppManifest.xml");
            //var SupportedFileTypesElement = (from manifestData in appxml.Descendants("SupportedFileTypes") select manifestData).SingleOrDefault();
            var SupportedFileTypesElement = (from manifestData in appxml.Descendants("FileType") select manifestData);
            if (SupportedFileTypesElement != null)
            {
                foreach (var item in SupportedFileTypesElement)
                {
                    wmData.Add(item.Value);                
                }               
                
            }
            
            return wmData;
        }

        public static void  ShowProgressBar(string text){
            
            SystemTray.IsVisible = true;
            SystemTray.ProgressIndicator = new ProgressIndicator() { IsIndeterminate = true, IsVisible = true, Text = text };
           
        }

        public static void HideProgressBar(int delay)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(delay);
            dt.Tick += (sender, e) =>
            {
                SystemTray.IsVisible = false;
                SystemTray.ProgressIndicator = new ProgressIndicator() { IsIndeterminate = false, IsVisible = false };
                dt.Stop();
                dt = null;
            };
            dt.Start();



            //SystemTray.IsVisible = false;
            //SystemTray.ProgressIndicator = new ProgressIndicator() { IsIndeterminate = false, IsVisible = false };
        }

        public static bool ShowCustomMessageBox(string Title, string Body, string TextFirstButton, string TextSecondButton)
        {

            IAsyncResult result = Microsoft.Xna.Framework.GamerServices.Guide.BeginShowMessageBox(
                  Title,
                  Body,
                  new string[] { TextFirstButton, TextSecondButton },
                  0,
                  Microsoft.Xna.Framework.GamerServices.MessageBoxIcon.None,
                  null,
                  null);

            result.AsyncWaitHandle.WaitOne();

            int? choice = Microsoft.Xna.Framework.GamerServices.Guide.EndShowMessageBox(result);
            if (choice.HasValue)
            {
                if (choice.Value == 0)
                {
                    return true;
                }
            }

            return false;

        }

        public static string GetAppVersion()
        {


            return Package.Current.Id.Version.Major.ToString() +
                              "." + Package.Current.Id.Version.Minor.ToString() +
                              "." + Package.Current.Id.Version.Build.ToString() +
                              "." + Package.Current.Id.Version.Revision.ToString();

        }


        //private string GetDeviceID()
        //{
        //    byte[] id = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
        //    //string   deviceID = Convert.ToBase64String(id);

        //    //Microsoft.Phone.Info.DeviceExtendedProperties
        //    //IBuffer hardwareId = 

        //    HashAlgorithmProvider hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
        //    IBuffer hashed = hasher.HashData(hardwareId);

        //    string hashedString = CryptographicBuffer.EncodeToHexString(hashed);
        //    return hashedString;
        //}

    }
}
