1D-Pong-Netduino
================

1D pong over network using a netduino
================
To use:
Decide on one netduin to run the game (Master)
Configure the ips of the netduinos in this style (matches current code)
1. Master -> 192.168.1.200
2. Slave1 -> 192.168.1.201
3. Slave2 -> 192.168.1.202

===============

Each slave is a game controller, with a ws2812b (NeoPixel) connected to it, using pin 11 for Data into the led, and a common ground between the netduino and the power supply for the netduino.
Make sure each netduino is linked on a network running in the same subnet of each other.
Dont forget to set the MAC address of each netduino
Each LED strip is designed to be 50 leds long.

--------------
How to play:
1.With the master running each node should show the animation countdown waiting for a player to select the game.
2. While the countdown is running, press and hold the button to join the game, LEDs will go green to confirm
3. The game will initalise after a delay to wait for everyone to decide
4. The person serving will have the closed LED to the netduino white to represent the ball
5. Press the button to 'serve' the ball
6. The ball will go from the current position and leave down the leds and then appear on another players strip (or possibly your own if your unlucky)
7. Press the button to bounce the ball back when it is aproximately just in front of the green end leds.
8. The amount of blue leds that stay lit are representative of your 'health', currently you have 3 lives (3 misses) in the implimentation
9. Enjoy :) Whilever the master device is running, the game will loop back to step 1
 

Notes:
To have more than 3 games, adjust the master devices code to include the IPs and also increase the number of nodes (next line). Having ips that are not online are okay, so a 3 node setup can also run 1 or 2 nodes :).

There is a timeout to account for the occasional loss of a udp packet on the network, so if a node times out after 5 minutes it will select a new node and try again. (This can be adjusted as needed).

===============
This project was made possible by the Microsoft IoT Hackathon and University of Newcastle.

--Ben

Please note that this code and information is subject to change, and is correct to my current knowledge :)
