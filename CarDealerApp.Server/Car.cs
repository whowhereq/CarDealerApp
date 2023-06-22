namespace CarDealerApp.Server
{
    public class Car
    {
        public string Brand { get; set; }
        public int Year { get; set; }
        public float EngineVolume { get; set; }
        public int? NumberOfDoors { get; set; } // nullable int

        public override string ToString()
        {
            return $"Марка: {Brand}, Год выпуска: {Year}, Объем двигателя: {EngineVolume}, Число дверей: {(NumberOfDoors.HasValue ? NumberOfDoors.ToString() : "Не указано")}";
        }
    }

}
