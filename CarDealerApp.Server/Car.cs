namespace CarDealerApp.Server
{
    internal class Car
    {
        public string Brand { get; set; }
        public ushort Year { get; set; }
        public float EngineVolume { get; set; }
        public byte? NumberOfDoors { get; set; }
    }
}