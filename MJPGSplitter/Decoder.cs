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
        private int DecodeCount = 0;
        public void GiveData(byte[] Inbound)
        {
            foreach (byte Input in Inbound)
            {
                Buffer[BufferPointer] = Input;
                BufferPointer++;
                CheckForJPEGHeaders();
            }
        }

        private void CheckForJPEGHeaders()
        {
            try
            {
                int track = -12;
                foreach (byte TestByte in JPEGHeader)
                {
                    int debughelper = BufferPointer + track;
                    if (Buffer[debughelper] != TestByte)
                    {
                        return;
                    }
                    track++;
                }
                //oshit call the flusher
                DecodeAndFlush(BufferPointer);
            }
            catch
            {

            }
        }

        private byte[] ExtractFromArray(int stoppoint)
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
                Buffer[i] = 0x00;
            }

            // Put the JPEG header back in the buffer
            for (int i = 2; i < JPEGHeader.Length; i++)
            {
                Buffer[i-2] = JPEGHeader[i];
            }
            BufferPointer = 10;

            return SendBack;
        }

        private void DecodeAndFlush(int x)
        {
            DecodeCount++;
            if(DecodeCount % 10 == 0)
                Console.Title = DecodeCount + "";
            // Get the byte array of JPEG
            try
            {
                MemoryStream JPEGRAW = new MemoryStream(ExtractFromArray(BufferPointer));
                Image MaybeJPEG = Image.FromStream(JPEGRAW);
                NewImageEventArgs args = new NewImageEventArgs();
                args.DecodedOutput = new Bitmap(MaybeJPEG);
                OnNewImage(args);
                MaybeJPEG.Dispose();
                JPEGRAW.Dispose();
            }
            catch
            {
                Console.WriteLine("Failed to decode buffer");
                //This can happen from time to time, Not always the users fault.
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
