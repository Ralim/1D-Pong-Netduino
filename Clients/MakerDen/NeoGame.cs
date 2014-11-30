using System;
using Microsoft.SPOT;
using NetMFNeoPixelSPI;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace MakerDen
{
    class NeoGame
    {
        NeoPixelSPI neoPixel = new NeoPixelSPI(Pins.GPIO_PIN_D10, SPI.SPI_module.SPI1);
        Pixel[] Pixels = new Pixel[50];//holder

        const ushort brightness = 100;
        Pixel pixelRed = new Pixel(40, 0, 0);
        Pixel pixelGreen = new Pixel(0, 40, 0);
        Pixel pixelBlue = new Pixel(0, 0, 40);
        Pixel pixelWhite1 = new Pixel(brightness, brightness, brightness);
        Pixel pixelWhite2 = new Pixel((int)(brightness * 0.75), (int)(brightness * 0.75), (int)(brightness * 0.75));
        Pixel pixelWhite3 = new Pixel((int)(brightness * 0.50), (int)(brightness * 0.50), (int)(brightness * 0.50));
        Pixel pixelWhite4 = new Pixel((int)(brightness * 0.25), (int)(brightness * 0.25), (int)(brightness * 0.25));
        Pixel pixeloff = new Pixel(0, 0, 0);
        int[] intQ = new int[50];
        object thisLock = new object();
        bool colourDirection = true;
        bool colourType = true;
        int health = 15; //1 health == 3 lights
        int ballSpeed = 3;
        bool swungEarly = false;

        InputPort button;
        const int player = 1;
        double currentSpeed = 50.0;
        double speed = 0.8;
        bool ballReturnable = false;
        /*bool ballEnd = false;
        bool ballIsReturning = false;
        bool returnHold = false;
        bool ballHasntStartedReturning = true;*/

        public NeoGame()
        {
            //initaliser - this stuff needs to be moved mostly
            //int waittime = 1000;
            for (int i = 0; i < 50; i++)
            {
                intQ[i] = -1;
                Pixels[i] = pixeloff;
            }
            neoPixel.ShowPixels(Pixels);
            neoPixel.ShowPixels(Pixels);
            button = new InputPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled);//init button
       /*     Thread newThread = new Thread(new ThreadStart(ballMove));
            newThread.Priority = ThreadPriority.Highest;
            newThread.Start();
            while (true)
            {
                if ((ballReturnable && ballEnd) && button.Read()) //Ball is returnable and button is held
                {
                    ballIsReturning = true;
                }
                else if (ballEnd && !returnHold) //Ball has reached the end and player has not held button
                {
                    //Lose flat % of total health and return or Lose health over time
                    //Flat % involves resetting booleans to false then calling a function to send a message to master Arduino
                }
                else if (!(ballReturnable || ballEnd) && button.Read()) //Button is held when ball is not returnable
                {
                    //100ms wait or so
                    //Start losing points at a (increasing?) rate
                }
                if (ballIsReturning && ballHasntStartedReturning)
                {
                    Thread retThread = new Thread(new ThreadStart(ballReturning));
                    retThread.Start();
                    ballHasntStartedReturning = false;
                }
                //System.Threading.Thread.Sleep(10);
            }*/
        }

        public byte GetHealth()
        {
            return (byte)health;
        }
        public void WeWon()
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    if (x % 2 == 0)
                        Pixels[x] = pixelGreen;
                    else
                        Pixels[x] = pixeloff;
                }
                neoPixel.ShowPixels(Pixels);
                for (int x = 0; x < 50; x++)
                {
                    if (x % 2 == 0)
                        Pixels[x] = pixeloff;
                    else
                        Pixels[x] = pixelGreen;
                }
                neoPixel.ShowPixels(Pixels);
            }
            for (int x = 0; x <= 49; x++)
                Pixels[x] = pixeloff;
            neoPixel.ShowPixels(Pixels);
            //we won the game, do a dance or something
        }
        public void WeLost()
        {
            //show red lights or something as we lost the game (hint hint)
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    if (x % 2 == 0)
                        Pixels[x] = pixelRed;
                    else
                        Pixels[x] = pixeloff;
                }
                neoPixel.ShowPixels(Pixels);
                for (int x = 0; x < 50; x++)
                {
                    if (x % 2 == 0)
                        Pixels[x] = pixeloff;
                    else
                        Pixels[x] = pixelRed;
                }
                neoPixel.ShowPixels(Pixels);
            }
            for (int x = 0; x <= 49; x++)
                Pixels[x] = pixeloff;
            neoPixel.ShowPixels(Pixels);
        }
        public void GetBoardReady()
        {
            fullClear();
            neoPixel.ShowPixels(Pixels);
        }
        public void ShowOthersLost()
        {
            //here we flash green or something to show that someone else lost
            for (int x = 0; x < 50; x++)
            {
                Pixels[x] = pixelGreen;
            }
            neoPixel.ShowPixels(Pixels);
            System.Threading.Thread.Sleep(500);
            fullClear();
            neoPixel.ShowPixels(Pixels);
        }
        public bool DoPlayerTurn()
        {
            fullClear();
            for (int x = 49; x >= 0; x -= ballSpeed)
            {
                fullClear();
                //Light up corresponding LED
                //  Pixels[x] = pixelWhite1;

                //Run x - 1 dim thread while x != 0


                if (x == 49)
                {
                    Pixels[x] = pixelWhite1;
                }
                else if (x == 48)
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x + 1] = pixelWhite2;
                }
                else if (x == 47)
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x + 1] = pixelWhite2;
                    Pixels[x + 2] = pixelWhite3;
                }
                else
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x + 1] = pixelWhite2;
                    Pixels[x + 2] = pixelWhite3;
                    Pixels[x + 3] = pixelWhite4;
                }
                if (x > 10 && button.Read())
                    swungEarly = true;
                if (x <= 10 && button.Read())
                    ballReturnable = true;
                //System.Threading.Thread.Sleep((int)(currentSpeed));
                neoPixel.ShowPixels(Pixels);
            }
            fullClear();
            neoPixel.ShowPixels(Pixels);
            //ballEnd = true;
            if (ballReturnable && !swungEarly)
                ballReturning();
            else
            {
                ballReturnable = false;
                swungEarly = false;
                return missedBall();
            }
            ballReturnable = false;
            swungEarly = false;
            return true;
        }

        public Boolean missedBall()
        {
            for (int x = 0; x <= 49; x++)
            {
                Pixels[x] = pixelRed;
            }
            neoPixel.ShowPixels(Pixels);
            System.Threading.Thread.Sleep(500);
            health -= 5;
            fullClear();
            neoPixel.ShowPixels(Pixels);
            return false;
        }

        public void DoServe()
        {
            fullClear();
            Pixels[0] = pixelWhite1;
            neoPixel.ShowPixels(Pixels);
            while (!button.Read()) { }
            for (int x = 0; x <= 49; x += ballSpeed)
            {
                fullClear();
                //Light up corresponding LED
                //  Pixels[x] = pixelWhite1;

                //Run x - 1 dim thread while x != 0


                if (x == 0)
                {
                    Pixels[x] = pixelWhite1;
                }
                else if (x == 1)
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x - 1] = pixelWhite2;
                }
                else if (x == 2)
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x - 1] = pixelWhite2;
                    Pixels[x - 2] = pixelWhite3;
                }
                else
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x - 1] = pixelWhite2;
                    Pixels[x - 2] = pixelWhite3;
                    Pixels[x - 3] = pixelWhite4;
                }
                neoPixel.ShowPixels(Pixels);
            }
            fullClear();
            neoPixel.ShowPixels(Pixels);
            //here we do the player server, and hang(wait) until they do
        }
        public bool getPing()
        {
            ballSpeed = 3;
            health = 15;
            fullClear();
            bool optIn = false;
            for (int x = 15; x >= 1; x--)
            {
                for (int y = (x * 3) + 4; y > (x * 3) + 1; y--)
                {
                    Pixels[y] = pixeloff;
                    if (button.Read())
                    {
                        y = -1;
                        x = -1;
                        optIn = true;
                    }
                }
                if (!optIn)
                {
                    System.Threading.Thread.Sleep(500);
                    neoPixel.ShowPixels(Pixels);
                }
            }
            if (optIn)
            {
                for (int x = 0; x <= 49; x++)
                {
                    Pixels[x] = pixelGreen;
                }
                neoPixel.ShowPixels(Pixels);
                return true;    //return true if player pushes the button to join the game
            }
            else
            {
                for (int x = 0; x <= 49; x++)
                {
                    Pixels[x] = pixeloff;
                }
                neoPixel.ShowPixels(Pixels);
                return false;
            }
        }
        public void setSpeed(int speedIn)
        {
            ballSpeed = speedIn;
        }

        public void fullClear()
        {
            for (int x = 0; x <= 4; x++)
            {
                Pixels[x] = pixelGreen;
            }
            for (int x = 5; x <= 49; x++)
            {
                if (x <= (health * 3) + 4)
                    Pixels[x] = pixelBlue;
                else
                    Pixels[x] = pixeloff;
            }
            if (colourDirection)
            {
                if (colourType)
                {
                    if (pixelBlue.Red < 40)
                        pixelBlue.Red++;
                    else
                        colourDirection = false;
                }
                else
                {
                    if (pixelBlue.Green < 40)
                        pixelBlue.Green++;
                    else
                        colourDirection = false;
                }
            }
            else
            {
                if (colourType)
                {
                    if (pixelBlue.Red > 0)
                        pixelBlue.Red--;
                    else
                    {
                        colourDirection = true;
                        colourType = false;
                    }
                }
                else
                {
                    if (pixelBlue.Green > 0)
                        pixelBlue.Green--;
                    else
                    {
                        colourDirection = true;
                        colourType = true;
                    }
                }
            }
        }

        //This needs to be a thread
        /*public void ballMove()
        {
            
            bool running = true;
            fullClear();
            while (running)
            {
                for (int x = 49; x >= 0; x -= ballSpeed)
                {
                    fullClear();
                    //Light up corresponding LED
                    //  Pixels[x] = pixelWhite1;

                    //Run x - 1 dim thread while x != 0


                    if (x == 49)
                    {
                        Pixels[x] = pixelWhite1;
                    }
                    else if (x == 48)
                    {
                        Pixels[x] = pixelWhite1;
                        Pixels[x + 1] = pixelWhite2;
                    }
                    else if (x == 47)
                    {
                        Pixels[x] = pixelWhite1;
                        Pixels[x + 1] = pixelWhite2;
                        Pixels[x + 2] = pixelWhite3;
                    }
                    else
                    {
                        Pixels[x] = pixelWhite1;
                        Pixels[x + 1] = pixelWhite2;
                        Pixels[x + 2] = pixelWhite3;
                        Pixels[x + 3] = pixelWhite4;
                    }

                    if (x >= 45)
                        ballReturnable = true;
                    //System.Threading.Thread.Sleep((int)(currentSpeed));
                    neoPixel.ShowPixels(Pixels);
                }
                if (button.Read())
                {
                    running = false;
                }
            }
            fullClear();
            neoPixel.ShowPixels(Pixels);
            //ballEnd = true;

        }*/

        public void ballReturning()
        {

            fullClear();
            for (int x = 0; x <= 49; x += ballSpeed)
            {
                fullClear();

                if (x == 0)
                {
                    Pixels[x] = pixelWhite1;
                }
                else if (x == 1)
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x - 1] = pixelWhite2;
                }
                else if (x == 2)
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x - 1] = pixelWhite2;
                    Pixels[x - 2] = pixelWhite3;
                }
                else
                {
                    Pixels[x] = pixelWhite1;
                    Pixels[x - 1] = pixelWhite2;
                    Pixels[x - 2] = pixelWhite3;
                    Pixels[x - 3] = pixelWhite4;
                }

                //System.Threading.Thread.Sleep((int)(currentSpeed));

                neoPixel.ShowPixels(Pixels);
            }


            fullClear();
            neoPixel.ShowPixels(Pixels);
            //Send message to master Netduino
        }

        public void messageBack(int messageID, object data)
        {

        }
    }
}
