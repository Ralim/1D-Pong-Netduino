using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace PingPongMasterControl
{
    class Const
    {
        //[The unit that sends the message] Description
        public const byte Ping = 1;//[svr]enquire about players
        public const byte Pong = 2;//[cli]im alive
        public const byte StartGame = 3;//[svr] Clear board for play
        public const byte Serve = 4;//[svr] Tell player to serve ball
        public const byte PlayerSuccess = 5;//[svr] Player has succseeded
        public const byte PlayerFail = 6;//[cli] I just lost, so get my health soon maybe? Also let others know
        public const byte OtherPlayerFailed = 7;//[svr] Other player just lost so show happy stuff
        public const byte GetPlayerHealth = 8;//[svr] Request a health update
        public const byte PlayerHeathResponse = 9;//[cli] Attached is clients current health
        public const byte PlayerLost = 10;//[svr] Show lose animation
        public const byte PlayerWin = 11;//[svr] Show win animation
        public const byte PlayerTurn = 12;//[svr] Player start your turn
        public const byte NODATA = 0xff;//No Data Attached
        public const byte PlayerGetRead = 13;//[svr] get ready other dudes serving
    }
    class SlaveComms
    {
        IPAddress slave;
        Socket Listener;
        public delegate void MessageRecievedHandler(byte Msg, byte MessageData,int index);
        public event MessageRecievedHandler MessageRecieved;
        private int index;

        Thread listenThread;
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="slaveip"></param>
        /// <param name="Slave"></param>
        public SlaveComms(IPAddress slaveip, Boolean Slave,int idx)
        {
            slave = slaveip;//save our ip
            index = idx;
            System.Threading.Thread.Sleep(1000);
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var localip = IPAddress.Parse(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);
            Listener.Bind(new IPEndPoint(localip, Slave == true ? 7777 : 4242));//set out listening ip/port
            Listener.Connect(new IPEndPoint(slave, Slave == false ? 7777 : 4242));//set other ends ip / port

            listenThread = new Thread(new ThreadStart(MessageListenerThread));//start the listening thread
            listenThread.Start();
        }
        public void SendMessage(byte messageID = 0xFF, byte Data = 0xFF)
        {
            Listener.Send(new byte[] { 0x42, messageID, Data, 0x42 });
        }
        private void MessageListenerThread()
        {
            do
            {
                if (Listener.Poll(10000, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[10];

                    int read = Listener.Receive(buffer);//,ref ie);
                    if (read == 4)
                    {
                        if (MessageRecieved != null)
                        {
                            MessageRecieved(buffer[1], buffer[2],index);
                        }
                    }
                    else Debug.Print(read.ToString());//oopsies
                }
            } while (true);
        }

    }
}
