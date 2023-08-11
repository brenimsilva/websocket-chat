using WSNotification;

await Task.WhenAll(new List<Task>()
{
    // WebSocketServer.Server.Start(),
    WebSocketServer.WSChatServer.Start(),
    Server.Run(args),
});