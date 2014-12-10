using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Utilities;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Settings;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Secure_Password_Repository
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationRoleManager : RoleManager<ApplicationRole, int>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, int> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager =  new ApplicationRoleManager(new CustomRoleStore(context.Get<ApplicationDbContext>()));
            manager.RoleValidator = new RoleValidator<ApplicationRole, int>(manager) {};
            return manager;
        }
    }


    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new CustomUserStore(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            //custom service providers
            manager.PasswordHasher = new CustomPasswordHasher();
            manager.EmailService = new EmailService();

            //account lockout config
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(15);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            return manager;
        }

        public virtual async Task<IdentityResult> AddUserToRolesAsync(int userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<ApplicationUser, int>)Store;

            var user = await FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);

            // Add user to each role using UserRoleStore
            foreach (var role in roles)
            {
                await userRoleStore.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are added
            return await UpdateAsync(user).ConfigureAwait(false);
        }

    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Credentials:
            var credentialUserName = ApplicationSettings.Default.SMTPServerUsername;
            var sentFrom = ApplicationSettings.Default.SMTPEmailAddress;
            var pwd = ApplicationSettings.Default.SMTPServerPassword;

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient(ApplicationSettings.Default.SMTPServerAddress);

            client.Port = 25;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.EnableSsl = false;

            if (!string.IsNullOrEmpty(credentialUserName))
            {
                // Create the credentials:
                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential(credentialUserName, pwd);

                client.Credentials = credentials;
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
            }

            // Create the message:
            var mail =
                new System.Net.Mail.MailMessage(sentFrom, message.Destination);

            mail.Subject = message.Subject;

            string plainBodyText = string.Empty;
            string bodyText = string.Empty;

            //attempt to create a plain text view of the email
            try
            {

                //plain text version is stored between html comments
                plainBodyText = Regex.Match(message.Body, "<!--(.*)-->", RegexOptions.Singleline).Groups[1].Value;
                bodyText = message.Body.Replace("<!--" + plainBodyText + "-->","");

                ContentType mimeType = new System.Net.Mime.ContentType("text/plain");
                AlternateView alternate = AlternateView.CreateAlternateViewFromString(plainBodyText, mimeType);

                mail.AlternateViews.Add(alternate);

                mimeType = new System.Net.Mime.ContentType("text/html");
                alternate = AlternateView.CreateAlternateViewFromString(bodyText, mimeType);

                mail.AlternateViews.Add(alternate);

            }
            catch(Exception){

                mail.Body = message.Body;
                mail.IsBodyHtml = true;

            }

            // Send:
            return client.SendMailAsync(mail);
        }
    }

}
