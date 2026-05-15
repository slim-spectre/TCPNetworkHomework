using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

class SimpleSocketClient
{
    static void Main()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);
        try
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            socket.Connect(ipEndPoint);
            if (socket.Connected)
            {
                Console.WriteLine("Connected");
                Console.WriteLine("Local port:" + socket.LocalEndPoint);
                Console.WriteLine("Remote port:" + socket.RemoteEndPoint);

                Console.WriteLine("Enter a message for server: ");
                string message = Console.ReadLine() ?? "Empty message";

                socket.Send(Encoding.UTF8.GetBytes(message));

                byte[] buffer = new byte[1024];
                int bytesSent = socket.Receive(buffer);
                timer.Stop();
                Console.WriteLine($"Server sent: {Encoding.UTF8.GetString(buffer, 0, bytesSent)}");
                Console.WriteLine("RTT: " + timer.ElapsedMilliseconds);
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Console.WriteLine("The connection wasss closed");
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e}");
        }

        
    }
}