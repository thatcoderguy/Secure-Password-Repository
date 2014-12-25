using System;
namespace Secure_Password_Repository.Exceptions
{
    public class UserAccountNotFoundException: Exception
    {
        public UserAccountNotFoundException() {}
        public UserAccountNotFoundException(string message)
        {

        }
        public UserAccountNotFoundException(string message, Exception inner)
        {

        }
    }

    public class PasswordItemNotFoundException : Exception
    {
        public PasswordItemNotFoundException() { }
        public PasswordItemNotFoundException(string message)
        {

        }
        public PasswordItemNotFoundException(string message, Exception inner)
        {

        }
    }

    public class CategoryItemNotFoundException : Exception
    {
        public CategoryItemNotFoundException() { }
        public CategoryItemNotFoundException(string message)
        {

        }
        public CategoryItemNotFoundException(string message, Exception inner)
        {

        }
    }

}