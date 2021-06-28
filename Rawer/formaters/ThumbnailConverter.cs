using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Windows.Storage;

namespace Rawer
{
    public class ThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           var a = Task.Run(() =>
            {
                if (value != null)
                {
                    DateTime now = DateTime.Now;
                    //var tatt = (Picture)value;
                    var tatt = (StorageFile)value;


                    var dd = tatt.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView,
                                                   200,
                                                    Windows.Storage.FileProperties.ThumbnailOptions.ReturnOnlyIfCached).AsTask().ConfigureAwait(false).GetAwaiter().GetResult(); ;

                    using (var inStr = dd.GetOutputStreamAt(0) as Stream)
                    {
                        var image = new BitmapImage();
                        image.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                        image.SetSource(inStr);

                        inStr.FlushAsync();
                        inStr.Close();
                        inStr.Dispose();
                        Debug.WriteLine("render time take:" + (DateTime.Now - now).TotalMilliseconds.ToString());

                        return image;

                    }


                }
                else {

                    return new BitmapImage(new Uri("/Assets/Logo.png", UriKind.Relative));
                
                }
            });


           return a.Result;

            //return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
