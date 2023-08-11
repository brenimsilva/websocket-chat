using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace WebSocketServer;

public static class Server
{
    public static HttpListener Listener = new();

    static Server()
    {
        Listener.Prefixes.Add("http://localhost:8090/");
        Console.WriteLine("Iniciando WebSocket");
    }
    public static async Task Start()
    {
        if (Listener.IsListening)
        {
            Console.WriteLine("JA ESTA RODANDO ARROMBADO");
            return;
        }
        Listener.Start();
        await AcceptConnection();
    }

    public static async Task AcceptConnection()
    {
        while (true)
        {
            var context = await Listener.GetContextAsync().ConfigureAwait(true);
            if (context.Request.IsWebSocketRequest)
            {
                var ctx = await context.AcceptWebSocketAsync(null);
                _ = Task.Run(() => ReceiveMessage(ctx));
            }
        }
    }

    static async Task ReceiveMessage(HttpListenerWebSocketContext ctx)
    {
        var buffer = new byte[2048];
        while (ctx.WebSocket.State is WebSocketState.Open or WebSocketState.Connecting)
        {
            var result = await ctx.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(true);
            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine(msg);
        }
    }
}