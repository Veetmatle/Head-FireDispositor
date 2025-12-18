using System;
using System.ComponentModel;

namespace FireRescueCommand.Models.States
{
    /// <summary>
    /// Stan w drodze - pojazd jedzie do miejsca zdarzenia
    /// </summary>
    public class EnRouteState : IVehicleState
    {
        private readonly double _travelTime;

        public string StateName => "W drodze";
        public string StateColor => "#AF52DE"; // Fioletowy

        public event PropertyChangedEventHandler? PropertyChanged;

        public EnRouteState(double travelTime)
        {
            _travelTime = travelTime;
        }

        public void Enter(Vehicle vehicle)
        {
            vehicle.TimeInState = 0;
        }

        public void Exit(Vehicle vehicle)
        {
        }

        public void Update(Vehicle vehicle, double deltaTime)
        {
            vehicle.TimeInState += deltaTime;

            if (vehicle.TimeInState >= _travelTime)
            {
                if (vehicle.CurrentEvent == null) return;
                
                // Dojechał na miejsce - sprawdź czy to alarm fałszywy
                if (vehicle.CurrentEvent.IsFalseAlarm)
                {
                    // Alarm fałszywy - natychmiastowy powrót
                    vehicle.LogMessage($"Zdarzenie #{vehicle.CurrentEventId} okazało się ALARMEM FAŁSZYWYM. Powrót do jednostki.");
                    vehicle.SetState(new ReturningState(vehicle.CurrentEvent.ReturnTime, true));
                }
                else
                {
                    // Prawdziwe zdarzenie - rozpoczęcie działań
                    vehicle.SetState(new OnSceneState(vehicle.CurrentEvent.ActionTime));
                }
            }
        }
    }
}
