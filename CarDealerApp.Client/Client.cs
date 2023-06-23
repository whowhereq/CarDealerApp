using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CarDealerApp.Client
{
    public class Client
    {
        private const int BufferSize = 1024;
        private const string ServerIp = "127.0.0.1";
        private const int ServerPort = 12345;

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
            while (true)
            {
                int bytesRead = clientSocket.Receive(buffer);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (response.Length > 0)
                {
                    Console.WriteLine(response);
                    break;
                }
            }
        }
    }
}
