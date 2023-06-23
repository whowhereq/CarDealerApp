using CarDealerApp.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CarDealerApp.Server
{
    public class Server
    {
        private const int BufferSize = 1024;
        private const int Port = 12345;

        private List<Car> carList;
        private byte[] buffer;

        public Server()
        {
            carList = new List<Car>
        {
            new Car { Brand = "Nissan", Year = 2008, EngineVolume = 1.6f, NumberOfDoors = 4 },
            new Car { Brand = "Toyota", Year = 2010, EngineVolume = 2.0f, NumberOfDoors = 4 },
            new Car { Brand = "Honda", Year = 2015, EngineVolume = 1.8f, NumberOfDoors = 4 }
        };

            buffer = new byte[BufferSize];
        }

        public void Start()
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();
                Console.WriteLine("Server started. Waiting for client connections...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

                    HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                int bytesRead = stream.Read(buffer, 0, BufferSize);
                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Process client's request
                if (request.Trim() == "1")
                {
                    // Send all cars data
                    SendAllCarsData(stream);
                }
                else if (request.Trim() == "2")
                {
                    // Receive car index from the client
                    bytesRead = stream.Read(buffer, 0, BufferSize);
                    string carIndexStr = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    int carIndex;

                    if (int.TryParse(carIndexStr, out carIndex) && carIndex >= 0 && carIndex < carList.Count)
                    {
                        // Send car data by index
                        SendCarData(stream, carIndex);
                    }
                    else
                    {
                        // Send error message to the client
                        string errorMessage = "Invalid car index.";
                        byte[] errorData = Encoding.ASCII.GetBytes(errorMessage);
                        stream.Write(errorData, 0, errorData.Length);
                    }
                }

                client.Close();
                Console.WriteLine("Client disconnected: " + client.Client.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while handling client: " + ex.Message);
            }
        }


        private byte[] GetCarDataBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(0x02); // Start of structure

            foreach (var car in carList)
            {
                bytes.Add(0x09); // String type
                byte[] brandBytes = Encoding.ASCII.GetBytes(car.Brand);
                bytes.Add((byte)brandBytes.Length); // String length
                bytes.AddRange(brandBytes); // String value

                bytes.Add(0x12); // Unsigned integer type
                byte[] yearBytes = BitConverter.GetBytes(car.Year);
                bytes.AddRange(yearBytes); // Unsigned integer value

                bytes.Add(0x13); // Floating point type
                byte[] engineVolumeBytes = BitConverter.GetBytes(car.EngineVolume);
                bytes.AddRange(engineVolumeBytes); // Floating point value

                bytes.Add(0x14); // Unsigned integer type
                byte doorCount = car.NumberOfDoors.HasValue ? (byte)car.NumberOfDoors.Value : (byte)0;
                bytes.Add(doorCount); // Unsigned integer value
            }

            return bytes.ToArray();
        }

        private void SendAllCarsData(NetworkStream stream)
        {
            string message = "All Cars Data:\n";
            int carIndex = 1;

            foreach (var car in carList)
            {
                message += $"Car {carIndex}:\n";
                message += $"Brand: {car.Brand}\n";
                message += $"Year: {car.Year}\n";
                message += $"Engine Volume: {car.EngineVolume}\n";
                message += $"Door Count: {car.NumberOfDoors}\n";
                message += "-----------------------\n";
                carIndex++;
            }

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void SendCarData(NetworkStream stream, int carIndex)
        {
            Car car = carList[carIndex];
            string message = $"Car {carIndex + 1}:\n";
            message += $"Brand: {car.Brand}\n";
            message += $"Year: {car.Year}\n";
            message += $"Engine Volume: {car.EngineVolume}\n";
            message += $"Door Count: {car.NumberOfDoors}\n";

            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }

}