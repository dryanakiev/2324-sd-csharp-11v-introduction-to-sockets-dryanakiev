using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatRoom.Client;

public class Client
{
    public string Name { get; set; } = null!;

    public IPAddress ServerIpAddress { get; set; }

    public int Port { get; set; }

    private Socket _clientSocket;

    public void StartConnection()
    {
        try
        {
            IPHostEntry hosts = Dns.GetHostEntry(Dns.GetHostName());
            
            ServerIpAddress = hosts.AddressList[0];
            
            IPEndPoint localEndPoint = new IPEndPoint(ServerIpAddress, Port);

            _clientSocket = new Socket(ServerIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _clientSocket.Connect(localEndPoint);

            if (_clientSocket.Connected)
            {
                Console.WriteLine("Socket connected to " + _clientSocket.RemoteEndPoint.ToString());
            }
            else
            {
                Console.WriteLine("Connection failed, retrying...");
                StartConnection();
            }
        }
        catch (Exception e)
        {
            // TODO: Add exception handle
        }
    }
    
    public void StopConnection()
    {
        _clientSocket.Close();
    }

    public string SendMessage(string message)
    {
        byte[] byteMessage = Encoding.ASCII.GetBytes(message);
        _clientSocket.Send(byteMessage);

        return ReceiveMessage();
    }

    public string ReceiveMessage()
    {
        byte[] messageBuffer = new byte[1024];

        int byteCount = _clientSocket.Receive(messageBuffer);

        return Encoding.ASCII.GetString(messageBuffer, 0, byteCount);
    }

    public static void Main(string[] args)
    {
        Client client = new Client()
        {
            Name = "Something",
            Port = 6666,
        };
        
        client.StartConnection();

        string message;

        while (true)
        {
            message = Console.ReadLine();

            Console.WriteLine(client.SendMessage(message));
        }
    }
}