# Mirth.Playground


This repository contains some things that can be used to learn about mirth.
Below, I am collecting, what I have learned.

## Guide
### Channel Setup
#### Source
To create a new TCP Listener Source that works with the `MirthSocketClient`, follow these steps:
1. Go to the Channels Overview Page
2. Right-Click and Select `New Channel`
3. Under `Channel Properties` > `Name` enter a name of your choice (in our example `HL7V2_File_Rest_TCP`)
4. Optionally set options like validation etc with the button `Set Data Types`
5. Open the `Source` tab
6. Enter the following configurations from the table below
7. Define the Destinations (see later section)
8. Click `Save Changes` under `Channel Tasks` in the left side bar.
9. Click `Deploy Channel` under `Channel Tasks` in the left side bar.

| Parameter | Value                                                                                                                                                                | 
|---|----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Connector Type | TCP Listener                                                                                                                                                         | 
| Listener Settings |                                                                                                                                                                      | 
| Local Address | All interfaces (if no conflicts expected)                                                                                                                            | 
| Local Port | 6661  (if not in use, if changed here, needs change in stack file)                                                                                                   | 
| Source Settings |                                                                                                                                                                      |
| Source Queue | OFF                                                                                                                                                                  | 
| Response | Auto-generate (Before processing)                                                                                                                                    |
| Process Batch | No                                                                                                                                                                   |
| Max Processing Threads | 1 (I worked with 1, but whatever you like)                                                                                                                           |
| TCP Listener Settings |                                                                                                                                                                      |
| Transmission Mode | MLLP                                                                                                                                                                 |
| Blue wrench next to MLLP | Start of message Bytes: 0x0B <br> End of Message Bytes: 0x1C0D <br> Use MLLPv2: yes <br> Commit ACK Bytes: 0x06 <br> Commit NACK Bytes: 0x15 <br> Max Retry Count: 0 | 
| Mode | Server | 
| Max Connections | 10 | 
| Receive Timout (ms) | 0 | <!-- not sure here --> 
| Buffer Size (bytes) | 65536 | 
| Keep Connection Open | No | 
| Data Type | Text | 
| Encoding | Default | 
| Respond on New Connection | No | 

#### TCP Writer Destination
TBD

#### Channel Reader + Channel Writer + Filter
##### Channel Reader
Create channel `ADT A01`.
Set Source to `Channel Reader`.

##### Channel Writer
Create channel with TCP Listener
Set Destination as Channel Writer
Select Empty channel from the dropdown
Template per default at ${message.encodedData} leave it like that.
Add Filter for Destination 1:
Behavior: Accept
Field: ${'mirth_type'}
Condition: Contains
Values: `"ADT-A01"`
Save and deploy both channels -> ADT A01 messages now go to the ADT A01 channel



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