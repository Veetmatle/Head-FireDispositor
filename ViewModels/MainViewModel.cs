using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using FireRescueCommand.Models;
using FireRescueCommand.Models.States;
using FireRescueCommand.Services;

namespace FireRescueCommand.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _simulationTimer;
        private readonly DispatcherTimer _eventGenerationTimer;
        private readonly CommandCenter _commandCenter;
        private bool _isRunning;
        private double _simSpeed = 1.0;
        private string _simSpeedText = "1x";

        public ObservableCollection<FireStation> Stations { get; }
        public ObservableCollection<EmergencyEvent> Events => _commandCenter.ActiveEvents;
        public ObservableCollection<LogEntry> Logs { get; }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public double SimSpeed
        {
            get => _simSpeed;
            set
            {
                if (value <= 0) return;
                
                _simSpeed = value;
                SimSpeedText = $"{value:F1}x";
                _eventGenerationTimer.Interval = TimeSpan.FromSeconds(10.0 / value);
        
                OnPropertyChanged(nameof(SimSpeed));
            }
        }


        public string SimSpeedText
        {
            get => _simSpeedText;
            set
            {
                _simSpeedText = value;
                OnPropertyChanged(nameof(SimSpeedText));
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ResetCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            Stations = new ObservableCollection<FireStation>();
            Logs = new ObservableCollection<LogEntry>();

            InitializeStations();

            _commandCenter = new CommandCenter(Stations);
            _commandCenter.LogGenerated += (message, color) => AddLog(message, color);
            
            foreach (var station in Stations)
            {
                foreach (var vehicle in station.Vehicles)
                {
                    vehicle.LogRequested += (msg) => AddLog(msg, "#34C759"); 
                    _commandCenter.SubscribeToVehicle(vehicle); 
                }
            }
            
            _simulationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _simulationTimer.Tick += SimulationTick;
            
            _eventGenerationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _eventGenerationTimer.Tick += GenerateEventTick;

            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            ResetCommand = new RelayCommand(Reset);

            AddLog("System gotowy na tragedie. Start aby wzniecić pożary.", "#34C759");
        }

        private void InitializeStations()
        {
            Stations.Add(new FireStation("JRG-1", 50.059984, 19.943153));
            Stations.Add(new FireStation("JRG-2", 50.033418, 19.935801));
            Stations.Add(new FireStation("JRG-3", 50.075705, 19.887319));
            Stations.Add(new FireStation("JRG-4", 50.037732, 20.005734));
            Stations.Add(new FireStation("JRG-5", 50.092085, 19.919915));
            Stations.Add(new FireStation("JRG-6", 50.015968, 20.015868));
            Stations.Add(new FireStation("JRG-7", 50.094073, 19.977532));
            Stations.Add(new FireStation("JRG Skawina", 49.97961, 19.82510));
            Stations.Add(new FireStation("SA PSP", 50.07436, 20.03605));
            Stations.Add(new FireStation("LSP Balice", 50.068735, 19.793512));
        }

        private void SimulationTick(object? sender, EventArgs e)
        {
            var deltaTime = 0.1; // 100ms - stały czas
            _commandCenter.Update(deltaTime);
        }

        private void GenerateEventTick(object? sender, EventArgs e)
        {
            _commandCenter.GenerateRandomEvent();
        }

        private void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                _simulationTimer.Start();
                _eventGenerationTimer.Start();
                AddLog("Symulacja włączona", "#34C759");
            }
        }

        private void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _simulationTimer.Stop();
                _eventGenerationTimer.Stop();
                AddLog("Symulacja zatrzymana", "#34C759");
            }
        }

        private void Reset()
        {
            Stop();
            
            Logs.Clear();
            _commandCenter.ActiveEvents.Clear();
            
            foreach (var station in Stations)
            {
                foreach (var vehicle in station.Vehicles)
                {
                    vehicle.SetState(new AvailableState());
                }
            }

            AddLog("Zresetowano. ", "#34C759");
        }

        private void AddLog(string message, string color)
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                Logs.Insert(0, new LogEntry(message, color));
                if (Logs.Count > 100) 
                {
                    Logs.RemoveAt(Logs.Count - 1);
                }
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => System.Windows.Input.CommandManager.RequerySuggested += value;
            remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
    }
}
