using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FireRescueCommand.Models
{
    /// <summary>
    /// Jednostka straży pożarnej 
    /// </summary>
    public class FireStation : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ObservableCollection<Vehicle> Vehicles { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public FireStation(string name, double latitude, double longitude)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Vehicles = new ObservableCollection<Vehicle>();
            
            for (int i = 1; i <= 5; i++)
            {
                Vehicles.Add(new Vehicle(i, name));
            }
        }

        public int AvailableVehicleCount => Vehicles.Count(v => v.IsAvailable);

        public string StatusText => $"{AvailableVehicleCount}/5 dostępnych";

        public string StatusColor
        {
            get
            {
                int available = AvailableVehicleCount;
                if (available == 5) return "#34C759"; // Zielony
                if (available == 0) return "#FF3B30"; // Czerwony
                if (available <= 2) return "#FF9500"; // Pomarańczowy
                return "#FFCC00"; // Żółty
            }
        }

        public double DistanceTo(double lat, double lon)
        {
            var dLat = Latitude - lat;
            var dLon = Longitude - lon;
            return Math.Sqrt(dLat * dLat + dLon * dLon);
        }

        public List<Vehicle> GetAvailableVehicles(int count)
        {
            return Vehicles.Where(v => v.IsAvailable).Take(count).ToList();
        }

        public void Update(double deltaTime)
        {
            foreach (var vehicle in Vehicles)
            {
                vehicle.Update(deltaTime);
            }

            OnPropertyChanged(nameof(AvailableVehicleCount));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(StatusColor));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
