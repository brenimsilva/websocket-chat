using System.Net;
using System.Net.WebSockets;

namespace WebSocketServer;

public class S1
{
   private static HttpListener _listener = new();
   static S1()
   {
      _listener.Prefixes.Add("http://localhost5321");
   }

   public static async Task Start()
   {
      if (_listener.IsListening)
      {
         
      }
      
      _listener.Start();
      await AcceptConnection();
   }

   public static async Task AcceptConnection()
   {
      while (true)
      {
         HttpListenerContext listenerContext = await _listener.GetContextAsync().ConfigureAwait(true);
         if (listenerContext.Request.IsWebSocketRequest)
         {
            var connection = await listenerContext.AcceptWebSocketAsync(null);
            
         }
      }
   }
}