using System;
using System.Threading;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware.LowLevel;
using GHIElectronics.NETMF.FEZ;

namespace uLCD_Game_Bounce
{
    public class GPS
    {
        SerialPort GPS_port;


        char readChar()
        {
            byte[] buffer = new byte[1];
            GPS_port.Read(buffer, 0, 1);
            return (Char)buffer[0];
        }

        String readLine()
        {
            String result = "";
            Char c;
            while ((c = readChar()) != 0 && (c != '\r') && (c != '\n'))
            {
                result = result + c;
            }
            readChar(); // swallow the '\r' or '\n'
            return result;
        }

        void check(uint port, uint state, DateTime time)
        {
            lock (this)
            {
                String line = readLine();
                if (line != null && line.Length != 0)
                    Debug.Print(line);
            }
        }

        public GPS(String port)
        {
            GPS_port = new SerialPort(port, 4800);
            GPS_port.Open();
            Repeater repeat = new Repeater(check, 100);
            new Thread(new ThreadStart(repeat.run)).Start();
        }
    }
}
