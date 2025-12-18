using System.Collections.Generic;
using FireRescueCommand.Models;

namespace FireRescueCommand.Strategies
{
    /// <summary>
    /// Interfejs (Strategy Pattern)
    /// </summary>
    public interface IDispatchStrategy
    {
        List<FireStation> SelectStations(
            List<FireStation> stations, 
            EmergencyEvent eventData, 
            int requiredVehicles);
    }
}
