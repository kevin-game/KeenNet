using System.Net.Sockets;

namespace KeenNet;

public class KeenNetServer
{
    public readonly TcpNetListener _Listener;
    public readonly List<TcpNetClient> _clients = new();
    
    public KeenNetServer(int port)
    {
        _Listener = new TcpNetListener(port);
        _Listener.OnAccept += OnAccept;
    }

    private void OnAccept(Socket socket)
    {
        TcpNetClient client = new(socket);
        _clients.Add(client);
    }
}