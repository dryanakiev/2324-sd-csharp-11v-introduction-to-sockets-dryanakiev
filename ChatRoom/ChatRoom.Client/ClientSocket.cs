using System.Net;
using System.Text;
using System.Net.Sockets;

namespace ChatRoom.Client;

public class ClientSocket
{
    public Client Client { get; set; }
    public Socket SocketClient { get; set; }

    public ClientSocket()
    {
        
    }

    public ClientSocket(Client client)
    {
        Client = client;
    }
    
    public void StartConnection()
    {
        try
        {
            IPHostEntry hosts = Dns.GetHostEntry(Dns.GetHostName());
            
            Client.ServerIpAddress = hosts.AddressList[0];
            
            IPEndPoint localEndPoint = new IPEndPoint(Client.ServerIpAddress, Client.Port);
            
            // TODO: Fix local endpoint conectivity

            SocketClient = new Socket(Client.ServerIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            SocketClient.Connect(localEndPoint);

            if (SocketClient.Connected)
            {
                Console.WriteLine("Socket connected to " + SocketClient.RemoteEndPoint);
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
            /*Unhandled exception. System.Net.Sockets.SocketException (10057): A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using a sendto call) no address was supplied.*/
            // labels: bug
        }
    }
    
    public void StopConnection()
    {
        SocketClient.Close();
    }

    public string SendMessage(string message)
    {
        byte[] byteMessage = Encoding.ASCII.GetBytes(message);
        SocketClient.Send(byteMessage);

        return ReceiveMessage();
    }

    public string ReceiveMessage()
    {
        byte[] messageBuffer = new byte[1024];

        int byteCount = SocketClient.Receive(messageBuffer);

        return Encoding.ASCII.GetString(messageBuffer, 0, byteCount);
    }
}