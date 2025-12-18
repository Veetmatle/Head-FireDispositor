# System Dyspozytorski Straży Pożarnej - Kraków

## Jak uruchomić

1. Otwórz `FireRescueCommand.sln` w JetBrains Rider
2. Naciśnij **F5** lub kliknij zielony przycisk **Run**
3. Aplikacja się uruchomi
4. Kliknij **▶ Start** aby rozpocząć symulację

## Struktura projektu

```
FireRescueCommand/
├── Models/
│   ├── States/              # State Pattern
│   │   ├── IVehicleState.cs
│   │   ├── AvailableState.cs
│   │   ├── EnRouteState.cs
│   │   ├── OnSceneState.cs
│   │   └── ReturningState.cs
│   ├── EventType.cs
│   ├── EmergencyEvent.cs
│   ├── Vehicle.cs
│   └── FireStation.cs
├── Services/
│   └── CommandCenter.cs     # Observer Pattern (Subject)
├── Strategies/              # Strategy Pattern
│   ├── IDispatchStrategy.cs
│   └── NearestStationStrategy.cs
├── ViewModels/
│   └── MainViewModel.cs
├── Converters/
│   └── CoordinateConverters.cs
├── App.xaml
├── MainWindow.xaml
└── FireRescueCommand.csproj
```

## Wzorce projektowe

- ✅ **State Pattern** - stany pojazdu
- ✅ **Observer Pattern** - komunikacja CommandCenter → Vehicle
- ✅ **Strategy Pattern** - strategia dyspozytorska
- ✅ **Iterator Pattern** - ObservableCollection + LINQ

## Wymagania

- .NET 8.0 SDK
- Windows 10+
- JetBrains Rider / Visual Studio 2022

## Status pojazdów (kolory kółek)

- 🔴 **Czerwony** - Dostępny
- 🟣 **Fioletowy** - W drodze
- ⚫ **Szary + ID** - Na miejscu
- 🟡 **Żółty** - Powrót

