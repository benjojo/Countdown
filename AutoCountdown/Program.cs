using System;
using System.IO;
using System.Drawing;
using MJPGSplitter;

namespace AutoCountdown
{
    class Program
    {
        static void Main(string[] args)
        {
            MJPEGDecode Decoder = new MJPEGDecode();
            Decoder.NewImage += IncomingImage;

            
            using (Stream stdin = Console.OpenStandardInput())
            {
                byte[] buffer = new byte[12];
                int bytes;
                while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Decoder.GiveData(buffer);
                }
            }
            /*
            byte[] TestFile = File.ReadAllBytes("../../../aaa.mjpeg");
            Decoder.GiveData(TestFile);*/
        }

        static void IncomingImage(Object sender, NewImageEventArgs e)
        {
            return;
            Bitmap Output = e.DecodedOutput;
            // First test to see if its the frame we want.
            Color Target = Color.FromArgb(255, 49, 76, 153);
            int Tolerance = 10;
            for (int i = 267; i < Output.Height; i++)
            {
                // Move the way down though the image to check.
                Color Pixel = Output.GetPixel(10, i);
                if(!ColTestWTolerance(Pixel, Target , Tolerance))
                    return;
            }
            Console.WriteLine("We seem to have a frame that matches what we want.");
        }


        static bool ColTestWTolerance(Color Input, Color Target, int Tolerance)
        {
            if (NumberTol(Input.R, Target.R, Tolerance) && NumberTol(Input.G, Target.G, Tolerance) && NumberTol(Input.B, Target.B, Tolerance))
                return true;
            else
                return false;
        }

        static bool NumberTol(int input, int target, int tol)
        {
            if (target - tol > input && input < target + tol)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
