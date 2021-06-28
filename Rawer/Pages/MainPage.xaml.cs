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
using Telerik.Windows.Controls;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;


namespace Rawer
{
    public partial class MainPage : PhoneApplicationPage
    {
        ListBoxManager listBoxManger;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //BuildlocalizedApplicationBar();
            this.Loaded += MainPage_Loaded;

            //double c = 5;
            //double a = 3;
            //double b = 7;

            //c = check(a, b, c);

            //Adding Tile effect on Item tap
            radDataBoundListBox.SetValue(InteractionEffectManager.IsInteractionEnabledProperty, true);
            InteractionEffectManager.AllowedTypes.Add(typeof(RadDataBoundListBoxItem));
           
            
            // Sample code to localize the ApplicationBar
            //BuildlocalizedApplicationBar();
        }


        //double check(double a, double b, double c)
        //{

        //    c = a + b;
        //    return c;

        //}

        ~MainPage()
        {

            System.Diagnostics.Debug.WriteLine("MainPage destroed");
        
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("MainPage");
            //throw new Exception("Bla Bla");
#if (!DEBUG)
            Reminder r = new Reminder();
            r.ShowReminder();
#endif

        }

       

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FilePickerConverter.Open();
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BuildlocalizedApplicationBar();

            if (listBoxManger == null)
            {
                var past = DateTime.Now;
                listBoxManger = new ListBoxManager(radDataBoundListBox);

                await listBoxManger.LoadDNGList();
                System.Diagnostics.Debug.WriteLine("Load Dng take:" + (DateTime.Now - past).TotalMilliseconds.ToString());
                var past2 = DateTime.Now;
                
                if (listBoxManger.NumberOfDNGFiles > 0)
                {
                    await listBoxManger.FilterJpegList();
                }
                System.Diagnostics.Debug.WriteLine("Filtering:" +(DateTime.Now - past2).TotalMilliseconds.ToString());
                System.Diagnostics.Debug.WriteLine("All filtering and rendering:" +(DateTime.Now - past).TotalMilliseconds.ToString());
                
            }

         
            
            base.OnNavigatedTo(e);
        }



        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            base.OnNavigatingFrom(e);
        }

        private void BuildlocalizedApplicationBar()
        {
            if (ApplicationBar == null)
            {
                // Set the page's ApplicationBar to a new instance of ApplicationBar.
                ApplicationBar = new ApplicationBar();
                //ApplicationBar.IsVisible = false;
                //ApplicationBar.Mode = ApplicationBarMode.Minimized;

                ApplicationBarIconButton appBarMenuButtonOpen = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.folder.open.png", UriKind.Relative));
                appBarMenuButtonOpen.Text = AppResources.MainPageAppBarOpen;
                appBarMenuButtonOpen.Click += appBarMenuButtonOpen_Click;
                ApplicationBar.Buttons.Add(appBarMenuButtonOpen);                
                
                if (!InAppHelper.CheckUserHaveProduct())
                {
                    ApplicationBarMenuItem appBarMenuItemUpgrade = new ApplicationBarMenuItem(AppResources.ViewerPageAppBarUpgrade);
                    appBarMenuItemUpgrade.Click += appBarMenuItemUpgrade_Click;
                    ApplicationBar.MenuItems.Add(appBarMenuItemUpgrade);
                }

                ApplicationBarIconButton appBarButtonLayout = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.list.png", UriKind.Relative));
                appBarButtonLayout.Text = AppResources.MainPageAppBarLayoutList;
                appBarButtonLayout.Click += appBarButtonLayout_Click;
                ApplicationBar.Buttons.Add(appBarButtonLayout);

                ApplicationBarIconButton appBarButtonCheckMode = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.list.check.png", UriKind.Relative));
                appBarButtonCheckMode.Text = AppResources.MainPageAppBarCheckMode;
                appBarButtonCheckMode.Click += appBarButtonCheckMode_Click;
                ApplicationBar.Buttons.Add(appBarButtonCheckMode);


                ApplicationBarIconButton appBarButtonDelete = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
                appBarButtonDelete.Text = AppResources.MainPageAppBarDelete;
                appBarButtonDelete.Click += appBarButtonDelete_Click;
                appBarButtonDelete.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarButtonDelete);


                ApplicationBarMenuItem appBarMenuItemAbout = new ApplicationBarMenuItem(AppResources.ViewerPageAppBarAbout);
                appBarMenuItemAbout.Click += appBarMenuItemAbout_Click;
                ApplicationBar.MenuItems.Add(appBarMenuItemAbout);
                
                ApplicationBarMenuItem appBarMenuItemSettings = new ApplicationBarMenuItem(AppResources.SettingsPageTitle);
                appBarMenuItemSettings.Click += appBarMenuItemSettings_Click;
                ApplicationBar.MenuItems.Add(appBarMenuItemSettings);
            }
            else
            {
                //if user update app
                if (InAppHelper.CheckUserHaveProduct())
                {
                    foreach (ApplicationBarMenuItem aa in ApplicationBar.MenuItems)
                    {
                        if (aa.Text == AppResources.ViewerPageAppBarUpgrade)
                        {

                            aa.IsEnabled = false;
                            break;
                        }
                    }
                }                
            }
        }

        void appBarMenuButtonOpen_Click(object sender, EventArgs e)
        {

            FilePickerConverter.Open();
            //throw new NotImplementedException();
        }

        void appBarButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxManger != null)
            {

                listBoxManger.DeleteSelectedItems();
            }
        }

        void appBarButtonCheckMode_Click(object sender, EventArgs e)
        {
            if (listBoxManger != null) {

                listBoxManger.CheckMode(this);
            }
        }

     

        void appBarButtonLayout_Click(object sender, EventArgs e)
        {
            if (listBoxManger != null) {


                
                listBoxManger.ChangeListBoxLayout(this);


            }
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

        private void Image_Unloaded(object sender, RoutedEventArgs e)
        {
           // ((Image)sender).Source = null;
        }

        private void radDataBoundListBox_ItemTap(object sender, Telerik.Windows.Controls.ListBoxItemTapEventArgs e)
        {
            (App.Current as App).TempFileFromListBox = (e.Item.Content as Models.ListImage).DngFile;
            PhoneHelper.NavigateToLoadingPageFromListBox();

        }
    }
}