using System;
using System.Threading;

namespace WebLMS.Utils
{
    public class WebLMSThread
    {

        public static void StartBackgroundThread(ThreadStart threadStart)
        {
            if (threadStart != null)
            {
                Thread thread = new Thread(threadStart);
                thread.IsBackground = true;
                thread.Start();
            }
        }
    }
}