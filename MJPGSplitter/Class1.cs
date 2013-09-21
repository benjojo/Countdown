using System;
using System.Collections.Generic;
using System.Drawing;

namespace MJPGSplitter
{
    /*
     * Okay so here is how I *think* it works.
     * 
     * When ffmpeg finally splits out a mjpeg file, it is basically just a 
     * bunch of jpeg's in a single flat file,
     * So I am just going to keep scanning for the JPEG header then chop off the buffer
     * and then try and decode that as the JPEG, if this works, then I will fire off a event
     * that will deliver the image object to the real program.
     */
    public class MJPEGDecode
    {
        private byte[] Buffer = new byte[500 * 1024]; // 500k buffer.
        private int BufferPointer = 0;
        public delegate void NewImageHandler(object sender, EventArgs data);

        public void GiveData(byte[] Inbound)
        {
            foreach (byte Input in Inbound)
            {
                Buffer[BufferPointer] = Input;
                BufferPointer++;
            }
        }
        private int threshold;
        private int total;

        public MJPEGDecode()
        {

        }

        public void DecodeAndFlush(int x)
        {

            NewImageEventArgs args = new NewImageEventArgs();
            args.DecodedOutput = new Bitmap(1,1);
            OnNewImage(args);
        }


        protected virtual void OnNewImage(NewImageEventArgs e)
        {
            NewImageEventHandler handler = NewImage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event NewImageEventHandler NewImage;
    }

    public class NewImageEventArgs : EventArgs
    {
        public Bitmap DecodedOutput;
    }

    public delegate void NewImageEventHandler(Object sender, NewImageEventArgs e);
}
