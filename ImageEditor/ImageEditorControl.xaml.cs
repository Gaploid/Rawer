using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Telerik.Windows.Controls;
using ImageEditor.Resources;
using System.IO;

namespace ImageEditor
{
    public partial class ImageEditorControl : UserControl
    {
        public StorageFile OriginBiGFile;
        public int OriginBiGFileWidth;
        public int OriginBiGFileHeight;
        public event EventHandler<EditToolAcceptedEventArgs> EditToolAccepted;

        
        internal StorageFile BigFile;
        internal int BiGFileWidth;
        internal int BiGFileHeight;


        private static volatile ImageEditorControl instance;
        private static object syncRoot = new Object();

        public class EditToolAcceptedEventArgs : EventArgs
        {
            public string Tool { get; set; }
        }

        public ImageEditorControl()
        {
            ImageEditorLocalizationManager.Instance.ResourceManager = ResourceEditor.ResourceManager;
            InitializeComponent();
            instance = this;


            //imageEditor2.ImageSaved += imageEditor2_ImageSaved;
            imageEditor2.ImageEditCancelled += imageEditor2_ImageEditCancelled;
            imageEditor2.ImageSaving += imageEditor2_ImageSaving;
            imageEditor2.Tap += imageEditor2_Tap;
        }

        void imageEditor2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (e != null)
            if (e.OriginalSource != null)
            {
                var parent = ElementTreeHelper.FindVisualAncestor<RadImageButton>(e.OriginalSource as FrameworkElement);

                if (parent != null)
                    if (parent.Name == "PART_AcceptButton")
                    {

                        if (EditToolAccepted != null)
                            if (imageEditor2.CurrentTool != null)
                            {
                                //Fire event for selected tool to log it later 
                                EditToolAccepted(this, new EditToolAcceptedEventArgs() { Tool = imageEditor2.CurrentTool.ToString() });
                            }

                    }
            }   
        }

        async void imageEditor2_ImageSaving(object sender, Telerik.Windows.Controls.ImageSavingEventArgs e)
        {
            e.Cancel = true;

            await BigFile.MoveAndReplaceAsync(OriginBiGFile);
            
            //try
            //{
            //    await BigFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //}
            //catch (FileNotFoundException ee) { }



            OriginBiGFileHeight = BiGFileHeight;
            OriginBiGFileWidth = BiGFileWidth;



            imageEditor2.Source = null;
            ClearAllEvents();

            imageEditor2 = null;


            GC.Collect();

            Finished("updated");           
         
        }

        public static ImageEditorControl Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ImageEditorControl();
                    }
                }

                return instance;
            }
        }


        public delegate void FinishedHandler(string status);        
        public event FinishedHandler Finished;

        async void imageEditor2_ImageEditCancelled(object sender, Telerik.Windows.Controls.ImageEditCancelledEventArgs e)
        {

            await BigFile.DeleteAsync(StorageDeleteOption.PermanentDelete);

            ClearAllEvents();
            imageEditor2 = null;
            GC.Collect();



            Finished("cancelled");
            //fire event for page
           
        }


        public WriteableBitmap PreviewSource
        {
            set
            {
                imageEditor2.Source = value;
            } 
        }

        public async void SetBigFile(StorageFile file, int w, int h) {

           OriginBiGFile = file;
           OriginBiGFileHeight = h;
           OriginBiGFileWidth = w;
        

           //create copy of the file for edit, we need to replace original one at the end and delete this one
            
           var folder = ApplicationData.Current.LocalFolder;
           BigFile = await folder.CreateFileAsync(file.Name + "_edit", CreationCollisionOption.ReplaceExisting);
           await OriginBiGFile.CopyAndReplaceAsync(BigFile);
           BiGFileWidth = w;
           BiGFileHeight = h;
        
        }

        void ClearAllEvents() {
            //imageEditor2.ImageSaved -= imageEditor2_ImageSaved;
            imageEditor2.ImageEditCancelled -= imageEditor2_ImageEditCancelled;
            imageEditor2.ImageSaving -= imageEditor2_ImageSaving;        
        
        }
    }
}
