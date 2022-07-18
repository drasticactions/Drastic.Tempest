# SimpleChatMaui

SimpleChatMaui is a simple chat application, written with .NET MAUI. It's intended as an example of how to set up a Drastic.Tempest client and server.

## Getting Started

### Protocol

From the Tempest docs:

> The first thing we'll want to do is to define our protocol.We'll define
a simple protocol that will simply broadcast a message to everyone else
that's connected. We're only going to want reliable messages here,
which we'll keep in mind for later.
>
> As Tempest supports multiple protocols over a single connection, we'll
give our protocol a unique identifier. `1` is reserved for Tempest, so
we'll use `42`.

```csharp
    public static class SimpleChatProtocol
    {
        public static readonly Protocol Instance = new Protocol(42, 1);

        static SimpleChatProtocol()
        {
             // We need to tell our protocol about all the message
            // types belonging to it. Discover() does this automatically.
            Instance.Discover();
        }
    }
```

### Server

The server program is contained with SimpleChatMaui.Server. This is a `net6` console program. However, the server itself should be able to run on any `net6` device, as long as you have an open port. Once we create a protocol, we can create the server.

```csharp
var target = new Target(ip, port);
var provider = new NetworkConnectionProvider(SimpleChatProtocol.Instance, target, 100);
var server = new ChatServer(provider);
server.ConnectionMade += Server_ConnectionMade;
server.Start();
```

The `Target` is the IP Address / Hostname and port you wish to setup. If you wish to get a list of available IP Addresses on your host machine, you can use the helper method `NetworkUtils.DeviceIps`.

From the Tempest Docs:

> Each message type needs a unique identifier. Any `ushort` value will do, but
personally I like to put the values in an enum to make things easier

```csharp
public enum SimpleChatMessageType
	: ushort
{
	ChatMessage = 1
}
```

```csharp
public ChatServer(IConnectionProvider provider) : base(provider, MessageTypes.Reliable)
        {
            this.RegisterMessageHandler<ChatMessage>(OnChatMessage);
        }

...

private void OnChatMessage(MessageEventArgs<ChatMessage> e)
        {
            ChatMessage msg = e.Message;

            // Messages come in on various threads, we'll need to make
            // sure we stay thread safe.
            lock (this.connections)
            {
                foreach (IConnection connection in this.connections)
                    connection.SendAsync(e.Message);
            }
        }
```

In order for your server to recieve messages, you need to register message handles. This is done with `RegisterMessageHandler`

Each message is based on the `Message` type. Once you create a message and add its handler, its method will be called whenever a client sends that message type. In this case, we want to relay all of the messages back to the underlying clients.

### Client

From the Tempest Docs:

> For most applications, Tempest has a built in `TempestClient` class you can use
to handle the fundamentals. We'll subclass to provide an easy method to send
a chat message and add a listener for other's chat messages:

```csharp
 public class ChatClient
               : TempestClient
    {
        public ChatClient(IClientConnection connection)
            : base(connection, MessageTypes.Reliable)
        {
            this.RegisterMessageHandler<ChatMessage>(OnSayMessage);
        }

        public event EventHandler<ChatEventArgs> Chat;
        public Task<bool> SendMessage(ChatMessage chat)
        {
            return Connection.SendAsync(chat);
        }

        private void OnSayMessage(MessageEventArgs<ChatMessage> obj)
        {
            this.Chat?.Invoke(this, new ChatEventArgs(new User(0, obj.Message.Nickname, UserState.Present), obj.Message.Message));
        }
    }
```

### Transports
There are currently two available transports:

 - TCP
   - Supports reliable messaging
   - Supports encryption and signing
   - `NetworkClientConnection` for client connections
   - `NetworkConnectionProvider` for connection listeners
 - UDP
   - _Experimental_
   - Supports reliable and unreliable messaging
   - Supports encryption and signing
   - `UdpClientConnection` for client connections
   - `UdpConnectionProvider` for connection listeners





