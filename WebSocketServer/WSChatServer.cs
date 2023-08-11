using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace WebSocketServer;

public class WSChatServer
{
    private static HttpListener _listener = new();
    private static ConcurrentDictionary<User, WebSocket> _users = new();
    static WSChatServer()
    {
        _listener.Prefixes.Add("http://localhost:7071/");
        Console.WriteLine("Rodando servidor de chat");
    }

    public static async Task Start()
    {
        if (_listener.IsListening)
        {
            Console.WriteLine("Already Running");
            return;
        }

        _listener.Start();
        await AcceptConnection();
    }

    public static async Task AcceptConnection()
    {
        while (true)
        {
            var ctx = await _listener.GetContextAsync();
            if (ctx.Request.IsWebSocketRequest)
            {
                var connection = await ctx.AcceptWebSocketAsync(null);
                Task.Run(() => HandleConnection(connection.WebSocket));
            }
        }
    }

    public static async Task HandleConnection(WebSocket ctx)
    { 
        byte[] buffer = new byte[4096];
        var user = await GetMessage<User>(ctx, buffer); 
        _users.TryAdd(user, ctx);
        Console.WriteLine($"Conectado {user.id}: {user.name}");
        try
        {
            while (ctx.State is WebSocketState.Open)
            {
                var msg = await GetMessage<Message>(ctx, buffer);
                switch (msg.type)
                {
                    case "BROADCAST":
                        await BroadCastMessage(msg);
                        continue;
                    case "PRIVATE":
                        await SendToUser(msg);
                        continue;
                    default:
                        continue;
                } 
            }
        }
        finally
        {
            Console.WriteLine("Closed Connection");
            _users.TryRemove(user, out ctx);
            ctx.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }

    public static async Task<T> GetMessage<T>(WebSocket ctx, byte[] buffer)
    {
        var result = await ctx.ReceiveAsync(buffer, CancellationToken.None);
        var msg =  Encoding.UTF8.GetString(buffer, 0, result.Count);
        var msgObject = JsonSerializer.Deserialize<T>(msg);
        return msgObject;
    }

    public static async Task SendToUser(Message message)
    {
        byte[] b = UTF8Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        ArraySegment<byte> messageSegment = new ArraySegment<byte>(b);
        if (_users.TryGetValue(message.to, out WebSocket ctx))
        {
            ctx.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
        };
    }

    public static async Task BroadCastMessage(Message msg)
    {
        byte[] bytemsg = UTF8Encoding.UTF8.GetBytes(msg.message);
        ArraySegment<byte> messageSegment = new ArraySegment<byte>(bytemsg);
        foreach (var ctx in _users.Values)
        {
            ctx.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}