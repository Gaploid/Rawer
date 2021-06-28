using System;
using Telerik.Windows.Controls;
namespace ImageEditor
{
    /// <summary>
    /// Represents an aspect ratio of the crop rect of CropTool.
    /// </summary>
    public class CropToolRatio : ViewModel
    {
        private int width;
        private int height;
        private bool isFlipped;
        private string text;
        private string customText;
        private bool canResize;
        private bool isSelected;
        private int collectionIndex;
        /// <summary>
        /// The index of the ratio in its parent CropTool.SupportedRatios collection.
        /// </summary>
        public int CollectionIndex
        {
            get
            {
                return this.collectionIndex;
            }
            internal set
            {
                if (this.collectionIndex == value)
                {
                    return;
                }
                this.collectionIndex = value;
                this.OnPropertyChanged("CollectionIndex");
            }
        }
        /// <summary>
        /// Gets or sets a value that determines if this ratio is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected == value)
                {
                    return;
                }
                this.isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }
        /// <summary>
        /// Gets or sets a value that determines if this ratio supports resizing.
        /// </summary>
        public bool CanResize
        {
            get
            {
                return this.canResize;
            }
            set
            {
                if (this.canResize == value)
                {
                    return;
                }
                this.canResize = value;
                this.OnPropertyChanged("CanResize");
            }
        }
        /// <summary>
        /// Gets or sets a value that determines if the Width and Height are swapped.
        /// </summary>
        public bool? IsFlipped
        {
            get
            {
                return new bool?(this.isFlipped);
            }
            set
            {
                if (this.isFlipped == value.Value)
                {
                    return;
                }
                bool oldValue = this.isFlipped;
                this.isFlipped = value.Value;
                this.OnIsFlippedChanged(this.isFlipped, oldValue);
                this.OnPropertyChanged("IsFlipped");
            }
        }
        /// <summary>
        /// Gets or sets the width of the ratio.
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
                this.width = value;
                this.OnPropertyChanged("Width");
                this.UpdateText();
            }
        }
        /// <summary>
        /// Gets or sets the height of the ratio.
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
                this.height = value;
                this.OnPropertyChanged("Height");
                this.UpdateText();
            }
        }
        /// <summary>
        /// Gets the text of representation of this ratio.
        /// </summary>
        public string CustomText
        {
            get
            {
                return this.customText;
            }
            set
            {
                if (this.customText == value)
                {
                    return;
                }
                this.customText = value;
                this.OnPropertyChanged("CustomText");
                this.OnPropertyChanged("Text");
            }
        }
        /// <summary>
        /// Gets the text of representation of this ratio. CustomText is returned if it is set instead.
        /// </summary>
        public string Text
        {
            get
            {
                if (this.customText != null)
                {
                    return this.customText;
                }
                return this.text;
            }
            private set
            {
                if (this.text == value)
                {
                    return;
                }
                this.text = value;
                this.OnPropertyChanged("Text");
            }
        }
        /// <summary>
        /// Initializes a new instance of the CropToolRatio class.
        /// </summary>
        public CropToolRatio()
        {
            this.CollectionIndex = -1;
        }
        /// <summary>
        /// A virtual method that is called when the <see cref="P:Telerik.Windows.Controls.CropToolRatio.IsFlipped" /> property changes.
        /// </summary>
        /// <param name="newValue">The new property value.</param>
        /// <param name="oldValue">The old property value.</param>
        protected virtual void OnIsFlippedChanged(bool newValue, bool oldValue)
        {
            int tmp = this.width;
            this.width = this.height;
            this.height = tmp;
            this.UpdateText();
            this.OnPropertyChanged("Width");
            this.OnPropertyChanged("Height");
        }
        private void UpdateText()
        {
            string text;
            if (this.width == this.height)
            {
                text = ImageEditorLocalizationManager.Instance.CropToolSquareText;
            }
            else
            {
                text = string.Format("{0}:{1}", this.width, this.height);
            }
            this.Text = text;
        }
    }
}
