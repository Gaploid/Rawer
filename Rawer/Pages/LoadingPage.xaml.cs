using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Storage.SharedAccess;
using Windows.Storage;
using Rawer.Resources;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace Rawer
{
    public partial class LoadingPage : PhoneApplicationPage
    {
        //LibRawProcessor proc;
        bool FirstRun = true;
        LibRawProcessor proc;
        DateTime start0;
        string UserComeFrom = "";

        public LoadingPage()
        {
            InitializeComponent();
            
            this.Loaded += LoadingPage_Loaded;
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _loader.Stop();
            _loader.Visibility = System.Windows.Visibility.Collapsed;

            if (proc != null) {
                proc.Dispose();
                proc = null;            
            }

            //if we go back then we dont need this temp file
            if (e.NavigationMode == NavigationMode.Back) {

                StorageExplorer.DeleteTempPixMapFile();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            

            base.OnNavigatedFrom(e);
        }

       void LoadingPage_Loaded(object sender, RoutedEventArgs e)
       {
          
            GoogleAnalytics.EasyTracker.GetTracker().SendView("LoadingPage");
            

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

       protected async override void OnNavigatedTo(NavigationEventArgs e)
       {
        

           if (FirstRun)
           {
               BuildlocalizedApplicationBar();

               //handle if it comes from regular way (email and etc)
               await HandleFileFromUrlAssoc();

               await HandleFileFromPhotoEditor();

               //Hndl file from FileOpenPicker
               await HandleFileFromFilePicker();

               //hndl if we come from mainpage;
               await HandleFileFromListBox();

               FirstRun = false;

           }
           else
           {
               //Hndl file from editor
               await HandleFileFromEditor();

               //Hndl file from FileSavePicker
               await HandleFileFromFileSavePicker();

              
           }

            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Image", "Open", UserComeFrom, 0);

            base.OnNavigatedTo(e);
       }


        private async System.Threading.Tasks.Task HandleFileFromEditor()
       {
           try
           {
               if ((App.Current as App).BundleImage != null)
               if ((App.Current as App).BundleImage.NeedToUpdate)
               {


                   UserComeFrom = "FromEditor";

                   ApplicationBar.IsVisible = true;
                   PhoneHelper.ShowProgressBar(AppResources.ViewerPageProgressBarTextSwap);


                   var file = await StorageExplorer.GetFileInBytes((App.Current as App).BundleImage.PixMapFile);

                   ImageEncoder.ReCalculateHistogram(file, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);

                   await ImageEncoder.RenderImage3(file,
                                             img,
                                             (App.Current as App).BundleImage.Width,
                                             (App.Current as App).BundleImage.Height);

                   PhoneHelper.HideProgressBar(0);
                   //file = null;
                   img.UpdateLayout();
                   (App.Current as App).BundleImage.NeedToUpdate = false;

               }
               GC.Collect();
           }
           catch (Exception e)
           {
               throw;

           }      
       
       }

        private async System.Threading.Tasks.Task HandleFileFromUrlAssoc()
        {
            var queryStrings = this.NavigationContext.QueryString;

            if (queryStrings.ContainsKey("fileToken"))
            {
                UserComeFrom = "FromURLAssoc";
                //string localFilePath = HttpUtility.UrlDecode(SharedStorageAccessManager.GetSharedFileName(queryStrings["fileToken"]));
                //descriptionTextFile.Text = Path.GetFileName(localFilePath);
                StorageFile file = await StorageExplorer.GetFileFromSharedStorage(queryStrings);
                (App.Current as App).TempFileFromURL = file;

                ApplicationBar.IsVisible = false;
                _loader.Visibility = System.Windows.Visibility.Visible;
                _loader.Start();
                string ext = Path.GetExtension(file.Path).ToLower();
                byte[] arr = null;
                if ((ext == ".jpeg") || (ext == ".jpg") || (ext == ".tiff") || (ext == ".tif") || (ext == ".bmp") || (ext == ".png")) {

                    arr = await ImageEncoder.PrepareJpegFile(file);
                    //await ImageEncoder.RenderImage3(arr, img, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
                    //arr = null;


                }
                else
                {

                    proc = new LibRawProcessor();
                    arr = await proc.Convert(file.Path);

                   
                }

                if (arr != null)
                {
                    await ImageEncoder.RenderImage3(arr, img, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
                    arr = null;
                    FinalizeAfterImageProc(file.Path);
                    ApplicationBar.IsVisible = true;
                }
                else
                {

                    FinalizeAfterImageProc(file.Path);

                    this.NavigationContext.QueryString.Clear();

                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {

                        PhoneHelper.NavigateToMainPage();

                    }
                }                 
            }
        }

        private async System.Threading.Tasks.Task HandleFileFromPhotoEditor()
        {
            var queryStrings = this.NavigationContext.QueryString;

            if (queryStrings.ContainsKey("FileId"))
            {
                //string localFilePath = HttpUtility.UrlDecode(SharedStorageAccessManager.GetSharedFileName(queryStrings["fileToken"]));
                //descriptionTextFile.Text = Path.GetFileName(localFilePath);

                // string ss = SharedStorageAccessManager.GetSharedFileName(queryStrings["FileId"]);
                UserComeFrom = "FromPhotoEditor";

                MediaLibrary library = new MediaLibrary();
                Picture photoFromLibrary = library.GetPictureFromToken(queryStrings["FileId"]);


                StorageFile file = await StorageExplorer.WritePictureToSharedAsync(photoFromLibrary);
                (App.Current as App).TempFileFromURL = file;

                ApplicationBar.IsVisible = false;
                _loader.Visibility = System.Windows.Visibility.Visible;
                _loader.Start();
                string ext = Path.GetExtension(photoFromLibrary.Name).ToLower();
                byte[] arr = null;
                if ((ext == ".jpeg") || (ext == ".jpg") || (ext == ".tiff") || (ext == ".tif") || (ext == ".bmp") || (ext == ".png"))
                {
                    arr = await ImageEncoder.PrepareJpegFile(file);
                    //await ImageEncoder.RenderImage3(arr, img, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
                    //arr = null;
                }
                else
                {

                    proc = new LibRawProcessor();
                    arr = await proc.Convert(file.Path);

                }

                if (arr != null)
                {
                    await ImageEncoder.RenderImage3(arr, img, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
                    arr = null;
                    FinalizeAfterImageProc(file.Path);
                    ApplicationBar.IsVisible = true;
                }
                else
                {

                    FinalizeAfterImageProc(file.Path);

                    this.NavigationContext.QueryString.Clear();

                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {

                        PhoneHelper.NavigateToMainPage();

                    }
                }
            }
        }

        private async System.Threading.Tasks.Task HandleFileFromListBox()
        {
            start0 = System.DateTime.Now;
            var queryStrings = this.NavigationContext.QueryString;

            if (queryStrings.ContainsKey("fromListBox"))
            {
                UserComeFrom = "FromListBox";
                //string localFilePath = HttpUtility.UrlDecode(SharedStorageAccessManager.GetSharedFileName(queryStrings["fileToken"]));
                //descriptionTextFile.Text = Path.GetFileName(localFilePath);
                if ((App.Current as App).TempFileFromListBox != null)
                {

                    StorageFile linkfile = (App.Current as App).TempFileFromListBox;
                    var file = await StorageExplorer.CopyFileToBufferStorage(linkfile);

                    ApplicationBar.IsVisible = false;
                    _loader.Visibility = System.Windows.Visibility.Visible;
                    _loader.Start();
                    
                    proc = new LibRawProcessor();


                    var arr = await proc.Convert(file.Path);
                    if (arr != null)
                    {
                        await ImageEncoder.RenderImage3(arr, img, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
                        arr = null;
//#if DEBUG
  //                      MessageBox.Show("All proccesing from open to render take time:" + (System.DateTime.Now - start0).TotalMilliseconds.ToString());

//#endif
                        System.Diagnostics.Debug.WriteLine("All proccesing from open to render take time:" + (System.DateTime.Now - start0).TotalMilliseconds.ToString());
            
                        ApplicationBar.IsVisible = true;
                        FinalizeAfterImageProc(file.Path);
                    }
                    else
                    {
                        FinalizeAfterImageProc(file.Path);
                      
                        this.NavigationContext.QueryString.Clear();

                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                        else
                        {

                            PhoneHelper.NavigateToMainPage();
                        }
                    }
                }
            }
        }

        private void FinalizeAfterImageProc(string file)
        {
           
            if (proc != null)
            {
                proc.Dispose();
                proc = null;
            }
            (App.Current as App).TempFileFromListBox = null;
            StorageExplorer.DeleteFileFromBufferStorage(file);

            _loader.Stop();
            _loader.Visibility = System.Windows.Visibility.Collapsed;

            GC.Collect();
        }


        private async System.Threading.Tasks.Task HandleFileFromFileSavePicker()
        {
            var app = App.Current as App;
            if (app.FileSavePickerContinuationArgs != null)
            {
                if ((app.FileSavePickerContinuationArgs.ContinuationData["Operation"] as string) == "SaveImage")
                {
                    var result = await FilePickerConverter.ContinueFileSavePicker(app.FileSavePickerContinuationArgs);

                    if (result)
                    {
                        string Folder = Path.GetDirectoryName(app.FileSavePickerContinuationArgs.File.Path);
                        PhoneHelper.HideProgressBar(0);
                        MessageBox.Show(AppResources.ViewerPageSaveCompletedNotDefault + " " + Folder);

                    }
                    else { 
                    
                    
                    
                    }

                   
                }
                app.FileSavePickerContinuationArgs = null;
            }

        }


        private async System.Threading.Tasks.Task HandleFileFromFilePicker()
        {
            var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null)
            {
                if ((app.FilePickerContinuationArgs.ContinuationData["Operation"]as string) == "GetImage")
                {
                    UserComeFrom = "FromFilePicker";

                    _loader.Visibility = System.Windows.Visibility.Visible;
                    ApplicationBar.IsVisible = false;
                    _loader.Start();
                    string s = await FilePickerConverter.ContinueFileOpenPicker(app.FilePickerContinuationArgs);
                    (App.Current as App).TempFilePathFromPicker = s;

                    DateTime nn = DateTime.Now;
                    string ext = Path.GetExtension(s).ToLower();
                    byte[] arr = null;

                    if ((ext == ".jpeg") || (ext == ".jpg") || (ext == ".tiff") || (ext == ".tif") || (ext == ".bmp") || (ext == ".png"))
                    {

                        arr = await ImageEncoder.PrepareJpegFile(s);
                        //await ImageEncoder.RenderImage3(arr, img, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
                        //arr = null;


                    }
                    else
                    {
                        proc = new LibRawProcessor();
                        arr = await proc.Convert(s);
                       
                    }


                    if (arr != null)
                    {
                        await ImageEncoder.RenderImage3(arr, img, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
                        arr = null;


                        FinalizeAfterImageProc(s);
                        ApplicationBar.IsVisible = true;
                    }
                    else
                    {

                        FinalizeAfterImageProc(s);

                        this.NavigationContext.QueryString.Clear();

                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                        else
                        {

                            PhoneHelper.NavigateToMainPage();

                        }

                    }
                }
            }
        }

        // Sample code for building a localized ApplicationBar
        private void BuildlocalizedApplicationBar()
        {
            if (ApplicationBar == null)
            {
                // Set the page's ApplicationBar to a new instance of ApplicationBar.
                ApplicationBar = new ApplicationBar();
                ApplicationBar.IsVisible = false;
                ApplicationBar.Mode = ApplicationBarMode.Default;

                // Create a new button and set the text value to the localized string from AppResources.
                ApplicationBarIconButton appBarButtonShare = new ApplicationBarIconButton(new Uri("/Assets/AppBar/share.png", UriKind.Relative));
                appBarButtonShare.Text = AppResources.ViewerPageAppBarShare;
                appBarButtonShare.Click += ApplicationBarIconButton_Click_1;
                ApplicationBar.Buttons.Add(appBarButtonShare);

                ApplicationBarIconButton appBarButtonSaveToMedia = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
                appBarButtonSaveToMedia.Text = AppResources.ViewerPageAppBarSave;
                appBarButtonSaveToMedia.Click += ApplicationBarIconButton_Click_3;
                ApplicationBar.Buttons.Add(appBarButtonSaveToMedia);

                ApplicationBarIconButton appBarButtonEdit = new ApplicationBarIconButton(new Uri("/Assets/AppBar/edit.png", UriKind.Relative));
                appBarButtonEdit.Text = AppResources.ViewerPageAppBarEdit;
                appBarButtonEdit.Click += appBarButtonEdit_Click;
                ApplicationBar.Buttons.Add(appBarButtonEdit);

                ApplicationBarIconButton appBarMenuItemExif = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.information.png", UriKind.Relative));
                appBarMenuItemExif.Text = AppResources.ViewerPageAppBarExif;
                appBarMenuItemExif.Click += appBarMenuItemExif_Click;
                ApplicationBar.Buttons.Add(appBarMenuItemExif);

                if (!InAppHelper.CheckUserHaveProduct())
                {
                    ApplicationBarMenuItem appBarMenuItemUpgrade = new ApplicationBarMenuItem(AppResources.ViewerPageAppBarUpgrade);
                    appBarMenuItemUpgrade.Click += appBarMenuItemUpgrade_Click;
                    ApplicationBar.MenuItems.Add(appBarMenuItemUpgrade);
                }


                ApplicationBarMenuItem appBarMenuItemAbout = new ApplicationBarMenuItem(AppResources.ViewerPageAppBarAbout);
                appBarMenuItemAbout.Click += appBarMenuItemAbout_Click;
                ApplicationBar.MenuItems.Add(appBarMenuItemAbout);

                //ApplicationBarMenuItem appBarMenuItemDelete = new ApplicationBarMenuItem(AppResources.ViewerPageAppBarDelete);
                //appBarMenuItemDelete.Click += appBarMenuItemDelete_Click;
                //ApplicationBar.MenuItems.Add(appBarMenuItemDelete);



                ApplicationBarMenuItem appBarMenuItemSettings = new ApplicationBarMenuItem(AppResources.SettingsPageTitle);
                appBarMenuItemSettings.Click += appBarMenuItemSettings_Click;
                ApplicationBar.MenuItems.Add(appBarMenuItemSettings);
            }
            else { 

                //if user update app
                if (InAppHelper.CheckUserHaveProduct())
                {
                    foreach (ApplicationBarMenuItem aa in ApplicationBar.MenuItems) {

                        if (aa.Text == AppResources.ViewerPageAppBarUpgrade) {

                            aa.IsEnabled = false;
                            break;
                        }
                    
                    }

                }
            
            }    

        }

        void appBarMenuItemDelete_Click(object sender, EventArgs e)
        {
            
            //(App.Current as App).BundleImage.pathToOriginalFile
            //throw new NotImplementedException();
        }

        void appBarMenuItemExif_Click(object sender, EventArgs e)
        {
            PhoneHelper.NavigateToInfoPage();
        }

        void appBarMenuItemSettings_Click(object sender, EventArgs e)
        {
            PhoneHelper.NavigateToSettingsPage();
        }

        void appBarMenuItemUpgrade_Click(object sender, EventArgs e)
        {
            PhoneHelper.UpgradeApp();
        }

        void appBarMenuItemAbout_Click(object sender, EventArgs e)
        {
            PhoneHelper.NavigateToAboutPage();          
        }

        void appBarButtonEdit_Click(object sender, EventArgs e)
        {

            PhoneHelper.NavigateToEditPage();
        }


        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            if ((App.Current as App).BundleImage.PixMapFile !=null)
            {
                PhoneHelper.SaveMedia((App.Current as App).BundleImage.PixMapFile, (App.Current as App).BundleImage.Width,  (App.Current as App).BundleImage.Height);
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            if ((App.Current as App).BundleImage.PixMapFile != null)
            {
                PhoneHelper.ShareMedia((App.Current as App).BundleImage.PixMapFile, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);
            }
        }
    }
}