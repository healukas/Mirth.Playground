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

Once you received that ACK, You have to ACK the ACK. Otherwise mirth will throw an exception: 
```java 
com.mirth.connect.connectors.tcp.TcpReceiver: Error sending response (TCP Listener "Source" on channel bf07f813-12ff-436b-9b22-61b767d336a2).
 java.io.IOException: Remote socket has closed.
 	at com.mirth.connect.connectors.tcp.TcpReceiver.sendResponse(TcpReceiver.java:937) ~[tcp-server.jar:?]
 	at com.mirth.connect.connectors.tcp.TcpReceiver.access$2300(TcpReceiver.java:78) ~[tcp-server.jar:?]
 	at com.mirth.connect.connectors.tcp.TcpReceiver$TcpReader.call(TcpReceiver.java:696) ~[tcp-server.jar:?]
 	at com.mirth.connect.connectors.tcp.TcpReceiver$TcpReader.call(TcpReceiver.java:511) ~[tcp-server.jar:?]
 	at java.util.concurrent.FutureTask.run(Unknown Source) ~[?:?]
 	at java.util.concurrent.ThreadPoolExecutor.runWorker(Unknown Source) ~[?:?]
 	at java.util.concurrent.ThreadPoolExecutor$Worker.run(Unknown Source) ~[?:?]
 	at java.lang.Thread.run(Unknown Source) ~[?:?]
```

### Receiving
Mirth expects an ACK for every message it sends you.
If MLLP v2 is enabled, it suffices to send the 4 byte ack. 
Otherwise you must construct an HL7 V2 ACK message.