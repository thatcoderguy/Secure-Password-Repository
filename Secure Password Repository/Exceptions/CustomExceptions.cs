using System;
namespace Secure_Password_Repository.Exceptions
{
    public class UserAccountNotFoundException: Exception
    {
        public UserAccountNotFoundException() {}
        public UserAccountNotFoundException(string message);
        public UserAccountNotFoundException(string message, Exception inner);
    }
}