using System;
using System.IO;
using System.Drawing;
using MJPGSplitter;
using System.Diagnostics;

namespace AutoCountdown
{
    class Program
    {
        static void Main(string[] args)
        {
            MJPEGDecode Decoder = new MJPEGDecode();
            Decoder.NewImage += IncomingImage;

            /*
            using (Stream stdin = Console.OpenStandardInput())
            {
                byte[] buffer = new byte[12];
                int bytes;
                while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Decoder.GiveData(buffer);
                }
            }*/
            
            byte[] TestFile = File.ReadAllBytes("../../../../xab");
            Decoder.GiveData(TestFile);
        }
        static int decodecount = 0;
        static int FramesTimeOut = 0;
        static void IncomingImage(Object sender, NewImageEventArgs e)
        {
            decodecount++;
            if (FramesTimeOut != 0)
            {
                FramesTimeOut--;
                return;
            }
            Bitmap Output = e.DecodedOutput;
            // First test to see if its the frame we want.
            Color Target = Color.FromArgb(255, 49, 76, 153);
            int Tolerance = 15;
            if(decodecount%10 == 0)
                Console.WriteLine("Decoding Frame {0}", decodecount);
            if (FrameCycle(Output, Target, Tolerance, 5) && FrameCycle(Output, Target, Tolerance, 560))
            {
                Console.WriteLine("We seem to have a frame that matches what we want.");
                Bitmap ToBeOCR = CropToText(Output);
                Console.WriteLine("THE TEXT IS {0}", OCRText(ToBeOCR));
                FramesTimeOut = 2000;
                return;
            }
        }

        static string OCRText(Bitmap Input)
        {
            Input.Save("./tmp.png");

            var proc = new System.Diagnostics.Process();
            var info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = @"C:\Program Files (x86)\Tesseract-OCR\tesseract";
            info.Arguments= @"tmp.png out";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            proc.StartInfo = info;
            proc.Start();
            proc.WaitForExit();

            File.Delete("./tmp.png");
            string output = File.ReadAllText("./out.txt");

            File.Delete("./out.txt");
            return output;
        }

        static Bitmap CropToText(Bitmap Input)
        {
            int Top = 272;
            int Bot = Input.Height;
            int Mid = Bot - ((Bot - Top) / 2);
            Color Target = Color.FromArgb(255, 49, 76, 153);

            int Lx = 0;
            while (ColTestWTolerance(Input.GetPixel(Lx,Mid), Target, 15))
            {
                Lx++;
            }


            int Rx = Input.Width -1;
            while (ColTestWTolerance(Input.GetPixel(Rx, Mid), Target, 15))
            {
                Rx--;
            }

            Console.WriteLine("Cropping to {0},{1} {2},{3}");

            Bitmap Returner = new Bitmap(Mid + 200, Bot - 272);
            int Rxx = 0;
            int Ry = 0;
            for (int Cx = Lx; Cx < Rx; Cx++)
            {
                for (int Cy = 272; Cy < Bot - 1; Cy++)
                {
                    if (ColTestWTolerance(Input.GetPixel(Cx, Cy), Color.FromArgb(255, 255, 255, 255), 70))
                        Returner.SetPixel(Rxx, Ry, Color.FromArgb(255, 0, 0, 0));
                    else
                        Returner.SetPixel(Rxx, Ry, Color.FromArgb(255, 255, 255, 255));

                    
                    Ry++;
                }
                Ry = 0;
                Rxx++;
            }
            return Returner;
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
