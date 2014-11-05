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
using V1_Import_Tool.Extensions;

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

            SqlConnection version1dbconnection = new SqlConnection("Data Source=" + txtInstance.Text + ";Initial Catalog=" + txtV1DBName.Text + ";Integrated Security=SSPI");
            SqlConnection version2dbconnection = new SqlConnection("Data Source=" + txtInstance.Text + ";Initial Catalog=" + txtV2DBName.Text + ";Integrated Security=SSPI");

            version1dbconnection.Open();
            version2dbconnection.Open();

            EncryptionAndHashing objEncryption = new EncryptionAndHashing();

            objEncryption.systemIterationCount = txtIterationCount.Text;
            objEncryption.systemIV = txtIV.Text;
            objEncryption.systemSalt = txtSalt.Text;

            byte[] privateKey;
            byte[] encryptionKey;

            //first retrieve DB key
            SqlCommand sqlcomm = new SqlCommand("select userPrivateKey, userEncryptionKey FROM AspNetUsers WHERE UserName = '" + txtUsername.Text + ';');
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

            string data = objEncryption.Encrypt_AES256_ToBytes(DataToEncrypt.ToBase64(), encryptionKey).ToBase64();



        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
