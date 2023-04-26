namespace KeenNet;

public class KeenNetClient
{
    TcpNetClient _client;

    public KeenNetClient(string ip, int port)
    {
        _client = new TcpNetClient(ip, port);
    }

    public Task SendAsync(string msg)
    {
        return _client.SendAsync(msg);
    }
}