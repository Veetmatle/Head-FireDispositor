using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FireRescueCommand.Models;
using FireRescueCommand.Strategies;

namespace FireRescueCommand.Services
{
    /// <summary>
    /// Typy logów dla kolorowania
    /// </summary>
    public enum LogType
    {
        Event,      // Czerwony - nowe zdarzenie lub zakończone
        Dispatch,   // Niebieski - dysponowanie
        System      // Zielony - komunikaty systemowe
    }

    /// <summary>
    /// Centrum dowodzenia 
    /// </summary>
    public class CommandCenter
    {
        private readonly ObservableCollection<FireStation> _stations;
        private readonly IDispatchStrategy _dispatchStrategy;
        private readonly Random _random;
        private readonly Dictionary<int, int> _eventVehicleCount; 
        private readonly Dictionary<int, int> _eventCompletedCount; 

        public ObservableCollection<EmergencyEvent> ActiveEvents { get; }
        public event Action<string, string>? LogGenerated; // message, color

        // Granice obszaru Kraków (WGS-84)
        private const double MinLat = 49.95855025648944;
        private const double MaxLat = 50.154564013341734;
        private const double MinLon = 19.688292482742394;
        private const double MaxLon = 20.02470275868903;

        public CommandCenter(ObservableCollection<FireStation> stations)
        {
            _stations = stations;
            _dispatchStrategy = new NearestStationStrategy();
            _random = new Random();
            ActiveEvents = new ObservableCollection<EmergencyEvent>();
            _eventVehicleCount = new Dictionary<int, int>();
            _eventCompletedCount = new Dictionary<int, int>();
        }

        public void SubscribeToVehicle(Vehicle vehicle)
        {
            vehicle.EventCompleted += OnVehicleEventCompleted;
        }

        private void OnVehicleEventCompleted(int eventId)
        {
            if (!_eventCompletedCount.ContainsKey(eventId))
            {
                _eventCompletedCount[eventId] = 0;
            }

            _eventCompletedCount[eventId]++;
            
            if (_eventVehicleCount.ContainsKey(eventId) && 
                _eventCompletedCount[eventId] >= _eventVehicleCount[eventId])
            {
                var eventToRemove = ActiveEvents.FirstOrDefault(e => e.Id == eventId);
                if (eventToRemove != null)
                {
                    System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                    {
                        ActiveEvents.Remove(eventToRemove);
                    });
                    
                    Log($"ZAKOŃCZONO: Zdarzenie #{eventId} zostało w pełni obsłużone. Wszystkie pojazdy wróciły.", LogType.Event);
                    
                    _eventVehicleCount.Remove(eventId);
                    _eventCompletedCount.Remove(eventId);
                }
            }
        }

        public EmergencyEvent? GenerateRandomEvent()
        {
            var lat = MinLat + _random.NextDouble() * (MaxLat - MinLat);
            var lon = MinLon + _random.NextDouble() * (MaxLon - MinLon);

            // 70% MZ, 30% PZ
            var type = _random.NextDouble() < 0.7 ? EventType.MZ : EventType.PZ;

            var newEvent = new EmergencyEvent(type, lat, lon);
            ActiveEvents.Add(newEvent);

            var typeStr = type == EventType.PZ ? "POŻAR" : "Miejscowe Zagrożenie";
            var requiredStr = type == EventType.PZ ? "3 pojazdy" : "2 pojazdy";
            Log($"NOWE ZDARZENIE #{newEvent.Id} ({typeStr}) zgłoszone na [{lat:F4}, {lon:F4}] - wymagane: {requiredStr}", LogType.Event);
            
            DispatchVehicles(newEvent);

            return newEvent;
        }

        private void DispatchVehicles(EmergencyEvent emergencyEvent)
        {
            var selectedStations = _dispatchStrategy.SelectStations(
                _stations.ToList(), 
                emergencyEvent, 
                emergencyEvent.RequiredVehicles);

            if (selectedStations.Count == 0)
            {
                Log($"UWAGA: Brak dostępnych jednostek dla zdarzenia #{emergencyEvent.Id}", LogType.System);
                ActiveEvents.Remove(emergencyEvent);
                return;
            }

            var dispatchedCount = 0;
            var remainingVehicles = emergencyEvent.RequiredVehicles;
            
            // Lista pojazdów do wysłania
            var vehiclesToDispatch = new List<Vehicle>();

            foreach (var station in selectedStations)
            {
                var vehiclesToTake = Math.Min(station.AvailableVehicleCount, remainingVehicles);
                var vehicles = station.GetAvailableVehicles(vehiclesToTake);
                
                vehiclesToDispatch.AddRange(vehicles);
                
                remainingVehicles -= vehiclesToTake;
                if (remainingVehicles <= 0) break;
            }

            // 1 czas dojazdu dla wszystkich pojazdów do zdarzenia
            var travelTime = _random.NextDouble() * 3.0; 
            
            foreach (var vehicle in vehiclesToDispatch)
            {
                vehicle.DispatchToEvent(emergencyEvent, travelTime);
                dispatchedCount++;
            }

            // Zapisz liczbę zadysponowanych pojazdów
            _eventVehicleCount[emergencyEvent.Id] = dispatchedCount;
            _eventCompletedCount[emergencyEvent.Id] = 0;

            Log($"DYSPONOWANIE: {dispatchedCount} pojazd(ów) zadysponowanych do zdarzenia #{emergencyEvent.Id}", LogType.Dispatch);
        }

        public void Update(double deltaTime)
        {
            foreach (var station in _stations)
            {
                station.Update(deltaTime);
            }
        }

        private void Log(string message, LogType type)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var color = type switch
            {
                LogType.Event => "#FF3B30",     // Czerwony
                LogType.Dispatch => "#007AFF",  // Niebieski
                LogType.System => "#34C759",    // Zielony
                _ => "#FFFFFF"
            };
            LogGenerated?.Invoke($"[{timestamp}] {message}", color);
        }
    }
}
