using System.Net.Sockets;

namespace Multi_Client_Chat_Server;

class ClientInfo
{
    public Socket Socket { get; set; }
    public string Username { get; set; }
    
}