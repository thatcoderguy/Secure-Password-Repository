using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
                throw new ArgumentNullException("password");
            else
                //generate a HMAC using the PBKDF2 method
                return EncryptionAndHashing.Hash_PBKDF2(Password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
                throw new ArgumentNullException("password");

            if (providedPassword == null)
                throw new ArgumentNullException("password");

            //grab the original salt used to generate the HMAC
            string salt = EncryptionAndHashing.Get_PBKDF2Salt(hashedPassword);

            //now use the salt and the plain-text password to generate a HMAC and verify
            if (hashedPassword.Equals(EncryptionAndHashing.Hash_PBKDF2(providedPassword, salt, true)))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;

        }
    }
}