using Glovebox.MicroFramework.Sensors;
using Glovebox.Netduino;
using Glovebox.Netduino.Sensors;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

using Microsoft.SPOT.Hardware;
using System;
using PingPongMasterControl;
namespace MakerDen
{
    public class Program
    {
        // NeoPixelFrameBase frame = new NeoPixelFrameBase(50, "nothing");
        // write your code here
        

        public static void Main()
        {
            // main code marker
            Program p = new Program();
            p.Run();
        }
        NeoGame game = new NeoGame();
        SlaveComms comms = new SlaveComms(new System.Net.IPAddress(new byte[] { 192, 168, 1, 200 }), false,1);

        public void Run()
        {
            comms.MessageRecieved += comms_MessageRecieved;
            do
            {
                System.Threading.Thread.Sleep(500); 
            } while (true);
        }

        void comms_MessageRecieved(byte Msg, byte MessageData, int index)
        {
            switch (Msg)
            {
                case Const.GetPlayerHealth:
                    comms.SendMessage(Const.PlayerHeathResponse, game.GetHealth());//send back health data
                    break;
                case Const.Ping:
                    
                    if (game.getPing())
                    {
                        comms.SendMessage(Const.Pong);//send back we want in!
                    }
                    break;
                case Const.Serve:
                    //we are serving the ball
                    game.setSpeed((int)MessageData);
                    game.DoServe();
                    comms.SendMessage(Const.PlayerSuccess);//send back that we have sent the ball on its way
                    break;
                case Const.PlayerTurn:
                    game.setSpeed((int)MessageData);
                    if (game.DoPlayerTurn())
                    {
                        //we went okay
                        comms.SendMessage(Const.PlayerSuccess, game.GetHealth());
                    }
                    else
                    {
                        //we failed (oops)
                        comms.SendMessage(Const.PlayerFail,game.GetHealth());//We failed so others know
                    }
                    break;
                case Const.OtherPlayerFailed:
                    game.ShowOthersLost();

                    break;
                case Const.PlayerGetReady:
                    game.GetBoardReady();
                    break;
                case Const.PlayerLost:
                    game.WeLost();
                    break;
                case Const.PlayerWin:
                    game.WeWon();
                    break;
                default:
                    break;
            }
        }

    }
}

