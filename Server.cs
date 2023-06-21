using CarDealerApp.Server;
using System.Net.Sockets;

internal class Server
{
    private List<Car> cars;
    private Socket serverSocket;

    public Server()
    {
        cars = new List<Car>();
    }
}