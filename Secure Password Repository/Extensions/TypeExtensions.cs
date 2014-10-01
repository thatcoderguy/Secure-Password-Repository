using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

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
        public static string CovertToString(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
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
        /// Convert a string from base64 to nortmal
        /// </summary>
        /// <param name="original">Base64 string to convert</param>
        /// <returns>A string</returns>
        public static string FromBase64(this string original)
        {
            byte[] converted = Convert.FromBase64String(original);
            return Encoding.Default.GetString(converted);
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
        /// Convert a string to a byte array
        /// </summary>
        /// <param name="original">String to convert to byte array</param>
        /// <returns>A byte array</returns>
        public static byte[] ToBytes(this string original)
        {
            return Encoding.Default.GetBytes(original);
        }
    }
}