using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Rawer.Resources;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Telerik.Windows.Controls;
using System.Threading;


namespace Rawer
{
    public partial class App : Application
    {
        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        // public static TelemetryClient TelemetryClient;

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }


        public StorageFile TempFileFromURL;

        public StorageFile TempFileFromListBox;

        public string TempFilePathFromPicker;

        public FileOpenPickerContinuationEventArgs FilePickerContinuationArgs { get; set; }

        public FileSavePickerContinuationEventArgs FileSavePickerContinuationArgs { get; set; }

        public Windows.ApplicationModel.DataTransfer.ShareTarget.ShareOperation ShareOperation { get; set; }

        public bool ShareOperationFirstTime;


        public Models.BundleImage BundleImage;

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            //TelemetryClient = new TelemetryClient();

            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru");

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            //Turn off display fade out
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            PhoneApplicationService.Current.ContractActivated += Application_ContractActivated;


            //init Diagnostic - email crash reports
            RDiagnostic d = new RDiagnostic();
            d.Init();

            //setting demension premium or not premium user
            try
            {
                GoogleAnalytics.EasyTracker.GetTracker().SetCustomDimension(1, (System.Convert.ToInt16(InAppHelper.CheckUserHaveProduct())).ToString());
            }
            catch (System.Exception e) { }

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when a contract activation such as a file open or save picker returns 
        // with the picked file or other return values
        private void Application_ContractActivated(object sender, Windows.ApplicationModel.Activation.IActivatedEventArgs e)
        {
            if (e != null)
            {
                var args = e as IContinuationActivatedEventArgs;

                if (args != null)
                    if (e.Kind == ActivationKind.PickFileContinuation)
                    {

                        var filePickerContinuationArgs = e as FileOpenPickerContinuationEventArgs;
                        if (filePickerContinuationArgs.Files != null)
                            if (filePickerContinuationArgs.Files.Count > 0)
                            {

                                this.FilePickerContinuationArgs = filePickerContinuationArgs;

                                //route to mainpage instead of default start page.
                                string incomingFileType = System.IO.Path.GetExtension(filePickerContinuationArgs.Files[0].Name).Replace(".", "");

                                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/LoadingPage.xaml?fileformat=" + incomingFileType, UriKind.Relative));
                            }
                    }
                if (e.Kind == ActivationKind.PickSaveFileContinuation)
                {
                    var filePickerContinuationArgs = e as FileSavePickerContinuationEventArgs;
                    if (filePickerContinuationArgs.File != null)
                    {

                        this.FileSavePickerContinuationArgs = filePickerContinuationArgs;

                        //string incomingFileType = System.IO.Path.GetExtension(filePickerContinuationArgs.Files[0].Name).Replace(".", "");

                        //(Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/LoadingPage.xaml", UriKind.Relative));
                    }
                }
            }
        }


        // Code to execute when a contract activation such as a file open or save picker returns 
        // with the picked file or other return values
        //private void Application_ContractActivated(object sender, Windows.ApplicationModel.Activation.IActivatedEventArgs e)
        //{
        //}

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            ApplicationUsageHelper.Init("2.2");
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            ApplicationUsageHelper.OnApplicationActivated();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {

            ClearAllData();

        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        async void ClearAllData()
        {

            if (ShareOperation != null)
            {

                ShareOperation = null;
            }

            //Clear global variable for next start// need to check that
            if (FilePickerContinuationArgs != null)
            {
                FilePickerContinuationArgs = null;
            }

            //delete main file
            if (BundleImage != null)
            {
                if (BundleImage.PixMapFile != null)
                {
                    await StorageExplorer.DeleteFileFromBufferStorage(BundleImage.PixMapFile);
                }

                //delete edit file
                if (BundleImage.PixMapFile != null)
                {
                    StorageExplorer.DeleteFileFromAppRootFolder(BundleImage.PixMapFile.Path + "_edit");
                }
            }


            //delete file from URLAssoc
            if (TempFileFromURL != null)
            {
                await StorageExplorer.DeleteFileFromBufferStorage(TempFileFromURL);
            }

            //delete file from FilePicker

            if (String.IsNullOrEmpty(TempFilePathFromPicker))
            {
                StorageExplorer.DeleteFileFromBufferStorage(TempFilePathFromPicker);
            }


        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            ClearAllData();


            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            //RadPhoneApplicationFrame frame = new RadPhoneApplicationFrame();



            //RadPlaneProjectionAnimation planeRight = new RadPlaneProjectionAnimation();
            // planeRight.Axes= PerspectiveAnimationAxis.Z;
            // planeRight.StartAngleZ= 180;
            // planeRight.EndAngleZ = 90;



            // RadScaleMoveAndRotateAnimation planeLeft = new RadScaleMoveAndRotateAnimation();
            // planeRight.Axes= PerspectiveAnimationAxis.X;
            // planeRight.CenterX = 0.5;
            // planeRight.StartAngleX = 360;
            // planeRight.EndAngleX = 0;

            //planeRight.StartAngleZ= 90;
            //planeRight.EndAngleZ = 0;




            //frame.ClockwiseOrientationChangeAnimation = planeRight;
            // frame.CounterClockwiseOrientationChangeAnimation = planeLeft;

            //RootFrame = frame;

            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            RootFrame.UriMapper = new AssociationUriMapper();

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Handle contract activation such as a file open or save picker
            PhoneApplicationService.Current.ContractActivated += Application_ContractActivated;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}