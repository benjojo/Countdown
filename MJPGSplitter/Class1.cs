using System;
using System.Collections.Generic;

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
        private List<byte> Buffer = new List<byte>();

        public void GiveData(byte[] Inbound)
        {
            foreach (byte Input in Inbound)
            {
                Buffer.Add(Input);
            }
        }
    }
}
