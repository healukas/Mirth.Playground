# Mirth.Playground

This repository contains some things that can be used to learn about mirth. 
Below, I am collecting, what I have learned.


## Tools
This repo contains the following tools: 
- MirthSocketClient: Writes messages to the socket
- MirthSocketServer: Receives messages on a socket
- Utility: Contains a bunch of function to process binary payloads etc
- Stack: A directory containing some exported channel definitions and a docker stack
  - The docker stack contains mirth, a database and a rest endpoint that simply logs all the requests it gets.
- V2 Test.xml contains a channel configuration for 1 channel with 3 destinations
  1. A FileWriter
  2. A TCPSender
  3. A HTTPSender



## Learnings
### MLLP V2
When using a TCP listener source with MLLP V2: set max retries to 0. Otherwise it destroys the SocketClient 

### ACK
If you send a message, Mirth will send you 2 ACK messages
  1. The 4 byte version (0x0B, 0x06, 0x1C, 0x0D)
  2. The HL7 V2 ACK message (I don't know how to switch that off)

You have to send an ACK in the 4 byte version to acknowledge the ACK.


