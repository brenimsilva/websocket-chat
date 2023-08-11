namespace WebSocketServer;

public class User
{
    public int id { get; set; }
    public string name { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is User))
            return false;

        return id == ((User)obj).id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}