using System.ComponentModel;

namespace FireRescueCommand.ViewModels
{
    public class LogEntry : INotifyPropertyChanged
    {
        public string Message { get; set; }
        public string Color { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public LogEntry(string message, string color)
        {
            Message = message;
            Color = color;
        }
    }
}
