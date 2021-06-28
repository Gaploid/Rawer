using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace Rawer.Models
{
    class UsedFilterList
    {
        public CropTool FinalCropToolValue;
        public bool FinalCropTool;

        public double FinalBrightnessToolValue;
        public bool FinalBrightnessTool;


        public bool FinalContrastTool;
        public double FinalContrastToolValue;

        public bool FinalSaturationTool;
        public double FinalSaturationToolValue;


        public bool FinalHueTool;
        public double FinalHueToolValue;

        public bool FinalSharpenTool;
        public double FinalSharpenToolValue;

        public OrientationTool FinalOrientationToolValue;
        public bool FinalOrientationTool; 


        public void ClearAllFilters(){
            FinalCropToolValue = null;
            FinalBrightnessTool = false;


            FinalContrastTool = false;
            FinalSaturationTool = false;
            FinalHueTool = false;
            FinalSharpenTool = false;
            FinalOrientationTool = false;
        
        }

    }
}
