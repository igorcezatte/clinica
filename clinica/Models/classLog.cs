using System;
using System.IO;

namespace clinica.Models
{
    public static class classLog
    {
        #region properties
        public static string path { get; set; }
        #endregion

        #region methods

        public static void writeLog()
        {
            path = @"c:\inetpub\log" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            StreamWriter writer = new StreamWriter(path);

            writer.WriteLine("Aqui posso escrever o que eu quiser...");

            writer.Close();
        }


        public static void writeLog2()
        {
            using (StreamWriter writer = new StreamWriter("C:\\macoratti.txt", true))
            {
                writer.WriteLine("Macoratti .net");
            }
        }
        #endregion

    }
}
