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

            int stage = 2;

            SqlConnection version2dbconnection = new SqlConnection("Data Source=" + txtInstance.Text + ";Initial Catalog=" + txtV2DBName.Text + ";Integrated Security=SSPI");

            version2dbconnection.Open();

            EncryptionAndHashing objEncryption = new EncryptionAndHashing();

            byte[] privateKey;
            byte[] encryptionKey = new byte[] { };
            DataTable dt = new DataTable();

            objEncryption.systemIterationCount = txtIterationCount.Text;
            objEncryption.systemIV = txtIV.Text;
            objEncryption.systemSalt = txtSalt.Text;

            //first retrieve DB key
            SqlCommand sqlcomm = new SqlCommand("SELECT userPrivateKey, userEncryptionKey FROM AspNetUsers WHERE UserName = '" + txtUsername.Text + "';");
            sqlcomm.Connection = version2dbconnection;
            SqlDataReader sqlreader = sqlcomm.ExecuteReader();


            if (sqlreader.HasRows)
            {
                sqlreader.Read();
                privateKey = sqlreader["userPrivateKey"].ToString().FromBase64().ToBytes();
                encryptionKey = sqlreader["userEncryptionKey"].ToString().FromBase64().ToBytes();

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

            switch(stage)
            {
                //update user passwords
                case 1:

                    sqlcomm = new SqlCommand("SELECT id,UserName,PasswordHash FROM AspNetUsers WHERE username<>'" + txtUsername.Text + "' and LEN([PasswordHash])>1 AND LEN([PasswordHash])<30");
                    sqlcomm.Connection = version2dbconnection;
                    sqlreader = sqlcomm.ExecuteReader();

                    if (sqlreader.HasRows)
                    {
                        dt.Load(sqlreader);
                    }

                    sqlreader.Close();
                    sqlreader.Dispose();
                    sqlcomm.Dispose();

                    foreach(DataRow row in dt.Rows) {

                        objEncryption.Generate_NewRSAKeys();
                        string newPrivateKey = objEncryption.Retrieve_PrivateKey();
                        string newPublicKey = objEncryption.Retrieve_PublicKey();
                        objEncryption.Destroy_RSAKeys();

                        string newPassword = row["PasswordHash"].ToString();

                        //hash the user's password
                        byte[] hashedPassword = objEncryption.Hash_SHA1_ToBytes(newPassword);
                        hashedPassword = objEncryption.Hash_PBKDF2_ToBytes(hashedPassword, txtSalt.Text).ToBase64();

                        newPassword = objEncryption.Hash_PBKDF2(newPassword).ToBase64();

                        //Encrypt privateKey with the user's encryptionkey (based on their password)
                        newPrivateKey = objEncryption.Encrypt_AES256_ToBytes(newPrivateKey.ToBase64Bytes(), hashedPassword).ToBase64String();

                        string newEncryptionKey = objEncryption.Encrypt_RSA(encryptionKey, newPublicKey).ToBase64();
              

                        SqlCommand sqlcomm2 = new SqlCommand("UPDATE AspNetUsers SET SecurityStamp='" + Guid.NewGuid().ToString() + "', [userPrivateKey]='" + newPrivateKey + "',[userPublicKey]='" + newPublicKey + "',[userEncryptionKey]='" + newEncryptionKey + "',[PasswordHash]='" + newPassword + "' WHERE [id]=" + row["id"].ToString());

                        sqlcomm2.Connection = version2dbconnection;
                        sqlcomm2.ExecuteNonQuery();
                        sqlcomm2.Dispose();
               

                    }

                    break;

            //encrypt passwords      
            case 2:

                    /*
             
                        INSERT INTO [Secure Password Repository].[dbo].[Password] ([PasswordId],[Description],[EncryptedUserName],[EncryptedSecondCredential],[EncryptedPassword],[Location],[Notes],[Parent_CategoryId],[Deleted],[PasswordOrder],[Creator_Id],[CreatedDate],[ModifiedDate])
                        SELECT p.[passwordID],[passwordSystem],[passwordLogin],'',[passwordPassword],[passwordLocation],'',c.CategoryId,[passwordDeleted],[passwordID],CASE WHEN [passwordCustodian] IS NULL THEN 3 ELSE [passwordCustodian] END,[passwordLastUpdated],[passwordLastUpdated]
                        FROM [passwords].dbo.tblPassword p
                        INNER JOIN [Secure Password Repository].dbo.Category c ON c.CategoryName  COLLATE Latin1_General_CI_AS  =p.passwordType

                        INSERT INTO [dbo].[UserPassword] ([Id],[PasswordId],[CanEditPassword],[CanDeletePassword],[CanViewPassword],[CanChangePermissions])
                        SELECT [userID],[passwordID],1,0,0,0
                        FROM [tblUserPassword]

                        update p
                        set p.EncryptedUserName=p2.passwordLogin,p.EncryptedPassword=p2.passwordPassword
                        FROM [Secure Password Repository].dbo.Password p
                        INNER JOIN [Passwords].dbo.tblPassword p2 ON p2.passwordID=p.PasswordId
                     
                    */


                    sqlcomm = new SqlCommand("SELECT PasswordId,EncryptedUserName,EncryptedPassword FROM Password");
                    sqlcomm.Connection = version2dbconnection;
                    sqlreader = sqlcomm.ExecuteReader();

                    if (sqlreader.HasRows)
                    {

                        dt = new DataTable();

                        if (sqlreader.HasRows)
                        {
                            dt.Load(sqlreader);
                        }
                    }

                    sqlreader.Close();
                    sqlreader.Dispose();
                    sqlcomm.Dispose();

                    foreach (DataRow row in dt.Rows)
                    {

                        byte[] byteEncrypedUsername = row["EncryptedUserName"].ToString().ToBytes();
                        string newEncrypedUsername = objEncryption.Encrypt_AES256_ToBytes(byteEncrypedUsername.ToBase64(), encryptionKey).ToBase64().ToBase64String();

                        byte[] byteEncryptedPassword = row["EncryptedPassword"].ToString().ToBytes();
                        string newEncryptedPassword = objEncryption.Encrypt_AES256_ToBytes(byteEncryptedPassword.ToBase64(), encryptionKey).ToBase64().ToBase64String();

                        SqlCommand sqlcomm2 = new SqlCommand("UPDATE Password SET EncryptedUserName='" + newEncrypedUsername + "',EncryptedPassword='" + newEncryptedPassword + "' WHERE [PasswordId]=" + row["PasswordId"].ToString());

                        sqlcomm2.Connection = version2dbconnection;
                        sqlcomm2.ExecuteNonQuery();
                        sqlcomm2.Dispose();

                    }
            
                    break;
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
