using System;
using Microsoft.SPOT;
using System.Threading;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace astra
{
    public class EEPROM
    {
        I2CDevice device = null;
        int deviceAddress;

        public EEPROM(int deviceAddress)
        {
            this.deviceAddress = deviceAddress;
        }

        public void write(short memAddress, byte value)
        {
            byte[] values = new byte[]{(byte)(memAddress>>8), (byte)(memAddress&0xff), value};
            device = IC2Bus.config(deviceAddress);
            I2CDevice.I2CTransaction[] cmd = new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(values) };
            if(device.Execute(cmd, 500) == 0)
                throw new Exception("Unable to write to EEPROM");
        }

        public byte read(short memAddress)
        {
            byte[] value = new byte[1];
            byte[] addr = new byte[2] {(byte)(memAddress>>8), (byte)(memAddress&0xff)};
            device = IC2Bus.config(deviceAddress);
            I2CDevice.I2CTransaction[] cmd = new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(addr), I2CDevice.CreateReadTransaction(value) };
            if (device.Execute(cmd, 500) == 0)
                throw new Exception("Unable to read from EEPROM");
            return value[0];
        }
    }
}
