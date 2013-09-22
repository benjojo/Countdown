using System;
using System.IO;
using TinyTwitter;

namespace AutoCountdown
{
    class Twitter
    {

        public void Tweet(string text)
        {
            string[] creds = File.ReadAllLines("./config.cfg");
            OAuthInfo TInfo = new OAuthInfo();
            TInfo.ConsumerKey = creds[0];
            TInfo.ConsumerSecret = creds[1];
            TInfo.AccessToken = creds[2];
            TInfo.AccessSecret = creds[3];
            TinyTwitter.TinyTwitter Manager = new TinyTwitter.TinyTwitter(TInfo);

            Manager.UpdateStatus(text);

        }
    }
}
