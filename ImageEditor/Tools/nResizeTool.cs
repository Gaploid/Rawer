using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls.SlideView;
using Telerik.Windows.Controls;
using System.Runtime.InteropServices.WindowsRuntime;
using Lumia.InteropServices.WindowsRuntime;
using Lumia.Imaging.Adjustments;
using Lumia.Imaging;
using Lumia.Imaging.Transforms;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ImageEditor
{
    public class nResizeTool: ImageEditorTool
    {
        private bool isUniform = true;
		private int width;
		private int height;
		private int maxWidth;
		private int maxHeight;
		private double ratio = 1.0;
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
		/// <summary>
		/// Gets the name of the tool.
		/// </summary>
		public override string Name
		{
			get
			{
				return Resources.ResourceEditor.ResizeTool;
			}
		}
		/// <summary>
		/// Gets the full path to the icon of the tool.
		/// </summary>
		public override string Icon
		{
			get
			{
				return @"/ImageEditor;Component/Assets/resize2.png";
			}
		}
		/// <summary>
		/// Gets or sets the max allowed width.
		/// </summary>
		public int MaxWidth
		{
			get
			{
				return this.maxWidth;
			}
			set
			{
				if (this.maxWidth == value)
				{
					return;
				}
				this.maxWidth = value;
				this.OnPropertyChanged("MaxWidth");
				if (this.width > this.maxWidth)
				{
					this.Width = this.maxWidth;
				}
			}
		}
		/// <summary>
		/// Gets or sets the max allows height.
		/// </summary>
		public int MaxHeight
		{
			get
			{
				return this.maxHeight;
			}
			set
			{
				if (this.maxHeight == value)
				{
					return;
				}
				this.maxHeight = value;
				this.OnPropertyChanged("MaxHeight");
				if (this.height > this.maxHeight)
				{
					this.Height = this.maxHeight;
				}
			}
		}
		/// <summary>
		/// Gets or sets the width of the resized image.
		/// </summary>
		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (this.width == value)
				{
					return;
				}
				this.width = ((value > this.maxWidth) ? this.maxWidth : value);
				this.OnPropertyChanged("Width");
				if (this.IsUniform)
				{
					if (this.ratio < 1.0)
					{
						this.height = (int)Math.Round((double)this.width / this.ratio);
					}
					else
					{
						if (this.ratio > 1.0)
						{
							this.height = (int)Math.Round((double)this.width / this.ratio);
						}
						else
						{
							this.height = this.width;
						}
					}
					this.height = Clamp<int>(this.height, 0, this.maxHeight);
					this.OnPropertyChanged("Height");
				}
			}
		}
		/// <summary>
		/// Gets or sets the height of the resized image.
		/// </summary>
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (this.height == value)
				{
					return;
				}
				this.height = ((value > this.maxHeight) ? this.maxHeight : value);
				this.OnPropertyChanged("Height");
				if (this.IsUniform)
				{
					if (this.ratio < 1.0)
					{
						this.width = (int)Math.Round((double)this.height * this.ratio);
					}
					else
					{
						if (this.ratio > 1.0)
						{
							this.width = (int)Math.Round((double)this.height * this.ratio);
						}
						else
						{
							this.width = this.height;
						}
					}
					this.width = Clamp<int>(this.width, 0, this.maxWidth);
					this.OnPropertyChanged("Width");
				}
			}
		}

        internal static T Clamp<T>(T value, T min, T max)
        {
            Comparer<T> comparer = Comparer<T>.Default;
            int c = comparer.Compare(value, min);
            if (c < 0)
            {
                return min;
            }
            c = comparer.Compare(value, max);
            if (c > 0)
            {
                return max;
            }
            return value;
        }


		/// <summary>
		/// Gets or sets whether the image is resized uniformly.
		/// </summary>
		public bool IsUniform
		{
			get
			{
				return this.isUniform;
			}
			set
			{
				if (this.isUniform == value)
				{
					return;
				}
				this.isUniform = value;
				this.OnPropertyChanged("IsUniform");
			}
		}
		/// <summary>
		/// Initializes a new instance of the ResizeTool class.
		/// </summary>
        public nResizeTool()
		{
            //Height = ImageEditorControl.Instance.BiGFileHeight;
            //Width = ImageEditorControl.Instance.BiGFileWidth;
             
            

            //this.maxWidth = MaxWidth;
            //this.maxHeight = MaxHeight;

            //height = ImageEditorControl.Instance.BiGFileHeight;
            //width = ImageEditorControl.Instance.BiGFileWidth;
            //MaxHeight = ImageEditorControl.Instance.BiGFileHeight;
            //MaxWidth = ImageEditorControl.Instance.BiGFileWidth;

            //this.maxWidth = 
           // this.maxHeight = 
		}

        
		/// <summary>
		/// A virtual callback that is called when the OriginalImage property changes.
		/// </summary>
		protected override void OnOriginalImageChanged()
		{
			base.OnOriginalImageChanged();
			if (base.OriginalImage == null)
			{
				this.ratio = 0.0;
				this.width = 0;
				this.height = 0;
			}
			else
			{
				//this.width = base.OriginalImage.PixelWidth;
				//this.height = base.OriginalImage.PixelHeight;

                this.width  =  maxWidth =  ImageEditorControl.Instance.BiGFileWidth;
                this.height = maxHeight =  ImageEditorControl.Instance.BiGFileHeight;
                
                this.ratio = (double)this.width / (double)this.height;
			}
			this.OnPropertyChanged("Width");
			this.OnPropertyChanged("Height");
		}
		/// <summary>
		/// This method is called when the tool will edit the original image in its full size.
		/// This method is called once by the image editor when the accept button is pressed.
		/// </summary>
		/// <param name="actualImage">The original image to be modified.</param>
		/// <returns>Returns a new modified image.</returns>
		protected override async Task<WriteableBitmap> ApplyCore(WriteableBitmap actualImage)
		{
            var File = ImageEditorControl.Instance.BigFile;
            int w = ImageEditorControl.Instance.BiGFileWidth;
            int h = ImageEditorControl.Instance.BiGFileHeight;          
            
           
            //if size is changed then resize images
            if (Width > 0 && Height > 0)
            if (w != Width || h != Height)
            {

                Bitmap CropWB = await this.Resize(File, Width, Height, w, h, false);

                //if the ration is changed then we need to resize preview image to show mistake 
                if (!IsUniform) {

                    double newWidth = ((double)Width / (double)w) * (double)actualImage.PixelWidth;
                    double newHeight = ((double)Height / (double)h) * (double)actualImage.PixelHeight;

                    actualImage = actualImage.Resize(
                                    (int)Math.Round(newWidth, 0),
                                    (int)Math.Round(newHeight, 0),
                                    System.Windows.Media.Imaging.WriteableBitmapExtensions.Interpolation.Bilinear);                
                
                }


               
                //wb = null;
                GC.Collect();

                if (CropWB != null)
                {
                    await Helper.WriteDataToFileAsync(File, CropWB.Buffers[0].Buffer);

                    ImageEditorControl.Instance.BiGFileHeight = Height;
                    ImageEditorControl.Instance.BiGFileWidth = Width;

                    CropWB.Dispose();
                    CropWB = null;
                }


               
                GC.Collect();
            }
            return actualImage;
		}

        async Task<Bitmap> Resize(StorageFile file, int neww, int newh, int w, int h, bool ext)
        {
            Bitmap b = new Bitmap(new Windows.Foundation.Size((double)neww, (double)newh), ColorMode.Bgra8888);

            int stride = w * 4;
            using (Bitmap bit = new Bitmap(new Windows.Foundation.Size(w, h),
                                    Lumia.Imaging.ColorMode.Bgra8888,
                                    (uint)stride,
                                    await Windows.Storage.FileIO.ReadBufferAsync(file))) 
            using (BitmapImageSource s = new BitmapImageSource(bit))
            using (BitmapRenderer renderer = new BitmapRenderer(s)) {

                renderer.Size = new Windows.Foundation.Size((double)neww, (double)newh);
                renderer.OutputOption = OutputOption.Stretch;
                b = await renderer.RenderAsync();

                bit.Dispose();
                s.Dispose();
                renderer.Dispose();               
            
            }

            return b;

        }
    }
}
