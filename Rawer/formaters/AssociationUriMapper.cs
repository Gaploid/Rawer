using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Windows.Phone.Storage.SharedAccess;

namespace Rawer
{
    class AssociationUriMapper : UriMapperBase
    {
        private string tempUri;
        private bool secondtime;

        public override Uri MapUri(Uri uri)
        {
            tempUri = uri.ToString();
            //tempUri = new Uri("/LibraryPage.xaml", UriKind.Relative).ToString();


           
            string mappedUri;

            // Launch from the photo edit picker.
            // Incoming URI example: /MainPage.xaml?Action=EditPhotoContent&FileId=%7Bea74a960-3829-4007-8859-cd065654fbbc%7D
            if ((tempUri.Contains("EditPhotoContent")) && (tempUri.Contains("FileId")))
            {
                // Redirect to PhotoEdit.xaml.
                mappedUri = tempUri.Replace("MainPage", "LoadingPage");
                return new Uri(mappedUri, UriKind.Relative);
            }


            // File association launch
            if (tempUri.Contains("/FileTypeAssociation"))
            {
                // Get the file ID (after "fileToken=").
                int fileIDIndex = tempUri.IndexOf("fileToken=") + 10;
                string fileID = tempUri.Substring(fileIDIndex);

                // Get the file name.
                string incomingFileName =
                    SharedStorageAccessManager.GetSharedFileName(fileID);

                // Get the file extension.
                string incomingFileType = Path.GetExtension(incomingFileName).Replace(".","");

                // Map the .sdkTest1 and .sdkTest2 files to different pages.
                //switch (incomingFileType)
                //{
                //    case "vsd":
                //        return new Uri("/MainPage.xaml?fileToken=" + fileID + "&fileFormat=" + incomingFileType, UriKind.Relative);                    
                //    default:
                //        return new Uri("/MainPage.xaml", UriKind.Relative);
                //}

                if (!String.IsNullOrEmpty(incomingFileType)) {
                    return new Uri("/Pages/LoadingPage.xaml?fileToken=" + fileID + "&fileformat=" + incomingFileType, UriKind.Relative);                    
                }

            }   
    
            
            //handle sharecontract first time startup 
            //if ((App.Current as App).ShareOperationFirstTime)
            //{

            //    //i dont know why this url mapper works two time every navigation but we need to workaround that.
            //    if (secondtime)
            //    {
            //        (App.Current as App).ShareOperationFirstTime = false;
            //    }
            //    else
            //    {
            //        secondtime = true;
            //    }


            //    return new Uri("/MainPage.xaml?from=anotherapp", UriKind.Relative);

            //}


            // Otherwise perform normal launch.
            return uri;
        }
    }
}
