using System;
using Microsoft.SPOT;
using System.Threading;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace astra
{
    public class IC2Bus
    {
        static I2CDevice bus = new I2CDevice(new I2CDevice.Configuration(0, 0));
        public static I2CDevice config(int address)
        {
            bus.Config = new I2CDevice.Configuration((ushort)address, 400);
            return bus;
        }
    }
}
