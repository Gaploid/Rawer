using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace ImageEditor
{
    public class CustomUISelector : ImageEditorToolUISelector
    {
        public DataTemplate nContrastToolUI
        {
            get;
            set;
        }

        public DataTemplate nNoiseToolUI
        {
            get;
            set;
        }

        public DataTemplate nExposureToolUI
        {
            get;
            set;
        }
        public DataTemplate nBrightnessToolUI
        {
            get;
            set;
        }

        public DataTemplate nSaturationToolUI
        {
            get;
            set;
        }

        public DataTemplate nSharpnessToolUI
        {
            get;
            set;
        }

        public DataTemplate nBlurToolUI
        {
            get;
            set;
        }

        public DataTemplate nHueToolUI
        {
            get;
            set;
        }

        public DataTemplate nTemperatureToolUI
        {
            get;
            set;
        }

        public DataTemplate nCropToolUI
        {
            get;
            set;
        }

        public DataTemplate nShadowsToolUI
        {
            get;
            set;
        }
        public DataTemplate nHighlightsToolUI
        {
            get;
            set;
        }

        public DataTemplate nResizeToolUI
        {
            get;
            set;
        }

        public DataTemplate nRotateToolUI
        {
            get;
            set;
        }

        public DataTemplate nColorEffectsToolUI
        {
            get;
            set;
        }

        public DataTemplate nVignettingToolUI
        {
            get;
            set;
        }
        

        public override System.Windows.DataTemplate SelectTemplate(object tool, System.Windows.DependencyObject container)
        {
            if (tool is nContrastTool)
            {
                return this.nContrastToolUI;
            }

            if (tool is nNoiseTool)
            {
                return this.nNoiseToolUI;
            }

            if (tool is nVignettingTool)
            {
                return this.nVignettingToolUI;
            }

            if (tool is nColorFiltersTool)
            {
                return this.nColorEffectsToolUI;
            }

            if (tool is nExposureTool)
            {
                return this.nExposureToolUI;
            }

            if (tool is nBrightnessTool)
            {
                return this.nBrightnessToolUI;
            }

            if (tool is nSaturationTool)
            {
                return this.nSaturationToolUI;
            }

            if (tool is nSharpnessTool)
            {
                return this.nSharpnessToolUI;
            }

            

            if (tool is nHueTool)
            {
                return this.nHueToolUI;
            }

            if (tool is nTemperatureTool)
            {
                return this.nTemperatureToolUI;
            }

            if (tool is nCropTool)
            {
                return this.nCropToolUI;
            }

            if (tool is nShadowsTool)
            {
                return this.nShadowsToolUI;
            }

            if (tool is nHighlightsTool)
            {
                return this.nHighlightsToolUI;
            }

            if (tool is nResizeTool)
            {
                return this.nResizeToolUI;
            }

            if (tool is nRotateTool)
            {
                return this.nRotateToolUI;
            }

            return base.SelectTemplate(tool, container);
        }
    }
}
