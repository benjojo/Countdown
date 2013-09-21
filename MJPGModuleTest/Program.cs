using System;
using System.Drawing;
using System.IO;
using MJPGSplitter;

namespace MJPGModuleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MJPEGDecode Decoder = new MJPEGDecode();
            Decoder.NewImage += c_TestImage;
            /*
            byte[] TestFile = File.ReadAllBytes("../../../aaa.mjpeg");
            Decoder.GiveData(TestFile);*/
            /*foreach (byte A in TestFile)
            {
                
            }*/
            
            using (Stream stdin = Console.OpenStandardInput())
            {
                byte[] buffer = new byte[12];
                int bytes;
                while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Decoder.GiveData(buffer);
                }
            }
        }

        static void c_TestImage(Object sender, NewImageEventArgs e)
        {
            Console.WriteLine("I've got a image!!1");
        }
    }
}
