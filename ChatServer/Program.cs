using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Multi_Client_Chat_Server;

class ChatServer
{
    static List<ClientInfo> clients = new List<ClientInfo>();
    static object _lock = new object();

    static void Main()
    {

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 8888);
        socket.Bind(ipEndPoint);
        socket.Listen(10);
        Console.WriteLine("Chat server was started on port 8888");

        while (true)
        {
            Socket client = socket.Accept();
            Console.WriteLine("Client connected: " + client.RemoteEndPoint + " Online now: " + clients.Count);
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    static void HandleClient(Socket clientSocket)
    {
        byte[] buffer = new byte[1024];
        ClientInfo currentClient = null;

        try
        {
            int bytesRead = clientSocket.Receive(buffer);
            string name = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            currentClient = new ClientInfo { Socket = clientSocket, Username = name };
            
            lock (_lock) { clients.Add(currentClient); }
            
            string joinMsg = $"[Server]: {name} connected to chat!";
            Console.WriteLine(joinMsg);
            Broadcast(joinMsg, null); 
            
            while (true)
            {
                bytesRead = clientSocket.Receive(buffer);
                if (bytesRead == 0) break;

                string text = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string formattedMsg = $"[{currentClient.Username}]: {text}";
                
                Console.WriteLine(formattedMsg);
                Broadcast(formattedMsg, clientSocket);
            }
        }
        catch { }
        finally
        {
            if (currentClient != null)
            {
                lock (_lock) clients.Remove(currentClient);
                Broadcast($"[Server]: {currentClient.Username} leaved chat.", null);
                clientSocket.Close();
                Console.WriteLine($"{currentClient.Username} disconected.");
            }
        }
    }

    static void Broadcast(string msg, Socket sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        lock (_lock) foreach (var client in clients.Where(client => client.Socket.Connected && client.Socket != sender))
            client.Socket.Send(data);
    }

}