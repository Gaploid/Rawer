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

namespace ImageEditor
{
    public class nNoiseTool : RangeTool
    {
        public nNoiseTool()
        {
            this.Value =0;
            
            
        }

        int count;

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
                return @"/ImageEditor;Component/Assets/contrast.png";
            }
        }

        // Inherited from ImageEditorTool
        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.NoiseTool;
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
            if (this.PreviewImage == null)
            {
                return;
            }

            // Sets the working bitmap to have the same pixels as the original image.
            this.ResetWorkingBitmap();
            
            // Apply algorithm to the temporary working bitmap.
            this.ModifiedImage =  await this.Noise(this.workingBitmap, newValue, this.workingBitmap.PixelWidth, this.workingBitmap.PixelHeight,false);
            //this.ApplyRedness(newValue, this.workingBitmap.Pixels);

            //if (count >= 5)
            //{
                GC.Collect();
            //    count = 0;
            //}
            //count += 1;
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

            actualImage = await this.Noise(actualImage, value, actualImage.PixelWidth, actualImage.PixelHeight, false);

            DateTime now = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Start proccess big picture");


            WriteableBitmap wb = await Helper.GetWriteableBitmap(File, w, h);


            wb = await this.Noise(wb, value, wb.PixelWidth, wb.PixelHeight, false);

            await Helper.WriteDataToFileAsync(File, wb);
            wb = null;

            System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());


            GC.Collect();

            return actualImage;
        }

     

        async Task<WriteableBitmap> Noise(WriteableBitmap wb ,double newValue, int w, int h, bool ext)
        {
            //var target = new WriteableBitmap(w, h);
            //Uri uri = new Uri("Assets/IMG_7136.jpg", UriKind.Relative);

            
            using (var source = new BitmapImageSource(wb.AsBitmap()))
            //using (var source = new StreamImageSource(Application.GetResourceStream(uri).Stream))
            {


                // Create effect collection with the source stream
                using (var filters = new FilterEffect(source))
                {

                    DespeckleFilter filter = new DespeckleFilter(DespeckleLevel.Maximum);

                    // Add the filter to the FilterEffect collection
                    filters.Filters = new IFilter[] { filter };

                    // Create a target where the filtered image will be rendered to
                    


                    // Create a new renderer which outputs WriteableBitmaps
                    using (var renderer = new WriteableBitmapRenderer(filters, wb))
                    {
                        // Render the image with the filter(s)
                        await renderer.RenderAsync();

                        renderer.Dispose();
                        source.Dispose();                        
                        filters.Dispose();
                    } 
                   

                } 
            }
            return wb;
        }
              

    }
}
