using System.Net;
using System.Net.Sockets;
using Utility;

namespace MirthSocketServer;

public class Program
{
    private const string HostIp = "172.17.0.1";
    private const int Port = 3001;
    public static void Main(string[] args)
    {
        var ipAddr = IPAddress.Parse(HostIp);
        var ipEndpoint = new IPEndPoint(ipAddr, 3001);

        using Socket server = new(ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        server.Bind(ipEndpoint);
        server.Listen(Port);

        while (true)
        {
            var handler = server.Accept();

            Console.WriteLine("New message:---------------------");
            var buffer = new byte[2048];
            var received = handler.Receive(buffer, SocketFlags.None);
            Console.WriteLine($"Received: {received}");
            
                var receivedPayload = new byte[received];
                Array.Copy(buffer, 0, receivedPayload, 0, received);
                Console.WriteLine(receivedPayload.HexDump());
                var response = new byte[] { 0x0B, 0x06, 0x1C, 0x0D };
                handler.Send(response, SocketFlags.None);
                Console.WriteLine("Ack sent");
        }
    }

    public static string buildAck()
    {
        return
        "MSH|^~\\&|CERNER|HOSPITAL-?A|x|000000|20230719141859.868|?|ACK|20230719141859.868|P|2.3\\r"+
        "MSA|AA|1912340911";
    }
}