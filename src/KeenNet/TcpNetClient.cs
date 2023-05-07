using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KeenNet;

public class TcpNetClient
{
    private readonly Socket _socket;
    private readonly Pipe _pipe = new Pipe(new PipeOptions(resumeWriterThreshold:1024, pauseWriterThreshold:4096));//TODO:参数配置化
    
    public static int minimumBufferSize = 1024;//TODO:参数配置化
    
    public TcpNetClient(Socket socket)
    {
        _socket = socket;
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _socket.NoDelay = true;
        // TODO
        // _socket.Ttl = 32;//default 32. 0-256 is valid
        // _socket.ReceiveBufferSize = 8192;//default 8192. Bigger maybe faster.
        // _socket.SendBufferSize = 8192;//default 8192. Bigger maybe faster.
        
        StartReceiveAsync();
    }
    
    public TcpNetClient(string ip, int port)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _socket.NoDelay = true;
        // TODO
        // _socket.Ttl = 32;//default 32. 0-256 is valid
        // _socket.ReceiveBufferSize = 8192;//default 8192. Bigger maybe faster.
        // _socket.SendBufferSize = 8192;//default 8192. Bigger maybe faster.

        _socket.Connect(IPAddress.Parse(ip), port);
        
        StartReceiveAsync();
    }

    public async Task StartReceiveAsync()
    {
        Task receiveTask = ReceiveMessageAsync();
        Task processTask = ProcessMessageAsync();

        await Task.WhenAll(receiveTask, processTask);
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

    private async Task ReceiveMessageAsync()
    {
        PipeWriter writer = _pipe.Writer;
        
        while (true)
        {
            try
            {
                Memory<byte> memory = writer.GetMemory(minimumBufferSize);
        
                int bytesRead = await _socket.ReceiveAsync(memory, SocketFlags.None);
                if (bytesRead == 0)
                {
                    break;
                }

                writer.Advance(bytesRead);

                var result = await writer.FlushAsync();

                if (result.IsCompleted)
                    break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                break;
            }

        }

        await writer.CompleteAsync();
        
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task ProcessMessageAsync()
    {
        PipeReader _pipeReader = _pipe.Reader;

        while (true)
        {
            var result = await _pipeReader.ReadAsync();
            var buffer = result.Buffer;
            
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                continue;
            }

            // process messages
            var line = buffer.Slice(0, position.Value);
            string msg = Encoding.UTF8.GetString(line);
            Console.WriteLine(msg);
            
            // advance buffer
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            _pipeReader.AdvanceTo(buffer.Start, buffer.End);
            
            // Stop reading if there's no more data coming.
            if (result.IsCompleted)
            {
                break;
            }
        }

        await _pipeReader.CompleteAsync();
    }
}