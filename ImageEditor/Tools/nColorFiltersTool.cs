using Lumia.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Lumia.InteropServices.WindowsRuntime;
using Lumia.Imaging.Adjustments;
using Lumia.Imaging.Artistic;

namespace ImageEditor
{
    public class nColorFiltersTool : ImageEditorTool
    {
        public List<IFilter> filters = new List<IFilter>();
        public GrayscaleFilter grayFilter = new GrayscaleFilter();
        public AutoEnhanceFilter autoEnhanceFilter = new AutoEnhanceFilter();
        public NegativeFilter negativeFilter = new NegativeFilter();


        private bool? greyscale = new bool?(false);
        private bool? colorFix = new bool?(false);
        private bool? invertColors = new bool?(false);

       //need to add !!!!!!!!!!!!!!!
        //AutoLevelsFilter
        //ColorAdjustFilter
        //AntiqueFilter
        //HdrEffect
        //LomoFilter
        //MilkyFilter
        //MonoColorFilter
        //MoonlightFilter
        //SepiaFilter
        //SketchFilter
        //WhiteBalanceFilter

        protected virtual async void OnIsColorInvertedChanged(bool newValue, bool oldValue)
        {
            if (newValue)
            {
                this.IsGreyscale = false;
                this.IsColorFixed = false;
                this.ResetWorkingBitmap();
                this.filters.Add(negativeFilter);
                this.ModifiedImage = await Renderer.Render(this.workingBitmap, filters);
            }
            else
            {
                this.ResetWorkingBitmap();
                this.filters.Clear();
                this.ModifiedImage = this.workingBitmap;
            }
        }

        protected virtual async void OnIsGreyscaleChanged(bool newValue, bool oldValue)
        {
            if (newValue)
            {
                this.IsColorInverted = false;
                this.IsColorFixed = false;
                this.ResetWorkingBitmap();
                this.filters.Add(grayFilter);
                this.ModifiedImage = await Renderer.Render(this.workingBitmap, filters);
            }
            else
            {
                this.ResetWorkingBitmap();
                this.filters.Clear();
                this.ModifiedImage = this.workingBitmap;
            }
        }

        protected virtual async void OnIsColorFixedChanged(bool newValue, bool oldValue)
        {
            if (newValue)
            {
                this.IsColorInverted = false;
                this.IsGreyscale = false;
                this.ResetWorkingBitmap();
                this.filters.Add(autoEnhanceFilter);
                this.ModifiedImage = await Renderer.Render(this.workingBitmap, filters);

            }
            else
            {
                this.ResetWorkingBitmap();
                this.filters.Clear();
                this.ModifiedImage = this.workingBitmap;
            }
        }

        protected override async Task<WriteableBitmap> ApplyCore(WriteableBitmap actualImage)
        {

            var File = ImageEditorControl.Instance.BigFile;
            int w = ImageEditorControl.Instance.BiGFileWidth;
            int h = ImageEditorControl.Instance.BiGFileHeight;
            DateTime now = DateTime.Now;

            var wb = await Renderer.Render(File, this.filters, w, h);

            await Helper.WriteDataToFileAsync(File, wb.Buffers[0].Buffer);
            wb = null;

            System.Diagnostics.Debug.WriteLine("Finish proccess big picture, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());


            GC.Collect();


            return await Renderer.Render(actualImage, this.filters, actualImage.PixelWidth, actualImage.PixelHeight);
        }


        public bool? IsColorInverted
        {
            get
            {
                return this.invertColors;
            }
            set
            {
                bool? flag = this.invertColors;
                bool? flag2 = value;
                if (flag.GetValueOrDefault() == flag2.GetValueOrDefault() && flag == flag2)
                {
                    return;
                }
                bool oldValue = this.invertColors.Value;
                this.invertColors = value;
                this.OnIsColorInvertedChanged(value.Value, oldValue);
                this.OnPropertyChanged("IsColorInverted");
            }
        }
        /// <summary>
        /// Gets or sets a value that converts the colors of the edited image to greyscale.
        /// </summary>
        public bool? IsGreyscale
        {
            get
            {
                return this.greyscale;
            }
            set
            {
                bool? flag = this.greyscale;
                bool? flag2 = value;
                if (flag.GetValueOrDefault() == flag2.GetValueOrDefault() && flag == flag2)
                {
                    return;
                }
                bool oldValue = this.greyscale.Value;
                this.greyscale = value;
                this.OnIsGreyscaleChanged(value.Value, oldValue);
                this.OnPropertyChanged("IsGreyscale");
            }
        }
        /// <summary>
        /// Gets or sets a value that automatically balances the colors of the edited image.
        /// </summary>
        public bool? IsColorFixed
        {
            get
            {
                return this.colorFix;
            }
            set
            {
                bool? flag = this.colorFix;
                bool? flag2 = value;
                if (flag.GetValueOrDefault() == flag2.GetValueOrDefault() && flag == flag2)
                {
                    return;
                }
                bool oldValue = this.colorFix.Value;
                this.colorFix = value;
                this.OnIsColorFixedChanged(value.Value, oldValue);
                this.OnPropertyChanged("IsColorFixed");
            }
        }


        public override string Name
        {
            get
            {
                return Resources.ResourceEditor.ColorEffectsTool;
            }
        }

        public override string Icon
        {
            get
            {
                return @"/ImageEditor;Component/Assets/icons-effects.png";
            }
        }
    }
}