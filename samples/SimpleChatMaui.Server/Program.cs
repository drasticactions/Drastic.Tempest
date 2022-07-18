using Drastic.Tempest;
using Drastic.Tempest.Providers.Network;
using Sharprompt;
using SimpleChat.Core;

Console.WriteLine("SimpleChatMaui.Server");


var ips = NetworkUtils.DeviceIps().ToList();
var ip = Prompt.Select("Select your IP Address", ips);
var port = Prompt.Input<int>("Enter port number", 8888);
var target = new Target(ip, port);
var provider = new NetworkConnectionProvider(SimpleChatProtocol.Instance, target, 100);
var server = new ChatServer(provider);
server.ConnectionMade += Server_ConnectionMade;

server.Start();

Console.WriteLine("Server Started");

// Stay running until you close the window.
while (true)
{
}

void Server_ConnectionMade(object? sender, ConnectionMadeEventArgs e)
{
    Console.WriteLine(e.ToString());
}