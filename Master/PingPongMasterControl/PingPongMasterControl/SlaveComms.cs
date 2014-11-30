/*
 * Created By Ben V. Brown & Matthew Muller with Help of Jarred Zeeman for the Microsoft Hackathon at University of Newcastle
 * This class maintains the communication layer (udp in this case ) of sending messages along with a constant table for the numbers we use to represent states
 */
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
        IPAddress TargetDeviceIP;//The endpoint we are talking to
        Socket Listener;
        public delegate void MessageRecievedHandler(byte Msg, byte MessageData, int index);
        public event MessageRecievedHandler MessageRecieved;
        private int index;//Used when addressing this device as part of it contacting a slave unit for easy tracking units

        Thread listenThread;//This thread monitors the port for data and fires an event when a message is incoming
        /// <summary>
        /// Init the class, setting up the Connection and allowing incoming connections
        /// </summary>
        /// <param name="Targetip">The target unit ip i want to connect to</param>
        /// <param name="AmiConnectingToASlave">Set this to true to flip the port numbers around</param>
        /// <param name="idx">Index of the node, can be used to tell them apart if all share an event</param>
        public SlaveComms(IPAddress Targetip, Boolean AmiConnectingToASlave, int idx)
        {
            TargetDeviceIP = Targetip;//save our ip
            index = idx;//save our index
            System.Threading.Thread.Sleep(1000);//pause for networking to come up
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//create the socket
            var localip = IPAddress.Parse(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);//get our ip
            Listener.Bind(new IPEndPoint(localip, AmiConnectingToASlave == true ? 7777 : 4242));//set out listening ip/port
            Listener.Connect(new IPEndPoint(TargetDeviceIP, AmiConnectingToASlave == false ? 7777 : 4242));//set other ends ip / port

            listenThread = new Thread(new ThreadStart(MessageListenerThread));//start the listening thread
            listenThread.Start();
        }
        /// <summary>
        /// Send out a message id and data (with defaults)
        /// </summary>
        /// <param name="messageID">The (byte) message to send out</param>
        /// <param name="Data">The (byte) data to send along with it</param>
        public void SendMessage(byte messageID = 0xFF, byte Data = 0xFF)
        {
            Listener.Send(new byte[] { 0x42, messageID, Data, 0x42 });//we use 0x42 as a marker, does nothing in software but makes packet capture easy
        }
        /// <summary>
        /// This method forms the base of the thread that checks the socket and allows event based messages
        /// </summary>
        private void MessageListenerThread()
        {
            do
            {
                if (Listener.Poll(10000, SelectMode.SelectRead))//if there is data continue or else timeout at 10000
                {
                    byte[] buffer = new byte[10];//a small buffer for recieving the data

                    int read = Listener.Receive(buffer);//read in the data 
                    if (read == 4)//if we read 4 bytes from the port
                    {
                        if (MessageRecieved != null)//if someone has subscribed to the event
                        {
                            MessageRecieved(buffer[1], buffer[2], index);//fire off the event
                        }
                    }
                    else Debug.Print(read.ToString());//oopsies
                }
            } while (true);//we run until the unit is powered down or we are killed
        }

    }
}
