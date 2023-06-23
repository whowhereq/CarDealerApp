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
                    try
                    {
                        client.ConnectToServer();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка подключения");
                    }
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