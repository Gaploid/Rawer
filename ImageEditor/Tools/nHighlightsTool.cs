using Microsoft.Xna.Framework.Media;
using Lumia.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Lumia.InteropServices.WindowsRuntime;
using Windows.UI.Core;
using System.Threading;
using Lumia.Imaging.Adjustments;
using Windows.Storage;

namespace ImageEditor
{
    public class nHighlightsTool : RangeTool
    {
        public nHighlightsTool()
        {
            this.Value =0;
            
        }

        int count;

        // Inherited from ImageEditorTool
        public override string Icon
        {
            get
            {
                // The icon must be added as a Resource to the project under the Assets folder for this path to work.
                return @"/ImageEditor;Component/Assets/Highligths.png";
            }
        }

        // Inherited from ImageEditorTool
        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.HighlightsTool;
            }
        }

        // Inherited from RangeTool
        public override double Max
        {
            get
            {
                return 32;
            }
        }

        // Inherited from RangeTool
        public override double Min
        {
            get
            {
                return -32;
            }
        }

        // Inherited from RangeTool
        async protected override void OnValueChanged(double newValue, double oldValue)
        {
              if (this.PreviewImage == null)
                {
                    return;
                }

                // Sets the working bitmap to have the same pixels as the original image.
               WriteableBitmap wb = new WriteableBitmap(this.PreviewImage.PixelWidth, this.PreviewImage.PixelHeight);       


                // Apply algorithm to the temporary working bitmap.
               this.ModifiedImage = await this.Highlights(this.PreviewImage, newValue, this.PreviewImage.PixelWidth, this.PreviewImage.PixelHeight, wb);
                //this.ApplyRedness(newValue, this.workingBitmap.Pixels);

                if (count >= 10)
                {
                    GC.Collect();
                    count = 0;

                }
                count += 1;
            // Set the result image so that the view can be updated.
            //this.ModifiedImage = this.workingBitmap;
        }

        // Inherited from ImageEditorTool
        protected override async System.Threading.Tasks.Task<WriteableBitmap> ApplyCore(WriteableBitmap actualImage)
        {
           var File = ImageEditorControl.Instance.BigFile;
           int w =  ImageEditorControl.Instance.BiGFileWidth;
           int h = ImageEditorControl.Instance.BiGFileHeight;         
           double value = this.Value;

           actualImage = await this.Highlights(actualImage, value, actualImage.PixelWidth, actualImage.PixelHeight, false);
           
           DateTime now = DateTime.Now;
           System.Diagnostics.Debug.WriteLine("Start proccess big picture");
         
           
           //WriteableBitmap wb = await Helper.GetWriteableBitmap(File, w, h);


           var wb = await this.Highlights(File, value, w, h);

           await Helper.WriteDataToFileAsync(File, wb.Buffers[0].Buffer);
           wb = null;

           System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());            
           

           GC.Collect(); 
          
           return actualImage;
        }

        async Task<WriteableBitmap> Highlights(WriteableBitmap wb, double newValue, int w, int h, bool ext)
        {
            return await Highlights(wb, newValue, w, h, wb);
        
        }

        async Task<Bitmap> Highlights(StorageFile file, double newValue, int w, int h)
        {
            int stride = w * 4;
            Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file));

            using (BitmapImageSource s = new BitmapImageSource(bit))
            using (var filters = new FilterEffect(s))
            {
                CurvesFilter filter = GetCurve(newValue);

                // Add the filter to the FilterEffect collection
                filters.Filters = new IFilter[] { filter };
              
                using (BitmapRenderer renderer = new BitmapRenderer(filters, bit))
                {
                    await renderer.RenderAsync();
                    s.Dispose();
                    filters.Dispose();
                    renderer.Dispose();
                }
            }

            return bit;

        }


        async Task<WriteableBitmap> Highlights(WriteableBitmap wb, double newValue, int w, int h, WriteableBitmap ext)
        {
            //var target = new WriteableBitmap(w, h);
            //Uri uri = new Uri("Assets/IMG_7136.jpg", UriKind.Relative);
            
            using (var source = new BitmapImageSource(wb.AsBitmap()))
            //using (var source = new StreamImageSource(Application.GetResourceStream(uri).Stream))
            {

                // Create effect collection with the source stream
                using (var filters = new FilterEffect(source))
                {
                    
                    //BlurFilter filter = new BlurFilter((int)newValue);

                    CurvesFilter filter = GetCurve(newValue);

                    // Add the filter to the FilterEffect collection
                    filters.Filters = new IFilter[] { filter };

                    // Create a new renderer which outputs WriteableBitmaps
                    using (var renderer = new WriteableBitmapRenderer(filters, ext))
                    {
                        // Render the image with the filter(s)
                        await renderer.RenderAsync();

                        renderer.Dispose();
                        source.Dispose();                        
                        filters.Dispose();
                    } 
                   

                } 
            }
            return ext;
        }

        private static CurvesFilter GetCurve(double newValue)
        {
            Curve mainCurve = new Curve();
            Windows.Foundation.Point[] points = new Windows.Foundation.Point[5];
            points[0].X = 0;
            points[0].Y = 0;

            points[1].X = 64;
            points[1].Y = 64;

            points[2].X = 128;
            points[2].Y = 128;

            points[3].X = 191;
            points[3].Y = newValue + 191;

            points[4].X = 255;
            points[4].Y = 255;

            mainCurve.Points = points;

            CurvesFilter filter = new CurvesFilter(mainCurve, mainCurve, mainCurve);
            return filter;
        }

    }


//    public static class TreeHelper
//    {
//        public static T FindParentByType<T>(this DependencyObject child) where T : DependencyObject
//        {
//            Type type = typeof(T);
//            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);

//            if (parent == null)
//            {
//                return null;
//            }
//            else if (parent.GetType() == type)
//            {
//                return parent as T;
//            }
//            else
//            {
//                return parent.FindParentByType<T>();
//            }
//        }
//    }
}
