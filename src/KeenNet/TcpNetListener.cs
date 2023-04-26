using System.Net;
using System.Net.Sockets;

namespace KeenNet;

public class TcpNetListener
{
    private readonly Socket _listener;
    public Action<Socket> OnAccept;
    
    public TcpNetListener(int port)
    {
        _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
        _listener.Listen();
        StartAsync();
    }

    public async Task StartAsync()
    {
        //TODO:可取消CancelationSourceToken
        while (true)
        {
            try
            {
                Socket client = await _listener.AcceptAsync();
                OnAccept?.Invoke(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
