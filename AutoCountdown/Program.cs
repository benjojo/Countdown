using System;
using System.IO;
using System.Drawing;
using MJPGSplitter;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using TinyTwitter;

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
            
            //byte[] TestFile = File.ReadAllBytes("../../../cntdwn.mjpeg");
            
            //byte[] TestFile = File.ReadAllBytes("urgh.mjpeg");
            //Decoder.GiveData(TestFile);
           
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
            Color Target = Color.FromArgb(255, 56, 74, 130);
            int Tolerance = 45;
            if(decodecount%10 == 0)
                Console.WriteLine("Decoding Frame {0}", decodecount);
            if (Img.FrameCycle(Output, Target, Tolerance, 50) && Img.FrameCycle(Output, Target, Tolerance, 615))
            {
                Console.WriteLine("We seem to have a frame that matches what we want.");
                Bitmap ToBeOCR = Img.CropToText(Output);
                string letters = OCRText(ToBeOCR);
                FramesTimeOut = 5;
                if (letters.Trim() == "" || letters.Length < 5)
                {
                    return;
                }
                letters = letters.Trim(new char[] { '\r', '\n', ' ' });
                Console.WriteLine("THE TEXT IS {0}", letters);

                Searcher Words = new Searcher(letters);
                int limit = 0;
                var aaa = Words.Results.OrderBy(key => key.Value);
                string results = "For '"+letters+"' ";
                foreach (KeyValuePair<string, int> item in aaa)
                {
                    if (Words.Results.Count < (limit + 3))
                        results += string.Format("Word {0} is {1} letters long. ", item.Key, item.Value);
                    limit++;
                }
                Console.WriteLine(results);
                FramesTimeOut = 2000;
                if (limit != 0)
                {
                    Tweet(results, Output);
                    // new Twitter().Tweet(results);
                }
                return;
            }
        }


        static public void Tweet(string tweet_text, Bitmap Input)
        {
            Input.Save("./tmp.png");

            var proc = new System.Diagnostics.Process();
            var info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = @"t";
            info.Arguments = string.Format("update -f tmp.png '{0}'",tweet_text.Replace("'","\'"));
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            proc.StartInfo = info;
            proc.Start();
            proc.WaitForExit();

            File.Delete("./tmp.png");
        }

        static public string OCRText(Bitmap Input)
        {
            Input.Save("./tmp.png");

            var proc = new System.Diagnostics.Process();
            var info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = @"tesseract";
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

    }
}
