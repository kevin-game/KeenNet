using KeenNet;

Console.WriteLine($"[Server]");

KeenNetServer server = new KeenNetServer(8080);

await Task.Delay(100 * 1000);

