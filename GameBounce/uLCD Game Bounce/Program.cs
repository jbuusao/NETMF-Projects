using System;
using Microsoft.SPOT;
using uLCD_144_Test1;
using System.Threading;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using astra;


// Set the g range to 2g - typical output: 17, -32, 257 (scale = 3.9 => 66 mg, -124 mg, 1002 mg)
// Set the g range to 16g - typical output: 2, -4, 31 (scale = 31.2 => 62.4 mg, -124.8 mg, 967 mg)

namespace uLCD_Game_Bounce
{
    class Rectangle
    {
        public int Left, Top, Width, Height;
        public Rectangle()
        {
        }
        public Rectangle(int Left, int Top, int Width, int Height)
        {
            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
        }
    }

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
        const int offsetAccelerationX = 18;
        const int offsetAccelerationY = 0;

        const int LCDScreenWidth = 128;
        const int LCDScreenHeight = 128;
        const int racketWidth = 20;
        const int racketHeight = 6;
        const int racketDelta = 4;

        Rectangle screenArea = new Rectangle();
        Rectangle racketArea = new Rectangle();
        Rectangle racket = new Rectangle();

        const int accelerationMax = 50;
        int accelerationX = 0;
        int accelerationY = 0;

        const int screenwidth = 128;
        const int screenheight = 128;

        const int windowXpos = 30;
        const int windowYpos = 30;
        const int windowWidth = 110;
        const int windowHeight = 60;

        const int TOPHIT = 9;
        const int BOTTOMHIT = 119;
        const int XSPEED = 3;
        const int YSPEED = 2;

        const int WALLWIDTH = 4;
        const int BALLSIZE = 4;

        const int racketSpeed = 5;
        const int ballSpeed = 10;

        const int screenWidth = 128;
        const int minX = racketDelta;
        const int maxX = screenWidth - racketWidth - racketDelta;
        const int colorBackground = (int)Color.BLACK;
        const int colorRacket = (int)Color.BLUE;
        int racketX = 0;
        int racketY = 128 - racketHeight;
        InterruptPort rightButton = null;
        InterruptPort leftButton = null;
        Thread repeatRight = null;
        Thread repeatLeft = null;
        
        int ball_x, ball_y, ball_colour;
        int xdir, ydir, xspeed, yspeed;
        int tophit, bottomhit, lefthit, righthit;

        uLCD144 LCD = null;
        ADX345 adxl345;

        const int WHITE = 0xFFFF;
        const int BLACK = 0x0000;
        const int YELLOW = 0xFFE0;
        const int LEFTCOLOUR = 0xF800;
        const int RIGHTCOLOUR = 0xFFFF;
        const int TOPCOLOUR = 0x001F;
        const int BOTTOMCOLOUR = 0x07E0;

        const bool testGPS = false;
        const bool testAccelerometer = true;
        const bool testEEPROM = true;

        //EEPROM eeprom = new EEPROM(0x50);

        public Program()
        {
            LCD = new uLCD144("COM2");
            Thread.Sleep(1000);
            LCD.cmdeAutoBaud();
            LCD.cmdeCLS();

            try
            {
                adxl345 = new ADX345(0x53);
            }
            catch (Exception e)
            {
                LCD.drawTextString(0, 4, e.Message, (int)Color.RED);
                while (true) ;
            }
            Debug.EnableGCMessages(false);

            if(testGPS)
                new GPS("COM1");

            /*
            if (testEEPROM)
            {
                if (eeprom.read(0x800) != 0x80)
                    throw new Exception("Invalid value from EEPROM");
            }
             **/
        }
        
        protected void calculateRectangles()
        {
            int x = getScaledValue(accelerationX);
            int y = getScaledValue(accelerationY);
            racketX = getRacketValue(-x);
            //Debug.Print("X=" + x + "=> " + racketX);
        }

        protected int getScaledValue(int val0)
        {
            int width = LCDScreenWidth;
            float ratio = (float)val0 / (float)accelerationMax;
            float val1 = (float)(width / 2) * ratio;
            return (int)val1;
        }

        protected int getRacketValue(int val0)
        {
            int width = LCDScreenWidth;
            float ratio = (float)val0 / (float)accelerationMax;
            float val1 = (float)(width / 2) * ratio;
            int val = (int)(LCDScreenWidth / 2 + (int)val1);
            if (val < 0)
                val = 0;
            if (val > LCDScreenWidth - racketWidth + 1)
                val = LCDScreenWidth - racketWidth + 1;
            return val;
        }

        void rightButtonEvent(uint port, uint state, DateTime time)
        {
            lock (this)
            {
                if (repeatRight == null)
                {
                    Repeater repeat = new Repeater(rightButtonFire, racketSpeed);
                    repeatRight = new Thread(new ThreadStart(repeat.run));
                    repeatRight.Start();
                    rightButtonFire(port, state, time);
                }
                else
                {
                    repeatRight.Abort();
                    repeatRight = null;
                }
            }
        }

        void rightButtonFire(uint port, uint state, DateTime time)
        {
            lock (this)
            {
                if (racketX < maxX)
                {
                    LCD.drawRectangle(racketX, racketY, racketX + racketWidth - 1, racketY + racketHeight - 1, colorBackground);
                    racketX += racketDelta;
                    LCD.drawRectangle(racketX, racketY, racketX + racketWidth - 1, racketY + racketHeight - 1, colorRacket);
                }
            }
        }

        void leftButtonEvent(uint port, uint state, DateTime time)
        {
            lock (this)
            {
                if (repeatLeft == null)
                {
                    Repeater repeat = new Repeater(leftButtonFire, racketSpeed);
                    repeatLeft = new Thread(new ThreadStart(repeat.run));
                    repeatLeft.Start();
                    leftButtonFire(port, state, time);
                }
                else
                {
                    repeatLeft.Abort();
                    repeatLeft = null;
                }
            }
        }

        void leftButtonFire(uint port, uint state, DateTime time)
        {
            lock (this)
            {
                if (racketX > minX)
                {
                    LCD.drawRectangle(racketX, racketY, racketX + racketWidth - 1, racketY + racketHeight - 1, colorBackground);
                    racketX -= racketDelta;
                    LCD.drawRectangle(racketX, racketY, racketX + racketWidth - 1, racketY + racketHeight - 1, colorRacket);
                }
            }
        }

        void moveBall(uint port, uint state, DateTime time)
        {
            lock (this)
            {

                LCD.drawCircle(ball_x, ball_y, BALLSIZE, BLACK);
                ball_x = ball_x + xdir * xspeed;
                ball_y = ball_y + ydir * yspeed;
                collision();
                LCD.drawCircle(ball_x, ball_y, BALLSIZE, ball_colour);
            }
        }

        void checkAccelerometer(uint port, uint state, DateTime time)
        {
            lock (this)
            {
                adxl345.readValues(values);
                if ((changed(values[0], oldValues[0])) || (changed(values[1], oldValues[1])) || (changed(values[2], oldValues[2])))
                {
                    //Debug.Print("Values: " + values[0] + ", " + values[1] + ", " + values[2]+"\n");
                    accelerationX = values[0];
                    accelerationY = values[1];
                    LCD.drawRectangle(racketX, racketY, racketX + racketWidth - 1, racketY + racketHeight - 1, colorBackground);
                    calculateRectangles();
                    LCD.drawRectangle(racketX, racketY, racketX + racketWidth - 1, racketY + racketHeight - 1, colorRacket);
                }
                oldValues[0] = values[0];
                oldValues[1] = values[1];
                oldValues[2] = values[2];
            }
        }

        void collision()
        {
            lock (this)
            {
                if (ball_x <= lefthit)
                {
                    ball_x = lefthit;
                    ball_colour = LEFTCOLOUR;
                    xdir = -xdir;
                }

                if (ball_x >= righthit)
                {
                    ball_x = righthit;
                    ball_colour = RIGHTCOLOUR;
                    xdir = -xdir;
                }

                if (ball_y <= tophit)
                {
                    ball_y = tophit;
                    ball_colour = TOPCOLOUR;
                    ydir = -ydir;
                }

                if (ball_y >= bottomhit)
                {
                    if (ball_x >= racketX && ball_x <= racketX + racketWidth)
                    {
                        ball_y = bottomhit;
                        ball_colour = BOTTOMCOLOUR;
                        ydir = -ydir;
                    }
                    else
                    {
                        LCD.drawRectangle(10, 10, screenwidth-10, screenheight-10, (int)Color.RED);
                        Thread.Sleep(100);
                        LCD.drawRectangle(10, 10, screenwidth - 10, screenheight - 10, colorBackground);
                        ball_x = screenwidth / 3;
                        ball_y = tophit;
                        ydir = 1;
                        ball_colour = TOPCOLOUR;
                    }
                }
            }
        }


        bool changed(short val1, short val2)
        {
            return (val1 < (val2 - 5)) || (val1 > val2 + 5);
        }

        void drawRacket()
        {
            lock (this)
            {
                LCD.drawRectangle(0, screenheight - WALLWIDTH, screenwidth - 1, screenheight - 1, BOTTOMCOLOUR);    // Draw Bottom Wall
            }
        }

        void run()
        {
            xspeed = XSPEED;       // keep correct ball speed aspect
            yspeed = YSPEED;

            ball_colour = WHITE;                       // initial ball colour
            xdir = 1; ydir = 1;                       // initial ball direction
            ball_x = 20; ball_y = 20;                 // initial ball position

            LCD.drawRectangle(0, 0, screenwidth - 1, WALLWIDTH - 1, TOPCOLOUR);                         // Draw Top Wall
            LCD.drawRectangle(0, WALLWIDTH - 1, WALLWIDTH - 1, screenheight - racketHeight - 1, LEFTCOLOUR);         // Draw Left Wall
            LCD.drawRectangle(screenwidth - WALLWIDTH, WALLWIDTH - 1, screenwidth - 1, screenheight - racketHeight - 1, RIGHTCOLOUR);      // Draw Right Wall

            tophit = WALLWIDTH + BALLSIZE;
            bottomhit = screenheight - WALLWIDTH - BALLSIZE - 1;
            lefthit = WALLWIDTH + BALLSIZE;
            righthit = screenwidth - WALLWIDTH - BALLSIZE - 1;

            leftButton = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.An0, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);
            rightButton = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.An1, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);
            rightButton.OnInterrupt += new NativeEventHandler(rightButtonEvent);
            leftButton.OnInterrupt += new NativeEventHandler(leftButtonEvent);

            LCD.drawRectangle(racketX, racketY, racketX + racketWidth - 1, racketY + racketHeight - 1, colorRacket);

            Repeater repeat1 = new Repeater(moveBall, ballSpeed);
            new Thread(new ThreadStart(repeat1.run)).Start();

            Repeater repeat2 = new Repeater(checkAccelerometer, ballSpeed);
            new Thread(new ThreadStart(repeat2.run)).Start();

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        public static void Main()
        {
            new Program().run();
        }
    }
}
