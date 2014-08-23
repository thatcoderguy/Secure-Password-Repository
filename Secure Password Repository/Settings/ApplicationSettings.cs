using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml.Serialization;
using System.Web.Caching;
using System.IO;
using Microsoft.VisualBasic;
using Secure_Password_Repository.Utilities;

namespace Secure_Password_Repository.Settings
{
    [Serializable]
    public sealed partial class ApplicationSettings
    {

        //create a new xml serializer to serlize this onject from disk
        private static readonly XmlSerializer serial = new XmlSerializer(typeof(ApplicationSettings));

        //an instance of this class - thus this is a Singleton
        private static ApplicationSettings instance;

        //for locking
        private static Object thisLock = new Object();

        //save changes made to the object
        public static void Save()
        {
            //serialize this object back to disk
            string filename = Path.Combine(HttpRuntime.AppDomainAppPath, "system-config.xml");
            using (StreamWriter sw = new StreamWriter(filename))
            {
                serial.Serialize(sw, instance);
            }
        }

        public static void ResetAppSettings()
        {
            Default.LogoImage = "logo.png";
            Default.SMTPServerAddress = "localhost";
            Default.SMTPServerUsername = "";
            Default.SMTPServerPassword = "";
            Default.SystemInitilisationVector = EncryptionAndHashing.Generate_RandomString(16);
            Default.SystemSalt = EncryptionAndHashing.Generate_RandomString(32);
            Default.SCryptHashCost = "262144";
            Default.DefaultAccountRole = "User";
            Default.PBKDF2IterationCount = "1000";
            Default.AdminsHaveAccessToAllPasswords = true;
            Default.OnlyAdminCanDeletePasswords = true;
            Default.OnlyAdminsCanDeleteCategories = true;
            Default.OnlyAdminsCanEditCategories = true;

            Save();
        }

        public static ApplicationSettings Default
        {
            get
            {

                Cache cache = HttpRuntime.Cache;
                string filename = Path.Combine(HttpRuntime.AppDomainAppPath, "system-config.xml");

                //check if an instance of this class is already in cache
                if (cache[filename] != null)
                {
                    return (ApplicationSettings)cache[filename];
                }

                //there isnt an instance in cache, so we need to create one
                //lock this thread, so that only one thread creates an insrance
                lock (thisLock)
                {

                    //the thread has been unlocked

                    //check that the object hasnt already been put into cache by another thread
                    if (cache[filename] != null)
                    {
                        return (ApplicationSettings)cache[filename];
                    }

                    if (!File.Exists(Path.Combine(HttpRuntime.AppDomainAppPath, "system-config.xml")))
                    {
                        instance = new ApplicationSettings();

                        //insert the object into cache - with no expiration (we want this to be persistant in memory)
                        cache.Insert(filename, instance, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);

                        Save();

                        ResetAppSettings();

                        //return the newly created object
                        return (ApplicationSettings)cache[filename];

                    }

                    //use a streamreader to read the contents of a file from disk
                    using (StreamReader sr = new StreamReader(filename))
                    {

                        //serialize the object from disk and create the singleton object
                        instance = (ApplicationSettings)serial.Deserialize(sr);

                        //insert the object into cache - with no expiration (we want this to be persistant in memory)
                        cache.Insert(filename, instance, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);

                        //return the newly created object
                        return (ApplicationSettings)cache[filename];

                    }

                }


            }
        }

        //class properties (settings for this app)
        public string LogoImage { get; set; }
        public string DefaultAccountRole { get; set; }
        public string SMTPServerAddress { get; set; }
        public string SMTPServerUsername { get; set; }
        public string SMTPServerPassword { get; set; }
        public string SystemSalt { get; set; }
        public string SystemInitilisationVector { get; set; }
        public string SCryptHashCost { get; set; }
        public string PBKDF2IterationCount { get; set; }
        public bool OnlyAdminsCanEditCategories { get; set; }
        public bool OnlyAdminsCanDeleteCategories { get; set; }
        public bool OnlyAdminCanDeletePasswords { get; set; }
        public bool AdminsHaveAccessToAllPasswords { get; set; }

    }
}