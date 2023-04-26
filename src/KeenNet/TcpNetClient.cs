using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KeenNet;

public class TcpNetClient
{
    private readonly Socket _socket;
    
    public TcpNetClient(Socket socket)
    {
        _socket = socket;
        StartReceiveAsync();
    }
    
    public TcpNetClient(string ip, int port)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(IPAddress.Parse(ip), port);
        StartReceiveAsync();
    }

    public async Task StartReceiveAsync()
    {
        //TODO:可取消CancelationSourceToken
        while (true)
        {
            try
            {
                //receive
                byte[] buffer = new byte[2048];//TODO: 内存池优化
                int length = await _socket.ReceiveAsync(buffer);

                // convert byts to string
                byte[] receivedBuffer = new byte[length];
                Array.Copy(buffer, receivedBuffer, length);
                string msg = Encoding.UTF8.GetString(receivedBuffer);
            
                // output
                Console.WriteLine(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public Task SendAsync(string msg)//TODO:发送Message而不是发送二进制
    {
        try
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            _socket.SendAsync(buffer);//TODO:线程安全问题
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Task.CompletedTask;
    }
}