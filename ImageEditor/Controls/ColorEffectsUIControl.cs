using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace ImageEditor
{
    public class ColorEffectsUIControl : ImageEditorToolUI
    {
        /// <summary>
        /// Identifies the <see cref="P:Telerik.Windows.Controls.ColorEffectsUIControl.IsColorFixed" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsColorFixedProperty = DependencyProperty.Register("IsColorFixed", typeof(bool), typeof(ColorEffectsUIControl), new PropertyMetadata(false));
        /// <summary>
        /// Identifies the <see cref="P:Telerik.Windows.Controls.ColorEffectsUIControl.IsColorInverted" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsColorInvertedProperty = DependencyProperty.Register("IsColorInverted", typeof(bool), typeof(ColorEffectsUIControl), new PropertyMetadata(false));
        /// <summary>
        /// Identifies the <see cref="P:Telerik.Windows.Controls.ColorEffectsUIControl.IsGreyscale" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGreyscaleProperty = DependencyProperty.Register("IsGreyscale", typeof(bool), typeof(ColorEffectsUIControl), new PropertyMetadata(false));
        /// <summary>
        /// Gets or set whether the image colors are inverted.
        /// </summary>
        public bool IsColorInverted
        {
            get
            {
                return (bool)base.GetValue(ColorEffectsUIControl.IsColorInvertedProperty);
            }
            set
            {
                base.SetValue(ColorEffectsUIControl.IsColorInvertedProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets whether the image colors are auto fixed.
        /// </summary>
        public bool IsColorFixed
        {
            get
            {
                return (bool)base.GetValue(ColorEffectsUIControl.IsColorFixedProperty);
            }
            set
            {
                base.SetValue(ColorEffectsUIControl.IsColorFixedProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets whether the image is converted to greyscale.
        /// </summary>
        public bool IsGreyscale
        {
            get
            {
                return (bool)base.GetValue(ColorEffectsUIControl.IsGreyscaleProperty);
            }
            set
            {
                base.SetValue(ColorEffectsUIControl.IsGreyscaleProperty, value);
            }
        }
        /// <summary>
        /// Initializes a new instance of the ColorEffectsUIControl.
        /// </summary>
        public ColorEffectsUIControl()
        {
            base.DefaultStyleKey = (typeof(ColorEffectsUIControl));
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes (such as a rebuilding layout pass) call System.Windows.Controls.Control.ApplyTemplate().
        /// In simplest terms, this means the method is called just before a UI element
        /// displays in an application.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button colorFix = base.GetTemplatePart<Button>("PART_ColorFix", true);
            colorFix.Content = (ImageEditorLocalizationManager.Instance.ColorEffectsToolColorFixText);
            Button grayscale = base.GetTemplatePart<Button>("PART_Grayscale", true);
            grayscale.Content = (ImageEditorLocalizationManager.Instance.ColorEffectsToolGreyscaleText);
            Button invertColors = base.GetTemplatePart<Button>("PART_InvertColors", true);
            invertColors.Content = (ImageEditorLocalizationManager.Instance.ColorEffectsToolInvertColorsText);
        }
    }
}
