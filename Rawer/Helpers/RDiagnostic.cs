using Rawer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace Rawer
{
    class RDiagnostic
    {

        RadDiagnostics diagnostics;
        public RDiagnostic() {

            diagnostics = new RadDiagnostics();
        }

        public void Init() {

            diagnostics.Init();
            diagnostics.ApplicationVersion = PhoneHelper.GetAppVersion();
            diagnostics.HandleUnhandledException = true;
            //diagnostics.IncludeScreenshot = true;

            diagnostics.EmailTo = AppResources.AboutPageEmail;
            diagnostics.EmailSubject = "WP rawer crash report";
            var a = new Telerik.Windows.Controls.Reminders.MessageBoxInfoModel();
            a.Title = AppResources.DiagnosticEmailTitle;
            a.Content = AppResources.DiagnosticEmailBody;
            a.Buttons = MessageBoxButtons.OKCancel;
            diagnostics.ExceptionOccurred += diagnostics_ExceptionOccurred;
           
            diagnostics.MessageBoxInfo = a;
        
        }

        void diagnostics_ExceptionOccurred(object sender, ExceptionOccurredEventArgs e)
        {

            if( (App.Current as App).BundleImage != null){

                if ((App.Current as App).BundleImage.pathToOriginalFile != null)
                {
                    e.CustomData = "Original File:" + (App.Current as App).BundleImage.pathToOriginalFile;
                }
            
            }

            
                //throw new NotImplementedException();
        }
    }
}
