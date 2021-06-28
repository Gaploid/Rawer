using Lumia.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Windows;
using Rawer.Resources;
using Telerik.Windows.Controls;
using Microsoft.Xna.Framework.Media;
using Lumia.InteropServices.WindowsRuntime;
using Telerik.Windows.Controls.SlideView;
using System.Windows.Controls;
using Lumia.Imaging.Compositing;
using Windows.Graphics.Imaging;
using Rawer.Models;
using Rawer.Helpers;


namespace Rawer
{
    public class ImageEncoder
    {
        const int MaxRenderSize = 4000;


        public static async Task<StorageFile> SaveFile(StorageFile infile, FileFormat fileFormat,int w, int h)
        {
            string FileName;
           
            FileName = System.IO.Path.GetFileNameWithoutExtension(infile.Name) + "_rawer.";

            var properties = AddMetaData((App.Current as App).BundleImage);

            //string FileName = "MyFile.";
            Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            switch (fileFormat)
            {
                case FileFormat.JPEG:
                    FileName += "jpeg";
                    BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                    break;

                //case FileFormat.PNG:
                //    FileName += "png";
                //    BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                //    break;

                //case FileFormat.BMP:
                //    FileName += "bmp";
                //    BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
                //    break;

                case FileFormat.TIFF:
                    FileName += "tiff";
                    BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
                    break;

                //case FileFormat.GIF:
                //    FileName += "gif";
                //    BitmapEncoderGuid = BitmapEncoder.GifEncoderId;
                //    break;
            }

            //WriteableBitmap WB = new WriteableBitmap(w, h);
            //WB.FromByteArray(await StorageExplorer.GetFileInBytes(infile));

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

            //var file = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);

                if (properties != null) {
                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                }
                
                byte[] pixels = await StorageExplorer.GetFileInBytes(infile);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                //System.Buffer.BlockCopy(WB.Pixels, 0, pixels, 0, pixels.Length);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                    (uint)w,
                                    (uint)h,
                                    96.0,
                                    96.0,
                                    pixels);
                await encoder.FlushAsync();
                pixels = null;
                encoder = null;

            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
                      
            return file;
        }

        private static BitmapPropertySet AddMetaData(BundleImage image)
        {

            
            var propertySet = new Windows.Graphics.Imaging.BitmapPropertySet();
            
            if (image != null)
            {
                try
                {
                    var ApplicationNameValue = new Windows.Graphics.Imaging.BitmapTypedValue(
                        "Windows Phone RAWER",
                        Windows.Foundation.PropertyType.String);
                    propertySet.Add("System.ApplicationName", ApplicationNameValue);


                    //Apperture
                    var aperture = Fraction.Parse(image.Exif.aperture);

                    var ApertureValueN = new Windows.Graphics.Imaging.BitmapTypedValue(
                         aperture.Numerator,
                         Windows.Foundation.PropertyType.UInt32);

                    var ApertureValueD = new Windows.Graphics.Imaging.BitmapTypedValue(
                         aperture.Denominator,
                         Windows.Foundation.PropertyType.UInt32);
                    propertySet.Add("System.Photo.ApertureNumerator", ApertureValueN);
                    propertySet.Add("System.Photo.ApertureDenominator", ApertureValueD);


                    //Focal length
                    var focal_len = Fraction.Parse(image.Exif.focal_len);

                    var focal_lenN = new Windows.Graphics.Imaging.BitmapTypedValue(
                        focal_len.Numerator,
                        Windows.Foundation.PropertyType.UInt32);

                    var focal_lenD = new Windows.Graphics.Imaging.BitmapTypedValue(
                        focal_len.Denominator,
                         Windows.Foundation.PropertyType.UInt32);
                    propertySet.Add("System.Photo.FocalLengthNumerator", focal_lenN);
                    propertySet.Add("System.Photo.FocalLengthDenominator", focal_lenD);


                    //ISO                   
                    var iso = new Windows.Graphics.Imaging.BitmapTypedValue(
                        Convert.ToInt16(Math.Round(image.Exif.iso_speed)),
                        Windows.Foundation.PropertyType.UInt16);
                    propertySet.Add("System.Photo.ISOSpeed", iso);


                    //shutter
                    var shutter = Fraction.Parse(image.Exif.shutter);
                    var shutterN = new Windows.Graphics.Imaging.BitmapTypedValue(
                        shutter.Numerator,
                        Windows.Foundation.PropertyType.UInt32);

                    var shutterD = new Windows.Graphics.Imaging.BitmapTypedValue(
                        shutter.Denominator,
                         Windows.Foundation.PropertyType.UInt32);
                    propertySet.Add("System.Photo.ShutterSpeedNumerator", shutterN);
                    propertySet.Add("System.Photo.ShutterSpeedDenominator", shutterD);


                    //System.Photo.CameraModel                   
                    var model = new Windows.Graphics.Imaging.BitmapTypedValue(
                        image.Exif.model,
                         Windows.Foundation.PropertyType.String);
                    propertySet.Add("System.Photo.CameraModel", model);


                    //System.Photo.DateTaken
                    var DateTaken = new Windows.Graphics.Imaging.BitmapTypedValue(
                        new DateTimeOffset(PhoneHelper.UnixTimeStampToDateTime(image.Exif.timestamp)),
                        Windows.Foundation.PropertyType.DateTime);
                    propertySet.Add("System.Photo.DateTaken", DateTaken);

                    //Flash
                    var Flash = new Windows.Graphics.Imaging.BitmapTypedValue(
                       Convert.ToInt16(Math.Round(image.Exif.flash)),
                       Windows.Foundation.PropertyType.Int16);
                    propertySet.Add("System.Photo.Flash", Flash);

                    //GPS 

                    //Lat
                    int degree = Convert.ToInt32(Math.Truncate(image.Exif.gps_latitude));
                    int minute = Convert.ToInt32(Math.Truncate((image.Exif.gps_latitude - degree) * 60));
                    int sec = Convert.ToInt32(Math.Truncate((((image.Exif.gps_latitude - degree) * 60) - minute) * 60));

                    int[] latitudeNumerator = new int[3] { degree, minute, sec };
                    int[] latitudeDenominator = new int[3] { 1, 1, 1 };


                    var latitudeN = new Windows.Graphics.Imaging.BitmapTypedValue(
                       latitudeNumerator,
                       Windows.Foundation.PropertyType.UInt32Array);

                    var latitudeD = new Windows.Graphics.Imaging.BitmapTypedValue(
                         latitudeDenominator,
                         Windows.Foundation.PropertyType.UInt32Array);


                    //Long
                    int degree2 = Convert.ToInt32(Math.Truncate(image.Exif.gps_longtitude));
                    int minute2 = Convert.ToInt32(Math.Truncate((image.Exif.gps_longtitude - degree2) * 60));
                    int sec2    = Convert.ToInt32(Math.Truncate((((image.Exif.gps_longtitude - degree2) * 60) - minute2) * 60));

                    int[] longitudeNumerator = new int[3] { degree2, minute2, sec2 };
                    int[] longitudeDenominator = new int[3] { 1, 1, 1 };

                    var longtN = new Windows.Graphics.Imaging.BitmapTypedValue(
                       longitudeNumerator,
                       Windows.Foundation.PropertyType.UInt32Array);

                    var longtD = new Windows.Graphics.Imaging.BitmapTypedValue(
                        longitudeDenominator,
                         Windows.Foundation.PropertyType.UInt32Array);




                    propertySet.Add("System.GPS.LongitudeNumerator", longtN);
                    propertySet.Add("System.GPS.LongitudeDenominator", longtD);
                    //propertySet.Add("System.GPS.LongitudeRef", longitudeRef);

                    propertySet.Add("System.GPS.LatitudeNumerator", latitudeN);
                    propertySet.Add("System.GPS.LatitudeDenominator", latitudeD);
                    //propertySet.Add("System.GPS.LatitudeRef", latitudeRef);                    
                    

                }
                catch (SystemException ee) { }

            }
            
            return propertySet;
        }

        public enum FileFormat
        {
            JPEG= 0,
            //PNG,
            //BMP,
            TIFF= 1
            //GIF
        }

        



        public static async Task<StorageFile> EncodeToJpegFromPixMap(StorageFile file, int w, int h) {




            string filename;

            filename = System.IO.Path.GetFileNameWithoutExtension(file.Name) + "_rawer" + ".jpeg";

            int stride = w * 4;
            using (Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file)))

            using (BitmapImageSource s = new BitmapImageSource(bit))                          
            using (JpegRenderer r = new JpegRenderer(s))
            {
                AppSettings set = new AppSettings();
                
                r.Quality = (double)set.JpegQualitySetting / 100;
                var jpeg = await r.RenderAsync();
                
                bit.Dispose();
                s.Dispose();
                r.Dispose();
               

                return await StorageExplorer.WriteDataToFileAsync(filename, jpeg);
            }  
        }

        public static async Task<WriteableBitmap> ResizeAndEncodeToWRBitmap(byte[] file, int w, int h, int MaxSize) {

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            System.DateTime date = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("current memmory at start ResizeAndEncodeToWRBitmap - " + Windows.System.MemoryManager.AppMemoryUsage.ToString());

            int maxSize = MaxSize;

            WriteableBitmap wb;
            //WriteableBitmap wb2;

            //Если размер картинки больше MaxSize(4000 для главного экрана и 2000 для едитора в превью) по одной из сторон то ее нужно уменьшить до этого размера.
            if (Math.Max(w, h) > maxSize)
            {
                if (w > h)
                {
                    double d = ((double)maxSize / (double)w) * (double)h;
                    wb = new WriteableBitmap(maxSize, (int)Math.Round(d, 0));
                }
                else
                {
                    double d = ((double)maxSize / (double)h) * (double)w;
                    wb = new WriteableBitmap((int)Math.Round(d, 0), maxSize);
                }


                 int stride = w * 4;

                 using (Bitmap b = new Bitmap(new Windows.Foundation.Size(w, h),
                                          Lumia.Imaging.ColorMode.Bgra8888,
                                          (uint)stride,
                                          file.AsBuffer()))
                 {
                     
                     using (BitmapImageSource s = new BitmapImageSource(b))
                     {
                         using (var r = new WriteableBitmapRenderer(s, wb))
                         {
                             r.OutputOption = OutputOption.PreserveAspectRatio;

                             wb = await r.RenderAsync();
                             
                             b.Dispose();
                             s.Dispose();
                             r.Dispose();
                             
                            
                         }
                     }
                 }
            }
            else
            {

                wb = new WriteableBitmap(w, h);
                wb.FromByteArray(file);                
            }

            //wb = new WriteableBitmap(1, 1);

            //file = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            System.Diagnostics.Debug.WriteLine("current memmory after resize - " + Windows.System.MemoryManager.AppMemoryUsage.ToString());
            System.Diagnostics.Debug.WriteLine("Time taken to resize " + (DateTime.Now - date).TotalMilliseconds.ToString());
            
            return wb;
        
        }

        public static async Task<WriteableBitmap> ResizeAndEncodeToWRBitmap2(StorageFile file, int w, int h, int MaxSize)
        {

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            System.DateTime date = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("current memmory at start ResizeAndEncodeToWRBitmap - " + Windows.System.MemoryManager.AppMemoryUsage.ToString());

            int maxSize = MaxSize;

            WriteableBitmap wb;
            //WriteableBitmap wb2;

            //Если размер картинки больше MaxSize(4000 для главного экрана и 2000 для едитора в превью) по одной из сторон то ее нужно уменьшить до этого размера.
            if (Math.Max(w, h) > maxSize)
            {
                if (w > h)
                {
                    double d = ((double)maxSize / (double)w) * (double)h;
                    wb = new WriteableBitmap(maxSize, (int)Math.Round(d, 0));
                }
                else
                {
                    double d = ((double)maxSize / (double)h) * (double)w;
                    wb = new WriteableBitmap((int)Math.Round(d, 0), maxSize);
                }


                int stride = w * 4;

                //var st = new Lumia.Imaging..StorageFileImageSource()
                //using (var s = await file.OpenReadAsync())
                using (Bitmap b = new Bitmap(new Windows.Foundation.Size(w, h),
                                         Lumia.Imaging.ColorMode.Bgra8888,
                                         (uint)stride,
                                         await Windows.Storage.FileIO.ReadBufferAsync(file)))
                {

                    using (BitmapImageSource s = new BitmapImageSource(b))
                    {
                        using (var r = new WriteableBitmapRenderer(s, wb))
                        {
                            r.OutputOption = OutputOption.PreserveAspectRatio;

                            wb = await r.RenderAsync();

                            b.Dispose();
                            s.Dispose();
                            r.Dispose();


                        }
                    }
                }
            }
            else
            {

                wb = new WriteableBitmap(w, h);
                var buffer = await FileIO.ReadBufferAsync(file);
                wb.FromByteArray(buffer.ToArray());
                buffer = null;
            }

            //wb = new WriteableBitmap(1, 1);

            //file = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            System.Diagnostics.Debug.WriteLine("current memmory after resize - " + Windows.System.MemoryManager.AppMemoryUsage.ToString());
            System.Diagnostics.Debug.WriteLine("Time taken to resize " + (DateTime.Now - date).TotalMilliseconds.ToString());

            return wb;

        }


        public static async Task RenderImage3(byte[] array, PanAndZoomImage img, int w, int h)
        {
            //img.Source = await ImageEncoder.ResizeAndEncodeToWRBitmap(array, w, h, MaxRenderSize);
            WriteableBitmap wb = await ImageEncoder.ResizeAndEncodeToWRBitmap(array, w, h, MaxRenderSize);
            img.MaximumZoom = CalculateMaxZoom(wb.PixelWidth, wb.PixelHeight);
            img.Source = wb;
            wb = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
        }

        public static async Task RenderImage4(StorageFile file, PanAndZoomImage img, int w, int h)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            //img.Source = await ImageEncoder.ResizeAndEncodeToWRBitmap(array, w, h, MaxRenderSize);
            WriteableBitmap wb = await ImageEncoder.ResizeAndEncodeToWRBitmap2(file, w, h, MaxRenderSize);
            img.MaximumZoom = CalculateMaxZoom(wb.PixelWidth, wb.PixelHeight);
            img.Source = wb;
            wb = null;         

        }

        private static async void SaveToDiskAndPutGlobalVar(string path, ulong imgsize, byte[] smallpicture)
        {
            DateTime beforeWriteData = System.DateTime.Now;

            StorageFile file = await StorageExplorer.WriteDataToFileAsync(System.IO.Path.GetFileNameWithoutExtension(path) + ".pixmap", smallpicture);
            System.Diagnostics.Debug.WriteLine("WriteData take time in milliseconds " + (DateTime.Now - beforeWriteData).TotalMilliseconds.ToString());
            (App.Current as App).BundleImage.PixMapFile = file;

            smallpicture = null;


            //set this file available on another pages


        }

        public static async Task<byte[]> PrepareJpegFile(String file)
        {
            return await PrepareJpegFile(await StorageFile.GetFileFromPathAsync(file));

        }

        public static async Task<byte[]> PrepareJpegFile(StorageFile file) {

            byte[] array = null;

            ulong imgsize = await StorageExplorer.GetFileSize(file.Path).ConfigureAwait(false);

            using (StorageFileImageSource source = new StorageFileImageSource(file)) {

                var t = await source.GetInfoAsync();
                var size = t.ImageSize;

                using (Bitmap b = await source.GetBitmapAsync(new Bitmap(size, ColorMode.Bgra8888), OutputOption.PreserveAspectRatio) )
                {
                    int w = (int)Math.Round(b.Dimensions.Width);
                    int h = (int)Math.Round(b.Dimensions.Height);

                    array = b.Buffers[0].Buffer.ToArray();
                    var histo = CalculateHistogram(array,  w, h);

                    LibrawRuntimeComponent.LibRawExif exif = default(LibrawRuntimeComponent.LibRawExif);
                    exif.empty = true;
                    

                    (App.Current as App).BundleImage = new Models.BundleImage(null,
                                                                        exif,
                                                                        w,
                                                                        h,
                                                                        imgsize,
                                                                        file.Path,
                                                                        histo);

                    SaveToDiskAndPutGlobalVar(file.Path, imgsize, array);

                   
                }
            }

            return array;

        }



        static double CalculateMaxZoom(int w, int h)
        {

            double factor = 0;
            if (w >= 480)
            {
                factor = w / 480 * 3;
            }

            return factor;

        }

        public static void NormilizeColorChannelsValuesForHistogram(ref int[] red, ref int[] green, ref int[] blue)
        {
            int[] red1 = new int[256];
            int[] green1 = new int[256];
            int[] blue1 = new int[256];

            red.CopyTo(red1, 0);
            green.CopyTo(green1, 0);
            blue.CopyTo(blue1, 0);

            red1[0] = 0;
            green1[0] = 0;
            blue1[0] = 0;
            red1[255] = 0;
            green1[255] = 0;
            blue1[255] = 0;
            if (red[0] > red1.Max()) { red[0] = red1.Max(); }
            if (green[0] > green1.Max()) { green[0] = green1.Max(); }
            if (blue[0] > blue1.Max()) { blue[0] = blue1.Max(); }

            if (red[255] > red1.Max()) { red[255] = red1.Max(); }
            if (green[255] > green1.Max()) { green[255] = green1.Max(); }
            if (blue[255] > blue1.Max()) { blue[255] = blue1.Max(); }
        }

        public static async Task<Bitmap> SetWaterMark(StorageFile file, int w, int h)
        {
            Windows.Foundation.Size frameSize = new Windows.Foundation.Size(w, h);
            Uri _blendImageUri = new Uri("Assets/watermark.png",UriKind.Relative);

            
            var scanlineByteSize = (uint)frameSize.Width * 4; // 4 bytes per pixel in BGRA888 mode
            var bitmap = new Bitmap(frameSize, ColorMode.Bgra8888, scanlineByteSize, await Windows.Storage.FileIO.ReadBufferAsync(file));

            StreamImageSource _blendImageProvider = new StreamImageSource((System.Windows.Application.GetResourceStream(_blendImageUri).Stream));

            BlendFilter _blendFilter = new BlendFilter(_blendImageProvider, BlendFunction.Normal, 1);
            _blendFilter.TargetArea = new Windows.Foundation.Rect(new Windows.Foundation.Point(0.2, 0.9), new Windows.Foundation.Point(0.4, 0.97));
            _blendFilter.TargetOutputOption = OutputOption.PreserveAspectRatio;

            //BlendFilter _blendFilter2 = new BlendFilter(_blendImageProvider, BlendFunction.Normal, 1);
            //_blendFilter2.TargetArea = new Windows.Foundation.Rect(new Point(0.4, 0.1), new Point(0.6, 0.17));
            //_blendFilter2.TargetOutputOption = OutputOption.PreserveAspectRatio;

            BlendFilter _blendFilter3 = new BlendFilter(_blendImageProvider, BlendFunction.Normal, 1);
            _blendFilter3.TargetArea = new Windows.Foundation.Rect(new Windows.Foundation.Point(0.6, 0.9), new Windows.Foundation.Point(0.8, 0.97));
            _blendFilter3.TargetOutputOption = OutputOption.PreserveAspectRatio;

            FilterEffect _filterEffect = new FilterEffect();

            BitmapImageSource s = new BitmapImageSource(bitmap);

            var filters = new List<IFilter> { _blendFilter, _blendFilter3 };
            _filterEffect.Filters = filters;
            _filterEffect.Source = s;
          

            var renderer = new BitmapRenderer(_filterEffect, bitmap);
            var bit = await renderer.RenderAsync();

            _blendFilter.Dispose();
            _blendFilter3.Dispose();
            _filterEffect.Dispose();
            s.Dispose();                    
            _blendImageProvider.Dispose();
            renderer.Dispose();   

            return bit;
        }

        static public  void ReCalculateHistogram(byte[] array, int w, int h)
        {
             (App.Current as App).BundleImage.histogramData = CalculateHistogram(array, w, h);
        }

        static Rawer.Models.Histogram CalculateHistogram(byte[] array, int w, int h) {


            int[] red = new int[256];
            int[] green = new int[256];
            int[] blue = new int[256];
            int index2 = 0;
            int length = w * h * 4;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    index2 = (x + y * w) * 4;

                    //Заполняем для гистограммы
                    red[array[index2 + 2]] = red[array[index2 + 2]] + 1;
                    //System.Diagnostics.Debug.WriteLine(array[index2] + "  color count-" + red[array[index2]]);
                    green[array[index2 + 1]] = green[array[index2 + 1]] + 1;
                    blue[array[index2]] = blue[array[index2]] + 1;
                }
            }

            NormilizeColorChannelsValuesForHistogram(ref red, ref green, ref blue);


            Rawer.Models.Histogram histo = new Models.Histogram();
            histo.Blue = blue;
            histo.Red = red;
            histo.Green = green;

            return histo;

        }

        //public static async Task<WriteableBitmap> ProcessFilter(Models.UsedFilterList FilterList, int w, int h, WriteableBitmap input)
        //{
           
        //    int FinalW = w;
        //    int FinalH = h;

        //    var target = new WriteableBitmap(FinalW, FinalH);
            
        //    using (BitmapImageSource source = new BitmapImageSource(input.AsBitmap()))
        //    {
        //        var filters = new FilterEffect(source);
        //        if (FilterList.FinalCropTool)
        //        {
        //            AddCropFilterIfNeeded(ref filters, FilterList.FinalCropToolValue, w, h);

        //            FinalW = (int)Math.Round(FilterList.FinalCropToolValue.CropRect.Width * w, 0);
        //            FinalH = (int)Math.Round(FilterList.FinalCropToolValue.CropRect.Height * h, 0);
        //        }

        //        if (FilterList.FinalOrientationTool)
        //        {

        //            AddFlipFilterIfNeeded(ref filters, FilterList.FinalOrientationToolValue);
        //        }

        //        if (FilterList.FinalBrightnessTool)
        //        {

        //            AddBrightnessFilterIfNeeded(ref filters, FilterList.FinalBrightnessToolValue);

        //        }

        //        if (FilterList.FinalContrastTool)
        //        {

        //            AddContrastFilterIfNeeded(ref filters, FilterList.FinalContrastToolValue);

        //        }

        //        if (FilterList.FinalHueTool)
        //        {

        //            AddHueFilterIfNeeded(ref filters, FilterList.FinalHueToolValue);

        //        }

        //        if (FilterList.FinalSaturationTool)
        //        {

        //            AddSaturationFilterIfNeeded(ref filters, FilterList.FinalSaturationToolValue);

        //        }

        //        if (FilterList.FinalSharpenTool)
        //        {

        //            AddSharpenFilterIfNeeded(ref filters, FilterList.FinalSharpenToolValue);

        //        }

               




        //        using (var renderer = new WriteableBitmapRenderer(filters, target))
        //        {
        //            // Render the image with the filter(s)
        //            await renderer.RenderAsync();
        //        }
        //    }

        //    return target;
        
        //}





        //public static async void ProcessAllFilters(Models.UsedFilterList FilterList, int w, int h)
        //{
        //    int FinalW = w;
        //    int FinalH = h;


        //    int stride = w * 4;
        //    using (Bitmap b = new Bitmap(new Windows.Foundation.Size(w, h),
        //                                  Nokia.Graphics.Imaging.ColorMode.Bgra8888,
        //                                  (uint)stride,
        //                                  await Windows.Storage.FileIO.ReadBufferAsync((App.Current as App).BundleImage.PixMapFile)))
        //    {
        //        using (BitmapImageSource source = new BitmapImageSource(b))
        //        {

        //            // Create effect collection with the source stream
        //            var filters = new FilterEffect(source);

        //            if (FilterList.FinalCropTool)
        //            {
        //                AddCropFilterIfNeeded(ref filters, FilterList.FinalCropToolValue, w, h);

        //                FinalW = (int)Math.Round(FilterList.FinalCropToolValue.CropRect.Width * w, 0);
        //                FinalH = (int)Math.Round(FilterList.FinalCropToolValue.CropRect.Height * h, 0);
        //            }

        //            if (FilterList.FinalOrientationTool)
        //            {

        //                AddFlipFilterIfNeeded(ref filters, FilterList.FinalOrientationToolValue);
        //            }

        //            if (FilterList.FinalBrightnessTool)
        //            {

        //                AddBrightnessFilterIfNeeded(ref filters, FilterList.FinalBrightnessToolValue);

        //            }

        //            if (FilterList.FinalContrastTool)
        //            {

        //                AddContrastFilterIfNeeded(ref filters, FilterList.FinalContrastToolValue);

        //            }

        //            if (FilterList.FinalHueTool)
        //            {

        //                AddHueFilterIfNeeded(ref filters, FilterList.FinalHueToolValue);

        //            }

        //            if (FilterList.FinalSaturationTool)
        //            {

        //                AddSaturationFilterIfNeeded(ref filters, FilterList.FinalSaturationToolValue);

        //            }

        //            if (FilterList.FinalSharpenTool)
        //            {

        //                AddSharpenFilterIfNeeded(ref filters, FilterList.FinalSharpenToolValue);

        //            }


        //            // Create a target where the filtered image will be rendered to
        //            var target = new WriteableBitmap(FinalW, FinalH);


        //            // Create a new renderer which outputs WriteableBitmaps
        //            using (var renderer = new WriteableBitmapRenderer(filters, target))
        //            {
        //                // Render the image with the filter(s)
        //                await renderer.RenderAsync();

        //                using (MemoryStream m = new MemoryStream())
        //                {
        //                    target.SaveJpeg(m, FinalW, FinalH, 0, 95);
        //                    m.Flush();
        //                    m.Seek(0, SeekOrigin.Begin);

        //                    MediaLibrary library = new MediaLibrary();
        //                    library.SavePicture("CROPED_BIG_ONE.jpeg", m);
        //                    target = null;
        //                    m.Dispose();

        //                }

        //                //JpegRenderer r = new JpegRenderer(;
        //                //r.Quality = 0.9;
        //                //var jpeg = await r.RenderAsync();

        //                // Set the output image to Image control as a source
        //                //ImageControl.Source = target;
        //                // }

        //            }
        //        }
        //    }

        //}

        //private static void AddSharpenFilterIfNeeded(ref FilterEffect Filters, double SharpenTool)
        //{
        //    var sharpnessFilter = new SharpnessFilter(5);

        //    Filters.Filters = Filters.Filters.Concat(new[] { sharpnessFilter });
        //}

        //private static void AddSaturationFilterIfNeeded(ref FilterEffect Filters, double SaturationTool)
        //{
        //    var hueFilter = new HueSaturationFilter(0,SaturationTool);

        //    Filters.Filters = Filters.Filters.Concat(new[] { hueFilter });
        //}

        //private static void AddHueFilterIfNeeded(ref FilterEffect Filters, double HueTool)
        //{
        //    //If you want to normalize to [x, y], first normalize to [0, 1] via:

        //    //     range = max(a) - min(a);
        //    //     a = (a - min(a)) / range;

        //    //    Then scale to [x,y] via:
        //    //     range2 = y - x;
        //    //     a = (a*range2) + x;
            
            
            
        //    double normalized_x =(((HueTool + 180) / 360 ) * 2 ) - 1;


        //    var hueFilter = new HueSaturationFilter(normalized_x, 0);

        //    Filters.Filters = Filters.Filters.Concat(new[] { hueFilter });
        //}

        //private static void AddContrastFilterIfNeeded(ref FilterEffect Filters, double ContrastTool)
        //{
        //    var contrastFilter = new ContrastFilter(ContrastTool/100);

        //    Filters.Filters = Filters.Filters.Concat(new[] { contrastFilter });
        //}

        //private static void AddBrightnessFilterIfNeeded(ref FilterEffect Filters, double BrightnessTool)
        //{
           
        //            var brightnessFilter = new BrightnessFilter(BrightnessTool) ;

        //            Filters.Filters = Filters.Filters.Concat(new[] { brightnessFilter });
        //       // 
        //}

        //private static void AddFlipFilterIfNeeded(ref FilterEffect Filters, OrientationTool OrientationTool)
        //{
        //    FlipFilter flipFilter = new FlipFilter();
            
        //    if (OrientationTool.IsFlippedVertical)
        //    {
        //        flipFilter = new FlipFilter(FlipMode.Vertical);
        //    }

        //    if (OrientationTool.IsFlippedHorizontal)
        //    {
        //        flipFilter = new FlipFilter(FlipMode.Horizontal);
        //    }

        //    if (OrientationTool.IsFlippedVertical && OrientationTool.IsFlippedHorizontal)
        //    {
        //        flipFilter = new FlipFilter(FlipMode.Both);
        //    }

        //    Filters.Filters = Filters.Filters.Concat(new[] { flipFilter });

        //}

        //private static void AddCropFilterIfNeeded(ref FilterEffect Filters, CropTool FinalCrop, int W, int H)
        //{
        //    CropFilter cropFilter = new CropFilter(new Windows.Foundation.Rect(FinalCrop.CropRect.X * W, FinalCrop.CropRect.Y * H, FinalCrop.CropRect.Width * W, FinalCrop.CropRect.Height * H));

        //    Filters.Filters = Filters.Filters.Concat(new[] { cropFilter });

        //}

    }
}
