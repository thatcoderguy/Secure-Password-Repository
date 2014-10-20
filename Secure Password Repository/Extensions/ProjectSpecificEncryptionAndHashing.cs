using Secure_Password_Repository.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Extensions
{
    public partial class EncryptionAndHashing
    {
        public static void EncryptData(byte[] EncryptionKey, ref byte[] DataToEncrypt)
        {
            //encrypt the details of the new password using AES
            DataToEncrypt = EncryptionAndHashing.Encrypt_AES256_ToBytes(DataToEncrypt.ToBase64(), EncryptionKey).ToBase64();

            //another layer of encryption
            //EncryptionAndHashing.Encrypt_DPAPI(ref DataToEncrypt);
        }

        public static void DecryptData(byte[] EncryptionKey, ref byte[] DataToDecrypt)
        {
            //first decryption padd
            //EncryptionAndHashing.Decrypt_DPAPI(ref DataToDecrypt);

            //convert from base64
            DataToDecrypt = DataToDecrypt.FromBase64();

            //decrypt the data
            DataToDecrypt = EncryptionAndHashing.Decrypt_AES256_ToBytes(DataToDecrypt, EncryptionKey);
        }

        public static void EncryptDatabaseKey(ref byte[] EncryptionKey, string PublicKey)
        {
            //EncryptionKey = EncryptionKey.ToBase64();

            //first level of encryption - using DPAPI
            //EncryptionAndHashing.Encrypt_DPAPI(ref EncryptionKey);

            //second level of encryption - using RSA
            EncryptionKey = EncryptionAndHashing.Encrypt_RSA_ToBytes(EncryptionKey, PublicKey);
        }

        public static void DecryptDatabaseKey(ref byte[] EncryptionKey, byte[] PrivateKey)
        {
            //decrypt the user's copy of the password encryption key
            EncryptionKey = EncryptionAndHashing.Decrypt_RSA_ToBytes(EncryptionKey, PrivateKey);

            //decrypt again
            //EncryptionAndHashing.Decrypt_DPAPI(ref EncryptionKey);

            //we dont need this anymore
            Array.Clear(PrivateKey, 0, PrivateKey.Length);
        }


        public static void EncryptPrivateKey(ref byte[] PrivateKey, string PasswordBasedKey)
        {
            //Encrypt private key with DPAPI
            //Encrypt_DPAPI(ref PrivateKey);

            //hash the user's password
            byte[] hashedPassword = EncryptionAndHashing.Hash_SHA1_ToBytes(PasswordBasedKey);
            hashedPassword = EncryptionAndHashing.Hash_PBKDF2_ToBytes(hashedPassword, ApplicationSettings.Default.SystemSalt).ToBase64();

            //Encrypt privateKey with the user's encryptionkey (based on their password)
            PrivateKey = EncryptionAndHashing.Encrypt_AES256_ToBytes(PrivateKey.ToBase64String(), hashedPassword);
        }

        public static void DecryptPrivateKey(ref byte[] PrivateKey, byte[] PasswordBasedKey)
        {
            //decrypt the key that is used to decrypt the user's private key
            EncryptionAndHashing.Decrypt_Memory_DPAPI(ref PasswordBasedKey);

            //decrypt the user private key
            PrivateKey = EncryptionAndHashing.Decrypt_AES256_ToBytes(PrivateKey, PasswordBasedKey).FromBase64();

            //reencrypt the password based key, as this needs to be persistant in memory
            EncryptionAndHashing.Encrypt_Memory_DPAPI(ref PasswordBasedKey);

            //decrypt again
            //EncryptionAndHashing.Decrypt_DPAPI(ref PrivateKey);

            PrivateKey = PrivateKey.Trim();
        }

    }
}