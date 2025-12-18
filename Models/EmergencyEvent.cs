using System;
using System.ComponentModel;

namespace FireRescueCommand.Models
{
    /// <summary>
    /// zdarzenie alarmowe
    /// </summary>
    public class EmergencyEvent : INotifyPropertyChanged
    {
        private static int _nextId = 1;
        private static readonly Random _random = new Random();

        public int Id { get; }
        public EventType Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RequiredVehicles { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsFalseAlarm { get; }
        public double ActionTime { get; }
        public double ReturnTime { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public EmergencyEvent(EventType type, double latitude, double longitude)
        {
            Id = _nextId++;
            Type = type;
            Latitude = latitude;
            Longitude = longitude;
            RequiredVehicles = type == EventType.PZ ? 3 : 2; // PZ wymaga 3, MZ wymaga 2
            CreatedAt = DateTime.Now;
            IsActive = true;
            
            // raz dla całego zdarzenia (wspólne dla wszystkich pojazdów)
            IsFalseAlarm = _random.NextDouble() < 0.05; // 5% szansa na false alarm
            ActionTime = _random.NextDouble() * 20.0 + 5.0; // Czas działań 5-25s
            ReturnTime = _random.NextDouble() * 3.0; // Czas powrotu 0-3s
        }

        public double DistanceTo(double lat, double lon)
        {
            // odległość euklidesowa 
            var dLat = Latitude - lat;
            var dLon = Longitude - lon;
            return Math.Sqrt(dLat * dLat + dLon * dLon);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
