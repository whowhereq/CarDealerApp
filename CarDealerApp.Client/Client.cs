using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

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

            while (clientSocket.Connected)
            {
                Console.WriteLine("Enter your choice (1 - Get all cars, 2 - Get car by index):");
                string choice = Console.ReadLine().Trim();

                if (choice == "1")
                {
                    SendRequest(choice);
                    ReceiveResponse();
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Enter the index of the car(0,1 or 2):");
                    string carIndex = Console.ReadLine();
                    if(carIndex == "0" || carIndex == "1" || carIndex == "2")
                    {
                        SendRequest(choice + carIndex);
                        ReceiveResponse();
                    }
                    else
                    {
                        Console.WriteLine("Invalid car index. Please try again.");
                    }
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
    private void SaveResponseToXml(string response)
    {
        try
        {
            const string fileName = "carslist.xml";

            XmlDocument xmlDoc = new XmlDocument();

            if (File.Exists(fileName))
            {
                xmlDoc.Load(fileName);

                XmlNode root = xmlDoc.SelectSingleNode("Cars");
                XmlNode newResponseNode = xmlDoc.CreateNode(XmlNodeType.Element, "Response", "");
                newResponseNode.InnerText = response;
                root.AppendChild(newResponseNode);
            }
            else
            {
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlNode rootNode = xmlDoc.CreateElement("Cars");

                XmlNode responseNode = xmlDoc.CreateElement("Response");
                responseNode.InnerText = response;

                rootNode.AppendChild(responseNode);
                xmlDoc.AppendChild(xmlDeclaration);
                xmlDoc.AppendChild(rootNode);
            }

            // Save the XML document
            xmlDoc.Save(fileName);

            Console.WriteLine($"Response saved to '{fileName}' as XML.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred while saving the response to XML: " + ex.Message);
        }
    }

    private void ProcessResponse(byte[] data, int length)
    {
        List<Car> cars = new List<Car>();

        int index = 0;
        while (index < length)
        {
            if (data[index] == 0x02)
            {
                index++;
                int carCount = data[index];
                index++;

                for (int i = 0; i < carCount; i++)
                {
                    Car car = new Car();

                    while (index < length && data[index] != 0x09)
                    {
                        index++;
                    }

                    index++; 
                    int stringLength = data[index]; 
                    index++;

                    byte[] brandBytes = new byte[stringLength];
                    Array.Copy(data, index, brandBytes, 0, stringLength);
                    car.Brand = Encoding.ASCII.GetString(brandBytes);
                    index += stringLength;

                    while (index < length && data[index] != 0x12) 
                    {
                        index++;
                    }

                    index++;
                    car.Year = BitConverter.ToUInt16(data, index);
                    index += 2;

                    while (index < length && data[index] != 0x13)
                    {
                        index++;
                    }

                    index++; 
                    car.EngineVolume = BitConverter.ToSingle(data, index);
                    index += 4;
                    if (index < length && data[index] == 0x12) 
                    {
                        index++; 
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
        foreach (var car in cars)
        {
            string brand = "Brand: " + car.Brand;
            string year = "Year: " + car.Year;
            string engineVolume = "Engine Volume: " + car.EngineVolume;
            string doorCount = "Door Count: ";
            string line = "-----------------------";
            if (car.NumberOfDoors.HasValue)
            {
                doorCount = "Door Count: " + car.NumberOfDoors.Value;
            }
            string response = brand + "\n" + year + "\n" + engineVolume + "\n" + doorCount + "\n" + line + "\n";
            Console.WriteLine(response);
            SaveResponseToXml(response);
        }
    }
}
