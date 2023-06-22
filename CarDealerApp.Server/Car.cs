namespace CarDealerApp.Server
{
    class Car
    {
        public string Brand { get; set; }
        public int Year { get; set; }
        public float EngineVolume { get; set; }
        public int? NumDoors { get; set; }

        public Car(string brand, int year, float engineVolume, int? numDoors)
        {
            Brand = brand;
            Year = year;
            EngineVolume = engineVolume;
            NumDoors = numDoors;
        }
    }
}
