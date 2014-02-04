using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace ADXL345
{
    public class Program
    {
        // Wiring from ADXL345 to FEZ Domino:
        //   a) adxl GND to Domino GND
        //   b) adxl VCC to Domino 3.3V
        //   c) adxl CS  to Domino 3.3V
        //   d) adxl SDO to Domino GND
        //   e) adxl SDA to Domino SDA (Di2)
        //   f) adxl SCL to Domino SCL (Di3)

        const int deviceAddress = 0x53;
        const int valuesRegister = 0x32;
        short[] values = new short[3];
        short[] oldValues = new short[3];
        Reporter reporter = new Reporter();

        public static void Main()
        {
            new Program().run();
        }

        void run()
        {
            values[0] = values[1] = values[2] = -1;
            oldValues[0] = oldValues[1] = oldValues[2] = -1;

            I2CDevice.Configuration configuration = new I2CDevice.Configuration(deviceAddress, 400);
            I2CDevice adxl345 = new I2CDevice(configuration);
            writeCommand(adxl345, 0x2d, 0);
            writeCommand(adxl345, 0x2d, 16);
            writeCommand(adxl345, 0x2d, 8);
            // Set the g range to 2g - typical output: 17, -32, 257 (scale = 3.9 => 66 mg, -124 mg, 1002 mg)
            writeCommand(adxl345, 0x31, 0);
            // Set the g range to 16g - typical output: 2, -4, 31 (scale = 31.2 => 62.4 mg, -124.8 mg, 967 mg)
            // writeCommand(adxl345, 0x31, 3);
            while (true)
            {
                readValues(adxl345, values);
                if ((changed(values[0], oldValues[0])) || (changed(values[1], oldValues[1])) || (changed(values[2], oldValues[2])))
                {
                    //Debug.Print("Values: " + values[0] + ", " + values[1] + ", " + values[2]+"\n");
                    reporter.report(values[0] + "," + values[1] + "," + values[2]+"\n\r");
                }
                oldValues[0] = values[0];
                oldValues[1] = values[1];
                oldValues[2] = values[2];
            }
        }

        bool changed(short val1, short val2)
        {
            return (val1 < (val2 - 5)) || (val1 > val2 + 5);
        }

        void writeCommand(I2CDevice adxl345, byte register, byte value)
        {
            byte[] values = new byte[2];
            values[0] = register;
            values[1] = value;
            I2CDevice.I2CTransaction[] xActions = new I2CDevice.I2CTransaction[1];
            xActions[0] = adxl345.CreateWriteTransaction(values);
            if (adxl345.Execute(xActions, 1000) == 0)
                throw new Exception("Unable to initialize accelerometer");
        }
        void readValues(I2CDevice adxl345, short[] result)
        {
            byte[] values = new byte[6];
            I2CDevice.I2CTransaction[] xActions = new I2CDevice.I2CTransaction[2];
            byte[] RegisterNum = new byte[1] { valuesRegister };
            xActions[0] = adxl345.CreateWriteTransaction(RegisterNum);
            xActions[1] = adxl345.CreateReadTransaction(values);
            if (adxl345.Execute(xActions, 1000) == 0)
                throw new Exception("Unable to read accelerometer");
            result[0] = (short)(values[0] + (values[1] << 8));
            result[1] = (short)(values[2] + (values[3] << 8));
            result[2] = (short)(values[4] + (values[5] << 8));
        }
    }
}