using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;

namespace Rawer.Models
{
    public class ListImage : IDisposable
    {
        DateTime dateCreated;


        public ListImage() { }

        public ulong FileSize { private get; set; }
        public string FileSizeDisplay {


            get {

                return String.Format(new FileSizeFormatProvider(), "{0:fs}", FileSize);
            
            }
        
        }

        public Picture JpegFile {  get;  set; }

        public StorageFile JpegStorageFile { get; set; }

        public BitmapImage Bitmap
        {
            get
            {
                if (JpegFile != null)
                {
                    using (var inStr = JpegFile.GetThumbnail())
                    {
                        var image = new BitmapImage();
                        image.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                        image.SetSource(inStr);

                        inStr.FlushAsync();
                        inStr.Close();
                        inStr.Dispose();
                        //Debug.WriteLine("render time take:" + (DateTime.Now - now).TotalMilliseconds.ToString());

                        return image;
                    }
                }
                else {
                    return new BitmapImage(new Uri("/Assets/Logo.png", UriKind.Relative));
                }
            }

            set { }
        }

        public StorageFile DngFile { get; set; }

        public DateTimeOffset DateCreated
        {            
            set
            {
                dateCreated = value.DateTime;
            }
        }

        public string DisplayDateTimeCreated {


            get {

                return dateCreated.ToShortDateString() + " - " + dateCreated.ToShortTimeString();
            
            }
        
        
        }

        public void Dispose()
        {
        }
    }
}
