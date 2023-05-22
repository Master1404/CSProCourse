using Logistic.Models.Enum;

namespace Logistic.WebApi.Model
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int MaxCargoWeightKg { get; set; }
        public double MaxCargoVolume { get; set; }
        public int CurrentCargoWeight { get; set; }
        public double CurrentCargoVolume { get; set; }
        public VehicleType Type { get; set; }
    }
}
