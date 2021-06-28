using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls.SlideView;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Phone.System.Memory;
using Lumia.Imaging;
using Lumia.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using System.Windows;
using Rawer.Resources;
using LibrawRuntimeComponent;

namespace Rawer
{
    class LibRawProcessor:IDisposable
    {
        //int _width;
        //int _height;

        LibrawRuntimeComponent.ImageProcessor proc;
        bool canceled;

        int[] red;
        int[] blue;
        int[] green;

        const string ImageSmallPostFix = "_4000.pixmap";

        public int Width { get; private set;}
        public int Height { get; private set; }

        public LibRawProcessor()
        {
            canceled = false;
        }

        public async Task<byte[]> Convert(string path)
        { 

            AppSettings settings =new AppSettings();

            ulong imgsize = await StorageExplorer.GetFileSize(path).ConfigureAwait(false);

            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Image", "Convert", System.IO.Path.GetExtension(path).ToUpper(), (long)imgsize);


            DateTime beforeCplusplus = DateTime.Now;
            proc = new LibrawRuntimeComponent.ImageProcessor();
          
            
           //Open file and prepare
           var Result = await Task.Run(() =>
                 {
                     LibRawUnpackResult r = proc.Unpack(path, settings.AutoBrigtnessSetting, !settings.WhiteBalanceSetting, settings.NoiseReductionSetting);
                     return r;
                 }).ConfigureAwait(false);


           if (Result.Error != 0)
           {
               HandleLibRawError(Result);
               return null;
           }


           if (canceled) {              
               return null;
           }
           

           System.Diagnostics.Debug.WriteLine("Unpack take time " + (DateTime.Now - beforeCplusplus).TotalMilliseconds.ToString());
           //proc.Cancel();

           DateTime now2 = DateTime.Now;

           //Proccessing Raw file
           var Result2 = await Task.Run(() =>
              {
                  if (settings.HalfSizeSetting)
                  {
                      return proc.Process(true);
                  }
                  else
                  {

                      //if we dont have enought memory then create half size image
                      if (CheckIfImageFitAvailableMemoryOnDevice(Result.Width, Result.Height))
                      {
                          return proc.Process(false);
                      }
                      else
                      {
                          return proc.Process(true);
                      }
                  }
              }).ConfigureAwait(false);

           if (Result2.Error != 0)
           {
               HandleLibRawError(Result2);
               return null;
           }

          System.Diagnostics.Debug.WriteLine("Process take time " + (DateTime.Now - now2).TotalMilliseconds.ToString());


          settings = null;

          Height = Result2.Height;
          Width = Result2.Width;

           //Getting bytes from libraw
          byte[] smallpicture = await Task.Run(() =>
             {
                 return proc.GetImageBytes();
             }).ConfigureAwait(false);

           //var a = proc.GetHistogram();
          
           
           //byte[] smallpicture = proc.GetImageBytes();

           //proc.Clear();
           System.Diagnostics.Debug.WriteLine("finished c++ proccessing, take time in milliseconds " + (DateTime.Now - beforeCplusplus).TotalMilliseconds.ToString());

            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    MessageBox.Show((DateTime.Now - beforeCplusplus).TotalMilliseconds.ToString());
            //});


           proc = null;
           //smallpicture = null;
           //GC.Collect();
           System.Diagnostics.Debug.WriteLine("current memmory after Convert - " + Windows.System.MemoryManager.AppMemoryUsage.ToString());
            

          
           //!!!!!!!!!!!!!!pass real size here 
           smallpicture = ConvertToBGRA(smallpicture, Result2.Width, Result2.Height);
           //smallpicture = null;

           //GC.Collect();
           

           DateTime beforRender = System.DateTime.Now;

           //!!!!!!!!!!!!!!pass real size here 
           //await ImageEncoder.RenderImage3(array4, img, Result2.Width, Result2.Height);
           System.Diagnostics.Debug.WriteLine("rendering, take time in milliseconds " + (DateTime.Now - beforRender).TotalMilliseconds.ToString());
            

           //Create unpacked Dump  
           //string name = System.IO.Path.GetRandomFileName() +".pixmap";
           (App.Current as App).BundleImage = new Models.BundleImage(null,
                                                                    Result2.EXIF,
                                                                    Result2.Width,
                                                                    Result2.Height,
                                                                    imgsize,
                                                                    path,
                                                                    new Models.Histogram() { Red = red, Blue = blue, Green = green });

            SaveToDiskAndPutGlobalVar(path, imgsize, Result2, smallpicture);
           //clear buffer array
           //array4 = null;        


            //MessageBox.Show((DateTime.Now - beforeCplusplus).TotalMilliseconds.ToString());
            return smallpicture;

        }

        private async void SaveToDiskAndPutGlobalVar(string path, ulong imgsize, LibRawUnpackResult Result2, byte[] smallpicture)
        {
            DateTime beforeWriteData = System.DateTime.Now;

            StorageFile file = await StorageExplorer.WriteDataToFileAsync(System.IO.Path.GetFileNameWithoutExtension(path) + ".pixmap", smallpicture);
            System.Diagnostics.Debug.WriteLine("WriteData take time in milliseconds " + (DateTime.Now - beforeWriteData).TotalMilliseconds.ToString());
            (App.Current as App).BundleImage.PixMapFile = file;

            smallpicture = null;

           
            //set this file available on another pages
           
           
        }

        bool CheckIfImageFitAvailableMemoryOnDevice(int w, int h) {
            bool flag = false;

            //MemoryManager.ProcessCommittedLimit

        
            int raw = sizeof(short) * w  *h;   //80Mb на RAW-данные
            int data= 4 * sizeof(short) * w * h; // = 320Mb на массив image
            
            

           
            //Проверяем хватит ли памяти с запасом +10мб.
            if (Windows.System.MemoryManager.AppMemoryUsageLimit - Windows.System.MemoryManager.AppMemoryUsage >= (ulong)(raw + data + 10485760))
            {

                flag = true;
            }
            else {

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(AppResources.ViewerPageLowMemoryBody, AppResources.ViewerPageLowMemoryTitle, MessageBoxButton.OK);
                });

                flag = false;
            
            }

            return flag;

        }


        private void HandleLibRawError(LibrawRuntimeComponent.LibRawUnpackResult Result)
        {
            switch ((LibRaw_errors)Result.Error)
            {
                case LibRaw_errors.LIBRAW_FILE_UNSUPPORTED:

                    Deployment.Current.Dispatcher.BeginInvoke(() => 
                         MessageBox.Show(AppResources.LIBRAW_FILE_UNSUPPORTED_Text, AppResources.LIBRAW_FILE_UNSUPPORTED_Title,MessageBoxButton.OK)
                    );

                    break;

                case LibRaw_errors.LIBRAW_UNSUFFICIENT_MEMORY:
                     Deployment.Current.Dispatcher.BeginInvoke(() => 
                          MessageBox.Show(AppResources.LIBRAW_UNSUFFICIENT_MEMORY_text, AppResources.LIBRAW_UNSUFFICIENT_MEMORY_title,MessageBoxButton.OK)
                   );
                    break;

                case LibRaw_errors.LIBRAW_CANCELLED_BY_CALLBACK:
                    //canceled = true;
                    break;

                default:
                     Deployment.Current.Dispatcher.BeginInvoke(() => 
                         MessageBox.Show(AppResources.LIBRAW_COMMON_ERROR_text, AppResources.LIBRAW_COMMON_ERROR_title, MessageBoxButton.OK)
                    );
                    break;
            }

        }

        private byte[] ConvertToBGRA(byte[] array, int w, int h)
        {
            System.DateTime date = DateTime.Now;
            red = new int[256];
            green = new int[256];
            blue = new int[256];
            
            int index, index2 = 0;
            int length = w * h * 4;


            byte[] newarray = new byte[length];

            //Parallel.For(0, h, y =>
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {

                    index = (x + y * w) * 4;
                    index2 = (x + y * w) * 3;

                    newarray[index] = array[index2 + 2];
                    newarray[index + 1] = array[index2 + 1];
                    newarray[index + 2] = array[index2];
                    newarray[index + 3] = 0xFF;


                    //Заполняем для гистограммы
                    red[array[index2]] += 1;
                    //System.Diagnostics.Debug.WriteLine(array[index2] + "  color count-" + red[array[index2]]);
                    green[array[index2 + 1]] += 1;
                    blue[array[index2 + 2]] += 1;
                }
            }

            //Normilize Black and White for histogram. There is alwase 
            ImageEncoder.NormilizeColorChannelsValuesForHistogram(ref red,ref green, ref blue);



            array = null;
            System.Diagnostics.Debug.WriteLine("Time taken to RGBA convert and histogram " + (DateTime.Now - date).TotalMilliseconds.ToString());
            return newarray;
        }

        


        

        

        void img_ImageFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        


        public enum LibRaw_errors:int
        {
            LIBRAW_SUCCESS = 0,
            LIBRAW_UNSPECIFIED_ERROR = -1,
            LIBRAW_FILE_UNSUPPORTED = -2,
            LIBRAW_REQUEST_FOR_NONEXISTENT_IMAGE = -3,
            LIBRAW_OUT_OF_ORDER_CALL = -4,
            LIBRAW_NO_THUMBNAIL = -5,
            LIBRAW_UNSUPPORTED_THUMBNAIL = -6,
            LIBRAW_INPUT_CLOSED = -7,
            LIBRAW_UNSUFFICIENT_MEMORY = -100007,
            LIBRAW_DATA_ERROR = -100008,
            LIBRAW_IO_ERROR = -100009,
            LIBRAW_CANCELLED_BY_CALLBACK = -100010,
            LIBRAW_BAD_CROP = -100011
        };


        public void Dispose()
        {
            if (proc != null && !canceled)
            {
                proc.Cancel();
                canceled = true;
            }
        }
    }
}
