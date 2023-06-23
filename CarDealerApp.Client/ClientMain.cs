namespace CarDealerApp.Client;

public class ClientMain
{
    public static void Main(string[] args)
    {
        Client client = new Client();
        client.ConnectToServer();
    }
}