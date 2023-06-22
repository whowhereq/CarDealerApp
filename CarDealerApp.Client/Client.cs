using System;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace CarDealerApp.Client
{
    public class Client
    {
        private Socket clientSocket;

        public Client()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Socket ClientSocket { get => clientSocket; set => clientSocket = value; }

        public void Connect(string ipAddress, int port)
        {
            ClientSocket.Connect(ipAddress, port);
            Console.WriteLine("Соединение установлено.");
        }

        public void RequestAllCars()
        {
            // Отправка запроса "спросить все"
            byte[] requestBytes = Encoding.ASCII.GetBytes("AskAll");
            ClientSocket.Send(requestBytes);

            // Получение ответа от сервера
            byte[] responseBytes = new byte[1024];
            int bytesRead = ClientSocket.Receive(responseBytes);
            byte[] responseDataBytes = new byte[bytesRead];
            Array.Copy(responseBytes, responseDataBytes, bytesRead);

            // Декодирование данных и сохранение в XML
            XmlDocument xmlDoc = DecodeAndSaveToXml(responseDataBytes);

            // Вывод данных в читаемом формате
            Console.WriteLine("Все автомобили:\n");
            foreach (XmlNode carNode in xmlDoc.SelectNodes("/Cars/Car"))
            {
                string brand = carNode.SelectSingleNode("Brand").InnerText;
                string year = carNode.SelectSingleNode("Year").InnerText;
                string engineVolume = carNode.SelectSingleNode("EngineVolume").InnerText;
                string numberOfDoors = carNode.SelectSingleNode("NumberOfDoors").InnerText;

                Console.WriteLine($"Марка: {brand}");
                Console.WriteLine($"Год выпуска: {year}");
                Console.WriteLine($"Объем двигателя: {engineVolume}");
                Console.WriteLine($"Число дверей: {numberOfDoors}");
                Console.WriteLine();
            }
        }

        public void RequestCarByIndex(int index)
        {
            // Отправка запроса "спросить по номеру записи"
            byte[] requestBytes = Encoding.ASCII.GetBytes($"AskByIndex:{index}");
            ClientSocket.Send(requestBytes);

            // Получение ответа от сервера
            byte[] responseBytes = new byte[1024];
            int bytesRead = ClientSocket.Receive(responseBytes);
            byte[] responseDataBytes = new byte[bytesRead];
            Array.Copy(responseBytes, responseDataBytes, bytesRead);

            // Декодирование данных и сохранение в XML
            XmlDocument xmlDoc = DecodeAndSaveToXml(responseDataBytes);

            // Вывод данных в читаемом формате
            XmlNode carNode = xmlDoc.SelectSingleNode("/Car");
            if (carNode != null)
            {
                string brand = carNode.SelectSingleNode("Brand").InnerText;
                string year = carNode.SelectSingleNode("Year").InnerText;
                string engineVolume = carNode.SelectSingleNode("EngineVolume").InnerText;
                string numberOfDoors = carNode.SelectSingleNode("NumberOfDoors").InnerText;

                Console.WriteLine($"Марка: {brand}");
                Console.WriteLine($"Год выпуска: {year}");
                Console.WriteLine($"Объем двигателя: {engineVolume}");
                Console.WriteLine($"Число дверей: {numberOfDoors}");
            }
            else
            {
                Console.WriteLine("Автомобиль с указанным номером записи не найден.");
            }
        }

        private XmlDocument DecodeAndSaveToXml(byte[] dataBytes)
        {
            // Декодирование байтовой последовательности
            // и сохранение в XML документ
            // TODO: Реализовать декодирование и сохранение в XML

            // Пример декодирования без сохранения в XML
            string decodedData = Encoding.ASCII.GetString(dataBytes);
            Console.WriteLine("Декодированные данные:");
            Console.WriteLine(decodedData);

            return new XmlDocument();
        }
    }
}