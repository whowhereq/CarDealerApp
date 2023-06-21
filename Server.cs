using CarDealerApp.Server;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Server
{
    private List<Car> cars;
    private Socket serverSocket;

    public Server()
    {
        cars = new List<Car>();
    }
    public void Start()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8080);
        serverSocket.Bind(ipEndPoint);
        serverSocket.Listen(10);
        Console.WriteLine("Сервер запущен. Ожидание подключений...");
        while (true)
        {
            Socket clientSocket = serverSocket.Accept();

            byte[] buffer = new byte[1024];
            int bytesRead = clientSocket.Receive(buffer);
            byte[] requestBytes = new byte[bytesRead];
            Array.Copy(buffer, requestBytes, bytesRead);
            ProcessClientRequest(requestBytes, clientSocket);

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
    private void ProcessClientRequest(byte[] requestBytes, Socket clientSocket)
    {
        List<Car> decodedCars = DecodeData(requestBytes);
        List<Car> responseCars = new List<Car>();
        string responseMessage = "";

        foreach (Car car in decodedCars)
        {
            if (car.NumberOfDoors == null)
            {
            }
            cars.Add(car);
            responseCars.Add(car);
        }
        byte[] responseBytes = EncodeData(responseCars);
        clientSocket.Send(responseBytes);
    }
    private List<Car> DecodeData(byte[] dataBytes)
    {
        List<Car> decodedCars = new List<Car>();

        int index = 0;

        byte header = dataBytes[index++];
        if (header != 0x02)
        {
        }

        byte numItems = dataBytes[index++];

        for (int i = 0; i < numItems; i++)
        {
            Car car = new Car();

            byte marker = dataBytes[index++];
            if (marker != 0x09)
            {
            }

            byte brandLength = dataBytes[index++];
            byte[] brandBytes = new byte[brandLength];
            Array.Copy(dataBytes, index, brandBytes, 0, brandLength);
            car.Brand = Encoding.ASCII.GetString(brandBytes);
            index += brandLength;

            byte yearMarker = dataBytes[index++];
            if (yearMarker != 0x12)
            {
            }

            byte[] yearBytes = new byte[2];
            Array.Copy(dataBytes, index, yearBytes, 0, 2);
            car.Year = BitConverter.ToUInt16(yearBytes, 0);
            index += 2;

            byte volumeMarker = dataBytes[index++];
            if (volumeMarker != 0x13)
            {
            }

            byte[] volumeBytes = new byte[4];
            Array.Copy(dataBytes, index, volumeBytes, 0, 4);
            car.EngineVolume = BitConverter.ToSingle(volumeBytes, 0);
            index += 4;

            if (index < dataBytes.Length && dataBytes[index] == 0x12)
            {
                byte doorsMarker = dataBytes[index++];
                if (doorsMarker != 0x12)
                {
                }

                byte[] doorsBytes = new byte[2];
                Array.Copy(dataBytes, index, doorsBytes, 0, 2);
                car.NumberOfDoors = (byte)BitConverter.ToUInt16(doorsBytes, 0);
                index += 2;
            }

            decodedCars.Add(car);
        }

        return decodedCars;
    }

    private byte[] EncodeData(List<Car> cars)
    {
        List<byte> encodedBytes = new List<byte>();
        encodedBytes.Add(0x02);
        encodedBytes.Add((byte)cars.Count);

        foreach (Car car in cars)
        {
            encodedBytes.Add(0x09);
            byte[] brandBytes = Encoding.ASCII.GetBytes(car.Brand);
            encodedBytes.Add((byte)brandBytes.Length);
            encodedBytes.AddRange(brandBytes);

            encodedBytes.Add(0x12);
            byte[] yearBytes = BitConverter.GetBytes(car.Year);
            encodedBytes.AddRange(yearBytes);

            encodedBytes.Add(0x13);
            byte[] volumeBytes = BitConverter.GetBytes(car.EngineVolume);
            encodedBytes.AddRange(volumeBytes);

            if (car.NumberOfDoors.HasValue)
            {
                encodedBytes.Add(0x12);
                byte[] doorsBytes = BitConverter.GetBytes(car.NumberOfDoors.Value);
                encodedBytes.AddRange(doorsBytes);
            }
        }

        return encodedBytes.ToArray();
    }
}