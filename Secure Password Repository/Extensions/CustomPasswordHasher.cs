using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Secure_Password_Repository.Extensions
{

    /// <summary>
    /// This class overrides the default PasswordHasher in Identity 2.0
    /// Why implement a custom version of the PBKDF2 function? So that the number of iterations can be tweaked to the environment
    /// PBKDF2 is supposed to be slow, and it's slow by using a number of iterations; if you're working on dev
    /// You'll probably want to set the iterations to 1, but when deployed to a live server, you'll want to increase
    /// the number of iterations to something more suiting to the environment, say... 1000, 2000, 3000... etc...
    /// </summary>
    public class CustomPasswordHasher : IPasswordHasher
    {

        public string HashPassword(string Password)
        {
            if (Password == null)
                throw new ArgumentNullException("Missing Password");
            else
                //generate a HMAC using the PBKDF2 method
                return EncryptionAndHashing.Hash_PBKDF2(Password).ToBase64();
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null || hashedPassword.Length ==0)
                throw new ArgumentNullException("Missing Password");

            if (providedPassword == null || providedPassword.Length == 0)
                throw new ArgumentNullException("Missing Password");

            //convert the stored hash from base64
            hashedPassword = hashedPassword.FromBase64();

            //grab the original salt used to generate the HMAC
            string salt = EncryptionAndHashing.Get_PBKDF2Salt(hashedPassword);

            //generate a hash to compare based on the user's password and the salt for the stored password
            providedPassword = EncryptionAndHashing.Hash_PBKDF2(providedPassword, salt);

            //now use the salt and the plain-text password to generate a HMAC and verify
            if (hashedPassword.Equals(providedPassword))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;

        }
    }
}