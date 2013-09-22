using System;
using System.Drawing;
using System.Collections.Generic;

namespace CDownTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Image TestImg = Image.FromFile("../../../ref/img19700.jpg");
            Bitmap boop = (Bitmap)TestImg;
            
            for (int n = 0; n < 50; n = n + 5)
            {
                for (int i = 0; i < 50; i++)
                {
                    Color Pix = boop.GetPixel(5 + n, 300 + i);
                    Console.WriteLine("{0},{1},{2}", Pix.R, Pix.G, Pix.B);
                    boop.SetPixel(5 +n, 300 + i, Color.FromArgb(255, 255, 0, 0));
                }
            }
            boop.Save("./test.png");
            Console.Read();
        }
    }
}
