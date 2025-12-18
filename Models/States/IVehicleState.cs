using System.ComponentModel;

namespace FireRescueCommand.Models.States
{
    /// <summary>
    /// Interfejs dla stanu pojazdu (State Pattern)
    /// </summary>
    public interface IVehicleState : INotifyPropertyChanged
    {
        string StateName { get; }
        string StateColor { get; }
        
        void Enter(Vehicle vehicle);
        void Exit(Vehicle vehicle);
        void Update(Vehicle vehicle, double deltaTime);
    }
}
