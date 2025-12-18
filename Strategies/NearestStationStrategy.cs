using System.Collections.Generic;
using System.Linq;
using FireRescueCommand.Models;

namespace FireRescueCommand.Strategies
{
    /// <summary>
    /// Strategia dysponowania z najbliższych jednostek z dostępnymi pojazdami
    /// </summary>
    public class NearestStationStrategy : IDispatchStrategy
    {
        public List<FireStation> SelectStations(
            List<FireStation> stations, 
            EmergencyEvent eventData, 
            int requiredVehicles)
        {
            var selectedStations = new List<FireStation>();
            var remainingVehicles = requiredVehicles;

            // Sortuj jednostki według odległości od zdarzenia
            var sortedStations = stations
                .Where(s => s.AvailableVehicleCount > 0)
                .OrderBy(s => s.DistanceTo(eventData.Latitude, eventData.Longitude))
                .ToList();

            foreach (var station in sortedStations)
            {
                if (remainingVehicles <= 0) break;

                var availableCount = station.AvailableVehicleCount;
                if (availableCount > 0)
                {
                    selectedStations.Add(station);
                    remainingVehicles -= System.Math.Min(availableCount, remainingVehicles);
                }
            }

            return selectedStations;
        }
    }
}
