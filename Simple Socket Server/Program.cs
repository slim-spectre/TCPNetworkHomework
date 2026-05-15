using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class SimpleSocketServer
{
    static void Main()
    {
        
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 5000);
        
        socket.Bind(ipEndPoint);
        socket.Listen(10);
        
        Console.WriteLine("Server listening on port 5000...");

        bool isRunning = true;
        while (isRunning) 
        {
            Console.WriteLine("\nWaiting for new client...");
            
            Socket client = socket.Accept(); 
            Console.WriteLine($"Client connected: {client.RemoteEndPoint}");

            byte[] buffer = new byte[1024];
            int bytesReceived = client.Receive(buffer);
            
            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine($"Recieved: {receivedData}");
            
            byte[] response = Encoding.UTF8.GetBytes("Server: Message received! OK.");
            client.Send(response);
            
            if (receivedData.Trim().ToLower() == "exit")
            {
                isRunning = false;
                Console.WriteLine("The serve go sleep...");
            }
            
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        socket.Close();
        Console.WriteLine("The server was closed!");
    }
}