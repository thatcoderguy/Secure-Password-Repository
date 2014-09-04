using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Utilities
{

    /// <summary>
    /// This static class contains functions that only this application would find useful
    /// </summary>
    public static class SecurePasswordRepositorySpecificFunctions
    {

        public static string GenerateEncryptedCookie(string PlainTextUserPassword)
        {
            return "";
        }

        public static string RetreivePlainTextUserPasswordFromEncryptedCookie(string CookieValue)
        {
            return "";
        }

        public static Password RetreivePlainTextStoredPasswordDetailsFromDatabase()
        {
            return new Password();
        }

    }
}