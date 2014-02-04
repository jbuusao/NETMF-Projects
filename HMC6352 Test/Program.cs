using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using astra;

namespace Test_HMC6352
{
    public class Program
    {
        public static void Main()
        {
            HMC6352 compass = new HMC6352(0x21);

            while (true)
            {
                Thread.Sleep(1);
                int heading = compass.getHeading();
            }
        }
    }
}
