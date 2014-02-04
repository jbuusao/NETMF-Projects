using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.IO;
using System.IO.Ports;
using System.Text;

namespace ADXL345
{
    public class Reporter
    {
        SerialPort UART = null;
        public Reporter()
        {
            try
            {
                UART = new SerialPort("COM1", 115200);
                UART.Open();
            }
            catch (Exception)
            {
            }
        }
        public void report(String s)
        {
            if (UART != null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(s);
                UART.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
