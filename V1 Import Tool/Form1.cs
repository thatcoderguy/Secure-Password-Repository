using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Extensions;

namespace V1_Import_Tool
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            SqlConnection version2dbconnection = new SqlConnection("Data Source=" + txtInstance.Text + ";Initial Catalog=" + txtV2DBName.Text + ";Integrated Security=SSPI");

            version2dbconnection.Open();

            EncryptionAndHashing objEncryption = new EncryptionAndHashing();

            objEncryption.systemIterationCount = txtIterationCount.Text;
            objEncryption.systemIV = txtIV.Text;
            objEncryption.systemSalt = txtSalt.Text;

            byte[] privateKey;
            byte[] encryptionKey = new byte[] {};

            //first retrieve DB key
            SqlCommand sqlcomm = new SqlCommand("SELECT userPrivateKey, userEncryptionKey FROM AspNetUsers WHERE UserName = '" + txtUsername.Text + ';');
            sqlcomm.Connection = version2dbconnection;
            SqlDataReader sqlreader = sqlcomm.ExecuteReader();
            
            if(sqlreader.HasRows)
            {
                sqlreader.Read();
                privateKey = sqlreader["userPrivateKey"].ToString().ToBytes();
                encryptionKey = sqlreader["userEncryptionKey"].ToString().ToBytes();

                //hash and encrypt the user's password - so this can be used to decrypt the user's private key
                byte[] hashedPassword = objEncryption.Hash_SHA1_ToBytes(txtPassword.Text);
                hashedPassword = objEncryption.Hash_PBKDF2_ToBytes(hashedPassword, txtSalt.Text).ToBase64();

                //decrypt the user private key
                privateKey = objEncryption.Decrypt_AES256_ToBytes(privateKey, hashedPassword).FromBase64();

                //decrypt the user's copy of the password encryption key
                encryptionKey = objEncryption.Decrypt_RSA_ToBytes(encryptionKey, privateKey);
            }

            sqlreader.Close();
            sqlreader.Dispose();
            sqlcomm.Dispose();

            //INSERT INTO [dbo].[AspNetUsers] ([userPrivateKey],[userPublicKey],[userEncryptionKey],[userFullName],[isAuthorised],[isActive],[userLastEncryptionKeyUpdate],[Email],[EmailConfirmed],
            //[PasswordHash],[SecurityStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEndDateUtc],[LockoutEnabled],[AccessFailedCount],[UserName])
            //SELECT 'pk','pk','ek', [userFullName], 1, [userActive], NULL, 'emil',1,[userPassword],NULL,'',0,0,NULL,1,0,[userLogin] FROM [tblUser]

            sqlcomm = new SqlCommand("SELECT id,UserName,PasswordHash FROM AspNetUsers");
            sqlcomm.Connection = version2dbconnection;
            sqlreader = sqlcomm.ExecuteReader();

            
            if (sqlreader.HasRows)
            {
                while(sqlreader.Read())
                {
                    objEncryption.Generate_NewRSAKeys();
                    string newPrivateKey = objEncryption.Retrieve_PrivateKey();
                    string newPublicKey = objEncryption.Retrieve_PublicKey();
                    objEncryption.Destroy_RSAKeys();

                    string newPassword = sqlreader["PasswordHash"].ToString();

                    //hash the user's password
                    byte[] hashedPassword = objEncryption.Hash_SHA1_ToBytes(newPassword);
                    hashedPassword = objEncryption.Hash_PBKDF2_ToBytes(hashedPassword, txtSalt.Text);

                    newPassword = objEncryption.Hash_PBKDF2(newPassword).ToBase64();

                    //Encrypt privateKey with the user's encryptionkey (based on their password)
                    newPrivateKey = objEncryption.Encrypt_AES256_ToBytes(newPrivateKey.ToBase64Bytes(), hashedPassword).ToBase64String();
                    
                    string newEncryptionKey = objEncryption.Encrypt_RSA_ToBytes(encryptionKey, newPublicKey).ToString();

                    SqlCommand sqlcomm2 = new SqlCommand("UPDATE AspNetUsers SET [userPrivateKey]='" + newPrivateKey + "',[userPublicKey]='" + newPublicKey + "',[userEncryptionKey]='" + newEncryptionKey + "',[PasswordHash]='" + hashedPassword.ToBase64String() + "' WHERE [id]=" + sqlreader["id"].ToString());

                    sqlcomm2.ExecuteNonQuery();
                    sqlcomm2.Dispose();

                }
            }

            sqlreader.Close();
            sqlreader.Dispose();
            sqlcomm.Dispose();

            /*
                INSERT INTO [dbo].[Password] ([Description],[EncryptedUserName],[EncryptedSecondCredential],[EncryptedPassword],[Location],[Notes],[Parent_CategoryId],[Deleted],[PasswordOrder],[Creator_Id],[CreatedDate],[ModifiedDate]
                SELECT [passwordSystem],[passwordLogin],'',[passwordPassword],[passwordLocation],'',???,[passwordDeleted],1,[passwordID],[passwordCustodian],[passwordLastUpdated],[passwordLastUpdated]
                FROM tblPassword
             */


            /*
             INSERT INTO [dbo].[UserPassword] ([Id],[PasswordId],[CanEditPassword],[CanDeletePassword],[CanViewPassword],[CanChangePermissions])
                SELECT [userID],[passwordID],1,0,0,0
                FROM [tblUserPassword]
	        */

            sqlcomm = new SqlCommand("SELECT PasswordId,EncryptedUserName,EncryptedPassword FROM Password");
            sqlcomm.Connection = version2dbconnection;
            sqlreader = sqlcomm.ExecuteReader();

            if (sqlreader.HasRows)
            {
                while (sqlreader.Read())
                {

                    string newEncrypedUsername = objEncryption.Encrypt_AES256(sqlreader["EncryptedUserName"].ToString().ToBase64(), encryptionKey).ToBase64();
                    string newEncryptedPassword = objEncryption.Encrypt_AES256(sqlreader["EncryptedPassword"].ToString().ToBase64(), encryptionKey).ToBase64();

                    SqlCommand sqlcomm2 = new SqlCommand("UPDATE AspNetUsers SET EncryptedUserName='" + newEncrypedUsername + ",EncryptedPassword='" + newEncryptedPassword + "' WHERE [id]=" + sqlreader["id"].ToString());

                    sqlcomm2.ExecuteNonQuery();
                    sqlcomm2.Dispose();

                }
            }

            sqlreader.Close();
            sqlreader.Dispose();
            sqlcomm.Dispose();

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
