namespace CarDealerApp.Client;

public class ClientMain
{
    public static void Main(string[] args)
    {
        Client client = new Client();
        try
        {
            while (true)
            {
                Console.WriteLine("Input 1 for connect to server");
                string? choose = Console.ReadLine();
                if (choose == "1")
                {
                    client.ConnectToServer();
                }
                else
                {
                    Console.WriteLine("Erorr: not correct inpud");
                }
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.ToString());
        }
    }
}