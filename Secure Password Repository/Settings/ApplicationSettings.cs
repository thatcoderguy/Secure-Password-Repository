using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml.Serialization;
using System.Runtime.Caching;
using System.IO;
using Microsoft.VisualBasic;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.ViewModels;

namespace Secure_Password_Repository.Settings
{
    /// <summary>
    /// Application settings class
    /// This stores the options that can be configured in this web application
    /// </summary>
    [Serializable]
    public sealed partial class ApplicationSettings
    {

        //create a new xml serializer to serlize this onject from disk
        private static readonly XmlSerializer serial = new XmlSerializer(typeof(ApplicationSettings));

        //an instance of this class - thus this is a Singleton
        private static volatile ApplicationSettings instance;

        //for locking
        private static Object thisLock = new Object();

        /// <summary>
        /// Persist changes to disk
        /// </summary>
        private static void Save()
        {
            //serialize this object back to disk
            string filename = Path.Combine(HttpRuntime.AppDomainAppPath, "system-config.xml");
            using (StreamWriter sw = new StreamWriter(filename))
            {
                serial.Serialize(sw, instance);
            }

            //make sure the changed values are copied back into cache
            MemoryCache.Default.Set(filename, instance, new CacheItemPolicy() { 
                                                                                AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration, 
                                                                                SlidingExpiration = MemoryCache.NoSlidingExpiration, 
                                                                                Priority = CacheItemPriority.Default });
        }

        /// <summary>
        /// Reset application settings to default values
        /// </summary>
        public static void ResetAppSettings()
        {
            Default.LogoImage = "logo.png";
            Default.SMTPServerAddress = "localhost";
            Default.SMTPServerUsername = string.Empty;
            Default.SMTPServerPassword = string.Empty;
            Default.SystemInitilisationVector = EncryptionAndHashing.Generate_Random_ReadableString(16);
            Default.SystemSalt = EncryptionAndHashing.Generate_Random_ReadableString(32);
            Default.SCryptHashCost = "262144";
            Default.PBKDF2IterationCount = "1000";
            Default.AdminsHaveAccessToAllPasswords = true;
            Default.RoleAllowAddCategories = "User";
            Default.RoleAllowDeleteCategories = "Administrator";
            Default.RoleAllowEditCategories = "Administrator";
            Default.RoleAllowAddPasswords = "User";
            Default.SMTPEmailAddress = "securepasswordrepository@local";
            Default.BroadcastCategoryPositionChange = false;
            Default.BroadcastPasswordPositionChange = false;

            Save();
        }

        /// <summary>
        /// Update application settings
        /// </summary>
        /// <param name="newSettingsModel"></param>
        public void UpdateSettings(SystemSettingViewModel newSettingsModel)
        {
            //TO DO:  extra validation checks at some point - as these are important values
            Default.LogoImage = newSettingsModel.LogoImage;
            Default.PBKDF2IterationCount = newSettingsModel.PBKDF2IterationCount;
            Default.SCryptHashCost = newSettingsModel.SCryptHashCost;
            Default.SMTPEmailAddress = newSettingsModel.SMTPEmailAddress;
            Default.SMTPServerAddress = newSettingsModel.SMTPServerAddress;
            Default.SMTPServerPassword = newSettingsModel.SMTPServerPassword;
            Default.SMTPServerUsername = newSettingsModel.SMTPServerUsername;
            Default.RoleAllowAddCategories = newSettingsModel.RoleAllowAddCategories.Name;
            Default.RoleAllowAddPasswords = newSettingsModel.RoleAllowAddPasswords.Name;
            Default.RoleAllowDeleteCategories = newSettingsModel.RoleAllowDeleteCategories.Name;
            Default.RoleAllowEditCategories = newSettingsModel.RoleAllowEditCategories.Name;
            Default.AdminsHaveAccessToAllPasswords = newSettingsModel.AdminsHaveAccessToAllPasswords;
            Default.BroadcastCategoryPositionChange = newSettingsModel.BroadcastCategoryPositionChange;
            Default.BroadcastPasswordPositionChange = newSettingsModel.BroadcastPasswordPositionChange;

            Save();
        }

        /// <summary>
        /// Returns default instance of this class
        /// </summary>
        public static ApplicationSettings Default
        {
            get
            {

                //get cache and the file name
                string filename = Path.Combine(HttpRuntime.AppDomainAppPath, "system-config.xml");

                //check if an instance of this class is already in cache
                if (MemoryCache.Default.Get(filename) != null)
                {
                    return (ApplicationSettings)MemoryCache.Default.Get(filename);
                }

                //there isnt an instance in cache, so we need to create one
                //lock this thread, so that only one thread creates an insrance
                lock (thisLock)
                {

                    //the thread has been unlocked

                    //check that the object hasnt already been put into cache by another thread
                    if (MemoryCache.Default.Get(filename) != null)
                    {
                        return (ApplicationSettings)MemoryCache.Default.Get(filename);
                    }

                    //if the file does not exist
                    if (!File.Exists(Path.Combine(HttpRuntime.AppDomainAppPath, "system-config.xml")))
                    {
                        //create a new instance of this class
                        instance = new ApplicationSettings();

                        //insert the object into cache - with no expiration (we want this to be persistant in memory)
                        MemoryCache.Default.Set(filename, instance, new CacheItemPolicy() { 
                                                                                            AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration, 
                                                                                            SlidingExpiration = MemoryCache.NoSlidingExpiration, 
                                                                                            Priority = CacheItemPriority.Default });

                        //save to disk
                        Save();

                        //set the default values
                        ResetAppSettings();

                        //return the newly created object
                        return (ApplicationSettings)MemoryCache.Default.Get(filename);

                    }

                    //use a streamreader to read the contents of a file from disk
                    using (StreamReader sr = new StreamReader(filename))
                    {

                        //serialize the object from disk and create the singleton object
                        instance = (ApplicationSettings)serial.Deserialize(sr);

                        //insert the object into cache - with no expiration (we want this to be persistant in memory)
                        MemoryCache.Default.Set(filename, instance, new CacheItemPolicy() { 
                                                                                            AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration, 
                                                                                            SlidingExpiration = MemoryCache.NoSlidingExpiration, 
                                                                                            Priority = CacheItemPriority.Default });

                        //return the newly created object
                        return (ApplicationSettings)MemoryCache.Default.Get(filename);

                    }

                }


            }
        }

        //class properties (settings for this app)
        public string LogoImage { get; set; }
        public string SMTPServerAddress { get; set; }
        public string SMTPEmailAddress { get; set; }
        public string SMTPServerUsername { get; set; }
        public string SMTPServerPassword { get; set; }
        public string SystemSalt { get; set; }
        public string SystemInitilisationVector { get; set; }
        public string SCryptHashCost { get; set; }
        public string PBKDF2IterationCount { get; set; }
        public string RoleAllowEditCategories { get; set; }
        public string RoleAllowDeleteCategories { get; set; }
        public string RoleAllowAddCategories { get; set; }
        public string RoleAllowAddPasswords { get; set; }
        public bool AdminsHaveAccessToAllPasswords { get; set; }
        public bool BroadcastCategoryPositionChange { get; set; }
        public bool BroadcastPasswordPositionChange { get; set; }


    }
}