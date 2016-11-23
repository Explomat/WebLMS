using System;

namespace WebLMS.Utils.Sender
{
    interface ISender
    {
        void SendFileLink(string pathTo, string link);
    }
}
