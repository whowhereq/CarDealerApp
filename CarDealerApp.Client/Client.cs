using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CarDealerApp.Client;

public class Client
{
    private const int BufferSize = 1024;
    private const string ServerIp = "127.0.0.1";
    private const int ServerPort = 8080;

    private Socket clientSocket;
    private byte[] buffer;

    public Client()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        buffer = new byte[BufferSize];
    }
    public void ConnectToServer()
    {
        try
        {
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));
            Console.WriteLine("Connected to the server.");

            while (true)
            {
                Console.WriteLine("Enter your choice (1 - Get all cars, 2 - Get car by index):");
                string choice = Console.ReadLine().Trim();

                if (choice == "1" || choice == "2")
                {
                    SendRequest(choice);
                    ReceiveResponse();
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
        }
        finally
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    private void SendRequest(string choice)
    {
        byte[] data = Encoding.ASCII.GetBytes(choice);
        clientSocket.Send(data);
    }

    private void ReceiveResponse()
    {
        byte[] receivedData = new byte[BufferSize];
        int bytesRead = clientSocket.Receive(receivedData);

        if (bytesRead > 0)
        {
            ProcessResponse(receivedData, bytesRead);
        }
    }

    private void ProcessResponse(byte[] data, int length)
    {
        List<Car> cars = new List<Car>();

        int index = 0;
        while (index < length)
        {
            if (data[index] == 0x02) // Признак начала структуры
            {
                index++;
                int carCount = data[index]; // Число элементов в структуре
                index++;

                for (int i = 0; i < carCount; i++)
                {
                    Car car = new Car();

                    while (index < length && data[index] != 0x09) // Поиск типа данных "строка"
                    {
                        index++;
                    }

                    index++; // Пропуск типа "строка"
                    int stringLength = data[index]; // Длина строки
                    index++;

                    byte[] brandBytes = new byte[stringLength];
                    Array.Copy(data, index, brandBytes, 0, stringLength);
                    car.Brand = Encoding.ASCII.GetString(brandBytes);
                    index += stringLength;

                    while (index < length && data[index] != 0x12) // Поиск типа данных "2 байта целое без знака"
                    {
                        index++;
                    }

                    index++; // Пропуск типа "2 байта целое без знака"
                    car.Year = BitConverter.ToUInt16(data, index);
                    index += 2;

                    while (index < length && data[index] != 0x13) // Поиск типа данных "4 байта с плавающей точкой"
                    {
                        index++;
                    }

                    index++; // Пропуск типа "4 байта с плавающей точкой"
                    car.EngineVolume = BitConverter.ToSingle(data, index);
                    index += 4;
                    if (index < length && data[index] == 0x12) // Проверяем наличие числа дверей
                    {
                        index++; // Пропуск типа "2 байта целое без знака"
                        car.NumberOfDoors = BitConverter.ToUInt16(data, index);
                        index += 2;
                    }

                    cars.Add(car);
                }
            }
            else
            {
                index++;
            }
        }

        // Обработка полученных данных
        foreach (var car in cars)
        {
            Console.WriteLine("Brand: " + car.Brand);
            Console.WriteLine("Year: " + car.Year);
            Console.WriteLine("Engine Volume: " + car.EngineVolume);
            if (car.NumberOfDoors.HasValue)
            {
                Console.WriteLine("Door Count: " + car.NumberOfDoors.Value);
            }
            Console.WriteLine("-----------------------");
        }
    }
}
