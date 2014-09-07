using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Secure_Password_Repository.Extensions
{

    /// <summary>
    /// This static class contains functions that only this application would find useful
    /// </summary>
    public static class SecurePasswordRepositorySpecificFunctions
    {

        //when decrypting the PK, assume this passwd has been modified - veryfy data, if bad, logout

        public static void GenerateEncryptedCookie(string PlainTextUserPassword)
        {
            byte[] bEncryptionKey = EncryptionAndHashing.Generate_RandomBytes(32);
            byte[] bEncryptedPassword = EncryptionAndHashing.Encrypt_AES256_To_Bytes(Encoding.Default.GetBytes(PlainTextUserPassword), bEncryptionKey);

            EncryptionAndHashing.Encrypt_DPAPI(ref bEncryptedPassword);
            EncryptionAndHashing.Encrypt_DPAPI(ref bEncryptionKey);

            //

            Convert.ToBase64String(bEncryptedPassword);


        }

        public static string RetreivePlainTextUserPasswordFromEncryptedCookie()
        {
            return "";
        }

        public static Password RetreivePlainTextStoredPasswordDetailsFromDatabase()
        {
            return new Password();
        }

    }
}