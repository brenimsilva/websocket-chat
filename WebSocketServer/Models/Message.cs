namespace WebSocketServer;

public record Message()
{
    public string type { get; set; }
    public User from { get; set; }
    public string message { get; set; }
    public User? to { get; set; }
}