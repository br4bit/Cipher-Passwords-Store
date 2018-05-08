using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace Cipher_passwords_store
{
    
    public partial class Form1 : Form
    {
        private new String Name;
        private String Password;
        //  List<KeyValuePair<String,User>> Store = new List<KeyValuePair<string, User>>();
        NameValueCollection Store= new NameValueCollection();
        public Form1()
        {
            InitializeComponent();
            Store.Add("ciao", new User("pippo",ConvertToSecureString("paperino")).ToString());
            Store.Add("ciao", new User("pippo2", ConvertToSecureString("paperino")).ToString());
            Console.WriteLine(Store.Get("ciao"));
            WriteToBinaryFile(@"D:\path.bin", Store);
        }
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                DESCryptoServiceProvider cipher = new DESCryptoServiceProvider
                {
                    Key = Encoding.ASCII.GetBytes("ciaociao")
                };
                CryptoStream crypto_stream = new CryptoStream(stream, cipher.CreateEncryptor(), CryptoStreamMode.Write);
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(crypto_stream, objectToWrite);
            }
        }

        public SecureString ConvertToSecureString(string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }

        static String SecureStringToString(SecureString value)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
        public class User
        {
            public User(string username, SecureString password)
            {
                Username = username;
                Password = password;
            }

            public override string ToString()
            {
                return string.Format("{0}:{1}", Username,SecureStringToString(Password));
            }

            protected String Username { get; set; }
            protected SecureString Password { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //opening the subkey  
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Cipher Passwords Store");

            //if it does exist, retrieve the stored values  
            if (key != null)
            {
                Password = key.GetValue("Password").ToString();
                Name = key.GetValue("Name").ToString();
                Console.WriteLine(key.GetValue("Name"));
                Console.WriteLine(key.GetValue("Password"));
                key.Close();
                button1.Hide();
            }
            else
            {
                panel1.Show();
              
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            button2.Show();
            label2.Show();
            label3.Show();
            textBox1.Show();
            textBox2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using(MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, textBox2.Text);
                //opening the subkey  
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Cipher Passwords Store");
                key.SetValue("Name", textBox1.Text);
                key.SetValue("Password", hash);
                key.Close();

            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
          foreach(string key in Store.AllKeys)
            {
                listBox1.Items.Add(key.ToString());
                listBox2.Items.Add(Store[key]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
