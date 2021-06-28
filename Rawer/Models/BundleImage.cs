using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Rawer.Models
{
    public class BundleImage
    {
        public BundleImage(StorageFile PixMap_File,
                     LibrawRuntimeComponent.LibRawExif exif,
                     int w,
                     int h,
            ulong ImageSize,
            string PathToOriginalFile,
            Histogram HistoData)
        {                         
                         PixMapFile = PixMap_File;
                         Exif = exif;
                         Width = w;
                         Height = h;
                         imageSize = ImageSize;
                         pathToOriginalFile = PathToOriginalFile;
                         histogramData = HistoData;
        
        }

        public StorageFile PixMapFile {get; set;}

        public LibrawRuntimeComponent.LibRawExif Exif { get; private set; }

        public ulong imageSize { get; private set; }

        public string pathToOriginalFile { get; private set; }

        public int Width { get; set; }

        public int Height{get; set;}

        public Histogram histogramData { get; set; }

        public bool NeedToUpdate { get; set; }
    }

    public struct Histogram{
        public int[] Red;
        public int[] Green;
        public int[] Blue;   
    
    }


}
