using System;
using System.Drawing;

namespace CDownTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Image TestImg = Image.FromFile("../../../ref/img19700.jpg");
            Bitmap boop = (Bitmap)TestImg;

            for (int i = 0; i < 50; i++)
            {
                Color Pix = boop.GetPixel(5, 300 + i);
                Console.WriteLine("{0} {1} {2}", Pix.R, Pix.G, Pix.B);
            }
            Console.Read();
        }
    }
}
