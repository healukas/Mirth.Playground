# Lessons learned

- When choosing MLLPv2 mirth still sends the HL7 V2 ACK as well as the 4 byte response. 
- A channel always has only 1 source
- To gather messages from multiple channels one has to set a destination of type `ChannelWriter` in every channel the messages should be collected from.
- When receiving a message, one has to ack with both the old and new ack in the current configuration.