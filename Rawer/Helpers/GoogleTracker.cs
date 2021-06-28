using GoogleAnalytics;
using GoogleAnalytics.Core;

using Rawer.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace Rawer
{
    class GoogleTracker
    {

        public static void TrackSelectedTool(string Tool) {

            //var tc = new TelemetryClient();
            //var properties = new Dictionary<string, string> { { "tool", Tool } };

            ////var measurements = new Dictionary<string, string> { { "tool", Tool } };

            //// Send the event:
            //tc.TrackEvent("SelectedTool", properties, null);
            

            Tracker myTracker = EasyTracker.GetTracker();
            var toolname = Tool.Replace("Tool", "").Replace("ImageEditor.n","");
            myTracker.SendEvent("EditTools", "ToolUsed", toolname, 0);

        
        
        }

        public static async Task TrackTransaction(Guid transactionId, string ProductID){

            try
            {
                //await TrackAppInsight(transactionId, ProductID);

                //ListingInformation listings = await CurrentApp.LoadListingInformationAsync();
                //ProductListing product = listings.ProductListings[ProductID];                
                //System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(listings.CurrentMarket);                
                //double price = double.Parse(product.FormattedPrice, System.Globalization.NumberStyles.Currency, culture);

                //System.Globalization.RegionInfo region = new System.Globalization.RegionInfo(listings.CurrentMarket);

                //var currencyCode = regionInfo.ISOCurrencySymbol;
                //double price = 3.5;  // in USD
                double price = 2.0;
                double tax = price * 0.3;

                Transaction myTrans = new Transaction(
                    transactionId.ToString(),
                    (long)(price * 1000000))
                    {
                        Affiliation = "In-App Store",
                        TotalTaxInMicros = (long)(tax * 1000000),
                        ShippingCostInMicros = 0,
                        CurrencyCode ="USD" // (String) Set currency code to USD.

                    };

                Tracker myTracker = EasyTracker.GetTracker();
                myTracker.SendTransaction(myTrans);
            }
            catch (SystemException e) { }
        
        }


        static async Task TrackAppInsight(Guid transactionId, string ProductID)
        {
            //ListingInformation listings = await CurrentApp.LoadListingInformationAsync();
           
            //var tc = new TelemetryClient();
            //var properties = new Dictionary<string, string> { { "Transaction ID", transactionId.ToString() } };

            //var measurements = new Dictionary<string, double>();

            //if (listings.CurrentMarket.ToLower() == "cn")
            //{
            //    measurements.Add("Price", 1.0);
            //}
            //else
            //{

            //    measurements.Add("Price", 2.0);

            //}
            //// Send the event:
            //tc.TrackEvent("Purchase", properties, measurements);

        }
    }
}
