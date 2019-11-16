using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Keylocker
{
    /**
        * KeyLocker 
        * creates 2x an md5 password which can be printed
        *
        * @author Ray Colt <ray_colt@pentagon.mil>
        * @copyright Copyright (c) 2019 Ray Colt
        *
        * for Lauren & Janick 2019/11/11
        */
    public class KeyLocker
    {
        private String date = "";

        public KeyLocker()
        {
            date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"));
        }

        private void createKeys()
        {
            Console.Clear();
            /* back-up in file
            string path = "keys.txt";
            if (!File.Exists(path))
            {
                try
                {
                    // Create the file.
                    using (FileStream fs = File.Create(path))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes("Keys;Date;Time:" + Environment.NewLine);
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            */
            String passwordRouter = GeneratKey();
            System.Console.WriteLine("########## PASSWORD KEYLOCKER ##########\n\r");
            System.Console.WriteLine("Your router PASSWORD for today[" + date + "]:\n\r\n\r" + passwordRouter);
            String codeR = "PASSWORD[" + date + "]:\n\r\n\r " + passwordRouter;
            Thread.Sleep(2000);
            String passwordWpaTkip = GeneratKey();
            System.Console.WriteLine("\n\rYour WPA/TKIP PASSWORD for today[" + date + "]:\n\r\n\r" + passwordWpaTkip);
            String codeWT = "PASSWORD[" + date + "]:\n\r\n\r " + passwordWpaTkip;
            /* back-up in file
            try
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(code);
                }
                Console.WriteLine("\nArchived: " + code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            */
            Console.WriteLine("\n\rDo you want to print the keys? [Y or N]");
            var print = Console.ReadLine();
            if (print.ToLower().Equals("n"))
            {
                Console.WriteLine("\n\rPress ENTER to close window...");
                Console.ReadLine();
            }
            else if (print.ToLower().Equals("y"))
            {
                try
                {
                    PrintDialog printDialog = new PrintDialog();
                    //PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                    PrintDocument printDocument = new PrintDocument();
                    printDialog.Document = printDocument;
                    //printPreviewDialog.Document = printDocument;
                    printDocument.PrintPage += delegate (object sender, PrintPageEventArgs e)
                    {
                        e.Graphics.DrawString(" " + codeR, new Font("Arial", 12), new SolidBrush(Color.Black), new RectangleF(50, 50, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));
                    };

                    printDocument.PrintPage += delegate (object sender, PrintPageEventArgs e)
                    {
                        e.Graphics.DrawString(" " + codeWT, new Font("Arial", 12), new SolidBrush(Color.Black), new RectangleF(50, 150, printDocument.DefaultPageSettings.PrintableArea.Width, printDocument.DefaultPageSettings.PrintableArea.Height));
                    };
                    DialogResult result = printDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        printDocument.Print();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Console.WriteLine("\nDone Printing...\n\rPress ENTER to close window...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("\nWrong input...\n\rPress ENTER to close window...");
                Console.ReadLine();
            }
        }

        private String GeneratKey()
        {
            int length_out = 32;
            Random rdm = new Random();
            String key = ComputeSha256Hash(RandomString(64));
            String result = "";
            for (int i = 0; i < length_out; i++)
            {
                result += key[rdm.Next(0, key.Length)];
            }
            return result;
        }

        private String RandomString(int length)
        {
            Random rdm = new Random();
            String dateKey = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff", new CultureInfo("en-US"));
            Regex pattern = new Regex(@"[/:\s]");
            dateKey = pattern.Replace(dateKey, "");
            dateKey = dateKey.Substring(8, 9);
            String result = "";
            String characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + dateKey + "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < length; i++)
            {
                result += characters[rdm.Next(0, characters.Length)];
            }
            return result;
        }

        private String ComputeSha256Hash(String rawData)
        {
            Random rdm = new Random();
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static void Main(string[] args)
        {
            KeyLocker kl = new KeyLocker();
            kl.createKeys();
        }
    }
}
