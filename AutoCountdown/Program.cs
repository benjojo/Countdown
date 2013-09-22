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
        static int decodecount = 0;
        static void IncomingImage(Object sender, NewImageEventArgs e)
        {
            //return;
            Bitmap Output = e.DecodedOutput;
            // First test to see if its the frame we want.
            Color Target = Color.FromArgb(255, 49, 76, 153);
            int Tolerance = 15;
            if(decodecount%10 == 0)
                Console.WriteLine("Decoding Frame {0}", decodecount);
            if (FrameCycle(Output, Target, Tolerance, 5) && FrameCycle(Output, Target, Tolerance, 560))
            {
                Console.WriteLine("We seem to have a frame that matches what we want.");
            }
            decodecount++;
        }

        static bool FrameCycle(Bitmap Output,Color Target, int Tolerance,int Shift)
        {
            for (int n = 0; n < 50; n = n + 5)
            {
                for (int i = 0; i < 50; i++)
                {
                    Color Pixel = Output.GetPixel(Shift + n, 300 + i);
                    if (!ColTestWTolerance(Pixel, Target, Tolerance))
                        return false;
                }
            }
            return true;
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
            return (target + tol > input && target - tol < input);
        }
    }
}
