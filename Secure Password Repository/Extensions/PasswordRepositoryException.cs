using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Extensions
{
    public class PasswordRepositoryException : Exception
    {
        public PasswordRepositoryException()
            : base() { }

        public PasswordRepositoryException(string message)
            : base(message) { }

        public PasswordRepositoryException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public PasswordRepositoryException(string message, Exception innerException)
            : base(message, innerException) { }

        public PasswordRepositoryException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}

