using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace Rawer
{
    class  FilePickerConverter
    {
        public static void Open()
        {

            var picker = new FileOpenPicker();

            //getting extensions from manifest 
            PhoneHelper.GetFileExtensionsFromManifest().ForEach(s => picker.FileTypeFilter.Add(s));


            picker.ContinuationData["Operation"] = "GetImage";
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;//.PicturesLibrary;
           
            picker.ViewMode = PickerViewMode.List;
            picker.PickSingleFileAndContinue();
        
        }


        public async static Task<string> ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            string path = "";

            if ((args.ContinuationData["Operation"] as string) == "GetImage" &&
                args.Files != null &&
                args.Files.Count > 0)
            {

                var BufferStorageFile = await StorageExplorer.CopyFileToBufferStorage(args.Files[0]);
                path = BufferStorageFile.Path;

            }

            return path;
        }

        public static void OpenSaveDialog(StorageFile file, Rawer.ImageEncoder.FileFormat format,int w, int h) {


            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            // Dropdown of file types the user can save the file as
            string extension = "."+format.ToString().ToLower();

            savePicker.FileTypeChoices.Add(extension, new List<string>() { extension });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(file.Name);
            savePicker.ContinuationData["Operation"] = "SaveImage";
            savePicker.ContinuationData["unpackfile"] = file.Path;
            savePicker.ContinuationData["fileFormat"] = (int)format;
            savePicker.ContinuationData["fileWidth"] = w;
            savePicker.ContinuationData["fileHeight"] = h;
         
            savePicker.PickSaveFileAndContinue();       
        
        }

        public async static Task<bool> ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {
            if ((args.ContinuationData["Operation"] as string) == "SaveImage" &&
                args.File != null)
            {               

                CachedFileManager.DeferUpdates(args.File);
                var format = (Rawer.ImageEncoder.FileFormat)((int)args.ContinuationData["fileFormat"]);
                var width = (int)args.ContinuationData["fileWidth"];
                var height = (int)args.ContinuationData["fileHeight"];

                StorageFile _UnPackedfile = await StorageFile.GetFileFromPathAsync((args.ContinuationData["unpackfile"] as string));
                var file = await ImageEncoder.SaveFile(_UnPackedfile, format, width, height);
                await file.MoveAndReplaceAsync(args.File);

                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                //return true;
                if (status == FileUpdateStatus.Complete)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //operation canceled
            else
            {               

                return false;
            }
        }

    }
}
