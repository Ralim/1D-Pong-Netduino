﻿#define SCORE
//#define DEBUG
/*
 * Created By Ben V. Brown & Matthew Muller with Help of Jarred Zeeman for the Microsoft Hackathon at University of Newcastle
 * This class maintains the connections to all nodes and the gameplay state machine
 */
#region usings
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Collections;
#endregion
namespace PingPongMasterControl
{
    public class Program
    {
        #region variables
        IPAddress[] Slaves = new IPAddress[] { new IPAddress(new byte[] { 192, 168, 1, 201 }), new IPAddress(new byte[] { 192, 168, 1, 202 }), new IPAddress(new byte[] { 192, 168, 1, 203 }) };
        //Above is the array of the ipaddress's we are going to be connected to
        const int ClientNum = 3;//The number of clients we have (used in loops)
        SlaveComms[] Clients = new SlaveComms[ClientNum];//stores our clients themselves
        bool[] ClientsActive = new bool[ClientNum];//the ones that are active in this game
        bool[] ClientsAlive = new bool[ClientNum];//the ones that are still alive

        int gameState = 0;//current game state machine
        int[] ClientHealth = new int[ClientNum];//Stores the health of each client in the gmae
        int LastClient = -1;//the last client we used a message from
        int LastPlayer = -1;//as above but player id No
        byte gamespeed = 3;//The speed of the game
        const byte gamespeedMin = 7;//the value that we cant cross to make it playable
        #endregion
        public static void Main()
        {
            // write your code here
            Program p = new Program(); p.run();
            //here we create a main running thread
        }
        /// <summary>
        /// Main method that runs it all
        /// </summary>
        public void run()
        {
            //create our clients on load
            for (int i = 0; i < ClientNum; ++i)
            {
                Clients[i] = new SlaveComms(Slaves[i], true, i);
                Clients[i].MessageRecieved += Program_MessageRecieved;
            }
            //initalised our clients
            long timer = 0; //timer that we use for timeouts
            int PC = 0;//Player count temp
            int AC = 0;//Active count temp
            Random R = new Random();//Random number generator
            int errorcount = 0;//error fail counter used in timeouts
            do
            {
                //If gamestate >=2 then the game is running and so this loop keeps us up to date on what peoples health and such is doing
                if (gameState >= 2)
                {
                    for (int i = 0; i < ClientNum; i++)
                    {
                        Clients[i].SendMessage(Const.GetPlayerHealth);//request the health of each player to get up to date info
                        if (ClientsActive[i])
                        {
                            if (ClientHealth[i] <= 0)
                                ClientsAlive[i] = false;
                            else
                                ClientsAlive[i] = true;
                        }
                        else
                        {
                            ClientsAlive[i] = false;
                        }
                    }
#if SCORE
                    //debug health outputs
                    Debug.Print("Player 1 : " + ClientHealth[0].ToString() + " Player 2 : " + ClientHealth[1].ToString() + " Player 3 " + ClientHealth[2].ToString());

#endif
                }
                int indx;
                switch (gameState)
                {
                    case 0:
                        //we are setting up the game
                        gamespeed = 3;
                        LastPlayer = -1;//init again
                        for (int i = 0; i < ClientNum; i++)
                        {
                            //resetting storage
                            ClientsActive[i] = false;
                            ClientsAlive[i] = false;
                            ClientHealth[i] = 15;
                        }
                        foreach (var c in Clients)
                        {
                            c.SendMessage(Const.Ping);//send out pings
                            System.Threading.Thread.Sleep(200);//wait for send
#if DEBUG
                            Debug.Print("Sent Ping");
#endif
                        }
                        timer = System.DateTime.Now.Ticks;
                        gameState++;
                        break;
                    case 1:
                        //now we are waiting for clients to answer back
                        if (System.DateTime.Now.Ticks - timer > (10000 * TimeSpan.TicksPerMillisecond))
                        {
                            gameState++;//we are done waiting for players
#if DEBUG
                            Debug.Print("Done Waiting");
#endif
                        }
                        break;
                    case 2:
                        indx = R.Next(ClientNum);
                        int cnt = 0;
                        gamespeed = 3;//reset speed
                        for (int i = 0; i < ClientNum; i++)
                        {
                            if (ClientsActive[i]) cnt++;
                        }
                        if (cnt == 0)
                        {
                            gameState = 0;
                        }

                        if (ClientsAlive[indx] == true && (ClientHealth[indx] > 0))
                        {

                            Clients[indx].SendMessage(Const.Serve, gamespeed);
#if DEBUG
                            Debug.Print("Serving : " + indx.ToString());
#endif
                            gameState++;
                        }
                        for (int i = 0; i < ClientNum; i++)
                        {
                            if (ClientsAlive[i] && i != indx)
                            {
                                Clients[i].SendMessage(Const.PlayerGetRead);
                            }
                        }
                        break;
                    case 3:
                        //here we are waiting for the player to have served
#if DEBUG
                        Debug.Print("Waiting for serve response");

#endif
                        Thread.Sleep(200);//waiting.......
                        errorcount++;
                        if (errorcount > 700)
                        {
                            gameState = 2; errorcount = 0;
                        }//go back :S we missed something
                        break;
                    case 4:
                        //The player has served, now we begin this round
                        indx = R.Next(ClientNum);
                        PC = 0;
                        AC = 0;

                        foreach (var c in ClientsAlive)
                        {
                            if (c) PC++;

                        }
                        foreach (var c in ClientsActive)
                        {
                            if (c) AC++;
                        }
                        if (ClientsAlive[indx] == true && ((LastPlayer != indx) || (PC == 1 && AC == 1)))
                        {
#if DEBUG
                            Debug.Print("Player Turn : " + indx.ToString());
#endif

                            Clients[indx].SendMessage(Const.PlayerTurn, (byte)gamespeed);//Tell the player its their turn
                            LastPlayer = indx;
                            gameState++;
                        }
                        break;
                    case 5:
                        //This means that we are waiting to hear if the player succseeded or not
#if DEBUG
                        Debug.Print("Waiting for player response");
#endif
                        Thread.Sleep(200);//waiting.......
                        errorcount++;
                        if (errorcount > 700)
                        {
                            gameState = 4; errorcount = 0;
                        }//go back :S we missed something
                        break;
                    case 6:
                        //This means that the player has lost
                        //Last Client will contain the loser!!
#if DEBUG
                        Debug.Print("Player Lost round : " + LastClient.ToString());

#endif
                        for (int i = 0; i < ClientNum; i++)
                        {
                            if (i != LastClient && ClientsAlive[i])
                            {
                                Clients[i].SendMessage(Const.OtherPlayerFailed);//Let them know that someone else failed so so they should celebrate (good times)
                            }

                        }
                        System.Threading.Thread.Sleep(500);//give em time to celebrate
                        PC = 0;
                        AC = 0;
                        foreach (var c in ClientsAlive)
                        {
                            if (c) PC++;
                        }
                        foreach (var c in ClientsActive)
                        {
                            if (c) AC++;
                        }

                        if ((AC == 1 && PC == 1) || (AC > 1 && PC > 1))//TODO
                        {
                            gameState = 2;//restart to a serve
                        }
                        else
                        {
                            gameState = 7;
                        }
                        Thread.Sleep(800);
                        break;
                    case 7:
                        //This is when the game has ended (ie, one player left)
#if DEBUG
                        Debug.Print("End Of Game");

#endif
                        for (int i = 0; i < ClientNum; i++)
                        {
                            if (ClientsActive[i] == true)
                            {
                                //client is in the game
                                if (ClientHealth[i] > 0)
                                {
                                    //You WIn
                                    Clients[i].SendMessage(Const.PlayerWin);
                                }
                                else
                                {
                                    //You Lose
                                    Clients[i].SendMessage(Const.PlayerLost);
                                }
                            }
                        }
                        System.Threading.Thread.Sleep(10000);
                        gameState = 0;//go back to the start (again)
                        break;
                    default:
                        break;
                }
            } while (true);//we run this code forever

        }

        void Program_MessageRecieved(byte Msg, byte MessageData, int index)
        {
            //a message has been recived
            if (Msg == Const.PlayerHeathResponse)
            {
                ClientHealth[index] = MessageData;
#if DEBUGH
                Debug.Print("Got Health of Player " + index.ToString() + " Of " + MessageData.ToString());
#endif
                LastClient = index;
                if (MessageData == 0)
                {
                    //this player is dead oh he is so dead

                }
            }
            if (gameState == 1)
            {
                if (Msg == Const.Pong && MessageData == Const.NODATA)
                {
#if DEBUG
                    Debug.Print("Got Pong of Player " + index.ToString());
#endif
                    ClientsActive[index] = true;
                }
            }
            if (gameState == 3)
            {
                if (Msg == Const.PlayerSuccess)
                {
                    //the player has served, move on
#if DEBUG
                    Debug.Print("Player " + index.ToString() + " Has served");
#endif
                    gameState++;
                }
            }
            if (gameState == 5 || gameState == 4)
            {
                //here we are waiting for a player move to complete
                if (Msg == Const.PlayerFail)
                {
                    //The player has failed 
#if DEBUG
                    Debug.Print("Player " + index.ToString() + " Failed " + MessageData.ToString());
#endif
                    LastClient = index;
                    ClientHealth[index] = MessageData;
                    gameState = 6;//6 = notify others
                }
                else if (Msg == Const.PlayerSuccess)
                {
                    //that player was fine, so lets go back a step and go again

                    ClientHealth[index] = MessageData;
                    if (ClientHealth[index] == 0)
                    {
                        Clients[index].SendMessage(Const.PlayerLost);
                        ClientHealth[index] = -1;//out of the game
                        ClientsActive[index] = false;//deactivate
                    }
                    gamespeed++;
                    if (gamespeed > gamespeedMin)
                    {
                        gamespeed = gamespeedMin;//so we dont go higher
                    }

                    gameState = 4;
#if DEBUG
                    Debug.Print("Player " + index.ToString() + " Sucsess " + MessageData.ToString() + " Speed : " + gamespeed.ToString());
#endif
                    LastClient = index;

                }
            }
            if (gameState == 6)
            {

            }
        }
    }
}
