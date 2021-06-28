
using Rawer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Store;

namespace Rawer
{
    class InAppHelper
    {
        const string WatermarkProductID = "watermark1";
        const string ImageEditorID = "imageeditor01";
        const string EventPurchaseName = "Purchase";
        const string EventPurchaseStartName = "startf";
        const string EventPurchaseEndName = "finish";


        public static bool CheckCheckUserHaveProductFake(bool backvalue)
        {
            return backvalue;
        }

        public static bool CheckUserHaveProduct(){
            //bool flag = false;

#if DEBUG
            if (System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("HaveProduct"))
            {
                return (bool)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["HaveProduct"];

            } else { return false; } 
           
          
        #endif
            try
            {
                ProductLicense productLicense = null;
                if (CurrentApp.LicenseInformation.ProductLicenses.TryGetValue(WatermarkProductID, out productLicense))
                {
                    if (productLicense.IsActive)
                    {
                        return true;

                    }
                }

                if (CurrentApp.LicenseInformation.ProductLicenses.TryGetValue(ImageEditorID, out productLicense))
                {
                    if (productLicense.IsActive)
                    {
                        return true;

                    }
                }
            }
            catch (SystemException e) { }

            


            return false;

        }

        public static async Task<bool> AskToBuyWithMessageBox()
        {

            //PhoneHelper.NavigateToUpgradePage();

            if (PhoneHelper.ShowCustomMessageBox(AppResources.ViewerPageTrialTitle,
                                                AppResources.ViewerPageTrialBody,
                                                AppResources.ViewerPageTrialOkButton,
                                                AppResources.ViewerPageTrialcontinueButton))
            {

                return await StartPurchase();
            }

            return false;

        }


        public static async void  AskToBuyWithBuyScreen()
        {

            PhoneHelper.NavigateToUpgradePage();

            //if (PhoneHelper.ShowCustomMessageBox(AppResources.ViewerPageTrialTitle,
            //                                    AppResources.ViewerPageTrialBody,
            //                                    AppResources.ViewerPageTrialOkButton,
            //                                    AppResources.ViewerPageTrialcontinueButton))
            //{

            //    return await StartPurchase();
            //}

            //return false;

        }


        public static async Task<bool> StartPurchase()
        {
#if DEBUG
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["HaveProduct"] = true;
            return true;
#endif

            try
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendEvent(EventPurchaseName, EventPurchaseStartName, null, 0);
                // start product purchase
                var result = await CurrentApp.RequestProductPurchaseAsync(ImageEditorID);
                //CurrentApp.LicenseInformation.LicenseChanged += LicenseInformation_LicenseChanged;
                //await CurrentApp.LoadListingInformationAsync();
               
                if (result.Status == ProductPurchaseStatus.Succeeded)
                {
                   
                    MessageBox.Show(AppResources.UpgradeFinal);
                                     
                    await GoogleTracker.TrackTransaction(result.TransactionId, ImageEditorID);
                    GoogleAnalytics.EasyTracker.GetTracker().SendEvent(EventPurchaseName, EventPurchaseEndName, null, 0);

                    return true;                               
                }

                if (result.Status == ProductPurchaseStatus.AlreadyPurchased)
                {
                    MessageBox.Show(AppResources.UpgradeFinal);
                    return true;
                }

                }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
      
    }
}
