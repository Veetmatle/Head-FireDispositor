using System;
using System.ComponentModel;

namespace FireRescueCommand.Models.States
{
    /// <summary>
    /// Stan na miejscu - pojazd prowadzi działania
    /// </summary>
    public class OnSceneState : IVehicleState
    {
        private readonly double _actionTime;

        public string StateName => "Na miejscu";
        public string StateColor => "#8E8E93"; // Szary

        public event PropertyChangedEventHandler? PropertyChanged;

        public OnSceneState(double actionTime)
        {
            _actionTime = actionTime;
        }

        public void Enter(Vehicle vehicle)
        {
            vehicle.TimeInState = 0;
            vehicle.LogMessage($"Pojazd {vehicle.Id} dotarł na miejsce zdarzenia #{vehicle.CurrentEventId}. Rozpoczęcie działań.");
        }

        public void Exit(Vehicle vehicle)
        {
            vehicle.LogMessage($"Pojazd {vehicle.Id} zakończył działania przy zdarzeniu #{vehicle.CurrentEventId}. Powrót do jednostki.");
        }

        public void Update(Vehicle vehicle, double deltaTime)
        {
            vehicle.TimeInState += deltaTime;

            if (vehicle.TimeInState >= _actionTime)
            {
                if (vehicle.CurrentEvent == null) return;
                
                // Zakończono działania - powrót do jednostki
                vehicle.SetState(new ReturningState(vehicle.CurrentEvent.ReturnTime, false));
            }
        }
    }
}
