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
    public class nSharpnessTool : RangeTool
    {
        public nSharpnessTool()
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
                return @"/ImageEditor;Component/Assets/Triangle-96.png";
            }
        }

        // Inherited from ImageEditorTool
        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.SharpnessTool;
            }
        }
        
       

        // Inherited from RangeTool
        public override double Max
        {
            get
            {
                return 0.6;
            }
        }

        // Inherited from RangeTool
        public override double Min
        {
            get
            {
                return -0.6;
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
            this.ModifiedImage = await this.Sharpness(this.PreviewImage, newValue, this.PreviewImage.PixelWidth, this.PreviewImage.PixelHeight, wb);
            //this.ApplyRedness(newValue, this.workingBitmap.Pixels);

            //this.workingBitmap = this.ModifiedImage;
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
            int w = ImageEditorControl.Instance.BiGFileWidth;
            int h = ImageEditorControl.Instance.BiGFileHeight;
            double value = this.Value;

            actualImage = await this.Sharpness(actualImage, value, actualImage.PixelWidth, actualImage.PixelHeight, false);

            DateTime now = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Start proccess big picture");


            //WriteableBitmap wb = await Helper.GetWriteableBitmap(File, w, h);


            var wb = await this.Sharpness(File, value, w, h);

            await Helper.WriteDataToFileAsync(File, wb.Buffers[0].Buffer);
            wb = null;

            System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());

           
            ResetWorkingBitmap();

            GC.Collect();

            return actualImage;
        }

        async Task<WriteableBitmap> Sharpness(WriteableBitmap wb, double newValue, int w, int h, bool ext)
        {
            return await Sharpness(wb, newValue, w, h, wb);
        }

        async Task<Bitmap> Sharpness(StorageFile file, double newValue, int w, int h)
        {
            int stride = w * 4;
            Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file));

            using (BitmapImageSource s = new BitmapImageSource(bit))
            using (var filters = new FilterEffect(s))
            {
                filters.Filters = new IFilter[] { GetFilter(newValue) };

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



        async Task<WriteableBitmap> Sharpness(WriteableBitmap wb, double newValue, int w, int h, WriteableBitmap ext)
        {
            //var target = new WriteableBitmap(w, h);
            //Uri uri = new Uri("Assets/IMG_7136.jpg", UriKind.Relative);
            
            using (var source = new BitmapImageSource(wb.AsBitmap()))
            //using (var source = new StreamImageSource(Application.GetResourceStream(uri).Stream))
            {

                // Create effect collection with the source stream
                using (var filters = new FilterEffect(source))
                {                 

                    filters.Filters = new IFilter[] { GetFilter(newValue) };
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

        private static IFilter GetFilter(double newValue)
        {
            if (newValue < 0)
            {
                int kernelsize = (int)Math.Round((newValue * (-32)) + 1, 0); //256 is to many for blur effect


                BlurFilter filter = new BlurFilter(kernelsize);

                return filter;

            }
            else
            {
                SharpnessFilter filter = new SharpnessFilter(newValue);
                return filter;
                // Add the filter to the FilterEffect collection

            }

        }

    }
}
