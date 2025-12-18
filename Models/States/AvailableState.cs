using System.ComponentModel;

namespace FireRescueCommand.Models.States
{
    /// <summary>
    /// Stan dostępności - pojazd w jednostce, gotowy do dyspozycji
    /// </summary>
    public class AvailableState : IVehicleState
    {
        public string StateName => "Dostępny";
        public string StateColor => "#FF3B30"; // Czerwony

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Enter(Vehicle vehicle)
        {
            vehicle.CurrentEventId = null;
            vehicle.TimeInState = 0;
        }

        public void Exit(Vehicle vehicle)
        {
        }

        public void Update(Vehicle vehicle, double deltaTime)
        {
            // Stan dostępności - brak aktualizacji
        }
    }
}
