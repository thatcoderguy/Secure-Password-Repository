using Secure_Password_Repository.Models;
using Secure_Password_Repository.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Web;

namespace Secure_Password_Repository.Extensions
{

    /// <summary>
    /// This static class contains functions that only this application would find useful
    /// </summary>
    public static class SecurePasswordRepositorySpecificFunctions
    {

        private static CacheEntryRemovedCallback onRemove = new CacheEntryRemovedCallback(RemovedCallback);

        public static string Generate_UserEncryptionKey(string plainTextPassword)
        {
            string hashedPassword = EncryptionAndHashing.Hash_SHA1(plainTextPassword);

            hashedPassword = EncryptionAndHashing.Hash_PBKDF2(hashedPassword, ApplicationSettings.Default.SystemSalt);

            return hashedPassword;
        }

        public static string Generate_Encrypted_UserEncryptionKey(string plainTextPassword)
        {
            string encryptedPassword = EncryptionAndHashing.Hash_SHA1(plainTextPassword);

            encryptedPassword = EncryptionAndHashing.Hash_PBKDF2(encryptedPassword, ApplicationSettings.Default.SystemSalt);


            byte[] hashedPasswordBytes = Encoding.Default.GetBytes(encryptedPassword);
            encryptedPassword = string.Empty;

            EncryptionAndHashing.Encrypt_DPAPI(ref hashedPasswordBytes);

            encryptedPassword = Convert.ToBase64String(hashedPasswordBytes);

            return encryptedPassword;
        }

        public static string Decrypt_UserEncryptionKey(string encryptedKey)
        {

            byte[] encryptedBytes = Convert.FromBase64String(encryptedKey);

            EncryptionAndHashing.Decrypt_DPAPI(ref encryptedBytes);

            return Encoding.Default.GetString(encryptedBytes);
        }

        public static void RemovedCallback(CacheEntryRemovedArguments arguments)
        {
            System.Runtime.Caching.MemoryCache.Default.Set(arguments.CacheItem.Key, arguments.CacheItem.Value, new CacheItemPolicy() { RemovedCallback = onRemove, Priority = CacheItemPriority.Default, SlidingExpiration = TimeSpan.FromHours(1), AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration });
        }

    }
}