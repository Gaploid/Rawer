using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Microsoft.Xna.Framework.Media;
using Lumia.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using System.IO;

namespace ImageEditor
{
    class Helper
    {

        public async static Task<WriteableBitmap> GetWriteableBitmap(StorageFile file, int w, int h)
        {            
           
            var d = await FileIO.ReadBufferAsync(file);
            var f = d.ToArray();

            WriteableBitmap wb = new WriteableBitmap(w, h);
            wb.FromByteArray(f);
            
            f = new byte[0];            
            f = null;
            d = null;
            GC.Collect();
            return wb;        
        }

        public static async Task WriteDataToFileAsync(StorageFile file, IBuffer data) {



            if (file != null)
            {
                //await PathIO.WriteBytesAsync(file.Path,wb.ToByteArray());

                var folder = ApplicationData.Current.LocalFolder;
                var file2 = await folder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting);

                using (Stream fileStream = await file.OpenStreamForWriteAsync())//OpenAsync(FileAccessMode.ReadWrite))
                {
                    var s = data.AsStream();

                    s.CopyTo(fileStream);
                    fileStream.Flush();
                    s.Flush();

                    s.Dispose();
                    s = null;
                    fileStream.Dispose();

                }



                //using (IRandomAccessStream fileStream = await file2.OpenAsync(FileAccessMode.ReadWrite))
                //{


                //    await fileStream.WriteAsync(data);

                //    await fileStream.FlushAsync();
                 
                //    data = null;

                //    fileStream.Dispose();
                //}
            }
        
        
        
        }

        public static async Task WriteDataToFileAsync(StorageFile file, WriteableBitmap wb)
        {
            DateTime beforeWriteData = System.DateTime.Now;


            IBuffer data = wb.ToByteArray().AsBuffer();

            await WriteDataToFileAsync(file, data);

            System.Diagnostics.Debug.WriteLine("WriteData take time in milliseconds " + (DateTime.Now - beforeWriteData).TotalMilliseconds.ToString());
            
        }
    }
}
