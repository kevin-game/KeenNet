using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine($"[Server]");
Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));
listener.Listen();

Socket client = listener.Accept();

byte[] buffer = new byte[1024];
int length = client.Receive(buffer);

string msg = Encoding.UTF8.GetString(buffer, 0, length);
Console.WriteLine($"{msg}");

Thread.Sleep(1000);
