using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security;
using Microsoft.AspNet.Identity;
using Secure_Password_Repository.Settings;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using System.Security.Principal;

namespace Secure_Password_Repository.Extensions
{
    /// <summary>
    /// Extension methods - just to make life easier
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Convert a byte array to a string
        /// </summary>
        /// <param name="bytes">Byte array to convert to a string</param>
        /// <returns>A string</returns>
        public static string ConvertToString(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        /// <summary>
        /// Convert a string from base64 to ASCII
        /// </summary>
        /// <param name="original">Base64 string to convert</param>
        /// <returns>A string</returns>
        public static string FromBase64(this string original)
        {
            byte[] converted = Convert.FromBase64String(original);
            return Encoding.Default.GetString(converted);
        }

        /// <summary>
        /// Convert a byte array from base64 to ASCII
        /// </summary>
        /// <param name="original">Base64 byte array to convert</param>
        /// <returns>A byte array</returns>
        public static byte[] FromBase64(this byte[] original)
        {
            byte[] converted = Convert.FromBase64String(original.ConvertToString());
            return converted;
        }

        /// <summary>
        /// Convert a byte array to a base64 encoded string
        /// </summary>
        /// <param name="bytes">Byte array to convert to a string</param>
        /// <returns>A base64 encoded string</returns>
        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convert a byte array to a base64 encoded byte array
        /// </summary>
        /// <param name="bytes">Byte array to convert to base64</param>
        /// <returns>A base64 encoded byte array</returns>
        public static byte[] ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).ToBytes();
        }

        /// <summary>
        /// Convert a string to base64
        /// </summary>
        /// <param name="original">String to convert to base64</param>
        /// <returns>A base64 encoded string</returns>
        public static string ToBase64(this string original)
        {
            byte[] converted = Encoding.Default.GetBytes(original);
            return Convert.ToBase64String(converted);
        }

        /// <summary>
        /// Convert a string to base64 byte array
        /// </summary>
        /// <param name="original">String to convert to base64</param>
        /// <returns>A base64 encoded byte array</returns>
        public static byte[] ToBase64Bytes(this string original)
        {
            byte[] converted = Encoding.Default.GetBytes(original);
            return converted.ToBase64();
        }

        /// <summary>
        /// Convert a string to a byte array
        /// </summary>
        /// <param name="original">String to convert to byte array</param>
        /// <returns>A byte array</returns>
        public static byte[] ToBytes(this string original)
        {
            return Encoding.Default.GetBytes(original);
        }

        /// <summary>
        /// Convert a string to an integer. Returns 0 if the value is not an integer
        /// </summary>
        /// <param name="original">String to convert to int</param>
        /// <returns>An integer</returns>
        public static int ToInt(this string original)
        {
            int returnValue;
            if (!int.TryParse(original, out returnValue))
                returnValue = 0;

            return returnValue;
        }

        /// <summary>
        /// Confirms if the user account has permission to edit categories
        /// </summary>
        /// <param name="user">the identity user</param>
        /// <returns>boolean</returns>
        public static bool CanEditCategories(this IPrincipal user)
        {
            if (user == null)
                user = HttpContext.Current.User;
            
            return ApplicationSettings.Default.RoleAllowAddPasswords != "None" && (user.IsInRole(ApplicationSettings.Default.RoleAllowEditCategories) || user.IsInRole("Administrator"));
        }

        /// <summary>
        /// Confirms if the user account has permission to delete categories
        /// </summary>
        /// <param name="user">the identity user</param>
        /// <returns>boolean</returns>
        public static bool CanDeleteCategories(this IPrincipal user)
        {
            if (user == null)
                user = HttpContext.Current.User;

            return ApplicationSettings.Default.RoleAllowAddPasswords != "None" && (user.IsInRole(ApplicationSettings.Default.RoleAllowDeleteCategories) || user.IsInRole("Administrator"));
        }

        /// <summary>
        /// Confirms is the user account has permission to add categories
        /// </summary>
        /// <param name="user">the identity user</param>
        /// <returns>boolean</returns>
        public static bool CanAddCategories(this IPrincipal user)
        {
            if (user == null)
                user = HttpContext.Current.User;

            return ApplicationSettings.Default.RoleAllowAddPasswords != "None" && (user.IsInRole(ApplicationSettings.Default.RoleAllowAddCategories) || user.IsInRole("Administrator"));
        }

        /// <summary>
        /// Confirms if the user account has permission to add passwords
        /// </summary>
        /// <param name="user">the identity user</param>
        /// <returns>boolean</returns>
        public static bool CanAddPasswords(this IPrincipal user)
        {
            if (user == null)
                user = HttpContext.Current.User;

            return ApplicationSettings.Default.RoleAllowAddPasswords != "None" && (user.IsInRole(ApplicationSettings.Default.RoleAllowAddPasswords) || user.IsInRole("Administrator"));
        }

        /// <summary>
        /// Returns the user Id of the identity user - casts the user Id to an intt
        /// </summary>
        /// <param name="user">the identity user</param>
        /// <returns>int</returns>
        public static int GetUserId(this IPrincipal user)
        {
            if (user == null)
                user = HttpContext.Current.User;

            int userId;
            if (!int.TryParse(user.Identity.GetUserId(), out userId))
                userId = 0;

            return userId;
        }

    }

}