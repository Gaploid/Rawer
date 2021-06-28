using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using Rawer.Resources;

namespace Rawer
{
    public partial class Loader : UserControl
    {
        int TickCount;
        public Loader()
        {
            TickCount = 0;
            InitializeComponent();
            InitArrayOfStrings();
            myStoryboard = (Storyboard)(this.FindName("mystoryboard"));

           
                      
        }

        System.Windows.Threading.DispatcherTimer myDispatcherTimer;
        string BaseName = "ViewerPageLoaderText";

        private void CreateList()
        {
            List<string> list = new List<string>();
            int i =1;
            try
            {
                System.Resources.ResourceManager mgr = new System.Resources.ResourceManager(typeof(AppResources));
                
                list.Add(mgr.GetString(BaseName));

                while (mgr.GetString(BaseName + i.ToString()) != null)
                {
                    //System.Diagnostics.Debug.WriteLine(mgr.GetString(BaseName + i.ToString()));

                    list.Add(mgr.GetString(BaseName + i.ToString()));

                    i += 1;

                }

            }
            catch (System.ArgumentNullException nul) { }
            catch (System.ArgumentOutOfRangeException nul2) { }

            FullList = list;

        }

        

        List<string> FullList = new List<string>();
        //{
        //                    "Openning and Proccessing…\r\nI will look at the picture if you don’t mind."	,
        //                    "Unbelievable photo! These colors are fabulous."	,
        //                    "You should definitely be proud of this picture."	,
        //                    "I will adjust the colors and add smiles to gloomy faces."	,
        //                    "This is an AMAZING shot! Color me impressed!"	,
        //                    "Almost done... Another second... Or two... Or five..."	,
        //                    "This shot is so good, I'm writing a letter to my family right now telling them about it."	,
        //                    "Looks like you are a real pro! A tip of my hat to you."	,
        //                    "This shot is so good, I think museums will get in a bidding war for it someday."	,
        //                    "This shot is so good, I'm getting all misty-eyed (and that's rare for an electronic device)",
        //                    "Bokeh is the aesthetic quality of the blur produced in the out-of-focus parts of an image produced by a lens."	

        //                        };
        int count;

        Random rand;
        Storyboard myStoryboard;

        class MyRowStruct
        {
            public string text { get; set; }
            public int isUsed { get; set; }

            public MyRowStruct(string t, int b) { text = t; isUsed = b; }
        }

        List<MyRowStruct> FinalList = new List<MyRowStruct> { };


        //bool DisabledText { get {AppSettings}; }
               

        void InitArrayOfStrings()
        {
            CreateList();

            rand = new Random();

            //init array
            foreach (var a in FullList)
            {
                FinalList.Add(new MyRowStruct(a, 0));

            }
        }




        private string GetNewStringThatWasNeverBefore()
        {
            MyRowStruct r;
            if (count == 0)
            {
                //Вначале всегда идет первый
               
                FinalList[0].isUsed = 1;
                r = FinalList[0];

            }
            else
            {
                int left = FinalList.FindAll(o => o.isUsed == 0).Count;

                //Если закончились все сначала
                if (left == 0)
                {
                    InitArrayOfStrings();
                    left = FinalList.FindAll(o => o.isUsed == 0).Count;
                }

                //Забираем случайный из оставшихся
                int random = rand.Next(left);
                r = FinalList.FindAll(o => o.isUsed == 0)[random];
                FinalList.FindAll(o => o.isUsed == 0)[random].isUsed = 1;
            }

            count++;

            return r.text;

        }

        public void Start()
        {

            AppSettings s = new AppSettings();

            //if (!Disabled)
            //{
                //myStoryboard = (Storyboard)(this.FindName("mystoryboard"));
                myStoryboard.Begin();


                if (!s.FunnyTextDisabledSetting)
                {
                    txtContent.Text = GetNewStringThatWasNeverBefore();
                    myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 15); // 100 Milliseconds 
                    myDispatcherTimer.Tick += new EventHandler(Each_Tick);
                    myDispatcherTimer.Start();
                }
            //}

        }

        public void Stop() {

            if (mystoryboard != null)
            {
                myStoryboard.Stop();
            }

            if (myDispatcherTimer != null)
            {
                myDispatcherTimer.Stop();
                myDispatcherTimer.Tick -= Each_Tick;
            }       
        }

        private void Each_Tick(object o, EventArgs sender)
        {
            txtContent.Text = GetNewStringThatWasNeverBefore();
        }
    }
}
