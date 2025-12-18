using System;
using System.ComponentModel;

namespace FireRescueCommand.Models.States
{
    /// <summary>
    /// Stan powrotu - pojazd wraca do jednostki po zdarzeniu
    /// </summary>
    public class ReturningState : IVehicleState
    {
        private readonly double _returnTime;
        private readonly bool _wasFalseAlarm;

        public string StateName => "Powrót";
        public string StateColor => "#FFCC00"; // Żółty

        public event PropertyChangedEventHandler? PropertyChanged;

        public ReturningState(double returnTime, bool wasFalseAlarm = false)
        {
            _returnTime = returnTime;
            _wasFalseAlarm = wasFalseAlarm;
        }

        public void Enter(Vehicle vehicle)
        {
            vehicle.TimeInState = 0;
        }

        public void Exit(Vehicle vehicle)
        {
            if (!_wasFalseAlarm)
            {
                vehicle.LogMessage($"Pojazd {vehicle.Id} wrócił do jednostki {vehicle.StationName}. Zdarzenie #{vehicle.CurrentEventId} ZAKOŃCZONE.");
            }
            else
            {
                vehicle.LogMessage($"Pojazd {vehicle.Id} wrócił do jednostki {vehicle.StationName}.");
            }
            
            // Powiadom o zakończeniu obsługi zdarzenia
            vehicle.NotifyEventCompleted();
        }

        public void Update(Vehicle vehicle, double deltaTime)
        {
            vehicle.TimeInState += deltaTime;

            if (vehicle.TimeInState >= _returnTime)
            {
                // Wrócił do jednostki - znowu dostępny
                vehicle.SetState(new AvailableState());
            }
        }
    }
}
