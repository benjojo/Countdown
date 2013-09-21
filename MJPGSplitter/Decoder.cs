using System;
using System.Drawing;
using System.IO;

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
        public MJPEGDecode()
        {

        }
        private byte[] JPEGHeader = new byte[12] {0xFF,0xD9,0xFF,0xD8,0xFF,0xE0,0x00,0x10,0x4A,0x46,0x49,0x46};
        private byte[] Buffer = new byte[500 * 1024]; // 500k buffer.
        private int BufferPointer = 0;
        public delegate void NewImageHandler(object sender, EventArgs data);

        public void GiveData(byte[] Inbound)
        {
            foreach (byte Input in Inbound)
            {
                Buffer[BufferPointer] = Input;
                BufferPointer++;
                CheckForJPEGHeaders();
            }
        }

        public void CheckForJPEGHeaders()
        {
            int track = 0;
            for (int i = BufferPointer - 12; i < BufferPointer; i++)
            {
                if (Buffer[BufferPointer] != JPEGHeader[track])
                {
                    return;
                }
                track++;
            }
            //oshit call the cops
            DecodeAndFlush(BufferPointer - 12);
        }

        private int threshold;
        private int total;



        public byte[] ExtractFromArray(int stoppoint)
        {
            byte[] SendBack = new byte[stoppoint];
            // JPEG.

            for (int i = 0; i < stoppoint; i++)
            {
                SendBack[i] = Buffer[i];
            }

            // Clear the buffer
            for (int i = 0; i < Buffer.Length; i++)
            {
                SendBack[i] = 0x00;
            }

            // Put the JPEG header back in the buffer
            for (int i = 0; i < JPEGHeader.Length; i++)
            {
                Buffer[i] = JPEGHeader[i];
            }
            BufferPointer = JPEGHeader.Length;

            return SendBack;
        }

        public void DecodeAndFlush(int x)
        {
            // Get the byte array of JPEG
            try
            {
                MemoryStream JPEGRAW = new MemoryStream(ExtractFromArray(BufferPointer - 12));
                Image MaybeJPEG = Image.FromStream(JPEGRAW); // I have no idea if its gonna be able to do this.
                NewImageEventArgs args = new NewImageEventArgs();
                args.DecodedOutput = new Bitmap(MaybeJPEG);
                OnNewImage(args);
            }
            catch
            {
                //NOTHING BAD EVER HAPPENED.
            }
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
