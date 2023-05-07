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
        _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        // TODO
        // _socket.Ttl = 32;//default 32. 0-256 is valid
        // _socket.ReceiveBufferSize = 8192;//default 8192. Bigger maybe faster.
        // _socket.SendBufferSize = 8192;//default 8192. Bigger maybe faster.
        
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
