using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Net;
using Windows.Phone.Storage.SharedAccess;
using Microsoft.Xna.Framework.Media;

namespace Rawer
{
    class StorageExplorer
    {

        public static async Task<StorageFile> WriteDataToFileAsync(string fileName, byte[] data)
        {
            //var folder = ApplicationData.Current.LocalFolder;
            //var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            //await PathIO.WriteBytesAsync(file.Path, data);

            var f = data.AsBuffer();
            var file = await WriteDataToFileAsync(fileName, f);

            f = null;

            return file;
          
        }

        public static async Task<StorageFile> WriteDataToFileAsync(string fileName, IBuffer data)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            //await  PathIO.WriteBufferAsync(file.Path, data);



            using (Stream fileStream = await file.OpenStreamForWriteAsync())//OpenAsync(FileAccessMode.ReadWrite))
            {
                var s = data.AsStream();
                    
                s.CopyTo(fileStream);
                fileStream.Flush();
                s.Flush();
                //await fileStream.WriteAsync(data);
                //await fileStream.FlushAsync();


                //fileStream.Seek(0);
                //fileStream.Size = 0;

                s.Dispose();
                s = null;
                fileStream.Dispose();
               
            }
            

            return file;
        }

        public static async Task<StorageFile> WritePictureToSharedAsync(Picture pic)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(pic.Name, CreationCollisionOption.ReplaceExisting);

            //await  PathIO.WriteBufferAsync(file.Path, data);



            using (Stream fileStream = await file.OpenStreamForWriteAsync())//OpenAsync(FileAccessMode.ReadWrite))
            {
                var s = pic.GetImage();

                s.CopyTo(fileStream);
                fileStream.Flush();
                //s.Flush();
                //await fileStream.WriteAsync(data);
                //await fileStream.FlushAsync();


                //fileStream.Seek(0);
                //fileStream.Size = 0;
               
                s.Dispose();
                s = null;
                fileStream.Dispose();

            }


            return file;
        }



        async public  static Task<ulong> GetFileSizeAsync(IStorageFile file)
        {
            //var properties = file.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            var properties = await file.GetBasicPropertiesAsync();
            return properties.Size;
        }


        public static void DeleteTempPixMapFile() {

            //Clear prev temp pixmap files if there were them
            if ((App.Current as App).BundleImage != null)
            {
                if ((App.Current as App).BundleImage.PixMapFile != null)
                {
                    try
                    {
                        (App.Current as App).BundleImage.PixMapFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch (System.Exception e) { //dont forward it cause we already deleted it somewhere else
                    }
                }
            }
        
        
        }

        public static async Task<StorageFile> CopyFileToBufferStorage(StorageFile file) {


            return await file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting);        
        
        
        }


        public static async Task<StorageFile> GetFileFromSharedStorage(IDictionary<string, string> queryStrings)
        {
            string BufferIncomingFileName = HttpUtility.UrlDecode(SharedStorageAccessManager.GetSharedFileName(queryStrings["fileToken"]));
     
            await SharedStorageAccessManager.CopySharedFileAsync(ApplicationData.Current.LocalFolder, BufferIncomingFileName,
                                                           NameCollisionOption.ReplaceExisting,
                                                           queryStrings["fileToken"]);

            // Access isolated storage.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Access the query file.
     
            StorageFile oldfile = await local.GetFileAsync(BufferIncomingFileName);


            return oldfile;
        }

        public static Stream GetStreamFile(StorageFile file) {

            return new FileStream(file.Path, FileMode.Open, FileAccess.Read);        
        
        }

        public static async Task<Byte[]> GetFileInBytes(StorageFile file)
        {

            var buf = await FileIO.ReadBufferAsync(file);
            byte[] array = buf.ToArray();
            buf = null;
            GC.Collect();

            return array;

            

        }


        public static async Task DeleteFileFromBufferStorage(StorageFile file)
        {
            try
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (FileNotFoundException)
            { }
        }

        public static void DeleteFileFromBufferStorage(string filePath)
        {
            try
            {
                if (!String.IsNullOrEmpty(filePath))
                {
                    File.Delete(filePath);

                }

            }
            catch (FileNotFoundException) { }


        }

        public static async Task<ulong> GetFileSize(string FilePath) {


            var s = await StorageFile.GetFileFromPathAsync(FilePath);
            var prop = await s.GetBasicPropertiesAsync();
            s = null;
            return prop.Size;
        
        }

        public static async Task ClearCache()
        {

            try
            {
                var folder = ApplicationData.Current.LocalFolder;

                string ExceptThisFile = GetCurrentPartialFileName();

                var files = await folder.GetFilesAsync();
                //List<StorageFile> filesToDelete = new List<StorageFile>();

                foreach (var file in files)
                {
                    if (file.Name.Contains("pixmap"))
                    {
                        if (!String.IsNullOrEmpty(ExceptThisFile))
                        {
                            if (!file.Name.Contains(ExceptThisFile))
                                await file.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().ConfigureAwait(false);
                        }
                        else
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().ConfigureAwait(false);
                        }
                    }
                }

            }
            catch (SystemException e) { }
        }

        private static string GetCurrentPartialFileName(){
        
        string filename = "";
            if ((App.Current as App).BundleImage != null)
            {
                filename = (App.Current as App).BundleImage.PixMapFile.Name.Substring(0, (App.Current as App).BundleImage.PixMapFile.Name.Length - ".pixmap".Length);            
            }
        
            return filename;
        }

        public async static Task<ulong> GetTotalLocalStorageSize() {

            ulong sum = 0;

            try
            {
                string ExceptThisFile = GetCurrentPartialFileName();

                var folder = ApplicationData.Current.LocalFolder;

                var files = await folder.GetFilesAsync();
                //List<StorageFile> filesToDelete = new List<StorageFile>();


                foreach (var file in files)
                {
                    if (file.Name.Contains("pixmap"))
                    {

                            if (!String.IsNullOrEmpty(ExceptThisFile))
                            {
                                if (!file.Name.Contains(ExceptThisFile))
                                {
                                    
                                    sum += (await file.GetBasicPropertiesAsync()).Size;
                                }
                            }
                            else {                                    
                                    sum += (await file.GetBasicPropertiesAsync()).Size;
                            }
                        
                    }
                }

            }
            catch (SystemException e) { }

            return sum;    
        
        } 


        public static void DeleteFileFromAppRootFolder(string fileName)
        {
            try
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    var folder = ApplicationData.Current.LocalFolder;
                    //var prop = await  folder.GetBasicPropertiesAsync();
                    
                    File.Delete(Path.Combine(folder.Path, fileName));
                
                }

            }
            catch (FileNotFoundException) { }


        }
    }
}
