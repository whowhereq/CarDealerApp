using CarDealerApp.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CarDealerApp.Server
{
    public class Server
    {
        private const int BufferSize = 1024;
        private const int Port = 8080;

        private List<Car> carList;
        private Socket serverSocket;
        private byte[] buffer;

        public Server()
        {
            carList = new List<Car>
            {
                new Car { Brand = "Nissan", Year = 2008, EngineVolume = 1.6f, NumberOfDoors = 4 },
                new Car { Brand = "Toyota", Year = 2010, EngineVolume = 2.0f, NumberOfDoors = 4 },
                new Car { Brand = "Honda", Year = 2015, EngineVolume = 1.8f, NumberOfDoors = 4 }
            };

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            buffer = new byte[BufferSize];
        }

        public void Start()
        {
            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
                serverSocket.Listen(10);

                Console.WriteLine("Server started. Waiting for client connections...");

                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();
                    Console.WriteLine("Client connected: " + clientSocket.RemoteEndPoint);

                    HandleClient(clientSocket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            try
            {
                while (clientSocket.Connected)
                {
                    int bytesRead = clientSocket.Receive(buffer);
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (request.Length < 3)
                    if (request.Trim() == "1")
                    {
                        SendCarData(clientSocket, carIndex: -1);
                    }
                    else if (request.StartsWith("2"))
                    {
                        int carIndex = Convert.ToInt32(request.Substring(1));
                        if (carIndex >= 0 && carIndex < carList.Count)
                        {
                            SendCarData(clientSocket, carIndex);
                        }
                        else
                        {
                                continue;
                        }
                    }
                }
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                Console.WriteLine("Client disconnected: " + clientSocket.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while handling client: " + ex.Message);
            }
        }


        private byte[] GetCarDataBytes(int carIndex)
        {
            List<Car> selectedCars = new List<Car>();

            if (carIndex >= 0 && carIndex <= carList.Count)
            {
                selectedCars.Add(carList[carIndex]);
            }
            else
            {
                selectedCars = carList;
            }

            List<byte> bytes = new List<byte>();
            bytes.Add(0x02);
            bytes.Add((byte)selectedCars.Count);

            foreach (var car in selectedCars)
            {
                bytes.Add(0x09); 
                byte[] brandBytes = Encoding.ASCII.GetBytes(car.Brand);
                bytes.Add((byte)brandBytes.Length); 
                bytes.AddRange(brandBytes); 

                bytes.Add(0x12);
                byte[] yearBytes = BitConverter.GetBytes((ushort)car.Year);
                bytes.AddRange(yearBytes);

                bytes.Add(0x13);
                byte[] engineVolumeBytes = BitConverter.GetBytes(car.EngineVolume);
                bytes.AddRange(engineVolumeBytes);

                if (car.NumberOfDoors.HasValue)
                {
                    bytes.Add(0x12);
                    byte[] doorCountBytes = BitConverter.GetBytes((ushort)car.NumberOfDoors.Value);
                    bytes.AddRange(doorCountBytes);
                }
            }

            return bytes.ToArray();
        }

        private void SendCarData(Socket clientSocket, int carIndex)
        {
                byte[] data = GetCarDataBytes(carIndex);
                clientSocket.Send(data);
        }
    }

}