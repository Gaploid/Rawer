using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.ApplicationModel;
using Rawer.Resources;
using Microsoft.Phone.Tasks;

namespace Rawer
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            this.Loaded += AboutPage_Loaded;

            BuildLocalizedApplicationBar();

            desrtext.Text = AppResources.AboutPageMainText;
            thanks.Text = AppResources.AboutPageThanksText;
            ver.Text = AppResources.AboutPageVersionText + PhoneHelper.GetAppVersion();
            email.Text = AppResources.AboutPageEmailText + " " + AppResources.AboutPageEmail;

           

        }

        void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("AboutPage");

            //throw new NotImplementedException();
        }

        private void email_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneHelper.ShowFeedbackEmail();
        }


        private void BuildLocalizedApplicationBar()
        {
            if (ApplicationBar == null)
            {
                // Set the page's ApplicationBar to a new instance of ApplicationBar.
                ApplicationBar = new ApplicationBar();
                //ApplicationBar.Mode = ApplicationBarMode.Minimized;

                ApplicationBarIconButton appBarButtonEmail = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.email.png", UriKind.Relative));
                appBarButtonEmail.Text = AppResources.AboutPageAppBarEmail;
                appBarButtonEmail.Click += ApplicationBarIconButton_Click_1;
                ApplicationBar.Buttons.Add(appBarButtonEmail);

                ApplicationBarIconButton appBarButtonFeedback = new ApplicationBarIconButton(new Uri("/Assets/AppBar/like.png", UriKind.Relative));
                appBarButtonFeedback.Text = AppResources.AboutPageAppBarFeedback;
                appBarButtonFeedback.Click += ApplicationBarIconButton_Click_2;
                ApplicationBar.Buttons.Add(appBarButtonFeedback);

                if (!InAppHelper.CheckUserHaveProduct())
                {
                    ApplicationBarMenuItem appBarMenuItemLibrary = new ApplicationBarMenuItem(AppResources.ViewerPageAppBarUpgrade);
                    appBarMenuItemLibrary.Click += ApplicationBarMenuItem_Click_1;
                    ApplicationBar.MenuItems.Add(appBarMenuItemLibrary);
                }
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


        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            PhoneHelper.ShowFeedbackEmail();
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }


        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            PhoneHelper.UpgradeApp();
        }

        private void Privacy_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneHelper.NavigateToPolicyPage();
        }
    }
}