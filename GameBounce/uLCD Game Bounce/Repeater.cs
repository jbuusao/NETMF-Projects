using System;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Microsoft.SPOT;

namespace uLCD_Game_Bounce
{
    public class Repeater
    {
        int repeatDelay;
        NativeEventHandler handler = null;

        public Repeater(NativeEventHandler handler, int repeatDelay)
        {
            this.handler = handler;
            this.repeatDelay = repeatDelay;
        }
        public void run()
        {
            lock (this)
            {
                while (true)
                {
                    Thread.Sleep(repeatDelay);
                    handler.Invoke(0, 0, new DateTime());
                }
            }
        }
    }
}
