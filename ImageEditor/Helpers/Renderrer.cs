using Lumia.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Lumia.InteropServices.WindowsRuntime;
using Lumia.Imaging.Adjustments;
using Windows.Storage;

namespace ImageEditor
{
    public static class Renderer
    {

        public async static Task<WriteableBitmap> Render(WriteableBitmap actualImage, List<IFilter> filters) {

            return await Render(actualImage, filters, actualImage.PixelWidth, actualImage.PixelHeight);
        
        }

        public async static Task<WriteableBitmap> Render(WriteableBitmap actualImage, List<IFilter> filters, int w, int h)
        {
            var bitmap = actualImage.AsBitmap();
            using (BitmapImageSource bitmapSource = new BitmapImageSource(bitmap))
            using (FilterEffect effects = new FilterEffect(bitmapSource))
            {

                effects.Filters = filters;
                WriteableBitmapRenderer renderer = new WriteableBitmapRenderer(effects, actualImage);

                return await renderer.RenderAsync();
            }

        }

        public async static Task<Bitmap> Render(StorageFile actualImage, List<IFilter> filters, int w, int h)
        {
            int stride = w * 4;
            Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(actualImage));



            using (BitmapImageSource s = new BitmapImageSource(bit))

            using (FilterEffect effects = new FilterEffect(s))
            {

                effects.Filters = filters;
                using (BitmapRenderer renderer = new BitmapRenderer(effects, bit))
                {
                    await renderer.RenderAsync();
                    s.Dispose();
                    effects.Dispose();
                    renderer.Dispose();
                }
            }
            

            return bit;

        }
    }
}