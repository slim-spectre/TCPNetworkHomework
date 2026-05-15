using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

class ChatClient
{
    static void Main()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(new IPEndPoint(IPAddress.Loopback, 8888));

        Console.WriteLine("Enter your name: ");
        var name = Console.ReadLine() ?? "Anonymous guest";
        
        socket.Send(Encoding.UTF8.GetBytes(name));
        
        
        Console.WriteLine("Connected to the chat");
        
        Thread receiveThread = new Thread(() => ReceiveMessage(socket));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        while (true)
        {
            string msg = Console.ReadLine();
            if (msg?.ToLower() == "exit") break;
            socket.Send(Encoding.UTF8.GetBytes(msg));
        }
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        
    }

    static void ReceiveMessage(Socket server)
    {
        byte[] buffer = new byte[1024];
        try
        {
            while (true)
            {
                var bytesRead = server.Receive(buffer);
                if (bytesRead == 0) break;
                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                
                
            }       
        }
        catch (SocketException e)
        {
            Console.WriteLine("The connection with server ended sorry maan");
        }
    }
}