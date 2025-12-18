namespace FireRescueCommand.Models
{
    /// <summary>
    /// Typy zdarzeń 
    /// </summary>
    public enum EventType
    {
        PZ,  // Pożar (Fire) - 30% prawdopodobieństwa
        MZ,  // Miejscowe Zagrożenie (Local Hazard) - 70% prawdopodobieństwa
        AF   // Alarm Fałszywy (False Alarm) - 5% prawdopodobieństwa po przyjeździe
    }
}
