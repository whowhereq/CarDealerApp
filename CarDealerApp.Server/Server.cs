using CarDealerApp.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CarDealerApp.Server
{
    public class Server
    {
        private const int Port = 1234; // Порт для прослушивания клиентских подключений
        private List<Car> cars; // Коллекция автомобилей

        public Server()
        {
            cars = new List<Car>();
        }

        public void Start()
        {
            try
            {
                // Создаем TcpListener для прослушивания подключений
                TcpListener listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    // Принимаем входящее подключение
                    TcpClient client = listener.AcceptTcpClient();

                    // Обрабатываем подключение в отдельном потоке
                    System.Threading.ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сервера: {ex.Message}");
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    // Читаем данные из сети
                    byte[] data = new byte[1024];
                    int bytesRead = stream.Read(data, 0, data.Length);

                    // Преобразуем полученные байты в список автомобилей
                    List<Car> receivedCars = DecodeData(data, bytesRead);

                    // Добавляем полученные автомобили в общую коллекцию
                    cars.AddRange(receivedCars);

                    Console.WriteLine("Принято автомобилей: " + receivedCars.Count);

                    // Отправляем ответ клиенту
                    string response = "Данные успешно приняты";
                    byte[] responseData = System.Text.Encoding.ASCII.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке подключения: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private List<Car> DecodeData(byte[] data, int length)
        {
            List<Car> decodedCars = new List<Car>();

            int index = 0;
            while (index < length)
            {
                // Читаем признак начала структуры
                byte startMarker = data[index];
                index++;

                if (startMarker != 0x02)
                {
                    throw new InvalidDataException("Неверный признак начала структуры");
                }

                // Читаем количество элементов в структуре
                byte numElements = data[index];
                index++;

                Car car = new Car();

                for (int i = 0; i < numElements; i++)
                {
                    // Читаем тип значения
                    byte valueType = data[index];
                    index++;

                    switch (valueType)
                    {
                        case 0x09: // Строка
                            int stringLength = data[index];
                            index++;

                            car.Brand = System.Text.Encoding.ASCII.GetString(data, index, stringLength);
                            index += stringLength;
                            break;
                        case 0x12: // Целое без знака
                            car.Year = BitConverter.ToUInt16(data, index);
                            index += 2;
                            break;
                        case 0x13: // С плавающей точкой
                            car.EngineVolume = BitConverter.ToSingle(data, index);
                            index += 4;
                            break;
                        default:
                            throw new InvalidDataException("Неверный тип значения");
                    }
                }

                decodedCars.Add(car);
            }

            return decodedCars;
        }
    }
}