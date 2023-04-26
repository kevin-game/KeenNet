using System.Diagnostics;
using KeenNet;

try
{
    KeenNetClient client = new KeenNetClient("127.0.0.1", 8080);
    string msg = $"This is a message from client [{Process.GetCurrentProcess().Id}]";
    await client.SendAsync(msg);

    await Task.Delay(100 * 1000);
}
catch (Exception e)
{
    Console.WriteLine(e);
}