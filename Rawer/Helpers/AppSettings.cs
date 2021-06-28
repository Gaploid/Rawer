using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawer
{
    public class AppSettings
    {
        // Our settings
        IsolatedStorageSettings settings;

        // The key names of our settings
        const string WhiteBalanceSettingKeyName = "WhiteBalanceSetting";
        const string HalfSizeSettingKeyName = "HalfSizeSetting";
        const string AutoBrigtnessSettingKeyName = "AutoBrigtnessSetting";
        const string NoiseReductionSettingKeyName = "NoiseReductionBefore";
        const string JpegQualitySettingKeyName = "JpegQuality";
        const string FunnyTextDisabledSettingKeyName = "FunnyTextDisabled";
        const string SaveFileFormatSettingKeyName = "SaveFileFormat";
        const string DeleteAllFilesSettingKeyName = "DeleteAllFilesSetting";
        // The default value of our settings

        const int DeleteAllFilesSettingDefault = 1;
        const bool WhiteBalanceFromShotSettingDefault = true;
        const bool HalfSizeSettingDefault = true;
        const bool AutoBrigtnessSettingDefault = true;
        const bool NoiseReductionSettingDefault = false;
        const int JpegQualitySettingDefault = 95;
        const bool FunnyTextDisabledSettingDefault = false;
        const int SaveFileFormatSettingDefault = 0;
        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
          
            // Get the settings for this application.
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                settings = IsolatedStorageSettings.ApplicationSettings;
            }
           
           
           // settings = IsolatedStorageSettings.ApplicationSettings;
        }


        //public Rawer.ImageEncoder.FileFormat GetFileFormat() {

        //    for (int i = 0; i <= SaveFileFormatSetting; i++) { 
            
                
            
        //    }

        //}

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;
           

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
           return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }


        public int SaveFileFormatSetting
        {
            get
            {
                return GetValueOrDefault<int>(SaveFileFormatSettingKeyName, SaveFileFormatSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(SaveFileFormatSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        public int DeleteAllFilesSetting
        {
            get
            {
                return GetValueOrDefault<int>(DeleteAllFilesSettingKeyName, DeleteAllFilesSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(DeleteAllFilesSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public bool HalfSizeSetting
        {
            get
            {
                return GetValueOrDefault<bool>(HalfSizeSettingKeyName, HalfSizeSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(HalfSizeSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        public bool FunnyTextDisabledSetting
        {
            get
            {
                return GetValueOrDefault<bool>(FunnyTextDisabledSettingKeyName, FunnyTextDisabledSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(FunnyTextDisabledSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        public int JpegQualitySetting
        {
            get
            {
                return GetValueOrDefault<int>(JpegQualitySettingKeyName, JpegQualitySettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(JpegQualitySettingKeyName, value))
                {
                    Save();
                }
            }
        }


        public bool WhiteBalanceSetting
        {
            get
            {
                return GetValueOrDefault<bool>(WhiteBalanceSettingKeyName, WhiteBalanceFromShotSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(WhiteBalanceSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        public bool AutoBrigtnessSetting
        {
            get
            {
                return GetValueOrDefault<bool>(AutoBrigtnessSettingKeyName, AutoBrigtnessSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(AutoBrigtnessSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        public bool NoiseReductionSetting
        {
            get
            {
                return GetValueOrDefault<bool>(NoiseReductionSettingKeyName,NoiseReductionSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(NoiseReductionSettingKeyName, value))
                {
                    Save();
                }
            }
        }
    }
}
