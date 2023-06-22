using CarDealerApp.Client;

public class Program
{
    public static void Main(string[] args)
    {
        Client client = new Client();
        client.Connect("127.0.0.1", 8080);

        // Запрос всех автомобилей
        client.RequestAllCars();

        // Запрос автомобиля по номеру записи
        client.RequestCarByIndex(1);

        client.ClientSocket.Close();
    }
}