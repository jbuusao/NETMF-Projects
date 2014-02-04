using System;
using System.Threading;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware.LowLevel;
using GHIElectronics.NETMF.FEZ;

namespace uLCD_144_Test1
{
    public enum Response
    {
        ACK = 0x06,
        NACK = 0x15,
    }
    public enum Color
    {
        //RED = 0xF800,
        //GREEN = 0x07E0,
        //BLUE = 0x001F,
        //BLACK = 0x0000,
        //WHITE = 0xFFFF,
        OPALE = 0x081F,     /// 10000 000000 11111
        P1 = 0xF800,        // 1111 1000 00000000 - R
        P2 = 0x07E0,        // 00000111 11100000 - G
        P3 = 0x001F,        // 00000 000 00011111 - B
        /// 
        ALICEBLUE = 0xF7DF,
        ANTIQUEWHITE = 0xFF5A,
        AQUA = 0x07FF,
        AQUAMARINE = 0x7FFA,
        AZURE = 0xF7FF,
        BEIGE = 0xF7BB,
        BISQUE = 0xFF38,
        BLACK = 0x0000,
        BLANCHEDALMOND = 0xFF59,
        BLUE = 0x001F,
        BLUEVIOLET = 0x895C,
        BROWN = 0xA145,
        BURLYWOOD = 0xDDD0,
        CADETBLUE = 0x5CF4,
        CHARTREUSE = 0x7FE0,
        CHOCOLATE = 0xD343,
        CORAL = 0xFBEA,
        CORNFLOWERBLUE = 0x64BD,
        CORNSILK = 0xFFDB,
        CRIMSON = 0xD8A7,
        CYAN = 0x07FF,
        DARKBLUE = 0x0011,
        DARKCYAN = 0x0451,
        DARKGOLDENROD = 0xBC21,
        DARKGRAY = 0xAD55,
        DARKGREEN = 0x0320,
        DARKKHAKI = 0xBDAD,
        DARKMAGENTA = 0x8811,
        DARKOLIVEGREEN = 0x5345,
        DARKORANGE = 0xFC60,
        DARKORCHID = 0x9999,
        DARKRED = 0x8800,
        DARKSALMON = 0xECAF,
        DARKSEAGREEN = 0x8DF1,
        DARKSLATEBLUE = 0x49F1,
        DARKSLATEGRAY = 0x2A69,
        DARKTURQUOISE = 0x067A,
        DARKVIOLET = 0x901A,
        DEEPPINK = 0xF8B2,
        DEEPSKYBLUE = 0x05FF,
        DIMGRAY = 0x6B4D,
        DODGERBLUE = 0x1C9F,
        FIREBRICK = 0xB104,
        FLORALWHITE = 0xFFDE,
        FORESTGREEN = 0x2444,
        FUCHSIA = 0xF81F,
        GAINSBORO = 0xDEFB,
        GHOSTWHITE = 0xFFDF,
        GOLD = 0xFEA0,
        GOLDENROD = 0xDD24,
        GRAY = 0x8410,
        GREEN = 0x0400,
        GREENYELLOW = 0xAFE5,
        HONEYDEW = 0xF7FE,
        HOTPINK = 0xFB56,
        INDIANRED = 0xCAEB,
        INDIGO = 0x4810,
        IVORY = 0xFFFE,
        KHAKI = 0xF731,
        LAVENDER = 0xE73F,
        LAVENDERBLUSH = 0xFF9E,
        LAWNGREEN = 0x7FE0,
        LEMONCHIFFON = 0xFFD9,
        LIGHTBLUE = 0xAEDC,
        LIGHTCORAL = 0xF410,
        LIGHTCYAN = 0xE7FF,
        LIGHTGOLDENRODYELLOW = 0xFFDA,
        LIGHTGREEN = 0x9772,
        LIGHTGREY = 0xD69A,
        LIGHTPINK = 0xFDB8,
        LIGHTSALMON = 0xFD0F,
        LIGHTSEAGREEN = 0x2595,
        LIGHTSKYBLUE = 0x867F,
        LIGHTSLATEGRAY = 0x7453,
        LIGHTSTEELBLUE = 0xB63B,
        LIGHTYELLOW = 0xFFFC,
        LIME = 0x07E0,
        LIMEGREEN = 0x3666,
        LINEN = 0xFF9C,
        MAGENTA = 0xF81F,
        MAROON = 0x8000,
        MEDIUMAQUAMARINE = 0x6675,
        MEDIUMBLUE = 0x0019,
        MEDIUMORCHID = 0xBABA,
        MEDIUMPURPLE = 0x939B,
        MEDIUMSEAGREEN = 0x3D8E,
        MEDIUMSLATEBLUE = 0x7B5D,
        MEDIUMSPRINGGREEN = 0x07D3,
        MEDIUMTURQUOISE = 0x4E99,
        MEDIUMVIOLETRED = 0xC0B0,
        MIDNIGHTBLUE = 0x18CE,
        MINTCREAM = 0xF7FF,
        MISTYROSE = 0xFF3C,
        MOCCASIN = 0xFF36,
        NAVAJOWHITE = 0xFEF5,
        NAVY = 0x0010,
        OLDLACE = 0xFFBC,
        OLIVE = 0x8400,
        OLIVEDRAB = 0x6C64,
        ORANGE = 0xFD20,
        ORANGERED = 0xFA20,
        ORCHID = 0xDB9A,
        PALEGOLDENROD = 0xEF55,
        PALEGREEN = 0x9FD3,
        PALETURQUOISE = 0xAF7D,
        PALEVIOLETRED = 0xDB92,
        PAPAYAWHIP = 0xFF7A,
        PEACHPUFF = 0xFED7,
        PERU = 0xCC27,
        PINK = 0xFE19,
        PLUM = 0xDD1B,
        POWDERBLUE = 0xB71C,
        PURPLE = 0x8010,
        RED = 0xF800,
        ROSYBROWN = 0xBC71,
        ROYALBLUE = 0x435C,
        SADDLEBROWN = 0x8A22,
        SALMON = 0xFC0E,
        SANDYBROWN = 0xF52C,
        SEAGREEN = 0x2C4A,
        SEASHELL = 0xFFBD,
        SIENNA = 0xA285,
        SILVER = 0xC618,
        SKYBLUE = 0x867D,
        SLATEBLUE = 0x6AD9,
        SLATEGRAY = 0x7412,
        SNOW = 0xFFDF,
        SPRINGGREEN = 0x07EF,
        STEELBLUE = 0x4416,
        TAN = 0xD5B1,
        TEAL = 0x0410,
        THISTLE = 0xDDFB,
        TOMATO = 0xFB08,
        TURQUOISE = 0x471A,
        VIOLET = 0xEC1D,
        WHEAT = 0xF6F6,
        WHITE = 0xFFFF,
        WHITESMOKE = 0xF7BE,
        YELLOW = 0xFFE0,
        YELLOWGREEN = 0x9E66,

    }
    public enum Command
    {
        VersionInfoDeviceRequestOnSerial = 0,
        VersionInfoDeviceRequestOnScreen = 1,

        // GSGC GENERAL COMMANDS DEFINITIONS
        GSGC_AUTOBAUD = 0x55,       // Auto Baud Command
        GSGC_VERSION = 0x56,        // Device Info Request
        GSGC_BACKGND = 0x42,        // Change Background Colour
        GSGC_CLS = 0x45,            // Clear Screen

        GSGC_DISPCONT = 0x59,       // Display Control Functions
        GSGC_SWITCHSTAT = 0x4A,     // Get Switch-Buttons Status
        GSGC_SWITCHSTATWAIT = 0x6A, // Get Switch-Buttons Status with Timeout
        GSGC_SOUND = 0x4E,          // Generate a Tone

        // GSGC GRAPHICS COMMANDS DEFINITIONS
        GSGC_ADDBM = 0x41,          // Add User Bitmap
        GSGC_CIRCLE = 0x43,         // Draw Circle
        GSGC_BM = 0x44,             // Draw User Bitmap
        GSGC_TRIANGLE = 0x47,       // Draw Triangle
        GSGC_IMAGE = 0x49,          // Draw Image-Icon
        GSGC_LINE = 0x4C,           // Draw Line
        GSGC_PIXEL = 0x50,          // Draw Pixel
        GSGC_RDPIXEL = 0x52,        // Read Pixel
        GSGC_SCRNCOPYPASTE = 0x63,  // Screen Copy-Paste
        GSGC_POLYGON = 0x67,        // Draw Polygon
        GSGC_SETPEN = 0x70,         // Set Pen Size
        GSGC_RECTANGLE = 0x72,      // Draw Rectangle

        // GSGC TEXT COMMANDS DEFINITIONS
        GSGC_SETFONT = 0x46,        // Set Font
        GSGC_SETOPAQUE = 0x4F,      // Set Transparent-Opaque Text
        GSGC_STRINGGFX = 0x53,      // “String” of ASCII Text (graphics format)
        GSGC_CHARTXT = 0x54,        // ASCII Character (text format)
        GSGC_BUTTONTXT = 0x62,      // Text Button
        GSGC_STRINGTXT = 0x73,      // “String” of ASCII Text (text format)
        GSGC_CHARGFX = 0x74,        // ASCII Character (graphics format)
        
        // GSGC EXTENDED COMMANDS HEADER DEFINITION
        GSGC_EXTCMD = 0x40,         // Extended Command Header
        // GSGC MEMORY CARD COMMANDS DEFINITIONS
        GSGC_MCAP = 0x41,           // Set Address Pointer of Memory Card
        GSGC_MCCOPYSAVE = 0x43,     // Screen Copy-Save to Memory Card
        GSGC_MCIMAGE = 0x49,        // Display Image-Icon from Memory Card
        GSGC_MCOBJ = 0x4F,          // Display Object from Memory Card
        GSGC_MCRUN = 0x50,          // Run Script (4DSL) Program from Card
        GSGC_MCRDSECTOR = 0x52,     // Read Sector Block Data from Memory Card
        GSGC_MCVIDEO = 0x56,        // Display Video Clip from Memory Card
        GSGC_MCWRSECTOR = 0x57,     // Write Sector Block Data to Memory Card
        GSGC_MCINIT = 0x69,         // Initialise Memory Card
        GSGC_MCRDBYTE = 0x72,       // Read Byte Data from Memory Card
        GSGC_MCWRBYTE = 0x77,       // Write Byte Data to Memory Card

        // GSGC SCRIPTING COMMANDS DEFINITIONS
        GSGC_DELAY = 0x07,          // Delay
        GSGC_SETCNTR = 0x08,        // Set Counter
        GSGC_DECCNTR = 0x09,        // Decrement Counter
        GSGC_JMPNZ = 0x0A,          // Jump to Address If Counter Not Zero
        GSGC_JMP = 0x0B,            // Jump to Address
        GSGC_EXIT = 0x0C,           // Exit-Terminate Script Program
    }
    class uLCD144
    {
        SerialPort LCD_Port = null;
        byte deviceType = 0;
        byte hardWare_rev = 0;
        byte firmWare_rev = 0;
        byte hor_res = 0;
        byte ver_res = 0;

        public uLCD144(String port)
        {
            LCD_Port = new SerialPort(port); //, 115200, Parity.None, 8, StopBits.One);
            LCD_Port.Open();
        }

        public bool cmdeAutoBaud()
        {
            return sendCommand((byte)Command.GSGC_AUTOBAUD);
        }

        public bool cmdeCLS()
        {
            return sendCommand((byte)Command.GSGC_CLS);
        }

        public bool drawRectangle(int x0, int y0, int x1, int y1, int color)
        {
            // Draw Rectangle 0,000 [72 00 01 0A 0B 07 E0] <No Display>

            sendByte((byte)Command.GSGC_RECTANGLE);
            sendByte((byte)x0);
            sendByte((byte)y0);
            sendByte((byte)x1);
            sendByte((byte)y1);
            sendByte((byte)(color >> 8));
            sendByte((byte)(color & 0xff));
            return readByte() == (byte)Response.ACK;
        }

        public bool drawCircle(int x, int y, int radius, int color)
        {
            // Circle 0,000 [43 06 07 08 00 1F] <No Display>
            sendByte((byte)Command.GSGC_CIRCLE);
            sendByte((byte)x);
            sendByte((byte)y);
            sendByte((byte)radius);
            sendByte((byte)(color >> 8));
            sendByte((byte)(color & 0xff));
            return readByte() == (byte)Response.ACK;
        }

        public bool setBackgroundColor(int color)
        {
            sendByte((byte)Command.GSGC_BACKGND);
            sendByte((byte)(color >> 8));
            sendByte((byte)(color & 0xff));
            return readByte() == (byte)Response.ACK;
        }

        public bool drawTextString(int x, int y, String text, int color)
        {
            sendByte((byte)Command.GSGC_STRINGTXT);
            sendByte((byte)x);
            sendByte((byte)y);
            sendByte((byte)0);
            sendByte((byte)(color >> 8));
            sendByte((byte)(color & 0xff));
            sendString(text);
            return readByte() == (byte)Response.ACK;
        }

        public bool drawGraphString(int x, int y, String text, int color)
        {
            sendByte((byte)Command.GSGC_STRINGGFX);
            sendByte((byte)x);
            sendByte((byte)y);
            sendByte((byte)0);
            sendByte((byte)(color >> 8));
            sendByte((byte)(color & 0xff));
            sendByte((byte)1);
            sendByte((byte)1);
            sendString(text);
            return readByte() == (byte)Response.ACK;
        }

        public void cmdeVersionDevice()
        {
            sendByte((byte)Command.GSGC_VERSION);
            Thread.Sleep(10);
            sendByte((byte)Command.VersionInfoDeviceRequestOnScreen);
            Thread.Sleep(10);
            deviceType = readByte();
            Thread.Sleep(10);
            hardWare_rev = readByte();
            Thread.Sleep(10);
            firmWare_rev = readByte();
            Thread.Sleep(10);
            hor_res = readByte();
            Thread.Sleep(10);
            ver_res = readByte();
            Thread.Sleep(10);
        }

        public bool copyScreenToSD(int x, int y, int width, int height, long address)
        {
            // Copy screen to Memory Card 0,109 [40 43 00 00 20 20 00 00 00] <Ack>
            sendByte((byte)Command.GSGC_EXTCMD);
            sendByte((byte)Command.GSGC_MCCOPYSAVE);
            sendByte((byte)x);
            sendByte((byte)y);
            sendByte((byte)width);
            sendByte((byte)height);
            sendByte((byte)((address >> 16) & 0xff));
            sendByte((byte)((address >> 8) & 0xff));
            sendByte((byte)(address & 0xff));
            byte result = readByte();
            return result == (byte)Response.ACK;
        }

        public bool copySDToScreen(int x, int y, int width, int height, long address)
        {
            // Copy Memory card to screen 0,015 [40 49 00 00 20 20 10 01 00 01] <Ack>
            sendByte((byte)Command.GSGC_EXTCMD);
            sendByte((byte)Command.GSGC_MCIMAGE);
            sendByte((byte)x);
            sendByte((byte)y);
            sendByte((byte)width);
            sendByte((byte)height);
            sendByte((byte)0x10); // color mode: 65K color = 2 bytes per pixel
            sendByte((byte)((address >> 16) & 0xff));
            sendByte((byte)((address >> 8) & 0xff));
            sendByte((byte)(address & 0xff));
            byte result = readByte();
            return result == (byte)Response.ACK;
        }

        void sendByte(byte b)
        {
            byte[] buffer = new byte[1];
            buffer[0] = b;
            LCD_Port.Write(buffer, 0, 1);
        }

        void sendString(String text)
        {
            char[] chars = text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                sendByte((byte)chars[i]);
            sendByte((byte)0);
        }

        byte readByte()
        {
            byte[] buffer = new byte[1];
            LCD_Port.Read(buffer, 0, 1);
            return buffer[0];
        }

        bool sendCommand(byte cmde)
        {
            sendByte(cmde);
            LCD_Port.Flush();
            Thread.Sleep(100);
            byte result = readByte();
            bool ok = result == (byte)Response.ACK;
            return ok;
        }
    }
}
