using System;
using System.ComponentModel;
using FireRescueCommand.Models.States;

namespace FireRescueCommand.Models
{
    /// <summary>
    /// Reprezentuje pojazd 
    /// </summary>
    public class Vehicle : INotifyPropertyChanged
    {
        private IVehicleState _currentState;
        private int? _currentEventId;
        private EmergencyEvent? _currentEvent;

        public int Id { get; set; }
        public string StationName { get; set; }
        
        public EmergencyEvent? CurrentEvent
        {
            get => _currentEvent;
            set => _currentEvent = value;
        }
        
        public IVehicleState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnPropertyChanged(nameof(CurrentState));
                    OnPropertyChanged(nameof(StateColor));
                    OnPropertyChanged(nameof(StateName));
                    OnPropertyChanged(nameof(IsAvailable));
                    OnPropertyChanged(nameof(DisplayEventId));
                }
            }
        }

        public int? CurrentEventId
        {
            get => _currentEventId;
            set
            {
                if (_currentEventId != value)
                {
                    _currentEventId = value;
                    OnPropertyChanged(nameof(CurrentEventId));
                    OnPropertyChanged(nameof(DisplayEventId));
                }
            }
        }

        public double TimeInState { get; set; }

        public string StateColor => CurrentState.StateColor;
        public string StateName => CurrentState.StateName;
        public bool IsAvailable => CurrentState is AvailableState;
        public string DisplayEventId => CurrentEventId.HasValue ? $"#{CurrentEventId}" : "";

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<string>? LogRequested;
        public event Action<int>? EventCompleted; 

        public Vehicle(int id, string stationName)
        {
            Id = id;
            StationName = stationName;
            _currentState = new AvailableState();
            _currentState.Enter(this);
        }

        public void SetState(IVehicleState newState)
        {
            CurrentState.Exit(this);
            CurrentState = newState;
            CurrentState.Enter(this);
        }

        public void Update(double deltaTime)
        {
            CurrentState.Update(this, deltaTime);
        }

        public void DispatchToEvent(EmergencyEvent emergencyEvent, double travelTime)
        {
            if (!IsAvailable) return;

            CurrentEvent = emergencyEvent;
            CurrentEventId = emergencyEvent.Id;
            LogMessage($"Pojazd {Id} z {StationName} wysłany do zdarzenia #{emergencyEvent.Id}");
            SetState(new EnRouteState(travelTime));
        }

        public void NotifyEventCompleted()
        {
            if (CurrentEventId.HasValue)
            {
                EventCompleted?.Invoke(CurrentEventId.Value);
                CurrentEvent = null; 
            }
        }

        public void LogMessage(string message)
        {
            LogRequested?.Invoke(message);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
