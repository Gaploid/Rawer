using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Rawer.Resources;
using System.Windows.Media.Imaging;


namespace Rawer
{
    public partial class EditPage : PhoneApplicationPage
    {

        //CropTool FinalCropTool;

        ////BrightnessTool FinalBrightnessTool;
        //double FinalBrightnessToolValue;

        ////ContrastTool FinalContrastTool;
        //double FinalContrastToolValue;


        //int countContrast = 0;
        ////SaturationTool FinalSaturationTool;
        //double FinalSaturationToolValue;

        ////HueTool FinalHueTool;
        //double FinalHueToolValue;

        ////SharpenTool FinalSharpenTool;
        //double FinalSharpenToolValue;

        //OrientationTool Orientation0;

        WriteableBitmap WrBtm_Small;
      
        
        const int MaxSizeForImageEditor = 1000;
        //Rawer.Models.UsedFilterList FilterList;

        public EditPage()
        {
            InitializeComponent();
            editor.EditToolAccepted += editor_EditToolAccepted;
            GoogleAnalytics.EasyTracker.GetTracker().SendView("EditPage");
            
        }

        void editor_EditToolAccepted(object sender, ImageEditor.ImageEditorControl.EditToolAcceptedEventArgs e)
        {
            GoogleTracker.TrackSelectedTool(e.Tool);

        }

        ~EditPage()
        {

            System.Diagnostics.Debug.WriteLine("EditPage destroed");
        
        }


        void editor_Finished(string status)
        {
            if (status == "updated")
            {
                (App.Current as App).BundleImage.NeedToUpdate = true;
                (App.Current as App).BundleImage.Height = editor.OriginBiGFileHeight;
                (App.Current as App).BundleImage.Width = editor.OriginBiGFileWidth;            
            }

            NavigationService.GoBack();     
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Forward)
            {
                editor.Finished -= editor_Finished;
                WrBtm_Small = null;
                //editor.PreviewSource = null;
                editor = null;



                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            base.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // (App.Current as App).BundleImage
           
            //FilterList = new Models.UsedFilterList();
            if (e.NavigationMode == NavigationMode.New)
            {
                //editor.Finished += editor_Finished;
                editor.Finished += editor_Finished;

                DateTime now = DateTime.Now;

                System.Diagnostics.Debug.WriteLine("Start page, start resize image");
                var file = await StorageExplorer.GetFileInBytes((App.Current as App).BundleImage.PixMapFile);

                WrBtm_Small = await ImageEncoder.ResizeAndEncodeToWRBitmap(file,
                                                                 (App.Current as App).BundleImage.Width,
                                                                 (App.Current as App).BundleImage.Height,
                                                                 MaxSizeForImageEditor);
                file = null;

                //editor.PreviewSource =   StorageExplorer.GetStreamFile((App.Current as App).BundleImage.PixMapFile)
                editor.PreviewSource = WrBtm_Small;
                editor.SetBigFile((App.Current as App).BundleImage.PixMapFile, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);

                System.Diagnostics.Debug.WriteLine("Finish resize image and render, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());
                WrBtm_Small = null;
                GC.Collect();
            }

            base.OnNavigatedTo(e);
        }

        //private void RadImageEditor_ImageSaving(object sender, Telerik.Windows.Controls.ImageSavingEventArgs e)
        //{
        //    PhoneHelper.ShowProgressBar(AppResources.EditPageProgressBarWorking);
        //    e.Cancel = true;
        //    DateTime now = DateTime.Now;
        //    System.Diagnostics.Debug.WriteLine("Start filter processing");
        //    ImageEncoder.ProcessAllFilters(FilterList, (App.Current as App).BundleImage.Width, (App.Current as App).BundleImage.Height);

        //    System.Diagnostics.Debug.WriteLine("Finish filter processing, take:" + (DateTime.Now - now).TotalMilliseconds.ToString());
        //    PhoneHelper.HideProgressBar();
            
        //}
        //private void CropTool_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.ToLower() == "CropRect".ToLower())
        //    {

        //        FilterList.FinalCropToolValue = (editor.CurrentTool as CropTool);
             
        //    }


        //    if (e.PropertyName.ToLower() == "isWorking".ToLower())
        //    {
                
        //        FilterList.FinalCropTool = true;
        //    }

        //    System.Diagnostics.Debug.WriteLine(e.PropertyName);

        //}

        //private void OrientationTool_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.ToLower() == "IsFlippedHorizontal".ToLower() || e.PropertyName.ToLower() == "IsFlippedVertical".ToLower())
        //    {

        //            FilterList.FinalOrientationToolValue = Orientation0;                   
        //            Orientation0 = (editor.CurrentTool as OrientationTool);

               
        //    }


        //    if (e.PropertyName.ToLower() == "isWorking".ToLower())
        //    {
        //        FilterList.FinalOrientationTool = true;
        //    }
           
        //    System.Diagnostics.Debug.WriteLine(e.PropertyName);

        //}

        //private void editor_ImageSaved(object sender, ImageSavedEventArgs e)
        //{

        //}

        //private void editor_ImageEditCancelled(object sender, ImageEditCancelledEventArgs e)
        //{

        //    FilterList.ClearAllFilters();

        //    System.Diagnostics.Debug.WriteLine("ImageEditCancelled!!!!!!!!!!");
        //}

        //private void BrightnessTool_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{

        //    if (e.PropertyName.ToLower() == "Value".ToLower())
        //    {
               
        //        FilterList.FinalBrightnessToolValue = FinalBrightnessToolValue;
        //        FinalBrightnessToolValue = (editor.CurrentTool as BrightnessTool).Value;
               
        //    }

        //    if (e.PropertyName.ToLower() == "isWorking".ToLower())
        //    {
        //        FilterList.FinalBrightnessTool = true;
        //    }

        //    System.Diagnostics.Debug.WriteLine(e.PropertyName);

        //}

        //private async void ContrastTool_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{

        //    if (e.PropertyName.ToLower() == "Value".ToLower())
        //    {
                
        //        //Deployment.Current.Dispatcher.BeginInvoke(() =>
        //        // {
                   
        //            FilterList.FinalContrastToolValue = FinalContrastToolValue;
        //            FinalContrastToolValue = (editor.CurrentTool as ContrastTool).Value;
        //         //});
        //    }


        //    if (e.PropertyName.ToLower() == "isWorking".ToLower())
        //    {
        //        countContrast += 1;
        //        FilterList.FinalContrastTool = true;

        //        if (countContrast == 2) {


        //           (editor.CurrentTool as ContrastTool).PreviewImage= await ImageEncoder.ProcessFilter(FilterList, WrBtm_Small.PixelWidth, WrBtm_Small.PixelHeight, WrBtm_Small);

                
        //        }
        //    }

        //    System.Diagnostics.Debug.WriteLine(e.PropertyName);
        //}

        //private void SaturationTool_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.ToLower() == "Value".ToLower())
        //    {
                
        //        FilterList.FinalSaturationToolValue = FinalSaturationToolValue;
        //        FinalSaturationToolValue = (editor.CurrentTool as SaturationTool).Value;

        //    }            

        //    if (e.PropertyName.ToLower() == "isWorking".ToLower())
        //    {
        //        FilterList.FinalSaturationTool = true;            
        //    }

        //    System.Diagnostics.Debug.WriteLine(e.PropertyName);

        //}

        //private void HueTool_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{

        //    if (e.PropertyName.ToLower() == "Value".ToLower())
        //    {
          
        //        FilterList.FinalHueToolValue = FinalHueToolValue;
        //        FinalHueToolValue = (editor.CurrentTool as HueTool).Value;

        //    }

        //    if (e.PropertyName.ToLower() == "isWorking".ToLower())
        //    {
        //        FilterList.FinalHueTool = true;
        //    }

        //    System.Diagnostics.Debug.WriteLine(e.PropertyName);

        //}

        //private void SharpenTool_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{

        //    if (e.PropertyName.ToLower() == "Value".ToLower())
        //    {
               
        //        FilterList.FinalSharpenToolValue = FinalSharpenToolValue;
        //        FinalSharpenToolValue  = (editor.CurrentTool as SharpenTool).Value;

        //    }

        //    if (e.PropertyName.ToLower() == "isWorking".ToLower())
        //    {
        //        FilterList.FinalSharpenTool = true;
        //    }

        //    System.Diagnostics.Debug.WriteLine(e.PropertyName);

        //}

    }
}