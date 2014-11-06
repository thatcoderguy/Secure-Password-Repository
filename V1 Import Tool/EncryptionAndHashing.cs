using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    /// <summary>
    /// This class contains all of the different types of encryption and hashing methods for this project
    /// </summary>
    public partial class EncryptionAndHashing
    {

        public string systemIV="";
        public string systemSalt = "";
        public string systemIterationCount = "";

        //used to store the Public and Private Keys after calling Generate_NewRSAKeys()
        //this is so that Retrieve_PublicKey and Retrieve_PrivateKey can be called
        private RSACryptoServiceProvider rsakeys;

        ///<summary>
        ///Generates a new Public Key and a new corresponding Private Key for retrieval
        ///</summary>
        public void Generate_NewRSAKeys()
        {
            rsakeys = new RSACryptoServiceProvider(1024);
            rsakeys.PersistKeyInCsp = false;
        }

        ///<summary>
        ///Destroys the RSA keys generated with Generate_NewRSAKeys()
        ///</summary>
        public void Destroy_RSAKeys()
        {
            rsakeys.Clear();
            rsakeys.Dispose();
        }

        ///<summary>
        ///Retrieves the Public Key that was generated with Generate_NewRSAKeys()
        ///</summary>
        ///<returns>
        ///A XML formatted Public Key for use with the RSA algoritm
        ///</returns>
        public string Retrieve_PublicKey()
        {
            //if the rsa keys havent been generated, then generate a new set
            if (rsakeys == null)
                Generate_NewRSAKeys();

            return rsakeys.ToXmlString(false);
        }

        ///<summary>
        ///Retrieves the Private Key that was generated with Generate_NewRSAKeys()
        ///</summary>
        ///<returns>
        ///A XML formatted Private Key for use with the RSA algoritm
        ///</returns>
        public string Retrieve_PrivateKey()
        {
            //if the rsa keys havent been generated, then generate a new set
            if (rsakeys == null)
                Generate_NewRSAKeys();

            return rsakeys.ToXmlString(true);
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///The text to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public string Encrypt_RSA(string PlainText, string PublicKey)
        {
            string encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText.ToBytes(), true).ConvertToString();

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///Bytes of data to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public string Encrypt_RSA(byte[] PlainText, string PublicKey)
        {
            string encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText, true).ConvertToString();

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///The text to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public byte[] Encrypt_RSA_ToBytes(string PlainText, string PublicKey)
        {
            byte[] encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText.ToBytes(), true);

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Encrypts the supplied text using the RSA algorithm and the Public Key provided
        ///</summary>
        ///<param name="PlainText">
        ///Bytes of data to be encrypted
        ///</param>
        ///<param name="PublicKey">
        ///The public key to encrypt the data with
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public byte[] Encrypt_RSA_ToBytes(byte[] PlainText, string PublicKey)
        {
            byte[] encryptedtext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //import the provided public key
            rsa.FromXmlString(PublicKey);

            encryptedtext = rsa.Encrypt(PlainText, true);

            rsa.Clear();

            return encryptedtext;
        }

        ///<summary>
        ///Decrypts the supplied text using the RSA algorithm and the Private Key provided
        ///</summary>
        ///<param name="EncryptedText">
        ///The text to be decrypted
        ///</param>
        ///<param name="PrivateKey">
        ///The private key to decrypt the data with
        ///</param>
        ///<returns>
        ///An string of decrypted data
        ///</returns>
        public string Decrypt_RSA(string EncryptedText, string PrivateKey)
        {
            string plaintext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //load in the private key for decrypting
            rsa.FromXmlString(PrivateKey);

            plaintext = rsa.Encrypt(EncryptedText.ToBytes(), true).ConvertToString();

            rsa.Clear();

            return plaintext;
        }

        ///<summary>
        ///Decrypts the supplied text using the RSA algorithm and the Private Key provided
        ///</summary>
        ///<param name="EncryptedBytes">
        ///The bytes to be decrypted
        ///</param>
        ///<param name="PrivateKey">
        ///The private key to decrypt the data with
        ///</param>
        ///<returns>
        ///An string of decrypted data
        ///</returns>
        public string Decrypt_RSA(byte[] EncryptedBytes, byte[] PrivateKey)
        {
            string plaintext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //load in the private key for decrypting
            rsa.FromXmlString(PrivateKey.ConvertToString());

            plaintext = rsa.Encrypt(EncryptedBytes, true).ConvertToString();

            rsa.Clear();

            return plaintext;
        }

        ///<summary>
        ///Decrypts the supplied text using the RSA algorithm and the Private Key provided
        ///</summary>
        ///<param name="EncryptedBytes">
        ///The bytes to be decrypted
        ///</param>
        ///<param name="PrivateKey">
        ///The private key to decrypt the data with
        ///</param>
        ///<returns>
        ///An byte array of decrypted data
        ///</returns>
        public byte[] Decrypt_RSA_ToBytes(byte[] EncryptedBytes, byte[] PrivateKey)
        {
            byte[] plaintext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //load in the private key for decrypting
            rsa.FromXmlString(PrivateKey.ConvertToString());

            plaintext = rsa.Decrypt(EncryptedBytes, true);

            rsa.Clear();

            return plaintext;
        }

        ///<summary>
        ///Decrypts the supplied text using the RSA algorithm and the Private Key provided
        ///</summary>
        ///<param name="EncryptedText">
        ///The text to be decrypted
        ///</param>
        ///<param name="PrivateKey">
        ///The private key to decrypt the data with
        ///</param>
        ///<returns>
        ///An byte array of decrypted data
        ///</returns>
        public byte[] Decrypt_RSA_ToBytes(string EncryptedText, string PrivateKey)
        {
            byte[] plaintext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;

            //load in the private key for decrypting
            rsa.FromXmlString(PrivateKey);

            plaintext = rsa.Decrypt(EncryptedText.ToBytes(), true);

            rsa.Clear();

            return plaintext;
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm and returns a string of encrypted text
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public string Encrypt_AES256(string PlainText, string EncryptionKey)
        {

            byte[] BytesToEncrypt = PlainText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length < 32)
                Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');
            else if (EncryptionKey.Length >= 32)
                EncryptionKey = EncryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray().ConvertToString();
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm and returns a string of encrypted text
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///Byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public string Encrypt_AES256(string PlainText, byte[] EncryptionKey)
        {

            byte[] BytesToEncrypt = PlainText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length >= 32)
                Array.Resize(ref EncryptionKey, 32);
            else
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray().ConvertToString();
        }

        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public string Encrypt_AES256(byte[] BytesToEncrypt, string EncryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length < 32)
                Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');
            else if (EncryptionKey.Length >= 32)
                EncryptionKey = EncryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray().ConvertToString();
        }


        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An string of encrypted data
        ///</returns>
        public string Encrypt_AES256(byte[] BytesToEncrypt, byte[] EncryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length >= 32)
                Array.Resize(ref EncryptionKey, 32);
            else
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray().ConvertToString();
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public byte[] Encrypt_AES256_ToBytes(string PlainText, string EncryptionKey)
        {
            byte[] BytesToEncrypt = PlainText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length < 32)
                Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');
            else if (EncryptionKey.Length >= 32)
                EncryptionKey = EncryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray();
        }

        ///<summary>
        ///Encrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="PlainText">
        ///The plain text to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///Byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public byte[] Encrypt_AES256_ToBytes(string PlainText, byte[] EncryptionKey)
        {

            byte[] BytesToEncrypt = PlainText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length >= 32)
                Array.Resize(ref EncryptionKey, 32);
            else
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray();
        }

        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public byte[] Encrypt_AES256_ToBytes(byte[] BytesToEncrypt, string EncryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length < 32)
                Add_StringPadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');
            else if (EncryptionKey.Length >= 32)
                EncryptionKey = EncryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray();
        }


        ///<summary>
        ///Encrypts the supplied bytes using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToEncrypt">
        ///The bytes to be encrypted
        ///</param>
        ///<param name="EncryptionKey">
        ///The byte array of the key used to encrypt the text
        ///</param>
        ///<returns>
        ///An byte array of encrypted data
        ///</returns>
        public byte[] Encrypt_AES256_ToBytes(byte[] BytesToEncrypt, byte[] EncryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (EncryptionKey.Length >= 32)
                Array.Resize(ref EncryptionKey, 32);
            else
                Add_BytePadding(ref EncryptionKey, 32 - EncryptionKey.Length, '\x00');

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;

            //I crypto transform is used to perform the actual decryption vs encryption, hash function are a version of crypto transforms.  
            ICryptoTransform Encryptor = null;

            //Crypto streams allow for encryption in memory.  
            CryptoStream Crypto_Stream = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = EncryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call  
                //If these are changed nothing will work right.  
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation  
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)  
                Crypto_Stream.Write(BytesToEncrypt, 0, BytesToEncrypt.Length);

            }
            finally
            {

                //if the crypto blocks are not clear lets make sure the data is gone  
                if (Crypto != null)
                    Crypto.Clear();

                //Close because of my need to close things then done.  
                Crypto_Stream.Close();

            }

            //Return the memory byte array  
            return MemStream.ToArray();
        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="EncryptedText">
        ///The encrypted text to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public string Decrypt_AES256(string EncryptedText, string DecryptionKey)
        {

            byte[] BytesToDecrypted = EncryptedText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_StringPadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                DecryptionKey = DecryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted.ConvertToString();
        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="EncryptedText">
        ///The encrypted text to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public string Decrypt_AES256(string EncryptedText, byte[] DecryptionKey)
        {

            byte[] BytesToDecrypted = EncryptedText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_BytePadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                Array.Resize(ref DecryptionKey, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted.ConvertToString();
        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToDecrypted">
        ///The encrypted bytes to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public string Decrypt_AES256(byte[] BytesToDecrypted, string DecryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_StringPadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                DecryptionKey = DecryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted.ConvertToString();
        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="EncryptedText">
        ///The encrypted text to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A byte array of decrypted data
        ///</returns>
        public byte[] Decrypt_AES256_ToBytes(string EncryptedText, string DecryptionKey)
        {

            byte[] BytesToDecrypted = EncryptedText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_StringPadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                DecryptionKey = DecryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted;

        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="EncryptedText">
        ///The encrypted text to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public byte[] Decrypt_AES256_ToBytes(string EncryptedText, byte[] DecryptionKey)
        {

            byte[] BytesToDecrypted = EncryptedText.ToBytes();

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_BytePadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                Array.Resize(ref DecryptionKey, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted;
        }


        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToDecrypted">
        ///The encrypted bytes to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public byte[] Decrypt_AES256_ToBytes(byte[] BytesToDecrypted, string DecryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_StringPadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                DecryptionKey = DecryptionKey.Substring(0, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey.ToBytes();
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted;
        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToDecrypted">
        ///The encrypted bytes to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public byte[] Decrypt_AES256_ToBytes(byte[] BytesToDecrypted, byte[] DecryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_BytePadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                Array.Resize(ref DecryptionKey, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted;
        }

        ///<summary>
        ///Decrypts the supplied string using the AES 256 algoritm
        ///</summary>
        ///<param name="BytesToDecrypted">
        ///The encrypted bytes to be decrypted
        ///</param>
        ///<param name="DecryptionKey">
        ///The key used to decrypt the text
        ///</param>
        ///<returns>
        ///A string of decrypted data
        ///</returns>
        public string Decrypt_AES256(byte[] BytesToDecrypted, byte[] DecryptionKey)
        {

            //we need the key to be 32 chars long (256 bits)
            if (DecryptionKey.Length < 32)
                Add_BytePadding(ref DecryptionKey, 32 - DecryptionKey.Length, '\x00');
            else if (DecryptionKey.Length >= 32)
                Array.Resize(ref DecryptionKey, 32);

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;

            try
            {

                Crypto = new RijndaelManaged();
                Crypto.Key = DecryptionKey;
                Crypto.IV = systemIV.ToBytes();
                Crypto.Padding = PaddingMode.PKCS7;

                MemStream = new MemoryStream(BytesToDecrypted);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.  
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.  
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.  
                Stream_Read = new StreamReader(Crypto_Stream);
                BytesToDecrypted = Stream_Read.ReadToEnd().ToBytes();

            }
            finally
            {

                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();

            }

            return BytesToDecrypted.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public string Hash_PBKDF2(string Password)
        {
            byte[] salt;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            //generate a salt and a number of random bytes
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, 32, systemIterationCount.ToInt()))
            {
                salt = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(salt, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public string Hash_PBKDF2(string Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            if (Salt == null || Salt.Length == 0)
                throw new ArgumentNullException("Missing Salt");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, Salt.ToBytes(), systemIterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public string Hash_PBKDF2(byte[] Password)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            saltbytes = Generate_RandomBytes(32);

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, saltbytes, systemIterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>
        /// <returns>
        /// A HMAC converted to a string
        /// </returns>
        public string Hash_PBKDF2(byte[] Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            if (Salt == null || Salt == String.Empty)
                throw new ArgumentNullException("Missing Salt");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, Salt.ToBytes(), systemIterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash.ConvertToString();
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A HMAC converted to Byte array
        /// </returns>
        public byte[] Hash_PBKDF2_ToBytes(string Password)
        {
            byte[] salt;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null)
                throw new ArgumentNullException("Missing Password");

            //generate a salt and a number of random bytes
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, 32, systemIterationCount.ToInt()))
            {
                salt = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(salt, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Password to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>  
        /// <returns>
        /// A HMAC converted to a Byte array
        /// </returns>
        public byte[] Hash_PBKDF2_ToBytes(string Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, Salt.ToBytes(), systemIterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Generates and returns a salted HMAC of the provided string using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <returns>
        /// A hashed byte array
        /// </returns>
        public byte[] Hash_PBKDF2_ToBytes(byte[] Password)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password == null || Password.Length == 0)
                throw new ArgumentNullException("Missing Password");

            //generate a random salt
            saltbytes = Generate_RandomBytes(32);

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, saltbytes, int.Parse(systemIterationCount)))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Generates and returns a salted  HMAC of the provided string and salt using the PBKDF2 algorithm
        /// </summary>
        /// <param name="Password">
        /// Byte array of Password chars to be converted into a HMAC
        /// </param>
        /// <param name="Salt">
        /// The salt, a unique salt means a unique PBKDF2 string
        /// </param>
        /// <returns>
        /// A hashed byte array
        /// </returns>
        public byte[] Hash_PBKDF2_ToBytes(byte[] Password, string Salt)
        {
            byte[] saltbytes;
            byte[] buffer;
            byte[] hash = new byte[1056];

            if (Password.Length == 0 || Password == null)
                throw new ArgumentNullException("Missing Password");

            if (Salt == string.Empty || Salt == null)
                throw new ArgumentNullException("Missing Salt");

            //user specified hash
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password, Salt.ToBytes(), systemIterationCount.ToInt()))
            {
                saltbytes = bytes.Salt;
                buffer = bytes.GetBytes(1024);
            }

            //copy the hash and generated bytes into a byte array (so the HMAC also contains the Salt)
            Buffer.BlockCopy(saltbytes, 0, hash, 0, 32);
            Buffer.BlockCopy(buffer, 0, hash, 32, 1024);

            return hash;
        }

        /// <summary>
        /// Gets the salt from a PBKDF2 generated HMAC
        /// </summary>
        /// <param name="HashedPassword">
        /// The full HMAC generated with the Hash_PBK2DF function
        /// </param>
        /// <returns>
        /// A string containing salt orginally used to generate the HMAC
        /// </returns>
        /// <remarks>
        /// Grabs the first 32 bytes from the string (the Hash_PBKDF2 implementations above use a 32 byte salt)
        /// </remarks>
        public string Get_PBKDF2Salt(string HashedPassword)
        {
            //convert the string to bytes
            byte[] hash = HashedPassword.ToBytes();
            byte[] salt = new byte[32];

            //grab first 32 bytes
            Buffer.BlockCopy(hash, 0, salt, 0, 32);

            //return just the salt
            return salt.ConvertToString();
        }

        /// <summary>
        /// Generates a SHA1 hash from the supplied string and returns a hashed string
        /// </summary>
        /// <param name="PlainText">text to hash</param>
        /// <returns>string of hashed text</returns>
        public string Hash_SHA1(string PlainText)
        {
            string sha1hash = string.Empty;

            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                sha1hash = sha.ComputeHash(PlainText.ToBytes()).ConvertToString();
            }

            return sha1hash;
        }

        /// <summary>
        /// Generates a SHA1 hash from the supplied string and returns hashed bytes
        /// </summary>
        /// <param name="PlainText">text to hash</param>
        /// <returns>byte array</returns>
        public byte[] Hash_SHA1_ToBytes(string PlainText)
        {
            byte[] sha1hash;

            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                sha1hash = sha.ComputeHash(PlainText.ToBytes());
            }

            return sha1hash;
        }

        ///<summary>
        ///Generates a number random bytes using cryptographically secure RNG
        ///</summary>
        ///<param name="NumberOfBytes">
        ///The number of bytes to return
        ///</param>
        ///<returns>
        ///An array of random bytes
        ///</returns>
        public byte[] Generate_RandomBytes(int NumberOfBytes)
        {
            byte[] randombytes = new byte[NumberOfBytes];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(randombytes);
            }

            return randombytes;
        }

        ///<summary>
        ///Generates a number random bytes using cryptographically secure RNG
        ///</summary>
        ///<param name="NumberOfBytes">
        ///The number of bytes to return
        ///</param>
        ///<returns>
        ///An string of random bytes
        ///</returns>
        public string Generate_RandomString(int NumberOfChars)
        {
            byte[] randombytes = new byte[NumberOfChars];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(randombytes);
            }

            return randombytes.ConvertToString();
        }

        ///<summary>
        ///Generates a number random string of readable chars using the cryptographically secure RNG
        ///</summary>
        ///<param name="NumberOfBytes">
        ///The number of chars to return
        ///</param>
        ///<returns>
        ///A string of random bytes
        ///</returns>
        public string Generate_Random_ReadableString(int NumberOfChars)
        {
            string characterSet = "0123456789ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz!^*()_-=~#@?/.,";
            var buffer = new char[NumberOfChars];
            var usableChars = characterSet.ToCharArray();
            var usableLength = usableChars.Length;

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {

                var random = new byte[NumberOfChars];
                rngCsp.GetNonZeroBytes(random);

                for (int index = 0; index < NumberOfChars; index++)
                {
                    buffer[index] = usableChars[random[index] % usableLength];
                }

            }

            return new string(buffer);
        }

        ///<summary>
        ///Generates a number random string of readable chars using the cryptographically secure RNG
        ///</summary>
        ///<param name="NumberOfBytes">
        ///The number of chars to return
        ///</param>
        ///<returns>
        ///An array of random bytes
        ///</returns>
        public byte[] Generate_Random_ReadableBytes(int NumberOfChars)
        {
            string characterSet = "0123456789ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz!^*()_-=~#@?/.,";
            var buffer = new byte[NumberOfChars];
            var usableChars = characterSet.ToCharArray();
            var usableLength = usableChars.Length;

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {

                var random = new byte[NumberOfChars];
                rngCsp.GetNonZeroBytes(random);

                for (int index = 0; index < NumberOfChars; index++)
                {
                    buffer[index] = (byte)usableChars[random[index] % usableLength];
                }

            }

            return buffer;
        }

        /// <summary>
        /// Pads the string with more chars from the original string
        /// </summary>
        /// <param name="OriginalString">
        /// The string to add padding to
        /// </param>
        /// <param name="NumberOfCharsToAdd">
        /// The number of chars to add to the string
        /// </param>
        /// <param name="PaddingChar">
        /// The char to use as padding (this will be repeated NumberOfCharsToAdd times)
        /// </param>
        public void Add_StringPadding(ref string OriginalString, int NumberOfCharsToAdd, char PaddingChar)
        {
            OriginalString = OriginalString.PadRight(OriginalString.Length + NumberOfCharsToAdd, PaddingChar);
        }

        /// <summary>
        /// Pads the byte array with more chars from the original array
        /// </summary>
        /// <param name="OriginalBytes">
        /// The byte array to add padding to
        /// </param>
        /// <param name="NumberOfBytesToAdd">
        /// The number of chars to add to the string
        /// </param>
        /// <param name="PaddingChar">
        /// The char to use as padding (this will be repeated NumberOfCharsToAdd times)
        /// </param>
        public void Add_BytePadding(ref byte[] OriginalBytes, int NumberOfBytesToAdd, char PaddingChar)
        {
            string padding = "";
            int intOriginalLength = OriginalBytes.Length;
            padding = padding.PadRight(NumberOfBytesToAdd, PaddingChar);
            Array.Resize(ref OriginalBytes, OriginalBytes.Length + NumberOfBytesToAdd);
            Buffer.BlockCopy(padding.ToBytes(), 0, OriginalBytes, intOriginalLength, NumberOfBytesToAdd);
        }

    }
}
