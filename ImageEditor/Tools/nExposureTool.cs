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
using Windows.Storage;

namespace ImageEditor
{

    public class nExposureTool : RangeTool
    {
        public nExposureTool()
        {
            this.Value =0;
           
            
        }

        bool firsttime = true;
        int count;
        bool iteration;
        WriteableBitmap b1;
        WriteableBitmap b2;

        // Inherited from ImageEditorTool
        public override string Icon
        {
            get
            {
                // The icon must be added as a Resource to the project under the Assets folder for this path to work.
                return @"/ImageEditor;Component/Assets/exposure2.png";
            }
        }

        // Inherited from ImageEditorTool
        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.ExposureTool;
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

        // Inherited from RangeTool
        async protected override void OnValueChanged(double newValue, double oldValue)
        {
            //var past = System.DateTime.Now;
            if (this.PreviewImage == null)
            {
                return;
            }
           
            // Apply algorithm to the temporary working bitmap.
            WriteableBitmap wb = new WriteableBitmap(this.PreviewImage.PixelWidth,this.PreviewImage.PixelHeight);       

          
            this.ModifiedImage = await this.Exposure(this.PreviewImage, newValue, this.PreviewImage.PixelWidth, this.PreviewImage.PixelHeight, wb);
        
            
            if (count >= 10)
            {
               GC.Collect();
               count = 0;
            }
 
            //System.Diagnostics.Debug.WriteLine((System.DateTime.Now - past).TotalMilliseconds.ToString());
            count += 1;
            // Set the result image so that the view can be updated.
            //this.ModifiedImage = this.workingBitmap;
        }

        // Inherited from ImageEditorTool
        protected override async System.Threading.Tasks.Task<WriteableBitmap> ApplyCore(WriteableBitmap actualImage)
        {

            var File = ImageEditorControl.Instance.BigFile;
            int w = ImageEditorControl.Instance.BiGFileWidth;
            int h = ImageEditorControl.Instance.BiGFileHeight;
            double value = this.Value;

            actualImage = await this.Exposure(actualImage, value, actualImage.PixelWidth, actualImage.PixelHeight, false);

            DateTime now = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Start proccess big picture");


           // WriteableBitmap wb = await Helper.GetWriteableBitmap(File, w, h);


            var wb = await this.Exposure(File, value, w, h);

            await Helper.WriteDataToFileAsync(File, wb.Buffers[0].Buffer);
            wb = null;

            System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());


            GC.Collect();

            return actualImage;
        }


        

        async Task<WriteableBitmap> Exposure(WriteableBitmap wb ,double newValue, int w, int h, bool ext)
        {
            return await Exposure(wb, newValue, w, h, wb);
        }

        async Task<Bitmap> Exposure(StorageFile file, double newValue, int w, int h)
        {
            int stride = w * 4;
            Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file));

            using (BitmapImageSource s = new BitmapImageSource(bit))
            using (var filters = new FilterEffect(s))
            {
                ExposureFilter filter = new ExposureFilter(ExposureMode.Natural, newValue);
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


        async Task<WriteableBitmap> Exposure(WriteableBitmap wb, double newValue, int w, int h, WriteableBitmap ext)
        {
            //var target = new WriteableBitmap(w, h);
            //Uri uri = new Uri("Assets/IMG_7136.jpg", UriKind.Relative);

            using (var source = new BitmapImageSource(wb.AsBitmap()))
            //using (var source = new StreamImageSource(Application.GetResourceStream(uri).Stream))
            {

                // Create effect collection with the source stream
                using (var filters = new FilterEffect(source))
                {

                    ExposureFilter filter = new ExposureFilter(ExposureMode.Natural, newValue);

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

    }
}
