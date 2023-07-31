using System.Net;
using System.Net.Sockets;
using Utility;

namespace MirthSocketClient;

static class Program
{
    private const int MsBetweenMessages = 1000;
    private const string HostName = "localhost";
    private const int Port = 16661;
    private const bool Verbose = true;
    private const int AmountOfMessages = 1;

    public static void Main()
    {
        var messages = GetMessages(AmountOfMessages);
        // setup
        Uri uri;
        try
        {
            uri = new Uri(HostName);
        }
        catch (Exception)
        {
            uri = new Uri("http://" + HostName);
        }
        
        var resolvedHost = Dns.GetHostEntry(uri.DnsSafeHost);
        var ipEndpoint = new IPEndPoint(resolvedHost.AddressList.First(), Port);
        using Socket client = new(resolvedHost.AddressList.First().AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        var i = 0;
        foreach (var message in messages)
        {
            client.ConnectAsync(ipEndpoint);

            // convert message to binary
            var binaryMessage = message.Pack(false, true);
            
            if(Verbose)
            {
                Console.WriteLine($"Message {i} bytes:\n{binaryMessage.HexDump()}");
            }
            
            // send
            _ = client.SendAsync(binaryMessage,SocketFlags.None).Result;
            Console.WriteLine($"Message {i} sent.");
            
            // wait for ack
            var responseBuffer = new byte[1024];
            var received = client.ReceiveAsync(responseBuffer, SocketFlags.None).Result;
            var rawResponse = new byte[received];
            Array.Copy(responseBuffer, 0, rawResponse, 0, received);
            if(Verbose)
            {
                Console.WriteLine($"Received response of length: {received}");
                Console.WriteLine($"Response was:\n{rawResponse.HexDump()}");
            }
            
            Console.WriteLine($"Message {i}: {rawResponse.CheckAck()}");
            
            // ack ack
            var ack = new byte[4] { 0x0b, 0x06, 0x1c, 0x0d };
            _ = client.SendAsync(ack, SocketFlags.None);
            
            Console.WriteLine($"Message {i}: Acknowledged ACK");
            
            client.Disconnect(true);
            i++;
            Thread.Sleep(MsBetweenMessages);
        }
        
    }

    private static List<string> GetMessages(int number)
    {
        List<string> result = new List<string>();
        for(var i = 0; i<number; i++)
        {
            var paddedNumber = i.ToString().PadLeft(6, '0');
            var message = $"MSH|^~\\&|x|aaa-{paddedNumber}|CERNER|HOSPITAL-A|201401291848||ADT^A01|1912340911|P|2.3|||AL|NE|\r\n" +
                          "EVN|A01|201401291848|||REJKB1\r\n" +
                          "PID||ABC123|987654|ALT789|PETTY^TOM^^^^||19781218|M||2106-3|10144 MAPLE AVE^^IRVINE^CA^92614||(949)123-1234|||||0053820452|220675537||AME||||1|||||\r\n" +
                          "PV1||I|S-2302-1^S-2302^A|C|||1111111^PINA|||SUR|||||A0||1111111^PINA|S||S|P||||||||||||||||||IAH|||||201401291848|\r\n" +
                          "PV2||D|42.41^Partial esophagectomy^I9|||||201401290900|201401310900|3|3|||||||||||||||||||||||||a|";

            result.Add(message);
        }

        return result;
    }
}