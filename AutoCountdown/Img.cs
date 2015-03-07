using System;
using System.Drawing;

namespace AutoCountdown
{
    class Img
    {
        static public Bitmap CropToText(Bitmap Input)
        {
            int Top = 425;
            int Bot = Input.Height;
            int Mid = Bot - ((Bot - Top) / 2);
            Color Target = Color.FromArgb(255, 0, 37, 119);

            int Lx = 0;
            while (!ColTestWTolerance(Input.GetPixel(Lx, Mid), Target, 15))
            {
                Input.SetPixel(Lx, Mid, Color.FromArgb(255, 255, 0, 0));
                Lx++;
            }


            int Rx = Input.Width - 1;
            while (!ColTestWTolerance(Input.GetPixel(Rx, Mid), Target, 15))
            {
                Input.SetPixel(Rx, Mid, Color.FromArgb(255, 255, 0, 0));
                Rx--;
            }
            //Input.Save("test3.png");

            Console.WriteLine("Cropping to {0},{1} {2},{3}");

            Bitmap Returner = new Bitmap(Mid + 200, Bot - 272);
            int Rxx = 0;
            int Ry = 0;
            for (int Cx = Lx; Cx < Rx; Cx++)
            {
                for (int Cy = 450; Cy < Bot - 1; Cy++)
                {
                    if (ColTestWTolerance(Input.GetPixel(Cx, Cy), Color.FromArgb(255, 255, 255, 255), 90))
                        Returner.SetPixel(Rxx, Ry, Color.FromArgb(255, 0, 0, 0));
                    else
                        Returner.SetPixel(Rxx, Ry, Color.FromArgb(255, 255, 255, 255));
                    Ry++;
                }

                //Returner.Save("test2.png");
                Ry = 0;
                Rxx++;
            }
            return Returner;
        }
        static public bool FrameCycle(Bitmap Output, Color Target, int Tolerance, int Shift)
        {
            for (int n = 0; n < 50; n = n + 5)
            {
                for (int i = 0; i < 50; i = i + 5)
                {
                    Color Pixel = Output.GetPixel(Shift + n, 427 + i);
                    //Output.Save("testtt.png");
                    Output.SetPixel(Shift + n, 450 + i, Color.FromArgb(255, 255, 0, 0));
                    if (!ColTestWTolerance(Pixel, Target, Tolerance))
                    {
                        return false;

                    } 
                }
            }
            //Output.Save("./test.png");
            //Console.WriteLine("hurr");
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
