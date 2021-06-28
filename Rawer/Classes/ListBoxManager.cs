using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using Rawer.Models;
using Rawer.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Rawer
{
    class ListBoxManager
    {
        List<StorageFile> DNGLIST;
        PictureCollection _pictures;
        RadDataBoundListBox radDataBoundListBox;
        ObservableCollection<ListImage> collection;
        Dictionary<string, Picture> dic;

        public int NumberOfDNGFiles 
        { 
            get {

                if (DNGLIST != null) { return DNGLIST.Count; } else {
                    return 0;
                
                }               
               
            } 
        }

        public ListBoxManager(RadDataBoundListBox box)
        {

            radDataBoundListBox = box;
            box.ItemStateChanged += box_ItemStateChanged;
            box.RefreshRequested += Box_RefreshRequested;
            radDataBoundListBox.ItemCheckedStateChanged += radDataBoundListBox_ItemCheckedStateChanged;
        }

        private async void Box_RefreshRequested(object sender, EventArgs e)
        {
            await LoadDNGList(false);

            if (NumberOfDNGFiles > 0)
            {
                await FilterJpegList();
            }

            radDataBoundListBox.StopPullToRefreshLoading(true);
        }

        void box_ItemStateChanged(object sender, ItemStateChangedEventArgs e)
        {
            if (e.State == ItemState.Recycling) {
                ((ListImage)e.DataItem).Bitmap = null;      
    
                
                RadDataBoundListBoxItem container = radDataBoundListBox.GetContainerForItem(e.DataItem) as RadDataBoundListBoxItem; 
                container.DataContext = null;
                container.Content = null;
                container.ContentTemplate = null;
            }
            
            //throw new NotImplementedException();
        }


        void radDataBoundListBox_ItemCheckedStateChanged(object sender, ItemCheckedStateChangedEventArgs e)
        {


            //If item was checked then activate delete button
            if (radDataBoundListBox.CheckedItems.Count > 0)
            {

                ((IApplicationBarIconButton)((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage).ApplicationBar.Buttons[3]).IsEnabled = true;

            }

            if (radDataBoundListBox.CheckedItems.Count == 0)
            {

                ((IApplicationBarIconButton)((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage).ApplicationBar.Buttons[3]).IsEnabled = false;
            }

        }


        public void ChangeListBoxLayout(object sender)
        {
            if (radDataBoundListBox.VirtualizationStrategyDefinition.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {


                VirtualizationStrategyDefinition str = new StackVirtualizationStrategyDefinition();
                str.Orientation = System.Windows.Controls.Orientation.Vertical;
                radDataBoundListBox.VirtualizationStrategyDefinition = str;
                radDataBoundListBox.ItemTemplate = (DataTemplate)((MainPage)sender).Resources["DataStackTemplate1"];
                //button.IconUri = new Uri("/Assets/AppBar/appbar.tiles.nine.png", UriKind.Relative);
                ((IApplicationBarIconButton)((MainPage)sender).ApplicationBar.Buttons[1]).IconUri = new Uri("/Assets/AppBar/appbar.tiles.nine.png", UriKind.Relative);
                ((IApplicationBarIconButton)((MainPage)sender).ApplicationBar.Buttons[1]).Text = AppResources.MainPageAppBarLayoutGrid;
                //btnChangeLayout.IconUri= new Uri("/Assets/AppBar/appbar.list.png",UriKind.Relative);
            }
            else
            {
                VirtualizationStrategyDefinition str = new WrapVirtualizationStrategyDefinition();
                str.Orientation = System.Windows.Controls.Orientation.Horizontal;
                radDataBoundListBox.VirtualizationStrategyDefinition = str;
                radDataBoundListBox.ItemTemplate = (DataTemplate)((MainPage)sender).Resources["DataWarpTemplate1"];
                //button.IconUri = new Uri("/Assets/AppBar/appbar.list.png", UriKind.Relative);
                ((IApplicationBarIconButton)((MainPage)sender).ApplicationBar.Buttons[1]).IconUri = new Uri("/Assets/AppBar/appbar.list.png", UriKind.Relative);
                ((IApplicationBarIconButton)((MainPage)sender).ApplicationBar.Buttons[1]).Text = AppResources.MainPageAppBarLayoutList;
                //btnChangeLayout.IconUri = new Uri("/Assets/AppBar/appbar.tiles.nine.png",UriKind.Relative);

            }

            if (collection != null)
                if (collection.Count > 0)
                {
                    radDataBoundListBox.ItemsSource = null;
                    radDataBoundListBox.ItemsSource = collection;
                }
            
            radDataBoundListBox.InvalidateMeasure();
            radDataBoundListBox.InvalidateArrange();
            radDataBoundListBox.UpdateLayout();
           

            //FilterJpegList();
        }


        async public Task LoadDNGList(bool ShowProgressBar)
        {
            if (ShowProgressBar)
            {
                PhoneHelper.ShowProgressBar(AppResources.MainPageListBoxSearching);
            }

            //var queryOptions = new Windows.Storage.Search.QueryOptions();
            //queryOptions.SetPropertyPrefetch(PropertyPrefetchOptions.BasicProperties, new string[] { "System.Size" });
            //StorageFileQueryResult queryResults = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions);
            //IReadOnlyList<StorageFile> files2 = await queryResults.GetFilesAsync();

            using (MediaLibrary lib = new MediaLibrary())
            {
                foreach (PictureAlbum album in lib.RootPictureAlbum.Albums)
                {
                    if (album.Name == "Camera Roll")
                    {
                        _pictures = album.Pictures;
                        break;
                    }
                }
            }
            if (_pictures != null)
                if (_pictures.Count > 0)
                {
                    dic = new Dictionary<string, Picture>(_pictures.Count);
                }



            await Task.Run(() =>
            {
                DateTime now = DateTime.Now;

                StorageFolder folder = KnownFolders.CameraRoll;
                if (folder != null)
                {
                    var files = folder.GetFilesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

                    if (files != null)
                        if (files.Count > 0)
                        {
                            DNGLIST = new List<StorageFile>();


                            foreach (var dng in files)
                            {

                                if (dng.FileType.ToLower() == ".dng")
                                {

                                    DNGLIST.Add(dng);

                                    if (dic != null)
                                    {
                                        var p = dng.Name.IndexOf("__highres");
                                        var str = dng.Name;
                                        if (p >= 0) { str = dng.Name.Substring(0, p); }


                                        dic[dng.Path.ToLower()] = (from obj in _pictures
                                                                   where Path.GetExtension(obj.GetPath()).ToLower() == ".jpg"
                                                                         && obj.Name.StartsWith(str)
                                                                   select obj).FirstOrDefault();

                                    }
                                }

                            }


                            DNGLIST.Sort((x, y) => DateTimeOffset.Compare(y.DateCreated, x.DateCreated));


                        }

                }

                Debug.WriteLine("Search for dng take time:" + (DateTime.Now - now).TotalMilliseconds.ToString());
                Debug.WriteLine("Search for dng end at:" + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString());
            });

            if (DNGLIST == null)
            {
                SetTextPlaceHolderForEmpty();

            }
            else
            {
                if (DNGLIST.Count == 0)
                {
                    SetTextPlaceHolderForEmpty();

                }
            }

            if (ShowProgressBar)
            {
                PhoneHelper.HideProgressBar(0);
            }
        }

        async public Task LoadDNGList()
        {
            await LoadDNGList(true);
        }

        private void SetTextPlaceHolderForEmpty()
        {
            if (radDataBoundListBox.FlowDirection == FlowDirection.RightToLeft)
            {
                radDataBoundListBox.EmptyContentTemplate = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage).Resources["DataEmptyTemplateAR"] as DataTemplate;
                radDataBoundListBox.EmptyContent = AppResources.MainPageNoDNGFiles;
                radDataBoundListBox.UpdateLayout();


            }
            else
            {
                radDataBoundListBox.EmptyContentTemplate = ((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage).Resources["DataEmptyTemplate2"] as DataTemplate;
                radDataBoundListBox.EmptyContent = AppResources.MainPageNoDNGFiles;
                radDataBoundListBox.UpdateLayout();
            }
        }


        public async Task FilterJpegList()
        {

            if (DNGLIST != null)
            {
                if (DNGLIST.Count > 0)
                {
                    _pictures = null;
                    collection = new ObservableCollection<ListImage>();
                    
                    radDataBoundListBox.ItemsSource = null;
                    radDataBoundListBox.ItemsSource = collection;
                    ListImage my = new ListImage();

                    foreach (var dng in DNGLIST)
                    {
                        try
                        {

                            //await Task.Run(() =>
                            //{
                                //DateTime now = DateTime.Now;
                                my = new ListImage();
                                my.DngFile = dng;

                                my.DateCreated = dng.DateCreated;
                                my.FileSize = await StorageExplorer.GetFileSizeAsync(dng); //This should be boost by WINRT PreFetch filestorage access


                                Picture t = null;

                                if (dic != null)
                                {
                                    dic.TryGetValue(dng.Path.ToLower(), out t);
                                    my.JpegFile = t;
                                    
                                }

                              
                                //Debug.WriteLine("FilterTake time:" + (DateTime.Now - now).TotalMilliseconds.ToString());
                            //});
                        
                            collection.Add(my);
                        }
                        catch (FileNotFoundException) { }

                    }

                    Debug.WriteLine("Filter end at:" + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString());
                }
             
            }
        }

        internal void CheckMode(MainPage mainPage)
        {

            if (!radDataBoundListBox.IsCheckModeEnabled)
            {
                radDataBoundListBox.IsCheckModeEnabled = true;
                radDataBoundListBox.IsCheckModeActive = true;
                ((IApplicationBarIconButton)((MainPage)mainPage).ApplicationBar.Buttons[1]).IsEnabled = false;
            }
            else
            {

                radDataBoundListBox.IsCheckModeEnabled = false;
                radDataBoundListBox.IsCheckModeActive = false;

                radDataBoundListBox.CheckedItems.Clear();
                ((IApplicationBarIconButton)((MainPage)mainPage).ApplicationBar.Buttons[1]).IsEnabled = true;
            }
        }

        public async void DeleteSelectedItems()
        {
            if (radDataBoundListBox.CheckedItems.Count > 0)
            {

                int count = radDataBoundListBox.CheckedItems.Count;
                ListImage[] array = new ListImage[count];

                radDataBoundListBox.CheckedItems.CopyTo(array, 0);

                AppSettings settings = new AppSettings();

                for (int i = 0; i < count; i++)
                {
                    ListImage a = array[i] as ListImage;

                    //delete dng
                    try
                    {
                        await a.DngFile.DeleteAsync();
                    }
                    catch (Exception e) { }

                    //If we need to delete all including jpegs
                    if (settings.DeleteAllFilesSetting == 1)
                    {
                        //delete jpeg
                        try
                        {
                            StorageFile f = await StorageFile.GetFileFromPathAsync(a.JpegFile.GetPath());

                            await f.DeleteAsync();
                        }
                        catch (Exception ee) { }
                    }

                    //delete thm and tnl files
                    try {
                        string tnl = Path.GetDirectoryName(a.DngFile.Path) + "\\" + Path.GetFileNameWithoutExtension(a.JpegFile.Name) + ".mp4.tnl";
                        string thm = Path.GetDirectoryName(a.DngFile.Path) + "\\" + Path.GetFileNameWithoutExtension(a.JpegFile.Name) + ".mp4.thm";

                        var ftnl = await StorageFile.GetFileFromPathAsync(tnl);
                        await ftnl.DeleteAsync();

                        var fthm = await StorageFile.GetFileFromPathAsync(thm);
                        await fthm.DeleteAsync();      
                    
                    }
                    catch (Exception eee) { }

                    collection.Remove(a);

                }
            }
        }
    }
}
