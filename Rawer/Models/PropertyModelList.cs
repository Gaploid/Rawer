using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Rawer.Resources;

namespace Rawer.Models
{
    class PropertyModelList : INotifyPropertyChanged
    {


        Dictionary<float, string> FlashUsed = new Dictionary<float, string>()
	    {
	        {	0	, AppResources.ImageInfoPageNo },           //No Flash
            {	1	, AppResources.ImageInfoPageYes},           //Fired
            {	5	, AppResources.ImageInfoPageYes},            //Fired, Return not detected
            {	7	, AppResources.ImageInfoPageYes},             //"Fired, Return detected	"
            {	8	, AppResources.ImageInfoPageNo},            //"	On, Did not fire	"
            {	9	, AppResources.ImageInfoPageYes},           //"	On, Fired	"
            {	13	, "??"},                                     //On, Return not detected	
            {	15	, "??"},                                    //On, Return detected	
            {	16	, AppResources.ImageInfoPageNo},             //"	Off, Did not fire	"
            {	20	, AppResources.ImageInfoPageNo},             //"	Off, Did not fire, Return not detected	"
            {	24	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageNo},   //"	Auto, Did not fire	"
            {	25	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageYes},  //"	Auto, Fired	"
            {	29	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageYes},  //Auto, Fired, Return not detected
            {	31	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageYes},   //Auto, Fired, Return detected	
            {	32	, AppResources.ImageInfoPageNo},            //No Flash function	
            {	48	, AppResources.ImageInfoPageNo},            //	Off, No flash function
            {	65	, AppResources.ImageInfoPageYes},            //Fired, Red-eye reduction
            {	69	, AppResources.ImageInfoPageYes},            //Fired, Red-eye reduction, Return not detected	
            {	71	, AppResources.ImageInfoPageYes },           //Fired, Red-eye reduction, Return detected
            {	73	, "??"},                                    //On, Red-eye reduction
            {	77	, "??"},                                    //On, Red-eye reduction, Return not detected
            {	79	, "??"},                                    //On, Red-eye reduction, Return detected	
            {	80	, AppResources.ImageInfoPageNo },           //Off, Red-eye reduction
            {	88	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageNo},  //Auto, Did not fire, Red-eye reduction	
            {	89	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageYes},  //	Auto, Fired, Red-eye reduction
            {	93	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageYes},  //Auto, Fired, Red-eye reduction, Return not detected	
            {	95	, AppResources.ImageInfoPageFlashAuto +", " + AppResources.ImageInfoPageYes}   //Auto, Fired, Red-eye reduction, Return detected	

	    };


        public ObservableCollection<MyData> PropertieList
        {
            get
            {
                if (_PropertieList == null)
                {

                    _PropertieList = new ObservableCollection<MyData>();
                }

                return _PropertieList;
            }

        }

        private ObservableCollection<MyData> _PropertieList;

        public void LoadPropertieList(Models.BundleImage bundleImage)
        {

            double size = Math.Round((double)((bundleImage.Width * bundleImage.Height) / 1000000), 1);

            _PropertieList = new ObservableCollection<MyData>();
            _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageName, Value = Path.GetFileName(bundleImage.pathToOriginalFile) });

            _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageWidth, Value = bundleImage.Width.ToString() });

            _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageHeight, Value = bundleImage.Height.ToString() });

            _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageSize, Value = size.ToString() + " " + AppResources.ImageInfoPageMp });

            _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageSizeOnDisk, Value = String.Format(new FileSizeFormatProvider(), "{0:fs}", bundleImage.imageSize) });


            if (bundleImage.Exif.empty == false)
            {

                DateTime t = PhoneHelper.UnixTimeStampToDateTime(bundleImage.Exif.timestamp);
                string shutter = "0";

                if (bundleImage.Exif.shutter != 0)
                {
                    shutter = bundleImage.Exif.shutter > 1 ? Math.Round(bundleImage.Exif.shutter, 1).ToString() : "1/" + (Math.Round(1 / bundleImage.Exif.shutter, 0)).ToString();
                }

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageFocal, Value = bundleImage.Exif.focal_len.ToString() + " mm" });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageAperture, Value = "f/" + Math.Round(bundleImage.Exif.aperture, 1).ToString().Replace(',', '.') });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageISO, Value = bundleImage.Exif.iso_speed.ToString() });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageShutter, Value = shutter + " " + AppResources.ImageInfoPageSec });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageTime, Value = t.ToShortDateString() + "  " + t.ToShortTimeString() });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageFlash, Value = FlashUsed[bundleImage.Exif.flash] });

                if (bundleImage.Exif.gps_parsed)
                {
                    string URLloc = "";
                    if (bundleImage.Exif.gps_latitude > 0 || bundleImage.Exif.gps_longtitude > 0)
                    {
                        URLloc = "http://maps.google.com/maps?q=" + bundleImage.Exif.gps_latitude + "," + bundleImage.Exif.gps_longtitude;
                    }
                    _PropertieList.Add(new MyData() { URL = URLloc, Title = AppResources.ImageInfoPageLocation, Value = bundleImage.Exif.gps_latitude + "°, " + bundleImage.Exif.gps_longtitude + "°" });
                }

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageManufacture, Value = bundleImage.Exif.manufacturer.ToString() });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageModel, Value = bundleImage.Exif.model.ToString() });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageSoftware, Value = bundleImage.Exif.Software.ToString() });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageDescription, Value = bundleImage.Exif.desc });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageShotOrder, Value = bundleImage.Exif.shot_order.ToString() });

                _PropertieList.Add(new MyData() { Title = AppResources.ImageInfoPageAuthor, Value = bundleImage.Exif.author.ToString() });

                
                //_PropertieList.Add(new MyData() { Title = "Bits", Value = bundleImage.Exif.bits.ToString() });

                //_PropertieList.Add(new MyData() { Title = "Colors", Value = bundleImage.Exif.colors.ToString() });  

                //_FileList = await StorageExplorer.GetFileListFromRoot();
            }
                this.RaisePropertyChanged("PropertieList");
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
    }






    public class MyData
    {
        public string Title{ get; set; }

        public string Value{ get; set; }

        public string URL { get; set; }
    }
}
