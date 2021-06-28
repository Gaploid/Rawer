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
using System.Windows.Markup;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls.SlideView;
using System.Collections.Specialized;
using System.Collections;
using Lumia.Imaging.Transforms;
using Windows.Storage;

namespace ImageEditor
{
    [ContentProperty("SupportedRatios")]
    public class nCropTool : ImageEditorTool
    {
        private ObservableCollection<CropToolRatio> supportedRatios = new ObservableCollection<CropToolRatio>();
        private CropToolRatio selectedRatio;
        private Rect cropRect;

        public nCropTool()
        {
            this.supportedRatios.CollectionChanged += OnSupportedRatiosCollectionChanged; 		
            
        }

        public CropToolRatio SelectedRatio
        {
            get
            {
                return this.selectedRatio;
            }
            set
            {
                if (this.selectedRatio == value)
                {
                    return;
                }
                this.selectedRatio = value;
                this.OnPropertyChanged("SelectedRatio");
            }
        }

        public Rect CropRect
        {
            get
            {
                return this.cropRect;
            }
            set
            {
                if (this.cropRect == value)
                {
                    return;
                }
                this.cropRect = value;
                this.OnPropertyChanged("CropRect");
            }
        }
        /// <summary>
        /// Gets or sets a value that contains the supported ratios of this crop tool.
        /// </summary>
        public ObservableCollection<CropToolRatio> SupportedRatios
        {
            get
            {
                return this.supportedRatios;
            }
            private set
            {
                if (this.supportedRatios == value)
                {
                    return;
                }
                this.supportedRatios = value;
                this.OnPropertyChanged("SupportedRatios");
            }
        }
        /// <summary>
        /// Gets the zoom mode of the tool.
        /// </summary>
        public override ImageZoomMode ZoomMode
        {
            get
            {
                return ImageZoomMode.None;
            }
        }

        int count;

        // Inherited from ImageEditorTool
        public override string Icon
        {
            get
            {
                // The icon must be added as a Resource to the project under the Assets folder for this path to work.
                return @"/ImageEditor;Component/Assets/Crop-96.png";
            }
        }

        // Inherited from ImageEditorTool
        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.CropTool;
            }
        }

        protected override void OnPreviewImageChanged()
        {
            base.OnPreviewImageChanged();
            if (this.supportedRatios != null && this.supportedRatios.Count > 0)
            {
                this.SelectedRatio = this.SupportedRatios[0];
                this.SelectedRatio.IsSelected = true;
            }
        }


        protected override void OnPropertyChanged(string propertyName)  
        {
            System.Diagnostics.Debug.WriteLine("OnPropertyChanged "+ propertyName);
            base.OnPropertyChanged(propertyName);
        }

        protected override void OnIsSelectedChanged(bool previousValue)
        {
            System.Diagnostics.Debug.WriteLine("OnIsSelectedChanged " + previousValue);
            base.OnIsSelectedChanged(previousValue);
        }

    
        // Inherited from ImageEditorTool
        protected override async System.Threading.Tasks.Task<WriteableBitmap> ApplyCore(WriteableBitmap actualImage)
        {
           var File = ImageEditorControl.Instance.BigFile;
           int w =  ImageEditorControl.Instance.BiGFileWidth;
           int h = ImageEditorControl.Instance.BiGFileHeight;         
         

           actualImage = await this.CropWB(actualImage, this.cropRect, actualImage.PixelWidth, actualImage.PixelHeight, false);

           DateTime now = DateTime.Now;
           System.Diagnostics.Debug.WriteLine("Start proccess big picture");
           GC.Collect(); 

           Bitmap Crop = await this.Crop(File, this.cropRect, w, h, false);
           if (Crop != null)
           {
               await Helper.WriteDataToFileAsync(File, Crop.Buffers[0].Buffer);

               ImageEditorControl.Instance.BiGFileHeight = (int)Math.Round(Crop.Dimensions.Height);
               ImageEditorControl.Instance.BiGFileWidth = (int)Math.Round(Crop.Dimensions.Width);

               Crop.Dispose();
               Crop = null;
           }

           System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());
           GC.Collect();
           GC.WaitForPendingFinalizers();
           GC.Collect();

          
          
           return actualImage;
        }



        async Task<Bitmap> Crop(StorageFile file, Rect rect, int w, int h, bool ext)
        {
            double Wnew = w * rect.Width;
            double Hnew = h * rect.Height;

            Bitmap target = new Bitmap(new Windows.Foundation.Size(Wnew, Hnew), ColorMode.Bgra8888);

            //load big file
            int stride = w * 4;
            using (Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file)))
            using (BitmapImageSource s = new BitmapImageSource(bit))
            using (var filters = new FilterEffect(s))
            {
                CropFilter filter = new CropFilter(new Windows.Foundation.Rect(
                                           (int)Math.Round(this.cropRect.X * (double)w),
                                           (int)Math.Round(this.cropRect.Y * (double)h),
                                           Wnew,
                                           Hnew));

                // Add the filter to the FilterEffect collection
                filters.Filters = new IFilter[] { filter };

                using (BitmapRenderer renderer = new BitmapRenderer(filters))
                {
                    renderer.Size = new Windows.Foundation.Size(Wnew, Hnew);
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





        async Task<WriteableBitmap> CropWB(WriteableBitmap wb, Rect newValue, int w, int h, bool ext)
        {
            var target = new WriteableBitmap((int)Math.Round((double)w * newValue.Width), (int)Math.Round((double)h * newValue.Height));
            //Uri uri = new Uri("Assets/IMG_7136.jpg", UriKind.Relative);
            
            using (var source = new BitmapImageSource(wb.AsBitmap()))
            //using (var source = new StreamImageSource(Application.GetResourceStream(uri).Stream))
            {

                // Create effect collection with the source stream
                using (var filters = new FilterEffect(source))
                {

                    CropFilter filter = new CropFilter(new Windows.Foundation.Rect(
                                            (int)Math.Round(this.cropRect.X * (double)w),
                                            (int)Math.Round(this.cropRect.Y * (double)h),
                                            target.PixelWidth,
                                            target.PixelHeight));

                    // Add the filter to the FilterEffect collection
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


        private void OnSupportedRatiosCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //e.Action == NotifyCollectionChangedAction.
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnRatiosAdded(e.NewItems);
                    return;
                case NotifyCollectionChangedAction.Remove:
                    this.OnRatiosRemoved(e.NewItems);
                    return;
                case NotifyCollectionChangedAction.Replace:
                    this.OnRatiosReplaced(e.NewItems, e.OldItems);
                    return;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.OnRatiosReplaced(e.NewItems, e.OldItems);
                    break;
                default:
                    return;
            }
        }
        private void OnRatiosAdded(IList addedTools)
        {
            if (addedTools == null)
            {
                return;
            }
            IEnumerator enumerator = addedTools.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                   CropToolRatio ratio = (CropToolRatio)enumerator.Current;
                    //ratio.CollectionIndex = this.supportedRatios.IndexOf(ratio);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
        private void OnRatiosRemoved(IList removedTools)
        {
            if (removedTools == null)
            {
                return;
            }
            IEnumerator enumerator = removedTools.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    CropToolRatio ratio = (CropToolRatio)enumerator.Current;
                    //ratio.CollectionIndex = -1;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
        private void OnRatiosReplaced(IList newTools, IList replacedTools)
        {
            this.OnRatiosAdded(newTools);
            this.OnRatiosRemoved(replacedTools);
        }

    }

}
