using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine($"[Client]");
Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));

string msg = "This is a message from client";
var buffer = Encoding.UTF8.GetBytes(msg);
client.Send(buffer);

Thread.Sleep(1000);