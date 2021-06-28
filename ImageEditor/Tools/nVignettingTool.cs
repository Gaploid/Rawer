using Microsoft.Xna.Framework.Media;

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
using Lumia.Imaging;
using Windows.Storage;
using Lumia.Imaging.Artistic;

namespace ImageEditor
{
    public class nVignettingTool : RangeTool
    {

        Windows.UI.Color gray = new Windows.UI.Color { R = 104, G = 103, B = 104 };
       

        public nVignettingTool()
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
                return @"/ImageEditor;Component/Assets/contrast.png";
            }
        }

        // Inherited from ImageEditorTool
        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.VignettingTool;
            }
        }

        // Inherited from RangeTool
        public override double Max
        {
            get
            {
                return 0.7;
            }
        }

        // Inherited from RangeTool
        public override double Min
        {
            get
            {
                return 0;
            }
        }

        // Inherited from RangeTool
        async protected override void OnValueChanged(double newValue, double oldValue)
        {
            if (this.PreviewImage == null)
            {
                return;
            }

           
            WriteableBitmap wb = new WriteableBitmap(this.PreviewImage.PixelWidth, this.PreviewImage.PixelHeight);   

            // Apply algorithm to the temporary working bitmap.
           
            this.ModifiedImage = await this.Contrast(this.PreviewImage, newValue, this.PreviewImage.PixelWidth, this.PreviewImage.PixelHeight, wb);
          

            if (count >= 10)
            {
                GC.Collect();
                count = 0;
            }
            count += 1;
           
        }

        // Inherited from ImageEditorTool
        protected override async System.Threading.Tasks.Task<WriteableBitmap> ApplyCore(WriteableBitmap actualImage)
        {
            var File = ImageEditorControl.Instance.BigFile;
            int w = ImageEditorControl.Instance.BiGFileWidth;
            int h = ImageEditorControl.Instance.BiGFileHeight;
            double value = this.Value;

            actualImage = await this.Contrast(actualImage, value, actualImage.PixelWidth, actualImage.PixelHeight, false).ConfigureAwait(false);

            DateTime now = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Start proccess big picture");

            Bitmap wb = await this.Contrast(File, value, w, h).ConfigureAwait(false);

            await Helper.WriteDataToFileAsync(File, wb.Buffers[0].Buffer).ConfigureAwait(false);
            wb = null;

            System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());


            GC.Collect();

            return actualImage;
        }

        async Task<WriteableBitmap> Contrast(WriteableBitmap wb, double newValue, int w, int h, bool ext)
        {

            return await Contrast(wb, newValue, w, h, wb);
        }

        async Task<Bitmap> Contrast(StorageFile file, double newValue, int w, int h)
        {
            int stride = w * 4;
            Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file));

            using (BitmapImageSource s = new BitmapImageSource(bit))
            using (var filters = new FilterEffect(s))
            {
                Windows.UI.Color black = new Windows.UI.Color { R = 0, G = 0, B = 0, A = 0 };
               
                VignettingFilter filter = new VignettingFilter(0.1,black);
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


        async Task<WriteableBitmap> Contrast(WriteableBitmap wb ,double newValue, int w, int h, WriteableBitmap ext)
        {

            using (var source = new BitmapImageSource(wb.AsBitmap()))
            {
                // Create effect collection with the source stream
                using (var filters = new FilterEffect(source))
                {

                    //byte a = Convert.ToByte(255 - Math.Round((newValue * 10.2)));

                    Windows.UI.Color black = new Windows.UI.Color { R = 0, G = 0, B = 0 };
                   
                    VignettingFilter filter = new VignettingFilter(1 - newValue + 0.3, black);
                    

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
