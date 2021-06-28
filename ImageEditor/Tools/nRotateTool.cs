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
using Lumia.Imaging.Adjustments;
using System.ComponentModel;
using System.Windows.Media;
using Lumia.Imaging.Transforms;
using Windows.Storage;

namespace ImageEditor
{
    public class nRotateTool : RangeTool, INotifyPropertyChanged
    {
        bool start = true;
        double originHeight; 
        double originWidth;


        public nRotateTool()
        {           

            this.Value = 0;
            PreviewScaleX = 1;
            PreviewScaleY = 1;           

        }

        int count;


        private RectangleGeometry clippingRectangleGeometry;
        public RectangleGeometry ClippingRectangleGeometry
        {
            get
            {
                return clippingRectangleGeometry;
            }
            set
            {
                clippingRectangleGeometry = value;
                base.OnPropertyChanged("ClippingRectangleGeometry");

            }
        }

        public override bool SupportsComparison
        {
            get
            {
                return false;
            }
        }

        // Inherited from ImageEditorTool
        public override string Icon
        {
            get
            {
                // The icon must be added as a Resource to the project under the Assets folder for this path to work.
                return @"/ImageEditor;Component/Assets/orientation2.png";
            }
        }

        // Inherited from ImageEditorTool
        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.RotateTool;
            }
        }

        private double _rotation;

       
        public double Rotation
        {
            get {return _rotation;}
            set {

                _rotation = value;
                base.OnPropertyChanged("Rotation");
            
            }
        }

        double _previewScaleX;
        double _previewScaleY;
        public double PreviewScaleX
        {

            get { return _previewScaleX; }
            set {

                _previewScaleX = value;
                base.OnPropertyChanged("PreviewScaleX");
            
            }
        }


        public double PreviewScaleY
        {

            get { return _previewScaleY; }
            set
            {

                _previewScaleY= value;
                base.OnPropertyChanged("PreviewScaleY");

            }
        }

        // Inherited from RangeTool
        public override double Max
        {
            get
            {
                return 1;
            }
        }

        // Inherited from RangeTool
        public override double Min
        {
            get
            {
                return -1;
            }
        }


        Size calculateLargestProportionalRect(double angleD, double origWidth, double origHeight)
        {
            double angle = angleD * 0.0174532925;
            
            
            var w =origWidth;
            var h = origHeight;
            bool width_is_longer = false;
       
            double side_long;
            double side_short;
            //"""
            //Given a rectangle of size wxh that has been rotated by 'angle' (in
            //radians), computes the width and height of the largest possible
            //axis-aligned rectangle (maximal area) within the rotated rectangle.
            //"""
            if (w <= 0 || h <= 0)
            return new Size(0,0);

            if(w >= h)
            {
                 width_is_longer = true;
                 side_long = w;
                 side_short = h;
            }else
            {
                side_long = h;
                side_short = w;            
            }
           
            double sin_a = Math.Abs(Math.Sin(angle));
            double cos_a = Math.Abs(Math.Cos(angle));

            double wr,hr;


            if (side_short <= 2.0 * sin_a * cos_a * side_long)
            {
                var x = 0.5 * side_short;

                if (width_is_longer)
                {
                    wr = x / sin_a;
                    hr = x / cos_a;
                }
                else
                {

                    wr = x / cos_a;
                    hr = x / sin_a;
                }
            }
            else
            {
                var cos_2a = cos_a * cos_a - sin_a * sin_a;

                wr = (w * cos_a - h * sin_a) / cos_2a;
                hr = (h * cos_a - w * sin_a) / cos_2a;


            }

            return new Size(wr, hr);
  
        }
             
        
        // Inherited from RangeTool
        protected override void OnValueChanged(double newValue, double oldValue)
        {
            if (this.PreviewImage == null)
            {
                return;
            }


            if (start) {
                originHeight = ModifiedImage.PixelHeight;
                originWidth = ModifiedImage.PixelWidth;               

                start = false;                        
            }

  
            this.ResetWorkingBitmap();

            this.Rotation = (newValue*25);
            var Size = calculateLargestProportionalRect(this.Rotation, originWidth, originHeight);


            if ( Size.Height > Size.Width)
            {
                PreviewScaleX = PreviewScaleY = (1.0 - (Size.Width / originWidth)) + 1.0;
            }
            else {
                PreviewScaleY = PreviewScaleX = (1.0 - (Size.Height / originHeight)) + 1.0;            
            
            }
   
        }

        // Inherited from ImageEditorTool
        protected override async System.Threading.Tasks.Task<WriteableBitmap> ApplyCore(WriteableBitmap actualImage)
        {
            var File = ImageEditorControl.Instance.BigFile;
            int w = ImageEditorControl.Instance.BiGFileWidth;
            int h = ImageEditorControl.Instance.BiGFileHeight;
            double value = this.Value;


            //Proccess Small Image
            var Size = calculateLargestProportionalRect(this.Rotation, actualImage.PixelWidth, actualImage.PixelHeight);

            double x = (actualImage.PixelWidth / 2) - (Size.Width / 2);
            double y = (actualImage.PixelHeight / 2) - (Size.Height / 2);

            Windows.Foundation.Rect rect = new Windows.Foundation.Rect(x, y, Size.Width, Size.Height);
            actualImage = await this.RotateCropWB(actualImage, Rotation, rect, actualImage.PixelWidth, actualImage.PixelHeight, false);
            GC.Collect();
           
            DateTime now = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Start proccess big picture");


            //Proccess BIG image
          
            var Size2 = calculateLargestProportionalRect(this.Rotation, w, h);
            double x2 = (w / 2) - (Size2.Width / 2);
            double y2 = (h / 2) - (Size2.Height / 2);

            Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(x2, y2, Size2.Width, Size2.Height);
            var wb = await this.RotateCrop(File, Rotation, rect2, w, h, false);
            if (wb != null)
            {
                ImageEditorControl.Instance.BiGFileHeight = (int)Math.Round(wb.Dimensions.Height);
                ImageEditorControl.Instance.BiGFileWidth = (int)Math.Round(wb.Dimensions.Width);
                await Helper.WriteDataToFileAsync(File, wb.Buffers[0].Buffer);
                wb.Dispose();
                wb = null;
            }
            System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());

           
            ResetWorkingBitmap();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            return actualImage;
        }

        async Task<WriteableBitmap> RotateCropWB(WriteableBitmap wb, double angle, Windows.Foundation.Rect rect, int w, int h, bool ext)
        {
            var target = new WriteableBitmap((int)Math.Round(rect.Width),(int)Math.Round( rect.Height));

            using (var source = new BitmapImageSource(wb.AsBitmap()))
            //using (var source = new StreamImageSource(Application.GetResourceStream(uri).Stream))
            {

                // Create effect collection with the source stream
                using (var filters = new FilterEffect(source))
                {

                    ReframingFilter filter = new ReframingFilter(rect, angle * (-1.0));
                    filters.Filters = new IFilter[] { filter };

                    // Create a new renderer which outputs WriteableBitmaps
                    using (var renderer = new WriteableBitmapRenderer(filters, target))
                    {
                        // Render the image with the filter(s)
                        await renderer.RenderAsync();

                        renderer.Dispose();
                        source.Dispose();
                        filters.Dispose();
                    }
                }
            }
            return target;        
            
        }

        
        async Task<Bitmap> RotateCrop(StorageFile file, double angle, Windows.Foundation.Rect rect, int w, int h, bool ext)
        {      
            //create target bitmap
            Bitmap target = new Bitmap(new Windows.Foundation.Size(rect.Width, rect.Height), ColorMode.Bgra8888);
            
            //load big file
            int stride = w * 4;
            using (Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file)))            
            using (BitmapImageSource s = new BitmapImageSource(bit))
            using (var filters = new FilterEffect(s))
            {
                ReframingFilter filter = new ReframingFilter(rect, angle * (-1.0));
                filters.Filters = new IFilter[] { filter };

                using (BitmapRenderer renderer = new BitmapRenderer(filters))
                {
                    renderer.Size = new Windows.Foundation.Size(rect.Width, rect.Height);
                    renderer.OutputOption = OutputOption.Stretch;
                    target = await renderer.RenderAsync();

                    bit.Dispose();
                    s.Dispose();
                    filters.Dispose();
                    renderer.Dispose();
                }
            }

            return target;
            
        }


    }
}
