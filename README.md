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
- HL7V2_File_Rest_TCP.xml contains a channel configuration for 1 channel with 3 destinations
  1. A FileWriter
  2. A TCPSender
  3. A HTTPSender



## Learnings
### Sending
#### ACK
If you send a message, Mirth will send you 2 ACK messages
  1. The 4 byte version (0x0B, 0x06, 0x1C, 0x0D)
  2. The HL7 V2 ACK message (I don't know how to switch that off)

Once you received that ACK, disconnect the socket and reconnect for the next message. Otherwise funny things happen to the messages you send. 
Alternatively you can ACK the ACK. I am not sure why, but it works without reconnecting.

### Receiving
Mirth expects an ACK for every message it sends you.
If MLLP v2 is enabled, it suffices to send the 4 byte ack. 
Otherwise you must construct an HL7 V2 ACK message.